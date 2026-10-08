using System.Text.RegularExpressions;

namespace OpenSleepMusic.Core.Catalog;

/// <summary>Complete visual layers for one collection, independent of shared audio membership.</summary>
public sealed partial record CollectionArtwork(
    Uri ArtworkUri,
    string ArtworkFileName,
    string ArtworkSha256,
    Uri? SongMotifUri = null,
    string? SongMotifFileName = null,
    string? SongMotifSha256 = null,
    Uri? FallbackMotifUri = null,
    string? FallbackMotifFileName = null,
    string? FallbackMotifSha256 = null)
{
    public static AudioTrack ForCollection(AudioTrack track, string? worldId)
    {
        if (worldId is null || track.CollectionArtwork is null
            || !track.CollectionArtwork.TryGetValue(worldId, out var artwork)) return track;
        return track with
        {
            ArtworkUri = artwork.ArtworkUri,
            ArtworkFileName = artwork.ArtworkFileName,
            ArtworkSha256 = artwork.ArtworkSha256,
            SongMotifUri = artwork.SongMotifUri,
            SongMotifFileName = artwork.SongMotifFileName,
            SongMotifSha256 = artwork.SongMotifSha256,
            FallbackMotifUri = artwork.FallbackMotifUri,
            FallbackMotifFileName = artwork.FallbackMotifFileName,
            FallbackMotifSha256 = artwork.FallbackMotifSha256
        };
    }

    internal static void Validate(IReadOnlyDictionary<string, CollectionArtwork>? entries, string trackId)
    {
        if (entries is null) return;
        foreach (var (worldId, artwork) in entries)
        {
            if (!PortableId().IsMatch(worldId) || artwork is null
                || !ValidLayer(artwork.ArtworkUri, artwork.ArtworkFileName, artwork.ArtworkSha256, required: true)
                || !ValidLayer(artwork.SongMotifUri, artwork.SongMotifFileName, artwork.SongMotifSha256)
                || !ValidLayer(artwork.FallbackMotifUri, artwork.FallbackMotifFileName, artwork.FallbackMotifSha256))
                throw new InvalidDataException($"Invalid collection artwork for '{trackId}' in '{worldId}'.");
        }
    }

    private static bool ValidLayer(Uri? uri, string? fileName, string? sha256, bool required = false)
    {
        if (!required && uri is null && fileName is null && sha256 is null) return true;
        return uri is { IsAbsoluteUri: true, Scheme: "https" }
            && fileName is not null && PortablePngName().IsMatch(fileName)
            && sha256 is not null && Sha256().IsMatch(sha256);
    }

    [GeneratedRegex("^[a-z0-9][a-z0-9_-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex PortableId();
    [GeneratedRegex("^[a-z0-9_-]+\\.png$", RegexOptions.CultureInvariant)]
    private static partial Regex PortablePngName();
    [GeneratedRegex("^[0-9a-f]{64}$", RegexOptions.CultureInvariant)]
    private static partial Regex Sha256();
}
