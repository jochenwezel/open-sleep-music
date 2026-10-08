namespace OpenSleepMusic.Core.Playback;

public sealed record BackgroundPlaybackSource(string Path, double Volume,
    int StartOffsetMilliseconds, int EndOffsetMilliseconds, double DurationSeconds)
{
    public string ToJson() => System.Text.Json.JsonSerializer.Serialize(this, Catalog.CatalogJsonContext.Default.BackgroundPlaybackSource);
    public static BackgroundPlaybackSource? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            var source = System.Text.Json.JsonSerializer.Deserialize(json, Catalog.CatalogJsonContext.Default.BackgroundPlaybackSource);
            return source is not null && !string.IsNullOrWhiteSpace(source.Path)
                && double.IsFinite(source.Volume) && source.Volume is >= 0 and <= 1
                && double.IsFinite(source.DurationSeconds) && source.DurationSeconds > 0
                && source.StartOffsetMilliseconds >= 0 && source.EndOffsetMilliseconds >= 0
                && (long)source.StartOffsetMilliseconds + source.EndOffsetMilliseconds < source.DurationSeconds * 1000
                    ? source : null;
        }
        catch (System.Text.Json.JsonException) { return null; }
    }
    public double LoopPosition(double mainPositionSeconds) => Math.Max(0, mainPositionSeconds) %
        Math.Max(0.001, DurationSeconds - (StartOffsetMilliseconds + (double)EndOffsetMilliseconds) / 1000);
}
