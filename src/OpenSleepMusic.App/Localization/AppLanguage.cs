using System.Globalization;

namespace OpenSleepMusic.App.Localization;

internal enum AppLanguage { System, German, English }

internal static class AppText
{
    private static readonly IReadOnlyDictionary<string, string> De = new Dictionary<string, string>
    {
        ["Collections"]="Themensammlungen", ["Subtitle"]="Kostenlose Klänge für eine ruhige Nacht", ["NoTrack"]="Noch kein Titel ausgewählt",
        ["Download"]="Herunterladen", ["Play"]="Abspielen", ["Back"]="Zurück", ["Favorite"]="Favorit", ["Block"]="Blockieren",
        ["Unblock"]="Blockierung aufheben", ["Volume"]="Lautstärke", ["AppVolume"]="App-Lautstärke", ["SystemVolume"]="Systemlautstärke",
        ["SleepTimer"]="Schlaf-Timer", ["Repeat"]="Wiederholen", ["Settings"]="Einstellungen", ["Language"]="Sprache",
        ["SystemLanguage"]="Systemsprache", ["German"]="Deutsch", ["English"]="Englisch", ["ReducedMotion"]="Bewegung reduzieren",
        ["Close"]="Schließen", ["Off"]="Aus", ["Track"]="Einzeltitel", ["Collection"]="Themensammlung", ["TrackInfo"]="Titelinformationen",
        ["Source"]="Quellseite öffnen", ["License"]="Lizenz öffnen", ["Creator"]="Urheber/Interpret", ["Duration"]="Dauer", ["LocalFile"]="Lokale Datei"
    };
    private static readonly IReadOnlyDictionary<string, string> En = new Dictionary<string, string>
    {
        ["Collections"]="Theme collections", ["Subtitle"]="Free sounds for a peaceful night", ["NoTrack"]="No track selected",
        ["Download"]="Download", ["Play"]="Play", ["Back"]="Back", ["Favorite"]="Favorite", ["Block"]="Block", ["Unblock"]="Unblock",
        ["Volume"]="Volume", ["AppVolume"]="App volume", ["SystemVolume"]="System volume", ["SleepTimer"]="Sleep timer", ["Repeat"]="Repeat",
        ["Settings"]="Settings", ["Language"]="Language", ["SystemLanguage"]="System language", ["German"]="German", ["English"]="English",
        ["ReducedMotion"]="Reduce motion", ["Close"]="Close", ["Off"]="Off", ["Track"]="Single track", ["Collection"]="Theme collection",
        ["TrackInfo"]="Track information", ["Source"]="Open source page", ["License"]="Open license", ["Creator"]="Creator/performer",
        ["Duration"]="Duration", ["LocalFile"]="Local file"
    };

    public static AppLanguage SelectedLanguage { get; private set; } = AppLanguage.System;
    public static bool IsGerman { get; private set; }

    public static void Apply(AppLanguage language)
    {
        SelectedLanguage = language;
        IsGerman = language == AppLanguage.German || language == AppLanguage.System && CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de";
        var culture = CultureInfo.GetCultureInfo(IsGerman ? "de" : "en");
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
    }

    public static string Get(string key) => (IsGerman ? De : En).TryGetValue(key, out var value) ? value : key;
    public static string Pick(string german, string english) => IsGerman ? german : english;

    public static string WorldName(string id, string fallback) => IsGerman ? fallback : id switch
    {
        "quiet-classics" => "Quiet classics", "rain" => "Gentle rain", "forest" => "Forest",
        "waves" => "Water & waves", "fireplace" => "Fireplace", _ => fallback
    };

    public static string WorldDescription(string id, string fallback) => IsGerman ? fallback : id switch
    {
        "quiet-classics" => "Nocturnes, mazurkas and gentle classical miniatures.",
        "rain" => "Light to steady rain without selected thunder peaks.",
        "forest" => "Long forest and rainforest recordings with wind, water and birds.",
        "waves" => "Calm ocean, shore, waves and a babbling brook.",
        "fireplace" => "Quiet crackling of a fireplace.", _ => fallback
    };
}
