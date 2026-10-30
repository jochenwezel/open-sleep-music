# Catalog feedback during field testing

Open Sleep Music does not transmit telemetry automatically. A tester can explicitly
choose **Katalog-Feedback teilen** from the main menu, optionally add a comment,
and review a summary that names **appsupport@compumaster.de**. After confirmation,
the app opens an email draft addressed to that mailbox with the JSON report
attached. The user reviews and sends it in the mail client. Returning from the
composer is not proof of delivery. Without a supported mail client, the app
shows the support address and does not fall back to an unrestricted share sheet.

The JSON report contains:

- schema version and UTC generation time;
- application version and platform name;
- catalog track count;
- only tracks deliberately marked as `favorite` or `blocked`;
- an optional free-text comment.

It does not contain a device identifier, account, playback history, local file
path, or download URL. The user's email provider may add sender information and
process the message under its own privacy policy. The app shows the report scope
before opening the email draft; it cannot control later edits in the mail client.
Reports are written below the app's `sharing-root` cache directory; Android's
FileProvider exposes only that cache subtree for attachments.

## Receiving test reports

App-related support email and feedback addressed to CompuMaster must be processed
through **appsupport@compumaster.de**, including reports from preview testing.
The app prepopulates that address in the email composer. It does not automatically
send the report. General company and data protection contacts remain
separate. Verify mailbox operation and the actual deletion process before launch.
Store received reports outside the repository because comments may contain
personal information. Never commit raw field reports.

For a quick aggregate, group `ratings` by `trackId` and `state`. Repeated reports are not linked to an installation and therefore must not be interpreted as unique-user counts. A block is a strong signal for manual listening review, not an automatic deletion instruction. Verify the recording, transition behavior, and source before changing the catalog generator.

If a central endpoint is introduced later, retain explicit consent and the same minimal schema. Do not add stable installation identifiers or background submission merely for deduplication.

## Preselection ratings

Downloadable Preview auditions use world ID `preselection`. External references
retain their actual list ID (`pre-qualify` for further selection/technical work,
`pre-qualify-licenses` for missing or conflicting recording permission) in feedback.
Both reference lists store personal choices locally under the existing Preselection
rating keys, so moving a source between lists does not lose its favorite/block.
`catalogTrackCount` counts unique downloadable recordings, not external references.

When a recording moves into a production collection, existing local Preselection
choices copy once into each new membership. Existing destination choices, including
explicit false choices, take precedence; source choices are retained. This is
local preference migration, with no automatic feedback transmission or identifier.
Favorites never approve recording rights. Raw reports/comments remain outside Git.
