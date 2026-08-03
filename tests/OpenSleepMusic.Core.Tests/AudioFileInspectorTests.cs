using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class AudioFileInspectorTests
{
    [Theory]
    [InlineData("49-44-33-04-00-00", true)]
    [InlineData("4F-67-67-53-00-02", true)]
    [InlineData("66-4C-61-43-00-00", true)]
    [InlineData("FF-FB-90-64-00-00", true)]
    [InlineData("3C-21-44-4F-43-54-59-50-45", false)]
    [InlineData("3C-68-74-6D-6C-3E", false)]
    public void RecognizesAudioSignaturesAndRejectsHtml(string bytes, bool expected)
    {
        var header = bytes.Split('-').Select(value => Convert.ToByte(value, 16)).ToArray();

        Assert.Equal(expected, AudioFileInspector.IsSupportedAudio(header));
    }
}
