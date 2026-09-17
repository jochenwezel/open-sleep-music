using System.Text.Json.Serialization;

namespace OpenSleepMusic.Core.Catalog;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CatalogManifest))]
[JsonSerializable(typeof(ArtworkManifest))]
internal sealed partial class CatalogJsonContext : JsonSerializerContext;
