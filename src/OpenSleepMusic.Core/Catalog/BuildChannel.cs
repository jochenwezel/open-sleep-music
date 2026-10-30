namespace OpenSleepMusic.Core.Catalog;

public static class BuildChannel
{
    public static bool IsPreviewOnlyWorld(string id) =>
        id is "preselection" or "pre-qualify" or "pre-qualify-licenses";

    public static bool IsReferenceWorld(string id) =>
        id is "pre-qualify" or "pre-qualify-licenses";

    public static bool IsPreview =>
#if OPEN_SLEEP_MUSIC_PREVIEW
        true;
#else
        false;
#endif
}
