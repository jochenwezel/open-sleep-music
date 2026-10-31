# Android privacy notice publication review

Status on 31 October 2026: GitHub-only delivery implemented and the Android
notice reconciled with the concrete assessment supplied by the central privacy
records task. Public copy remains a publication candidate in
[privacy-notice.de.md](privacy-notice.de.md); this is not a new app release or a
blanket approval of direct online service use by children under 13.

## Resolved drafting and operational points

- Scope is Android/Google Play; the unreviewed Apple section is omitted.
- Persistent private Audio storage, legacy-file limits and Android audio backup
  exclusions match `docs/audio-storage.md` and the source manifest/XML. Device
  restore tests remain app-release evidence, not public editorial placeholders.
- The maintainer explicitly confirmed the configured support mailbox, authorized
  handling and working-copy deletion/depersonalization on 30 October 2026. Original
  correspondence follows the central email concept, consistent with records
  017/008. No historical six-year blanket period is represented as approved.
- The candidate describes retention criteria, not invented numerical periods or
  immediate erasure of mail-client/backup copies. Article 13(2)(a) permits the
  applicable criteria where specifying a duration is not possible.
- Purposes, conditional contract/legitimate-interest grounds, statutory retention,
  voluntary support information, local report cache and rights are described.
- Dynamic sources use a recipient category in the public text. Article 13(1)(e)
  allows recipients or recipient categories; the category must remain meaningful.
  A fixed provider list is not a substitute for reviewing actual providers.
- The adult instruction in the notice does not implement an age gate or establish
  Families compliance. Child-facing release checks stay in the launch checklist.

## Actual GitHub delivery and provider evidence

- [media-v1](https://github.com/jochenwezel/open-sleep-music/releases/tag/media-v1)
  contains 167 byte-identical audio assets (166 Stable plus one cleared Preview
  audition), `media-delivery.json` and the attribution README. Audio totals
  1,296,986,854 bytes. Every uploaded GitHub SHA-256 digest and public audio
  signature was verified. Audio was not added to Git or app packages.
- Both generated media catalogs, runtime candidates and all image/motif layers
  use project GitHub URLs. Core validates initial content URLs and every followed
  redirect before requesting the next destination; foreign/HTTP destinations
  cannot become an automatic fallback. Original source/licence/file links remain
  recording provenance and deliberate browser destinations only.
- A live probe through the actual Core client returned HTTP 200 for audio, a PNG
  and the media manifest through `release-assets.githubusercontent.com`. TLS and
  hostname validation were not disabled. Details: [delivery workflow](../github-content-delivery.md).
- The central privacy-records task independently checked GitHub's official DPF
  entry on 31 October 2026: active EU-US DPF, Non-HR, next certification due
  3 August 2027. Its concrete assessment assigns covered transfers to GitHub,
  Inc. to Article 45 and the necessary technical requests to Article 6(1)(f).
  This closes the former missing-register-evidence item; no individual SCCs for
  every original source are invented or required merely because those provenance
  links remain in the catalog. Primary references:
  [DPF register](https://www.dataprivacyframework.gov/list),
  [GitHub privacy](https://docs.github.com/en/site-policy/privacy-policies/github-general-privacy-statement).
- Corporate support mail is confirmed IONOS Mail Basic and matched by the central
  records task to the existing IONOS processing agreement/product-specific
  information. Do not map Hosted Exchange's chain to this address or promise
  unconditional EU-only processing. The support/working-copy deletion process is
  already confirmed; no new numerical retention period is adopted.
- Original-host role/transfer uncertainties are historical for the old delivery
  model, not a continuing automatic-download audit for this implementation.
  Deliberate external web visits remain separate. Historical binaries are not
  retroactively changed; the GitHub-only promise applies to the updated build.

## Specific remaining child/online distinction

GitHub Terms A.11/B.3 set a minimum service-user age of 13. Catalog-selected
files address content suitability; they do not establish an age exception in
those terms. The app currently automatically refreshes the catalog on loading
and fetches artwork as needed, without a persistent app-wide offline switch.

Adult-managed online setup/downloads followed by playback with device networking
disabled is technically possible with the existing app. Direct online service
use by under-13s is not unconditionally approved. If child use on an online device
must produce no automatic requests, an actual network gate/product change or
provider clarification remains necessary; a parental sentence is not evidence
of such a control. No birthdate collection or age gate was introduced by this
transport migration. See [GitHub Terms](https://docs.github.com/en/site-policy/github-terms/github-terms-of-service)
and the delivery document's explicit offline-workflow limits.

The website task should synchronize the reviewed candidate, perform its visual
check and retain the existing sitemap exclusion. Public publication still needs
its concrete authorization. Final app builds, child-facing Store declarations
and the remaining release checks are tracked separately, without a general
waiting loop for unspecified provider/DSB work.

## Legal and store references

- [GDPR, official BMF reproduction, Articles 6 and 13](https://lsth.bundesfinanzministerium.de/ao/2026/Datenschutz-Grundverordnung/inhalt.html).
- [Google Play User Data policy](https://support.google.com/googleplay/android-developer/answer/10144311?hl=de).

The public notice describes the actual Android GitHub delivery; this review file
keeps the specific child/online limitation and publication status explicit.
