# Google Play publication preparation

Open Sleep Music is being prepared for publication by **CompuMaster** using an
organization account. Account verification is pending, as reported by the
maintainer on 29 October 2026. No Play Console submission has been made by this
preparation work.

Tracking issue: [Google Play publication under CompuMaster, issue 22](https://github.com/jochenwezel/open-sleep-music/issues/22).

## Android distribution decision

As directed by the maintainer on 29 October 2026, official Android Stable
releases are distributed through Google Play. GitHub APKs remain available for
development and Preview testing, not as an official Stable distribution channel.
The GitHub release workflow runs its APK publication job only for prereleases;
Windows distribution is unaffected. Existing historical release assets remain
unchanged.

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
release. Qualify offline claims in the full store description and app help;
document the relevant Play processing in the app privacy notice. Google does
not publish a fixed maximum offline interval:
[automatic protection](https://support.google.com/googleplay/android-developer/answer/10183279?hl=en).

## Publication materials

- [German store listing](store-listing.de.json): copyable text, without fixed
  catalog totals or promises of medical benefits.
- [German app privacy notice draft](privacy-notice.de.md): app-specific text for
  review and publication on the company website.
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

Proposed destination, not yet published:
`https://www.compumaster.de/datenschutzhinweis/apps/open-sleep-music/`.
Use one stable URL for this app, with clearly labeled Android/Google Play and
iOS/App Store sections when platform processing differs. Separate URLs are an
option if the practices diverge substantially, not the default. The current
worksheet inspects Android; iOS backup, dependencies and actual data flows still
need their own review before making an iOS declaration. Each store's disclosure
form must match its distributed app even when both reference the same notice.
Apple also requires the privacy-policy link in its listing and inside the app:
[App Review Guideline 5.1.1](https://developer.apple.com/app-store/review/guidelines/#privacy).
The app currently has no privacy-notice entry. Add the final reviewed notice or
its published URL to the app before submission. Google requires a publicly
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
- `.github/workflows/publish-release.yml` publishes signed development APKs for
  prereleases and Windows ZIPs,
  but does not yet produce a Google Play Android App Bundle.
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

- [ ] Verify the [privacy commitments](../privacy-policy.md), including HTTPS
  redirects/certificate failures on supported platforms and the real operator
  process for deleting received feedback before publishing that promise.
- [ ] Organization verification completed; public legal identity and contacts checked.
- [ ] Verify `appsupport@compumaster.de` is provisioned, reachable and assigned to
  the people handling app support/feedback; implement the actual deletion process.
- [ ] Final app privacy notice reviewed, published and accessible inside the app.
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
- [ ] Build a signed Stable `.aab` and SHA-256 from a tested release commit.
  Keep GitHub APKs for development/Preview only; no official Stable APK assets.
- [ ] Configure Play App Signing deliberately. A new Google-managed app-signing
  identity is acceptable; migration from GitHub development APKs is not required.
  Separate upload and app-signing keys where appropriate; never put private keys
  in issues, repository files or public artifacts. See [Android signing](https://developer.android.com/studio/publish/app-signing)
  and [existing release signing](../android-signing.md).
- [ ] Verify the chosen Play automatic installation protection setting; test the
  delivered build and reflect relevant data flows and online checks in disclosures.
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

The existing release workflow explicitly selects APK output. A future focused
change should add signed AAB output, checksums and verification while preserving
`release.prerelease` as the channel source. Do not upload the existing Preview APK
as the public store version. Microsoft's [MAUI publishing guide](https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-10.0)
documents the `AndroidPackageFormats=aab` publish option. Sign through protected
credentials; package creation alone does not submit anything to Google Play.
