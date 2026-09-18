namespace OpenSleepMusic.App.Visuals;

internal sealed record PlaybackVisualTheme(
    string StartColor,
    string EndColor,
    string MotifAsset,
    TimeSpan ColorPhaseDuration,
    TimeSpan? MotifFadeDuration = null,
    double MotifMinimumOpacity = 0.88);

internal static class PlaybackVisualCatalog
{
    private static readonly IReadOnlyDictionary<string, PlaybackVisualTheme> Themes = new Dictionary<string, PlaybackVisualTheme>
    {
        ["quiet-classics"] = new("#101B45", "#080E28", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["rain"] = new("#071C31", "#102D43", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["forest"] = new("#071F25", "#102C27", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["waves"] = new("#061B38", "#102F45", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["fireplace"] = new("#211015", "#321716", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(3), 0.75),
        ["lullabies"] = new("#07142D", "#10103A", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15))
    };

    public static PlaybackVisualTheme For(string? worldId, string? trackId)
    {
        var theme = worldId is not null && Themes.TryGetValue(worldId, out var selected)
            ? selected
            : new("#10172C", "#1B2140", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15));

        return theme;
    }
}
