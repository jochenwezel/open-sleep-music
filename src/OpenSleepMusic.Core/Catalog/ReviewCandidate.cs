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
    int EndOffsetMilliseconds = 0)
{
    public bool CanPromote => LicenseReviewStatus == "verified"
        && LicenseEvidenceUri?.Scheme == Uri.UriSchemeHttps
        && !string.IsNullOrWhiteSpace(Creator)
        && !string.IsNullOrWhiteSpace(DeclaredLicense)
        && !string.IsNullOrWhiteSpace(ReviewNotes)
        && ApprovedLicense is "CC0 1.0" or "CC BY 3.0" or "CC BY 4.0"
            or "CC BY-SA 3.0" or "CC BY-SA 4.0" or "Public Domain" or "Public Domain Dedication"
        && ApprovedLicenseUri?.Scheme == Uri.UriSchemeHttps;

    public void EnsurePromotionAllowed()
    {
        if (!CanPromote)
            throw new InvalidOperationException($"Candidate '{Id}' requires a documented recording-rights review before promotion.");
    }
}

public sealed record ReviewCandidateManifest(int SchemaVersion, IReadOnlyList<ReviewCandidate> Candidates);
