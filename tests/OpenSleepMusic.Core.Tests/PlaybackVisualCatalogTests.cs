using OpenSleepMusic.App.Visuals;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackVisualCatalogTests
{
    [Theory]
    [InlineData("quiet-classics")]
    [InlineData("rain")]
    [InlineData("forest")]
    [InlineData("waves")]
    [InlineData("fireplace")]
    [InlineData("lullabies")]
    public void EveryBuiltInWorldHasPackagedFallback(string worldId)
    {
        var theme = PlaybackVisualCatalog.For(worldId, null);

        Assert.Equal("sleepy_lamb_star.png", theme.MotifAsset);
    }

    [Fact]
    public void UnknownWorldUsesAVisibleFallbackDifferentFromLullabies()
    {
        var lullabies = PlaybackVisualCatalog.For("lullabies", null);
        var fallback = PlaybackVisualCatalog.For("unknown", null);

        Assert.Equal("sleepy_lamb_star.png", fallback.MotifAsset);
        Assert.Equal(lullabies.MotifAsset, fallback.MotifAsset);
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
