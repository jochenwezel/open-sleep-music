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
    public void EveryBuiltInWorldHasItsOwnMotif(string worldId, string expectedAsset)
    {
        var theme = PlaybackVisualCatalog.For(worldId, "any-track");

        Assert.Equal(expectedAsset, theme.MotifAsset);
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
