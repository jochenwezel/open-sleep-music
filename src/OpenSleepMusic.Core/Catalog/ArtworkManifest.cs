namespace OpenSleepMusic.Core.Catalog;

internal sealed record ArtworkManifest(
    int SchemaVersion,
    IReadOnlyList<ArtworkManifestEntry> Tracks);

internal sealed record ArtworkManifestEntry(
    string TrackId,
    Uri ArtworkUri,
    string ArtworkFileName,
    string ArtworkSha256,
    Uri? SongMotifUri = null,
    string? SongMotifFileName = null,
    string? SongMotifSha256 = null);
