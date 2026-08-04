using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Library;

public sealed record LocalLibraryTrack(AudioTrack Track, SleepWorld SleepWorld, string FilePath);
