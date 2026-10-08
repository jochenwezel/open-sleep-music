using System.Text.Json.Serialization;

namespace OpenSleepMusic.Core.Catalog;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CatalogManifest))]
[JsonSerializable(typeof(Playback.BackgroundPlaybackSource))]
[JsonSerializable(typeof(ArtworkManifest))]
[JsonSerializable(typeof(ReviewCandidateManifest))]
internal sealed partial class CatalogJsonContext : JsonSerializerContext;
