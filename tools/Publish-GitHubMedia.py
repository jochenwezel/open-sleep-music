"""Stage, validate and publish byte-identical catalog audio as versioned assets.

Only maintainer tooling contacts original hosts. Nothing is transcoded or added
to Git. Existing recording-specific catalog admission decisions are retained.
"""
import argparse
import concurrent.futures
import hashlib
import json
from pathlib import Path
import subprocess
import time
import urllib.request
import urllib.parse
import urllib.error

ROOT = Path(__file__).resolve().parent.parent
REPO = "jochenwezel/open-sleep-music"
LICENSES = {"CC0 1.0", "CC BY 3.0", "CC BY 4.0", "CC BY-SA 2.0",
            "CC BY-SA 3.0", "CC BY-SA 3.0 Germany", "CC BY-SA 4.0",
            "Public Domain", "Public Domain Dedication", "Free Art License 1.3", "CC BY-NC 4.0"}


def gh(*args):
    return subprocess.check_output(["gh", *args], cwd=ROOT, text=True, encoding="utf-8").strip()


def find_release(tag):
    # Draft releases are not returned by the public get-by-tag endpoint.
    value = gh("api", f"repos/{REPO}/releases?per_page=100", "--paginate", "--jq",
               f'.[] | select(.tag_name == "{tag}")')
    return json.loads(value) if value else None


def release_assets(release_id):
    pages = json.loads(gh("api", f"repos/{REPO}/releases/{release_id}/assets?per_page=100", "--paginate", "--slurp"))
    return [asset for page in pages for asset in page]


def hashes(path):
    sha1, sha256 = hashlib.sha1(), hashlib.sha256()
    with path.open("rb") as stream:
        while chunk := stream.read(1024 * 1024):
            sha1.update(chunk)
            sha256.update(chunk)
    return sha1.hexdigest(), sha256.hexdigest()


def audio_signature(data):
    return (data.startswith((b"ID3", b"OggS", b"fLaC"))
            or len(data) >= 2 and data[0] == 255 and data[1] & 224 == 224
            or len(data) >= 12 and data[:4] == b"RIFF" and data[8:12] == b"WAVE"
            or len(data) >= 12 and data[4:8] == b"ftyp")


def request(url, *, probe=False, complete_range=False):
    headers = {"User-Agent": "OpenSleepMusicMediaPublisherBot/1.0 (+https://github.com/jochenwezel/open-sleep-music; appsupport@compumaster.de)"}
    if probe:
        headers["Range"] = "bytes=0-255"
    elif complete_range:
        headers["Range"] = "bytes=0-"
    parts = urllib.parse.urlsplit(url)
    if parts.scheme != "https":
        raise ValueError("Acquisition must use HTTPS")
    url = urllib.parse.urlunsplit((parts.scheme, parts.netloc,
                                  urllib.parse.quote(parts.path, safe="/%:@+"), parts.query, ""))
    return urllib.request.urlopen(urllib.request.Request(url, headers=headers), timeout=90)


def original_request(url):
    try:
        return request(url, complete_range=True)
    except urllib.error.HTTPError as error:
        parts = urllib.parse.urlsplit(url)
        segments = parts.path.split("/", 3)
        if error.code not in (500, 502, 503, 504) or parts.hostname != "archive.org" or len(segments) != 4 or segments[1] != "download":
            raise
        # Maintainer acquisition only: use storage nodes explicitly advertised
        # by the canonical item API. These routes never enter runtime catalogs.
        item, filename = segments[2], segments[3]
        with request(f"https://archive.org/metadata/{item}") as response:
            metadata = json.load(response)
        for host in (metadata.get("d1"), metadata.get("d2")):
            if not host or not host.endswith(".archive.org") or not metadata.get("dir"):
                continue
            try:
                return request(f"https://{host}{metadata['dir']}/{filename}", complete_range=True)
            except urllib.error.HTTPError as storage_error:
                if storage_error.code == 429:
                    raise
        raise error


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--tag", required=True)
    parser.add_argument("--publish", action="store_true", help="Create/publish a media release, never an app release")
    parser.add_argument("--workers", type=int, default=4)
    args = parser.parse_args()
    if not args.tag.startswith("media-") or any(c not in "abcdefghijklmnopqrstuvwxyz0123456789-." for c in args.tag):
        parser.error("Use a dedicated lower-case media-* tag, never an app tag.")
    stage = ROOT / "artifacts" / "github-media" / args.tag
    stage.mkdir(parents=True, exist_ok=True)
    catalog = json.loads((ROOT / "src/OpenSleepMusic.Core/Catalog/media-catalog.json").read_text(encoding="utf-8"))
    tracks = {t["id"]: t for w in catalog["sleepWorlds"] for t in w["tracks"]}
    if len({t["fileName"].lower() for t in tracks.values()}) != len(tracks):
        raise ValueError("Colliding audio filenames")
    for t in tracks.values():
        if t.get("licenseReviewStatus", "verified") != "verified" or t["license"] not in LICENSES:
            raise ValueError(f"No redistribution clearance recorded: {t['id']}; retain as an external audition reference")

    def prepare(t):
        path = stage / t["fileName"]
        original = t.get("originalDownloadUri", t["downloadUri"])
        receipt_path = path.with_suffix(path.suffix + ".receipt.json")
        receipt = json.loads(receipt_path.read_text(encoding="utf-8")) if receipt_path.exists() else None
        if path.exists():
            sha1, sha256 = hashes(path)
            if not receipt or receipt.get("originalDownloadUri") != original or receipt.get("sha256") != sha256:
                raise ValueError(f"Unverified staging file: {path.name}")
        else:
            temporary = path.with_suffix(path.suffix + ".part")
            for attempt in range(3):
                try:
                    with original_request(original) as response, temporary.open("wb") as output:
                        while chunk := response.read(1024 * 1024):
                            output.write(chunk)
                    with temporary.open("rb") as audio:
                        if not audio_signature(audio.read(64)):
                            raise ValueError("Response is not supported audio")
                    sha1, sha256 = hashes(temporary)
                    if t.get("sha1") and sha1 != t["sha1"].lower():
                        raise ValueError("Upstream SHA-1 mismatch")
                    temporary.replace(path)
                    receipt_path.write_text(json.dumps({"originalDownloadUri": original, "sha1": sha1,
                                                       "sha256": sha256}, indent=2) + "\n", encoding="utf-8")
                    break
                except Exception as error:
                    temporary.unlink(missing_ok=True)
                    if isinstance(error, urllib.error.HTTPError) and error.code == 429:
                        raise
                    if attempt == 2:
                        raise
                    time.sleep(2 * (attempt + 1))
        if t.get("sha1") and sha1 != t["sha1"].lower():
            raise ValueError(f"Upstream SHA-1 mismatch: {t['id']}")
        with path.open("rb") as audio:
            if not audio_signature(audio.read(64)):
                raise ValueError(f"Invalid staged audio: {t['id']}")
        return {"id": t["id"], "fileName": t["fileName"], "originalDownloadUri": original,
                "downloadUri": f"https://github.com/{REPO}/releases/download/{args.tag}/{t['fileName']}",
                "sha1": sha1, "sha256": sha256, "bytes": path.stat().st_size,
                "title": t["title"], "creator": t["creator"], "sourcePageUri": t["sourcePageUri"],
                "license": t["license"], "licenseUri": t["licenseUri"],
                "licenseReviewStatus": t.get("licenseReviewStatus", "verified"),
                "durationSeconds": t["durationSeconds"], "modifications": "Byte-identical mirror; no audio encoding changes"}

    entries, failures = [], []
    with concurrent.futures.ThreadPoolExecutor(max_workers=args.workers) as pool:
        tasks = {pool.submit(prepare, t): t["id"] for t in tracks.values()}
        for future in concurrent.futures.as_completed(tasks):
            try:
                entries.append(future.result())
                if len(entries) % 10 == 0:
                    print(f"Validated {len(entries)}/{len(tracks)}", flush=True)
            except Exception as error:
                failures.append({"id": tasks[future], "error": str(error)})
                print(f"FAILED {tasks[future]}: {error}", flush=True)
    (stage / "failures.json").write_text(json.dumps(failures, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if failures:
        raise SystemExit(f"{len(failures)} files unavailable; no release or catalog was changed. Rerun resumes verified files.")
    entries.sort(key=lambda e: e["id"])
    manifest = {"schemaVersion": 1, "repository": REPO, "releaseTag": args.tag, "tracks": entries}
    manifest_path = stage / "media-delivery.json"
    manifest_path.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    notes = stage / "README.md"
    notes.write_text("# Catalog audio delivery\n\nByte-identical mirrors of the recordings admitted to Open Sleep Music.\n"
                     "Each recording retains its individual license, independently of the app's MIT license.\n"
                     "See media-delivery.json for titles, creators, source pages, original file URLs, exact licenses,\n"
                     "license links and SHA-1/SHA-256 checksums. Playback trims, speed and backgrounds are\n"
                     "application metadata and do not alter these audio files. NC recordings are offered\n"
                     "only as part of the project's noncommercial offering and retain their NC restrictions.\n",
                     encoding="utf-8")
    print(f"Prepared {len(entries)} files, {sum(e['bytes'] for e in entries)} bytes", flush=True)
    if not args.publish:
        return
    release = find_release(args.tag)
    if release is None:
        gh("release", "create", args.tag, "--repo", REPO, "--draft", "--latest=false",
           "--target", gh_commit(), "--title", f"Catalog media {args.tag}", "--notes-file", str(notes))
        release = find_release(args.tag)
        if release is None:
            raise ValueError("Created media draft could not be recovered")
    expected = {e["fileName"]: "sha256:" + e["sha256"] for e in entries}
    expected.update({p.name: "sha256:" + hashes(p)[1] for p in [manifest_path, notes]})
    existing = {a["name"]: a for a in release_assets(release["id"])}
    for name, digest in expected.items():
        if name in existing and existing[name].get("digest") != digest:
            raise ValueError(f"Never overwrite a versioned asset: {name}")
    missing = [stage / name for name in expected if name not in existing]
    for start in range(0, len(missing), 12):
        gh("release", "upload", args.tag, "--repo", REPO, *map(str, missing[start:start + 12]))
        print(f"Uploaded {min(start+12,len(missing))}/{len(missing)} missing assets", flush=True)
    release = json.loads(gh("api", f"repos/{REPO}/releases/{release['id']}"))
    actual = {a["name"]: a.get("digest") for a in release_assets(release["id"])}
    if actual != expected:
        raise ValueError("GitHub asset set/digests do not match staged bytes")
    if release["draft"]:
        gh("release", "edit", args.tag, "--repo", REPO, "--draft=false", "--latest=false")
    def probe(e):
        with request(e["downloadUri"], probe=True) as response:
            if not audio_signature(response.read(256)):
                raise ValueError(f"GitHub delivery probe failed: {e['id']}")
    with concurrent.futures.ThreadPoolExecutor(max_workers=args.workers) as pool:
        list(pool.map(probe, entries))
    registry = ROOT / "tools/github-media-delivery.json"
    registry.write_text(manifest_path.read_text(encoding="utf-8"), encoding="utf-8")
    print(f"Published and verified {len(entries)} assets; wrote {registry}", flush=True)


def gh_commit():
    return subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()


if __name__ == "__main__":
    main()
