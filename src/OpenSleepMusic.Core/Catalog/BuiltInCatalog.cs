namespace OpenSleepMusic.Core.Catalog;

public static class BuiltInCatalog
{
    private const string CommonsDownload = "https://commons.wikimedia.org/wiki/Special:Redirect/file/";

    public static IReadOnlyList<SleepWorld> SleepWorlds { get; } =
    [
        new(
            "quiet-classics",
            "Ruhige Klassik",
            "Sanfte Klaviermusik zum Einschlafen.",
            "🎹",
            [
                Track(
                    "chopin-nocturne-op9-2",
                    "Nocturne Op. 9 Nr. 2",
                    "Frédéric Chopin; Aufnahme: Musopen",
                    "Nocturne Op. 9 no. 2 in E flat major.mp3",
                    "https://commons.wikimedia.org/wiki/File:Nocturne_Op._9_no._2_in_E_flat_major.mp3",
                    "CC0 1.0",
                    "https://creativecommons.org/publicdomain/zero/1.0/",
                    "chopin-nocturne-op9-no2.mp3"),
                Track(
                    "satie-gymnopedie-1",
                    "Gymnopédie Nr. 1",
                    "Erik Satie; Aufnahme: Kevin MacLeod",
                    "Gymnopedie No. 1 (ISRC USUAN1100787).mp3",
                    "https://commons.wikimedia.org/wiki/File:Gymnopedie_No._1_(ISRC_USUAN1100787).mp3",
                    "CC BY 3.0",
                    "https://creativecommons.org/licenses/by/3.0/",
                    "satie-gymnopedie-no1.mp3"),
                Track(
                    "debussy-clair-de-lune",
                    "Clair de Lune",
                    "Claude Debussy; Aufnahme: Laurens Goedhart",
                    "Clair de lune (Claude Debussy) Suite bergamasque.ogg",
                    "https://commons.wikimedia.org/wiki/File:Clair_de_lune_(Claude_Debussy)_Suite_bergamasque.ogg",
                    "CC BY 3.0",
                    "https://creativecommons.org/licenses/by/3.0/",
                    "debussy-clair-de-lune.ogg")
            ]),
        new(
            "rain",
            "Sanfter Regen",
            "Gleichmäßiger Regen als ruhige Geräuschkulisse.",
            "🌧️",
            [
                Track(
                    "rain-field-recording",
                    "Rain",
                    "ジダネ",
                    "Rain.ogg",
                    "https://commons.wikimedia.org/wiki/File:Rain.ogg",
                    "Public Domain",
                    "https://creativecommons.org/publicdomain/mark/1.0/",
                    "rain.ogg")
            ]),
        new(
            "waves",
            "Wasser & Wellen",
            "Ruhige Wassergeräusche vom Ufer.",
            "🌊",
            [
                Track(
                    "lake-ontario-waves",
                    "Waves",
                    "Dsw4",
                    "Waves.ogg",
                    "https://commons.wikimedia.org/wiki/File:Waves.ogg",
                    "Public Domain",
                    "https://creativecommons.org/publicdomain/mark/1.0/",
                    "waves.ogg")
            ]),
        new(
            "forest",
            "Wald",
            "Leise Waldatmosphäre mit Wind, Insekten und Vögeln.",
            "🌲",
            [
                Track(
                    "forest-ambience",
                    "Forest ambience",
                    "nille",
                    "20090610 0 ambience.ogg",
                    "https://commons.wikimedia.org/wiki/File:20090610_0_ambience.ogg",
                    "Public Domain",
                    "https://creativecommons.org/publicdomain/mark/1.0/",
                    "forest-ambience.ogg")
            ]),
        new(
            "brown-noise",
            "Braunes Rauschen",
            "Tiefes, gleichmäßiges Rauschen ohne plötzliche Spitzen.",
            "🟤",
            [
                Track(
                    "brown-noise",
                    "Brownian noise",
                    "Kieff / LucasVB",
                    "Brownnoise.ogg",
                    "https://commons.wikimedia.org/wiki/File:Brownnoise.ogg",
                    "Public Domain (nicht schutzfähig)",
                    "https://creativecommons.org/publicdomain/mark/1.0/",
                    "brown-noise.ogg")
            ])
    ];

    private static AudioTrack Track(
        string id,
        string title,
        string creator,
        string commonsFileName,
        string sourcePage,
        string license,
        string licenseUrl,
        string fileName) =>
        new(
            id,
            title,
            creator,
            new Uri(CommonsDownload + Uri.EscapeDataString(commonsFileName)),
            new Uri(sourcePage),
            license,
            new Uri(licenseUrl),
            fileName);
}
