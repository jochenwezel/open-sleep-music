using System.Text.Json.Serialization;

namespace OpenSleepMusic.App;

internal sealed record CatalogFeedbackReport(
    int SchemaVersion,
    DateTimeOffset GeneratedAtUtc,
    string AppVersion,
    string Platform,
    int CatalogTrackCount,
    IReadOnlyList<CatalogFeedbackEntry> Ratings,
    string? Comment);

internal sealed record CatalogFeedbackEntry(
    string WorldId,
    string WorldName,
    string TrackId,
    string TrackTitle,
    string State);

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
[JsonSerializable(typeof(CatalogFeedbackReport))]
internal sealed partial class FeedbackJsonContext : JsonSerializerContext;
