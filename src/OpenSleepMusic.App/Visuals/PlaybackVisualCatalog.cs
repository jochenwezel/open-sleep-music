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
        ["quiet-classics"] = new("#0A1742", "#183B78", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["rain"] = new("#081B38", "#164D78", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["forest"] = new("#08243A", "#15536A", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["waves"] = new("#071B42", "#155A86", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15)),
        ["fireplace"] = new("#211015", "#321716", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(3), 0.45),
        ["lullabies"] = new("#081638", "#243E82", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15))
    };

    public static PlaybackVisualTheme For(string? worldId, string? trackId)
    {
        var theme = worldId is not null && Themes.TryGetValue(worldId, out var selected)
            ? selected
            : new("#10172C", "#1B2140", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15));

        return theme;
    }
}
