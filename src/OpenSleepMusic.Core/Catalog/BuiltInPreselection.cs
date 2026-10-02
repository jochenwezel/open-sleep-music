using System.Text.Json;

namespace OpenSleepMusic.Core.Catalog;

public static class BuiltInPreselection
{
    public static IReadOnlyList<ReviewCandidate> Candidates { get; } = Load();

    public static void EnsureProductionTrackAllowed(AudioTrack track)
    {
        if (track.LicenseReviewStatus != "verified")
            throw new InvalidOperationException($"Track '{track.Id}' requires a recording-rights review before production admission.");
        foreach (var candidate in Candidates.Where(candidate =>
            candidate.Id == track.Id || candidate.SourcePageUri == track.SourcePageUri))
        {
            candidate.EnsurePromotionAllowed();
            if (track.License != candidate.ApprovedLicense || track.LicenseUri != candidate.ApprovedLicenseUri)
                throw new InvalidDataException($"Candidate '{candidate.Id}' does not retain its approved recording license.");
        }
    }

    private static IReadOnlyList<ReviewCandidate> Load()
    {
        if (!BuildChannel.IsPreview) return [];
        using var stream = typeof(BuiltInPreselection).Assembly
            .GetManifestResourceStream("OpenSleepMusic.Core.Catalog.preselection-candidates.json")
            ?? throw new InvalidOperationException("The preselection resource was not found.");
        var manifest = JsonSerializer.Deserialize(stream, CatalogJsonContext.Default.ReviewCandidateManifest)
            ?? throw new InvalidDataException("The preselection resource is invalid.");
        if (manifest.SchemaVersion != 1)
            throw new InvalidDataException("Unsupported preselection schema.");
        return manifest.Candidates;
    }
}
