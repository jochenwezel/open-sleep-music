namespace OpenSleepMusic.Core.Downloads;

public static class AudioFileInspector
{
    public static bool IsSupportedAudio(ReadOnlySpan<byte> header)
    {
        if (header.Length < 4)
        {
            return false;
        }

        return header.StartsWith("ID3"u8)
            || header.StartsWith("OggS"u8)
            || header.StartsWith("fLaC"u8)
            || IsMpegFrame(header)
            || IsWave(header)
            || IsMp4Audio(header);
    }

    private static bool IsMpegFrame(ReadOnlySpan<byte> header) =>
        header.Length >= 2 && header[0] == 0xff && (header[1] & 0xe0) == 0xe0;

    private static bool IsWave(ReadOnlySpan<byte> header) =>
        header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WAVE"u8);

    private static bool IsMp4Audio(ReadOnlySpan<byte> header) =>
        header.Length >= 12 && header[4..8].SequenceEqual("ftyp"u8);
}
