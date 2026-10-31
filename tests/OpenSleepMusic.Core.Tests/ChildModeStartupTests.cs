using OpenSleepMusic.Core.Privacy;

namespace OpenSleepMusic.Core.Tests;

public sealed class ChildModeStartupTests
{
    [Fact]
    public void OnlyAllowedDownloadedCollectionStartsAndBackOrResumeDoesNotRepeatIt()
    {
        var control = new ParentalControl(true, allowedCollections: ["lullabies", "piano"]);
        ChildCollectionAvailability[] collections = [new("lullabies", true, true), new("piano", false, false), new("nature", true, true)];
        var startup = new ChildModeStartup();
        Assert.Equal("lullabies", startup.TakeAutoStartCollection(control, collections));
        Assert.Null(startup.TakeAutoStartCollection(control, collections));
        Assert.Equal("lullabies", new ChildModeStartup().TakeAutoStartCollection(control, collections));
    }

    [Fact]
    public void SeveralVisibleCollectionsKeepTheOverviewEvenIfOnlyOneIsPlayable()
    {
        var control = new ParentalControl(true);
        Assert.Null(new ChildModeStartup().TakeAutoStartCollection(control,
            [new("lullabies", true, true), new("piano", true, false)]));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void MissingDownloadsOrAllBlockedTracksCannotAutoplay(bool downloaded, bool playable)
    {
        Assert.Null(new ChildModeStartup().TakeAutoStartCollection(new ParentalControl(true),
            [new("lullabies", downloaded, playable)]));
    }

    [Fact]
    public void NormalStartupDoesNotAutoplayAndLaterEnablingChildModeIsNotAFreshLaunch()
    {
        var control = new ParentalControl();
        var startup = new ChildModeStartup();
        ChildCollectionAvailability[] collections = [new("lullabies", true, true)];
        Assert.Null(startup.TakeAutoStartCollection(control, collections));
        control.Enable();
        Assert.Null(startup.TakeAutoStartCollection(control, collections));
    }
}
