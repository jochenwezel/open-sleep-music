using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Tests;

public sealed class PreselectionTests
{
    [Fact]
    public void RenamingAnUncheckedCandidateDoesNotBypassProductionAdmission()
    {
        if (!BuildChannel.IsPreview) return;
        var candidate = BuiltInPreselection.Candidates[0];
        var track = new AudioTrack("renamed-for-production", candidate.Title, candidate.Creator,
            new Uri("https://example.org/audio.mp3"), candidate.SourcePageUri, "CC0 1.0",
            new Uri("https://creativecommons.org/publicdomain/zero/1.0/"), "renamed.mp3", 120);
        Assert.Throws<InvalidOperationException>(() => BuiltInPreselection.EnsureProductionTrackAllowed(track));
    }

    [Fact]
    public void ResearchCandidatesHaveSourcesTagsAndExplicitReviewStates()
    {
        var candidates = BuiltInPreselection.Candidates;
        Assert.Equal(BuildChannel.IsPreview ? 75 : 0, candidates.Count);
        Assert.Equal(candidates.Count, candidates.Select(candidate => candidate.Id).Distinct().Count());
        Assert.Equal(candidates.Count, candidates.Select(candidate => candidate.SourcePageUri).Distinct().Count());
        var downloadableTracks = BuiltInCatalog.SleepWorlds.Where(world => world.Id != "preselection").SelectMany(world => world.Tracks).ToArray();
        Assert.All(candidates, candidate =>
        {
            Assert.Equal("https", candidate.SourcePageUri.Scheme);
            Assert.NotEmpty(candidate.Title);
            Assert.NotEmpty(candidate.Instrumentation);
            Assert.Contains(candidate.LicenseReviewStatus, new[] { "unchecked", "verified", "rejected" });
            Assert.DoesNotContain(downloadableTracks, track => track.Id == candidate.Id);
            if (!candidate.CanPromote)
                Assert.Throws<InvalidOperationException>(candidate.EnsurePromotionAllowed);
        });
    }

    [Fact]
    public void PromotionRequiresRecordingRightsEvidenceEvenWithVerifiedStatus()
    {
        var candidate = new ReviewCandidate("test-candidate", "Test recording", "Performer",
            new Uri("https://example.org/recording"), ["piano"], "unchecked", "CC BY 4.0", null, "Pending review.");
        Assert.False(candidate.CanPromote);
        Assert.False((candidate with { LicenseReviewStatus = "verified" }).CanPromote);
        var reviewed = candidate with
        {
            LicenseReviewStatus = "verified",
            Creator = "Verified performer",
            DeclaredLicense = "CC BY 4.0",
            LicenseEvidenceUri = new Uri("https://example.org/recording-rights"),
            ApprovedLicense = "CC BY 4.0",
            ApprovedLicenseUri = new Uri("https://creativecommons.org/licenses/by/4.0/"),
            ReviewNotes = "Specific recording grant checked with the rights holder."
        };
        reviewed.EnsurePromotionAllowed();
        Assert.False((reviewed with { LicenseReviewStatus = "rejected" }).CanPromote);
        Assert.False((reviewed with { ApprovedLicense = "CC BY-NC 4.0" }).CanPromote);
        Assert.False((reviewed with { LicenseEvidenceUri = new Uri("http://example.org/rights") }).CanPromote);
    }

    [Fact]
    public void PreviewCandidatesUseTheOrdinaryOfflineDownloadCatalog()
    {
        var world = BuiltInCatalog.SleepWorlds.SingleOrDefault(world => world.Id == "preselection");
        if (!BuildChannel.IsPreview)
        {
            Assert.Null(world);
            return;
        }
        Assert.NotNull(world);
        Assert.Equal(55, world.Tracks.Count);
        Assert.All(world.Tracks, track =>
        {
            var candidate = Assert.Single(BuiltInPreselection.Candidates, candidate => candidate.Id == track.Id);
            Assert.Equal(candidate.DownloadUri, track.DownloadUri);
            Assert.Equal(candidate.FileName, track.FileName);
            Assert.Equal(candidate.LicenseReviewStatus, track.LicenseReviewStatus);
            Assert.NotNull(candidate.DeliveryCheckedAtUtc);
            Assert.Equal("https", track.DownloadUri.Scheme);
            Assert.True(track.DurationSeconds > 0);
            if (candidate.CanPromote)
                BuiltInPreselection.EnsureProductionTrackAllowed(track);
            else
                Assert.Throws<InvalidOperationException>(() => BuiltInPreselection.EnsureProductionTrackAllowed(track));
        });
        Assert.Equal(20, BuiltInPreselection.Candidates.Count(candidate => candidate.DownloadUri is null));
    }

    [Fact]
    public void GuitarReviewRetainsTheDistinctionBetweenDeclarationAndRecordingGrant()
    {
        if (!BuildChannel.IsPreview) return;
        var romanza = Assert.Single(BuiltInPreselection.Candidates, candidate => candidate.Id == "candidate-romanza-espanola");
        Assert.Equal("unchecked", romanza.LicenseReviewStatus);
        Assert.False(romanza.CanPromote);
        var recuerdos = Assert.Single(BuiltInPreselection.Candidates, candidate => candidate.Id == "candidate-recuerdos-de-la-alhambra");
        Assert.Equal("verified", recuerdos.LicenseReviewStatus);
        Assert.Equal("CC BY-SA 3.0", recuerdos.ApprovedLicense);
        Assert.Contains("Carlo Alberto Boni", recuerdos.Creator);
        Assert.True(recuerdos.CanPromote);
        Assert.Equal(5, BuiltInPreselection.Candidates.Count(candidate => candidate.SelectionKind == "gentle-guitar"));
    }

    [Fact]
    public void LullabyExpansionStaysInResearchUntilRightsAreReviewed()
    {
        var candidates = BuiltInPreselection.Candidates.Where(candidate => candidate.IntendedWorldId == "lullabies").ToArray();
        Assert.Equal(BuildChannel.IsPreview ? 20 : 0, candidates.Length);
        Assert.All(candidates, candidate =>
        {
            Assert.Equal("unchecked", candidate.LicenseReviewStatus);
            Assert.False(candidate.CanPromote);
            Assert.Contains(candidate.SelectionKind, new[] { "traditional-lullaby", "modern-lullaby", "gentle-arrangement" });
        });
        Assert.Equal(4, BuiltInCatalog.SleepWorlds.Single(world => world.Id == "lullabies").Tracks.Count);
    }

    [Fact]
    public void CandidateResourceIsOnlyPackagedInPreviewBuilds()
    {
        var assembly = typeof(BuiltInCatalog).Assembly;
        var names = assembly.GetManifestResourceNames();
        Assert.Equal(BuildChannel.IsPreview, names.Contains("OpenSleepMusic.Core.Catalog.preselection-candidates.json"));
        using var stream = assembly.GetManifestResourceStream("OpenSleepMusic.Core.Catalog.media-catalog.json");
        Assert.NotNull(stream);
        using var json = System.Text.Json.JsonDocument.Parse(stream!);
        var worlds = json.RootElement.GetProperty("sleepWorlds").EnumerateArray();
        Assert.Equal(BuildChannel.IsPreview, worlds.Any(world => world.GetProperty("id").GetString() == "preselection"));
    }
}
