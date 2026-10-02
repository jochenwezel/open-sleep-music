namespace OpenSleepMusic.Core.Catalog;

public static class BuildChannel
{
    public static bool IsPreview =>
#if OPEN_SLEEP_MUSIC_PREVIEW
        true;
#else
        false;
#endif
}
