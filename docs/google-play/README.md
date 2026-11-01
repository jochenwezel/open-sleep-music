# Google Play publication preparation

Open Sleep Music is being prepared for publication by **CompuMaster** using an
organization account. Account verification is pending, as reported by the
maintainer on 29 October 2026. The app record was created in Play Console on
29 October 2026 after the maintainer explicitly authorized the three creation
declarations. The first internal test was activated on 1 November 2026 with
version 0.1.43 (version code 43). No production release was submitted.
English (United States, `en-US`) is the default store language, with German
(`de-DE`) as an additional translation. Both text drafts are saved in the
Console. The app itself supports English and German; the default store language
does not restrict the app interface languages. English is the intended fallback
where no matching translation exists; Play may also offer machine translation.

Tracking issue: [Google Play publication under CompuMaster, issue 22](https://github.com/jochenwezel/open-sleep-music/issues/22).

## Android distribution decision

On 31 October 2026 the maintainer selected Google Play for Android Preview
testing (internal/closed tracks) as well as Stable releases. This supersedes the
earlier GitHub Preview APK distribution plan. Automatic Android APK publishing
has been removed from the GitHub release workflow; Windows distribution and
existing historical assets remain unchanged. The internal Play test is active;
enrollment is restricted to the selected tester list in Console.

GitHub remains the runtime delivery provider for music, artwork and catalogs.
Changing where the app is installed from does not remove those connections or
their privacy disclosures.

There is no requirement for Play releases to update GitHub development APKs in
place. A Google-managed app-signing key is therefore acceptable; keep the
development signing key separate and preserve it for developer APK updates.
Select the Play upload key deliberately. An existing development installation
with the same application ID and an incompatible signature may need to be
uninstalled before installing the Play version. A distinct development
application ID can be considered separately for side-by-side installation.

The maintainer accepts occasional Google Play online installation checks and
has chosen to retain the default automatic installation protection for the
planned Play version. This is a distribution setting, separate from app signing;
it does not add a developer-operated music licence check. Review the delivered
build's actual behavior, network/data implications and offline operation before
release. The maintainer does not want an explicit online-check notice in the
store description, app help or privacy draft. Describe local playback factually
and assess the final bundle's actual data processing for required disclosures.
Google does
not publish a fixed maximum offline interval:
[automatic protection](https://support.google.com/googleplay/android-developer/answer/10183279?hl=en).

## Publication materials

- [English store listing](store-listing.en.json): default-language text draft.
- [German store listing](store-listing.de.json): translation, without fixed
  catalog totals or promises of medical benefits.
- [German app privacy notice source](privacy-notice.de.md): published on the
  [company website](https://www.compumaster.de/datenschutzhinweis/apps/open-sleep-music/#de).
- [English app privacy notice source](privacy-notice.en.md): published on the
  [same company page](https://www.compumaster.de/datenschutzhinweis/apps/open-sleep-music/#en)
  after the maintainer's explicit publication approval on 1 November 2026.
- [Data safety assessment](data-safety.md): source evidence and unresolved answers.
- [Store assets and reviewer instructions](store-assets.md): screenshot plan,
  graphics requirements and background-playback declaration.

Public operator details come from the
[company imprint](https://www.compumaster.de/impressum/): CompuMaster GmbH,
Im Vogelsang 3, 56154 Boppard, Germany; `info@compumaster.de`.
The [company privacy notice](https://www.compumaster.de/datenschutzhinweis/)
also names the data protection contact `dsb@keydata.gmbh`.
App-related support email and feedback addressed to CompuMaster must be processed
through **appsupport@compumaster.de**. The app support phone number is
**+49 6742 9799111** (+4967429799111), as directed by the maintainer and adopted
by this project on 29 October 2026. General company and data protection contacts
remain separate. This adoption is not verification that the mailbox is configured,
reachable or covered by an implemented deletion process.

## Privacy publication decision

The company notice already covers mobile applications in its introduction, but
also describes marketing, profiling and web analytics. It does not explain this
app's download providers, local playback settings, catalog updates or feedback
export. A link to that generic text alone would leave those practices unclear.

Publish a dedicated **Open Sleep Music privacy notice**, linked directly from
Play Console and the app. Keep it separate from the general website notice and
site index according to the website project's established app-page rules.
Shared text for future apps must still describe each app accurately. Do not
imply that website analytics operate inside
the app, or that clicking a website link inherits the app's lack of tracking.

German and English notices published on 1 November 2026:
`https://www.compumaster.de/datenschutzhinweis/apps/open-sleep-music/`.
Use one stable URL for this app, with clearly labeled Android/Google Play and
iOS/App Store sections when platform processing differs. Separate URLs are an
option if the practices diverge substantially, not the default. The current
worksheet inspects Android; iOS backup, dependencies and actual data flows still
need their own review before making an iOS declaration. Each store's disclosure
form must match its distributed app even when both reference the same notice.
Apple also requires the privacy-policy link in its listing and inside the app:
[App Review Guideline 5.1.1](https://developer.apple.com/app-store/review/guidelines/#privacy).
Version 0.1.43 adds the published German notice under About, bundled for offline
reading even in locked child mode. Its online link remains parent-gated. The
website now offers both languages through `#de` and `#en`; the existing 0.1.43
bundle still contains only the German offline text. Google requires a publicly
accessible privacy URL and in-app access, including for apps without sensitive
data collection. See the [User Data policy](https://support.google.com/googleplay/android-developer/answer/10144311?hl=en).

## Current implementation evidence

Baseline: commit `d368d5e`, inspected on 29 October 2026.
The app project uses `org.opensleepmusic.app`, version code 38. Store publication
must use a subsequently selected, tested release version; this baseline is not a
release authorization.

- Stable is the default in `Directory.Build.props`. Core embeds the production
  catalog; Preview candidates and `PreselectionPage` are excluded from Stable.
- The production manifest contains six collection definitions and 138 unique
  recordings. Counts are an inspection snapshot, not store-description promises.
- As of 31 October 2026, `.github/workflows/publish-release.yml` publishes Windows
  ZIPs only. The first signed Google Play AAB was published to the internal track
  on 1 November 2026; see the internal-test record below.
- The source Android manifest declares network access, media foreground service
  and wake-lock permissions. The playback service declares `mediaPlayback`.
- No advertising or analytics SDK is configured in the inspected app startup
  and direct package references. Verify transitive dependencies and the final
  merged manifest rather than treating this as a completed SDK audit.
- `android:allowBackup="true"` retains backup eligibility for settings. The
  manifest's legacy and modern XML rules exclude downloaded `Audio/` from cloud
  backup and device transfer. Verify the merged bundle and real restore behavior;
  this does not establish that settings can never leave the device.

## Launch checklist

- [x] Create the Play Console app record with authorized creation declarations.
- [x] Save English and German store text drafts with English as the default.
- [x] Add the store icon, feature graphic and localized phone screenshots to
  the Console draft; retain source files and hashes in the repository.
- [ ] Reconcile the [preliminary export assessment](export-assessment.md) with
  the final delivered Stable bundle and intended distribution.
- [ ] Verify the [privacy commitments](../privacy-policy.md), including HTTPS
  redirects/certificate failures on supported platforms and the real operator
  process for deleting received feedback before publishing that promise.
- [ ] Organization verification completed; public legal identity and contacts checked.
- [x] Maintainer confirmed on 30 October 2026 that `appsupport@compumaster.de`
  is configured and handled by authorized people, with personal working-copy
  deletion/depersonalization and original email retention under the central concept.
- [x] German Android app privacy notice reviewed and published on the company
  website, with the permanent sitemap/navigation exclusions preserved.
- [x] Publish the English translation after explicit publication approval.
- [ ] Include the published English notice in a subsequent app update for
  English offline reading; the current 0.1.43 bundle contains German only.
- [x] Add offline access to the published German notice inside the app (0.1.43).
- [x] Save the published German privacy notice URL in Play Console.
  The Android [publication review](privacy-publication-review.md) separates the
  published notice from the remaining app and Store checks.
- [ ] Provider roles, redirect destinations, retention and international transfers
  assessed; Data safety answers reconciled with the actual distributed bundle.
- [ ] Mixed adult/child audience declared accurately. Select only age groups the
  app is designed for; complete the IARC questionnaire separately. Review the
  [Families policy](https://support.google.com/googleplay/android-developer/answer/9893335?hl=en).
- [ ] Decide on and implement adult-only handling of free-text feedback before
  children can send personal information. Review external browser links and
  downloads for the child experience; a generic age prompt is not a substitute
  for compliant SDKs and content.
- [ ] Production recordings, background credits and download/update delivery
  reviewed. Apply `AGENTS.md` recording-rights requirements and assess the actual
  commercial/noncommercial use before including any NC recording.
- [ ] Fix and physically validate [Android task-close cleanup, issue 19](https://github.com/jochenwezel/open-sleep-music/issues/19).
- [x] Build a signed Stable `.aab` and SHA-256 from a tested release commit.
  Use Google Play internal testing; public GitHub APK publishing is retired.
- [x] Configure Play App Signing deliberately. A new Google-managed app-signing
  identity is acceptable; migration from GitHub development APKs is not required.
  Separate upload and app-signing keys where appropriate; never put private keys
  in issues, repository files or public artifacts. See [Android signing](https://developer.android.com/studio/publish/app-signing)
  and [existing release signing](../android-signing.md).
- [ ] Verify the chosen Play automatic installation protection setting; test the
  delivered build and reflect relevant data flows in required privacy disclosures.
- [ ] Verify target API 36 or later in the final manifest for current phone-app
  submissions, supported ABIs, native-library compatibility and increasing version
  code. See the [current target API requirement](https://developer.android.com/google/play/requirements/target-sdk).
- [ ] Verify Stable contains no unchecked audition collections/resources and no
  bundled catalog audio; test both channel variants if gates change.
- [ ] Complete foreground-service declaration and demonstration video.
- [ ] Export final icon, feature graphic and actual Stable screenshots; approve
  store text, Music & Audio category proposal, distribution countries and price.
- [ ] Run internal testing and physical-device checks in
  [playback-regression-checks.md](../playback-regression-checks.md) and
  [roadmap.md](../roadmap.md); record results and resolve pre-launch report findings.
- [ ] Submit the reviewed bundle and declarations after account access is available.

The 12-testers/14-days production-access rule is documented for new **personal**
accounts. Do not treat it as an automatic organization-account requirement;
follow the tasks actually shown in this account's Console. Internal testing is
still appropriate. See [Google's app setup guide](https://support.google.com/googleplay/android-developer/answer/9859152?hl=en).

## Bundle preparation

GitHub release automation no longer produces Android APKs. A focused Play build
workflow should produce signed AAB output, checksums and verification with an
explicit tested channel and pinned publication date. Do not upload the existing Preview APK
as the public store version. Microsoft's [MAUI publishing guide](https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-10.0)
documents the `AndroidPackageFormats=aab` publish option. Sign through protected
credentials; package creation alone does not submit anything to Google Play.

`tools/Publish-PlayBundle.ps1` builds a signed AAB from committed app inputs,
pins the publication date and runtime channel, and records source revision and
SHA-256. Supply the protected upload keystore, alias and a temporary password
file; passwords are passed through MSBuild's `file:` support, not command-line
values. Build credentials and keystores must remain outside Git and public
artifacts. Example parameters use placeholders, not actual secrets:

```powershell
./tools/Publish-PlayBundle.ps1 -KeyStorePath <protected-upload-keystore> `
  -KeyAlias <upload-alias> -PasswordFile <temporary-password-file> `
  -ReleaseDate 2026-11-01 -Channel stable -OutputDirectory artifacts/google-play/internal
```

The Google Play testing track and the app's catalog channel are independent:
the first internal test uses the production catalog, keeping unchecked audition
references out of the candidate for later production promotion.

## First internal test: 1 November 2026

Google Play shows the track as **Active** and **0.1.43 Internal** as available
to internal testers. The existing tester list is selected; personal tester
addresses and enrollment links are maintained in Console, outside this public
repository. Until the app configuration and review are complete, Google displays
the temporary name `org.opensleepmusic.app (unreviewed)`.

- Application source: `a9860c7cbcf26af2af8047518669e1a99c8d8d58`.
- Bundle: `open-sleep-music-v0.1.43-play.aab`, 42,819,171 bytes.
- SHA-256: `944596de7df289fad38c49084d73dc65b3332519ae2475adab8bb4ed707be1ac`.
- Production catalog channel; pinned publication date `2026-11-01`.
- Package `org.opensleepmusic.app`, version code 43, minimum API 26, target API 36;
  ARM64 and x86-64.
- 309 Core tests passed in each catalog channel, both Windows Release builds
  passed, and the signed Android publish succeeded. The source commit's
  [Tests](https://github.com/jochenwezel/open-sleep-music/actions/runs/41252874206)
  and [Build and test](https://github.com/jochenwezel/open-sleep-music/actions/runs/41252874235)
  workflows passed.
- Bundletool validation and JAR signature verification passed. All 232 native
  libraries' ELF load-segment alignments are at least 16 KB. No catalog audio
  files are bundled; the German privacy asset matches the published source.
- Google accepted the bundle with two advisory warnings: no R8/ProGuard mapping
  file and no native debug symbols uploaded. These do not block the internal test;
  improve crash diagnostics before broader distribution where applicable.
- Automatic installation protection remains enabled as selected by the
  maintainer. No Play Integrity API integration was added.

This records distribution availability, not completed tester acceptance. Install
the Play-delivered build and perform the physical-device playback, child-mode,
offline and storage/backup checks before a wider release. The production launch
checklist above remains applicable. The English website notice was subsequently
published with explicit approval on 1 November 2026; the immutable 0.1.43 build
includes the approved German offline notice only.
