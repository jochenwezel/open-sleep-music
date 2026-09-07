# Catalog feedback during field testing

Open Sleep Music does not transmit telemetry automatically. A tester can explicitly choose **Katalog-Feedback teilen** from the main menu, optionally add a comment, review a summary, and select a destination in the operating system's share dialog.

The JSON report contains:

- schema version and UTC generation time;
- application version and platform name;
- catalog track count;
- only tracks deliberately marked as `favorite` or `blocked`;
- an optional free-text comment.

It does not contain a device identifier, account, playback history, local file path, or download URL. The selected share destination may apply its own privacy policy; the app shows the report scope before opening that destination.

## Receiving test reports

During the preview phase, testers should send the generated `.json` file through the agreed private channel. Store received reports outside the repository because comments may contain personal information. Never commit raw field reports.

For a quick aggregate, group `ratings` by `trackId` and `state`. Repeated reports are not linked to an installation and therefore must not be interpreted as unique-user counts. A block is a strong signal for manual listening review, not an automatic deletion instruction. Verify the recording, transition behavior, and source before changing the catalog generator.

If a central endpoint is introduced later, retain explicit consent and the same minimal schema. Do not add stable installation identifiers or background submission merely for deduplication.
