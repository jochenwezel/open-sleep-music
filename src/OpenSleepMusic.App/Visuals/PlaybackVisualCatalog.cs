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
    private static readonly IReadOnlyDictionary<string, string> LullabyMotifs = new Dictionary<string, string>
    {
        ["faure-berceuse-op-56-no-1"] = "motif_lullaby_sleepy_child.png",
        ["burgmuller-berceuse-op-109-no-7"] = "motif_lullaby_vine.png",
        ["music-box-schlafe-mein-prinzchen"] = "sleepy_bear_moon.png",
        ["music-box-guten-abend-gute-nacht"] = "sleepy_lamb_star.png",
        ["antti-luode-another-lullaby"] = "motif_lullaby_boat.png"
    };

    private static readonly IReadOnlyDictionary<string, PlaybackVisualTheme> Themes = new Dictionary<string, PlaybackVisualTheme>
    {
        ["quiet-classics"] = new("#101B45", "#080E28", "motif_quiet_classics.png", TimeSpan.FromSeconds(15)),
        ["rain"] = new("#071C31", "#102D43", "motif_rain.png", TimeSpan.FromSeconds(15)),
        ["forest"] = new("#071F25", "#102C27", "motif_forest.png", TimeSpan.FromSeconds(15)),
        ["waves"] = new("#061B38", "#102F45", "motif_waves.png", TimeSpan.FromSeconds(15)),
        ["fireplace"] = new("#211015", "#321716", "motif_fireplace_a.png", TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(3), 0.80),
        ["lullabies"] = new("#07142D", "#10103A", "sleepy_bear_moon.png", TimeSpan.FromSeconds(15))
    };

    public static PlaybackVisualTheme For(string? worldId, string? trackId)
    {
        var theme = worldId is not null && Themes.TryGetValue(worldId, out var selected)
            ? selected
            : new("#10172C", "#1B2140", "sleepy_lamb_star.png", TimeSpan.FromSeconds(15));

        if (worldId == "lullabies" && trackId is not null)
        {
            var motif = LullabyMotifs.TryGetValue(trackId, out var selectedMotif)
                ? selectedMotif
                : LullabyMotifs.Values.ElementAt(StableIndex(trackId, LullabyMotifs.Count));
            return theme with { MotifAsset = motif };
        }

        return theme;
    }

    private static int StableIndex(string value, int count)
    {
        uint hash = 2166136261;
        foreach (var character in value)
        {
            hash ^= character;
            hash *= 16777619;
        }

        return (int)(hash % count);
    }
}
