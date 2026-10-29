# Export assessment for Play preparation

Reviewed on 29 October 2026 for the initial Play Console app record. This is a
technical preliminary assessment, not a government classification or a record
of an export licence, filing or legal approval.

## Inspected functions

- Primary purpose: downloading and playing a curated music/ambient catalog.
- `MainPage.xaml.cs` uses the ordinary .NET `HttpClient`; transport uses HTTPS.
- `AudioFileValidator`, `ArtworkCache`, `AudioLibraryMigrator` and motif cache
  code use SHA-1/SHA-256 for file integrity/cache identification.
- Package signing and the planned Google Play installation protection concern
  origin/integrity and software distribution.
- Feedback composes an email in the user's external email client; the app is
  not a general-purpose messaging, VPN, encryption or key-management product.
- No custom encryption implementation or general-purpose cryptographic feature
  exposed to app users was found in the inspected application/Core source.

The restored `project.assets.json` includes AndroidX SecurityCrypto and Google
Tink Android transitively through the framework. These are real cryptographic
dependencies; source inspection is not proof that they are absent from the
delivered app. Final trimming, actual use and the Google-modified Play package
must be considered when fixing the final classification. No key material was
opened or transmitted for this review.

## Applicable guidance and conclusion

BIS's [Cryptography for Data Confidentiality guidance](https://media.bis.gov/learn-support/encryption-controls/cryptography-for-data-confidentiality)
excludes authentication, digital signatures, data integrity, DRM and
encryption/decryption supporting entertainment from the relevant 5A002.a
definition. Its examples expressly include dedicated music players/organizers.
It calls for review of other applicable CCL entries before an EAR99 conclusion.
The [primary-function guidance](https://www.bis.gov/learn-support/encryption-controls/5a002-a.1-a.5)
also distinguishes application-specific functions from general-purpose
information-security, communications and computing products.

The inspected app functions support the entertainment rationale. This review
does not establish an unconditional ECCN/EAR99 classification for every future
binary or separately distributed library. Revisit the assessment for a change
to encryption functionality, general-purpose communications, packaging or use.
Free/open-source distribution alone is not the basis of the assessment.

Google's [export guidance](https://support.google.com/googleplay/android-developer/answer/113770?hl=en)
states that it blocks downloads in certain embargoed countries, while the
[Developer Distribution Agreement, section 16.5](https://play.google/developer-distribution-agreement.html)
retains developer compliance obligations. The account representative must
explicitly authorize the binding declarations in the creation form; this note
does not itself accept them or certify compliance with all sanctions rules.

## Release follow-up

- Fix the actual signed Stable AAB and inspect its resolved/runtime dependencies.
- Confirm the product-level classification against the delivered functionality;
  do not invent an ECCN, filing, certificate or approval.
- Review the intended distribution countries and any applicable restrictions.
- Record the representative's actual acceptance separately from technical review.
