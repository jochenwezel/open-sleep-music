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
uploading. Android currently sets `allowBackup="true"`. Do not promise that all
app state always remains on one device or that uninstalling removes external
music files and already shared reports. Describe the actual backup configuration
of the final Android bundle. Review iOS separately before publishing iOS claims.
See [Android backup behavior](https://developer.android.com/identity/data/autobackup).

## Feedback received by the operator

Open Sleep Music confirms the operator's routing rule from 29 October 2026:
app-related support email and feedback sent to CompuMaster must be processed
through **appsupport@compumaster.de**. Use that address in app support information
and Store metadata. The general company address and data protection officer's
address retain their separate roles. Users may still choose whether and through
which application to share a report; this rule does not cause automatic sending.

This project-level adoption does not confirm that the mailbox has been
provisioned, is reachable or is being processed. Verify those operational facts
and the deletion process before publishing corresponding assurances.

The website task proposed deleting personal feedback after processing once no
further retention reason exists, with statutory obligations preserved. This is
a suitable purpose-based drafting approach, but its operational adoption and
implementation still require confirmation by CompuMaster. A repository edit
does not delete reports in mailboxes, support tools or their backups.

Before publishing that promise, establish who reviews received reports, what
counts as completed processing, where raw reports and copies are stored, how
deletion is performed and which actual exceptions apply. Do not invent a fixed
duration or imply that a user-initiated share is automatically deleted everywhere.
Keep raw reports and personal comments out of Git. Retain useful catalog findings
only in a form that no longer identifies the reporter; preserve the required
track-quality improvement history without importing personal report contents.

## Providers and publication review

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
3. Confirm the actual operator deletion process before publishing its promise.
4. Reconcile the app notice, each store's disclosures and the source/provider
   inventory. Keep website copy and the repository draft aligned.

The remaining launch evidence is tracked in
[Google Play publication issue 22](https://github.com/jochenwezel/open-sleep-music/issues/22).
These rules do not claim a completed transport, provider, backup or legal audit.

On 29 October 2026, the existing Core suite passed with 206 tests in Stable and
206 in Preview, including the catalog HTTPS invariants and remote metadata
validation tests. This run did not exercise live platform redirects, certificate
failures, operating-system backups or deletion of received support reports.
