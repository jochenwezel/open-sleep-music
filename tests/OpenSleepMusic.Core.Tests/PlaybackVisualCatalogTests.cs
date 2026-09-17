using OpenSleepMusic.App.Visuals;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackVisualCatalogTests
{
    [Theory]
    [InlineData("quiet-classics", "motif_quiet_classics.png")]
    [InlineData("rain", "motif_rain.png")]
    [InlineData("forest", "motif_forest.png")]
    [InlineData("waves", "motif_waves.png")]
    [InlineData("fireplace", "motif_fireplace_a.png")]
    [InlineData("lullabies", "sleepy_bear_moon.png")]
    public void EveryBuiltInWorldHasItsOwnMotif(string worldId, string expectedAsset)
    {
        var theme = PlaybackVisualCatalog.For(worldId, null);

        Assert.Equal(expectedAsset, theme.MotifAsset);
    }

    [Fact]
    public void UnknownWorldUsesAVisibleFallbackDifferentFromLullabies()
    {
        var lullabies = PlaybackVisualCatalog.For("lullabies", null);
        var fallback = PlaybackVisualCatalog.For("unknown", null);

        Assert.NotEqual(lullabies.MotifAsset, fallback.MotifAsset);
        Assert.Equal("sleepy_lamb_star.png", fallback.MotifAsset);
    }

    [Theory]
    [InlineData("faure-berceuse-op-56-no-1", "motif_lullaby_sleepy_child.png")]
    [InlineData("burgmuller-berceuse-op-109-no-7", "motif_lullaby_vine.png")]
    [InlineData("music-box-schlafe-mein-prinzchen", "sleepy_bear_moon.png")]
    [InlineData("music-box-guten-abend-gute-nacht", "sleepy_lamb_star.png")]
    [InlineData("antti-luode-another-lullaby", "motif_lullaby_boat.png")]
    public void LullabyTracksHaveIndividualMotifs(string trackId, string expectedAsset)
    {
        var theme = PlaybackVisualCatalog.For("lullabies", trackId);

        Assert.Equal(expectedAsset, theme.MotifAsset);
    }

    [Fact]
    public void UnknownLullabyTrackGetsStableMotif()
    {
        var first = PlaybackVisualCatalog.For("lullabies", "future-lullaby");
        var second = PlaybackVisualCatalog.For("lullabies", "future-lullaby");

        Assert.Equal(first.MotifAsset, second.MotifAsset);
    }

    [Fact]
    public void StarryClassicsUsesFifteenSecondColorPhases()
    {
        var theme = PlaybackVisualCatalog.For("quiet-classics", null);

        Assert.Equal(TimeSpan.FromSeconds(15), theme.ColorPhaseDuration);
        Assert.Equal("#101B45", theme.StartColor);
        Assert.Equal("#080E28", theme.EndColor);
    }

    [Fact]
    public void FireplaceUsesThreeSecondLowContrastFades()
    {
        var theme = PlaybackVisualCatalog.For("fireplace", null);

        Assert.Equal(TimeSpan.FromSeconds(3), theme.MotifFadeDuration);
        Assert.InRange(theme.MotifMinimumOpacity, 0.75, 0.88);
    }
}
