using System.Text.Json.Serialization;

namespace OpenSleepMusic.Core.Catalog;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CatalogManifest))]
internal sealed partial class CatalogJsonContext : JsonSerializerContext;
