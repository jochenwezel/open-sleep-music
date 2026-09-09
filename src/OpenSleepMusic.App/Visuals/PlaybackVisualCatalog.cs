namespace OpenSleepMusic.App.Visuals;

internal sealed record PlaybackVisualTheme(string StartColor, string EndColor, string MotifAsset);

internal static class PlaybackVisualCatalog
{
    private static readonly IReadOnlyDictionary<string, PlaybackVisualTheme> Themes = new Dictionary<string, PlaybackVisualTheme>
    {
        ["quiet-classics"] = new("#10132F", "#251844", "sleepy_bear_moon.png"),
        ["rain"] = new("#071C31", "#123652", "sleepy_bear_moon.png"),
        ["forest"] = new("#071F25", "#15352C", "sleepy_bear_moon.png"),
        ["waves"] = new("#061B38", "#123F55", "sleepy_bear_moon.png"),
        ["fireplace"] = new("#211015", "#3A1918", "sleepy_bear_moon.png")
    };

    public static PlaybackVisualTheme For(string? worldId, string? trackId)
    {
        var theme = worldId is not null && Themes.TryGetValue(worldId, out var selected)
            ? selected
            : new("#07142D", "#10103A", "sleepy_bear_moon.png");
        var motif = trackId is not null && trackId.Sum(character => character) % 2 == 0
            ? "sleepy_lamb_star.png"
            : "sleepy_bear_moon.png";
        return theme with { MotifAsset = motif };
    }
}
