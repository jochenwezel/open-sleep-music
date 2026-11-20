using OpenSleepMusic.App.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackResumeTests
{
    [Theory]
    [InlineData("song", "world", false, 120, 42)]
    [InlineData("song", null, false, 120, 42)]
    [InlineData("song", "other-world", false, 120, 17)]
    [InlineData("other-song", "world", false, 120, 17)]
    [InlineData(null, null, false, 0, 17)]
    [InlineData("song", "world", true, 120, 17)]
    [InlineData("song", "world", false, 0, 17)]
    public void ResumeUsesAValidMatchingSnapshotOtherwiseTheRetainedPosition(
        string? id, string? world, bool preparing, double duration, double expected)
    {
        var snapshot = new PlaybackSnapshot(id, false, TimeSpan.FromSeconds(42),
            TimeSpan.FromSeconds(duration), WorldId: world, IsPreparing: preparing);
        Assert.Equal(expected, snapshot.ResumePositionFor("song", "world", 17));
    }
}
