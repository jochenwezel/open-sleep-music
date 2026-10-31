# App privacy commitments and operating rules

Open Sleep Music's privacy notice must describe its actual app behavior. These
rules preserve the technical promises while keeping unresolved operator and
platform decisions explicit. They supplement `AGENTS.md` and the
[Google Play assessment](google-play/data-safety.md).

## Transport protection

All app-managed audio, background audio, images, catalog updates and artwork
manifests must be retrieved over HTTPS. Every followed redirect must remain
HTTPS. Keep normal certificate-chain and hostname verification; do not add
trust-all callbacks, bypass certificate errors or retry through HTTP.

A transport failure must preserve the existing behavior: skip the unavailable
track/image, clean up partial files, continue the collection and retain valid
offline catalog/image fallbacks. The policy also applies to new sources and
future dynamic catalog updates. A source URL being HTTPS does not establish
that every redirect and platform handler has been tested.

`BuiltInCatalogTests` checks HTTPS audio, source, license and artwork metadata;
`CatalogUpdateClient` rejects insecure catalog entries and
`ArtworkManifestClient` validates HTTPS image metadata. The current HTTP client
does not override certificate validation. The standard .NET handler documents
that HTTPS-to-HTTP redirects are not followed on modern .NET, but supported
platform handlers must still be verified before treating all runtime paths as
tested. See [Microsoft's redirect documentation](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.allowautoredirect?view=net-10.0).

## Local state and optional sharing

- Retain settings, favorites, blocked titles and resume state for local app use.
  Do not upload them automatically or add a stable installation identifier.
- Keep advertising, advertising IDs, profiling and automatic usage analytics out
  of the app. Review new packages and APIs for indirect data collection too.
- Keep report sharing explicitly initiated by the user, with its existing scope
  summary and cancellation opportunity. Do not silently upload reports or expand
  their documented fields. Follow [catalog-feedback.md](catalog-feedback.md).
- Do not add account, cloud-sync or permission requirements merely to support
  ordinary offline listening. Assess child privacy and adult-action requirements
  before making free-text sharing available in the final Store child experience.

Operating-system backups and device transfers are distinct from app-operated
uploading. Audio uses persistent app-owned storage; 2026 releases migrate the old
app-specific Music subfolder, and 2027+ releases compile out that access. Android
keeps `allowBackup="true"` for eligible app state but excludes `file:Audio/` from
legacy backups and modern cloud backup/device transfer. Apple targets mark the
Audio directory as excluded from backup; packaged Windows uses persistent
`LocalCacheFolder/Audio`. See [audio-storage.md](audio-storage.md) for exact paths,
failure handling, cutoff and platform limits. Do not promise that all app state
always remains on one device or that uninstalling removes unmigrated legacy files,
already shared reports or user-controlled full-device backups. Verify backup and
restore behavior in the final bundles; source configuration is not a device test.
See [Android backup behavior](https://developer.android.com/identity/data/autobackup).

## Feedback received by the operator

Open Sleep Music confirms the operator's routing rule from 29 October 2026:
app-related support email and feedback sent to CompuMaster must be processed
through **appsupport@compumaster.de**. Use that address in app support information
and Store metadata. The general company address and data protection officer's
address retain their separate roles. The app prepares a mail draft addressed to
the support mailbox, with the report attached. The user decides whether to send
it in the mail client; this is not automatic sending or proof of delivery. Later
changes made by the user in that external client are outside the app's control.

On 30 October 2026, the maintainer explicitly confirmed that the support mailbox
is configured and handled by the maintainer or authorized employees; unneeded
personal working copies are deleted or depersonalized, while the original email
follows the central email retention/deletion concept. The confirmation does not
approve a new blanket retention period. Internal processing record 017 refers
email correspondence to record 008; the latter's historical six-year figure is
marked for review, not newly adopted for every support message.

The maintainer subsequently confirmed that `appsupport@compumaster.de` is an
IONOS Mail Basic mailbox. Use that product when mapping the applicable provider
and subprocessor evidence; do not classify this address as Hosted Exchange or
infer additional forwarding destinations.

Describe retention by its actual purposes and criteria: case handling, necessary
follow-up and documentation, applicable statutory retention and legal claims.
Do not invent a fixed duration or promise immediate deletion from every mailbox,
client, archive or backup. Additional working copies and depersonalized catalog
findings are distinct from retention of the underlying correspondence. The
user-initiated report file remains in the app cache; opening the mail composer
does not establish that sending or deletion has occurred.
Keep raw reports and personal comments out of Git. Retain useful catalog findings
only in a form that no longer identifies the reporter; preserve the required
track-quality improvement history without importing personal report contents.

## Providers and publication review

Distinguish developer-held records from access logs held by independent content
providers. Do not promise to inspect or delete provider logs that we cannot
access, or generate purported deletion evidence for them. Our own records and
any processing performed on our behalf remain subject to our actual duties;
calling a service a third party does not change its role.

Users choose audio downloads. Catalog checks and some artwork requests are
automatic in the current app. Describe both; never claim that all off-device
requests occur only after a download click or that a download click grants
blanket permission for provider tracking.

When admitting or changing a source, retain its canonical source link and review
the applicable provider information, noting provider and review date in the
publication assessment. Reassess relevant changes when they become known. Do not
promise continuous detection of undisclosed provider-policy changes. Keep current
source information accessible per track rather than hard-coding a supposedly
permanent provider list into the notice.

Keep the serving-provider inventory current when adding a new download source
or materially changing redirects. Explain server-visible IP addresses and
requested files without calling them anonymous. Do not assume every hosting
provider is our processor, promise third-party deletion dates or reuse a website
tracking description for this app. Identify the actual relationships, legal
bases and transfer arrangements before finalizing those notice sections.

Before launch or a relevant behavior change:

1. Compare the notice with the delivered bundle's permissions, dependencies,
   traffic, report contents, local deletion controls and backup behavior.
2. Exercise successful HTTPS redirects, rejection of HTTPS-to-HTTP redirects and
   invalid certificates on supported platforms. Verify that failures retain
   resilient download and offline behavior. Record the platform and result.
3. Keep the confirmed operator process and any later changes aligned with the
   public retention criteria; do not substitute a new blanket email period.
4. Reconcile the app notice, each store's disclosures and the source/provider
   inventory. Keep website copy and the repository draft aligned.

The remaining launch evidence is tracked in
[Google Play publication issue 22](https://github.com/jochenwezel/open-sleep-music/issues/22).
These rules do not claim a completed transport, provider, backup or legal audit.

On 29 October 2026, the existing Core suite passed with 206 tests in Stable and
206 in Preview, including the catalog HTTPS invariants and remote metadata
validation tests. This run did not exercise live platform redirects, certificate
failures, operating-system backups or deletion of received support reports.
