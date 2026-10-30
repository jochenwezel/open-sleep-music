# Google Play data safety assessment

This is the submission worksheet for the Stable Android app, based on source
inspection through 30 October 2026. It contains provisional classifications, not a
submitted Data safety declaration. Do not select "no data collected" solely
because the project has no analytics SDK.

## Data flows and evidence

| Flow | Source evidence | Assessment and remaining work |
| --- | --- | --- |
| Player settings, favorites, blocks and saved position | `AppStateStore.cs`, `AndroidPlaybackService.cs` | Locally persisted; no automatic developer upload identified. Check Android backup and device-transfer behavior. |
| Catalog update at initial page loading and manual library refresh | `MainPage.xaml.cs`, `CatalogUpdateClient.cs` | HTTPS request to GitHub release assets, including server-visible connection metadata. Review redirects, recipient roles and server processing. |
| Artwork manifest and PNG downloads | `ArtworkManifestClient.cs`, `ArtworkCache.cs` | HTTPS GitHub requests; cache remains on the device. Check final catalog endpoints and provider handling. |
| Selected audio and background dependencies | `SleepWorldDownloader.cs`, `AudioLibraryStorage.cs`, Android backup XML and production catalog | Persistent app-owned `Audio/` storage without music-library permissions. Audio is excluded from configured Android cloud backups/device transfers. Requests reveal IP address and requested file to serving infrastructure. Review recipients and exact purpose; audio content itself is downloaded, not microphone data uploaded. Verify final bundle and real backup/restore. |
| Optional catalog feedback | `MainPage.ShareCatalogFeedbackAsync`, `AppSupport.cs`, `CatalogFeedbackReport.cs`, `docs/catalog-feedback.md` | User-triggered email draft to appsupport@compumaster.de with JSON attachment: timestamp, app version, platform, catalog count, favorite/blocked titles and optional free text. No automatic developer upload. Review user-initiated sharing exemptions and mail-provider behavior; free text and email sender details can contain personal data. |
| Source/license/project links | `TrackDetailsPage.xaml.cs`, `AboutPage.xaml.cs` | Opens an external browser. External website behavior is distinct from in-app behavior; review child-facing access. |
| Installation and store operations | Google Play and the operating system | Do not misdescribe Google Play's own processing as an app analytics implementation. Explain platform processing separately. |

The first-party HTTP client sends a generic application User-Agent; it is not an
installation identifier. The inspected built-in production audio hosts are
`archive.org`, `commons.wikimedia.org`, `upload.wikimedia.org`, `ks15.imslp.org`,
`raw.githubusercontent.com`, `opengameart.org` and `cdn.freesound.org`. Catalog/artwork assets also use `github.com`.
Redirects and later catalog updates can add serving hosts. This list describes
endpoints, not verified legal entities or contracted processors.

## Answers supported by the inspected source

- No app account or login is required. An account-deletion workflow is therefore
  not applicable unless accounts are introduced later.
- No ads implementation is configured. The listing's ads answer can reflect this
  after final bundle/dependency inspection.
- No camera, microphone, contacts, location or advertising-ID permission is
  declared in the source manifest. Verify the merged bundle manifest too.
- Downloads and manifest requests use HTTPS. This is not, on its own, a completed
  answer about all possible support/share destinations or all data encryption.
- Local preferences and playback state must not be described as automatically
  uploaded playback history. Conversely, optional exported ratings must not be
  described as impossible to share.

## Required decisions before submission

1. Identify the legal entities serving each download path and their actual
   processing, including logs, cookies, onward use, retention and transfer basis.
   Do not assume every public download host is our processor or that all requests
   are anonymous, ephemeral, or exempt from disclosure.
2. Map each actual off-device data use to Google's categories and collection/
   sharing exceptions. An IP address is not automatically a location declaration;
   using it to derive location would affect the classification.
3. Decide feedback availability in the child experience. A confirmation asking
   whether to share is not evidence of verified adult action. Retained feedback
   received by CompuMaster also needs a retention/deletion policy. App support
   email and feedback addressed to the operator must be processed through
   `appsupport@compumaster.de`. On 30 October 2026 the maintainer confirmed mailbox
   operation, authorized handling, deletion/depersonalization of unneeded working
   copies and central email retention for original correspondence; no new blanket
   period was adopted. General company and data protection
   contacts remain separate. Sharing remains explicitly user-initiated.
4. Review `allowBackup`, the merged manifest and both audio-exclusion XML files,
   platform backups, cached report files and app preferences. Test cloud restore
   and device transfer with settings restored and `Audio/` absent. Check legacy
   migration failures and files retained in the old Music subfolder; 2027+ builds
   do not access it. Confirm actual deletion scope rather than total erasure.
5. Reconcile the final worksheet, hosted notice, app UI and complete dependencies
   with the exact Stable bundle and catalog-update delivery used for launch.

Use Google's [Data safety definitions](https://support.google.com/googleplay/android-developer/answer/10787469?hl=en)
and [User Data policy](https://support.google.com/googleplay/android-developer/answer/10144311?hl=en).
Have the company's data protection contact review unresolved legal bases,
provider relationships and international transfers before publication.
See [privacy-publication-review.md](privacy-publication-review.md) for the
specific source checks and the current publication status, rather than keeping
editorial placeholders in the user-facing notice.
