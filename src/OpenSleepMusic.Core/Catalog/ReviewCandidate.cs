namespace OpenSleepMusic.Core.Catalog;

public sealed record ReviewCandidate(
    string Id,
    string Title,
    string Creator,
    Uri SourcePageUri,
    IReadOnlyList<string> Instrumentation,
    string LicenseReviewStatus,
    string DeclaredLicense,
    Uri? LicenseEvidenceUri,
    string ReviewNotes,
    string? ApprovedLicense = null,
    Uri? ApprovedLicenseUri = null,
    string? IntendedWorldId = null,
    string? SelectionKind = null,
    Uri? DownloadUri = null,
    string? FileName = null,
    double? DurationSeconds = null,
    string? Sha1 = null,
    Uri? DeclaredLicenseUri = null,
    DateTimeOffset? DeliveryCheckedAtUtc = null,
    int StartOffsetMilliseconds = 0,
    int EndOffsetMilliseconds = 0,
    double VolumeGain = 1,
    double PlaybackSpeed = 1,
    IReadOnlyList<string>? IntendedWorldIds = null,
    IReadOnlyList<string>? PromotedWorldIds = null,
    string? LicenseHoldReason = null)
{
    public bool CanPromote => LicenseReviewStatus == "verified"
        && string.IsNullOrWhiteSpace(LicenseHoldReason)
        && LicenseEvidenceUri?.Scheme == Uri.UriSchemeHttps
        && !string.IsNullOrWhiteSpace(Creator)
        && !string.IsNullOrWhiteSpace(DeclaredLicense)
        && !string.IsNullOrWhiteSpace(ReviewNotes)
        && ApprovedLicense is "CC0 1.0" or "CC BY 3.0" or "CC BY 4.0"
            or "CC BY-SA 2.0" or "CC BY-SA 3.0" or "CC BY-SA 3.0 Germany" or "CC BY-SA 4.0" or "Public Domain" or "Public Domain Dedication"
            or "Free Art License 1.3" or "CC BY-NC 4.0"
        && ApprovedLicenseUri?.Scheme == Uri.UriSchemeHttps
        && (ApprovedLicense != "CC BY-SA 2.0"
            || ApprovedLicenseUri.AbsoluteUri == "https://creativecommons.org/licenses/by-sa/2.0/")
        && (ApprovedLicense != "CC BY-SA 3.0 Germany"
            || ApprovedLicenseUri.AbsoluteUri == "https://creativecommons.org/licenses/by-sa/3.0/de/");

    public void EnsurePromotionAllowed()
    {
        if (!CanPromote)
            throw new InvalidOperationException($"Candidate '{Id}' requires a documented recording-rights review before promotion.");
    }
}

public sealed record ReviewCandidateManifest(int SchemaVersion, IReadOnlyList<ReviewCandidate> Candidates);
