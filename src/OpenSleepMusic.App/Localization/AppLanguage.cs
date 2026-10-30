using System.Globalization;

namespace OpenSleepMusic.App.Localization;

internal enum AppLanguage { System, German, English }

internal static class AppText
{
    // Capture the OS language before an explicit app selection changes CurrentUICulture.
    private static readonly CultureInfo SystemCulture = CultureInfo.CurrentUICulture;
    private static readonly IReadOnlyDictionary<string, string> De = new Dictionary<string, string>
    {
        ["Collections"]="Themensammlungen", ["Subtitle"]="Kostenlose Klänge für eine ruhige Nacht", ["NoTrack"]="Noch kein Titel ausgewählt",
        ["Download"]="Herunterladen", ["Play"]="Abspielen", ["Back"]="Zurück", ["Favorite"]="Favorit", ["Block"]="Blockieren",
        ["Unblock"]="Blockierung aufheben", ["Volume"]="Lautstärke", ["AppVolume"]="App-Lautstärke", ["SystemVolume"]="Systemlautstärke",
        ["SleepTimer"]="Schlaf-Timer", ["Repeat"]="Wiederholen", ["Settings"]="Einstellungen", ["Language"]="Sprache",
        ["SystemLanguage"]="Systemsprache", ["German"]="Deutsch", ["English"]="Englisch", ["ReducedMotion"]="Bewegung reduzieren",
        ["Close"]="Schließen", ["Off"]="Aus", ["Track"]="Einzeltitel", ["Collection"]="Themensammlung", ["TrackInfo"]="Titelinformationen",
        ["Source"]="Quellseite öffnen", ["License"]="Lizenz öffnen", ["Creator"]="Urheber/Interpret", ["Duration"]="Dauer", ["LocalFile"]="Lokale Datei",
        ["MyMusic"] = "Meine Musik",
        ["EmptyLibrary"] = "Für diese Themensammlung ist noch keine Musik heruntergeladen.",
        ["SelectTrack"] = "Themensammlung auswählen oder einen Titel anklicken.",
        ["NextTrackEmpty"] = "Nächster Titel: –",
        ["NextTrack"] = "Nächster Titel: {0}",
        ["Ready"] = "Bereit",
        ["PlaybackSettings"] = "Wiedergabe-Einstellungen",
        ["TimerHelp"] = "Die gewählte Dauer beginnt beim Start einer Wiedergabe. Eine Änderung während der Wiedergabe startet den Timer neu.",
        ["VolumeHelp"] = "Die Hardwaretasten des Handys regeln zusätzlich die Android-Medienlautstärke.",
        ["About"] = "Über Open Sleep Music",
        ["AboutIntro"] = "Kostenlose Klänge für eine ruhige Nacht – ohne Werbung, Tracking, Benutzerkonto oder Cloud-Zwang.",
        ["AboutOffline"] = "Nach dem Herunterladen funktioniert die Musikbibliothek offline. Nicht mehr erreichbare oder ungültige Quelldateien werden übersprungen, ohne eine ganze Sammlung zu blockieren.",
        ["AboutLicenses"] = "Die App steht unter der MIT-Lizenz. Jede Aufnahme behält ihre eigene, in den Titelinformationen ausgewiesene Lizenz. Die Angaben dokumentieren die Lizenz und Herkunft der jeweiligen Quelle; sie sind keine unabhängige Rechtsgarantie des Projekts.",
        ["AboutBackground"] = "Hintergrundwiedergabe und System-Mediensteuerung werden auf Mobilgeräten unterstützt.",
        ["Repository"] = "Projekt auf GitHub öffnen",
        ["TrackInfoSubtitle"] = "Quelle, Lizenz und lokale Datei",
        ["Instrumentation"] = "Instrumentierung",
        ["LicenseName"] = "Lizenz",
        ["LicenseHelp"] = "Die Lizenzangabe stammt von der dokumentierten Quellseite der konkreten Aufnahme.",
        ["AddFavorite"] = "☆ Favorisieren",
        ["BlockTrack"] = "Titel blockieren",
        ["CloseTrackList"] = "Titelliste schließen",
        ["Menu"] = "Menü",
        ["PreviousTrack"] = "Vorheriger Titel",
        ["NextTrackAction"] = "Nächster Titel",
        ["PlayPause"] = "Wiedergabe / Pause",
        ["Pause"] = "Pausieren",
        ["ManageCollection"] = "Sammlung verwalten",
        ["FeedbackTitle"] = "Katalog-Feedback",
        ["FeedbackHelp"] = "Optional kannst du ergänzen, warum ein Titel besonders gut oder ungeeignet ist. Im nächsten Schritt siehst du vor dem Teilen eine Zusammenfassung.",
        ["Continue"] = "Weiter",
        ["Cancel"] = "Abbrechen",
        ["OptionalComment"] = "Optionaler Kommentar",
        ["NoFeedback"] = "Kein Feedback vorhanden",
        ["NoFeedbackHelp"] = "Es wurden noch keine Favoriten oder Blockierungen gesetzt und kein Kommentar eingegeben.",
        ["PrepareEmail"] = "Feedback per E-Mail vorbereiten?",
        ["FeedbackSummary"] = "Empfänger: {0}\n\nEnthalten sind {1} Favorit(en), {2} Blockierung(en), App-Version und Plattform{3} Keine Gerätekennung, Abspielhistorie oder Dateipfade werden aufgenommen. Du prüfst und sendest die Nachricht anschließend selbst in deiner E-Mail-App.",
        ["WithComment"] = " sowie dein Kommentar.",
        ["OpenEmail"] = "E-Mail öffnen",
        ["NoEmailApp"] = "Keine E-Mail-App verfügbar",
        ["NoEmailAppHelp"] = "Bitte richte eine E-Mail-App ein und versuche es erneut. App-Support: {0}. Es wurde kein Feedback gesendet.",
        ["FeedbackSubject"] = "Open Sleep Music – Katalog-Feedback",
        ["FeedbackBody"] = "Mein Katalog-Feedback befindet sich in der angehängten JSON-Datei.",
        ["EmailOpened"] = "Der E-Mail-Entwurf wurde geöffnet. Bitte prüfe und sende ihn in deiner E-Mail-App.",
        ["EmailFailed"] = "E-Mail-Entwurf konnte nicht geöffnet werden",
        ["EmailFailedHelp"] = "Bitte prüfe deine E-Mail-App und versuche es erneut. App-Support: {0}. Die App hat kein Feedback automatisch gesendet.",
        ["Shuffle"] = "Zufällig",
        ["InOrder"] = "Reihenfolge",
        ["RepeatTrack"] = "Titel wiederholen",
        ["RepeatCollection"] = "Themensammlung wiederholen",
        ["PausedAt"] = " · pausiert bei {0}",
        ["CanResume"] = "„{0}“ kann bei {1} fortgesetzt werden.",
        ["DownloadFolder"] = "Download-Ordner: {0}",
        ["Minutes"] = "{0} Min.",
        ["MinutesLong"] = "{0} Minuten",
        ["MinutesRemaining"] = "noch {0} Min.",
        ["SecondsRemaining"] = "noch {0} Sek."
    };
    private static readonly IReadOnlyDictionary<string, string> En = new Dictionary<string, string>
    {
        ["Collections"]="Theme collections", ["Subtitle"]="Free sounds for a peaceful night", ["NoTrack"]="No track selected",
        ["Download"]="Download", ["Play"]="Play", ["Back"]="Back", ["Favorite"]="Favorite", ["Block"]="Block", ["Unblock"]="Unblock",
        ["Volume"]="Volume", ["AppVolume"]="App volume", ["SystemVolume"]="System volume", ["SleepTimer"]="Sleep timer", ["Repeat"]="Repeat",
        ["Settings"]="Settings", ["Language"]="Language", ["SystemLanguage"]="System language", ["German"]="German", ["English"]="English",
        ["ReducedMotion"]="Reduce motion", ["Close"]="Close", ["Off"]="Off", ["Track"]="Single track", ["Collection"]="Theme collection",
        ["TrackInfo"]="Track information", ["Source"]="Open source page", ["License"]="Open license", ["Creator"]="Creator/performer",
        ["Duration"]="Duration", ["LocalFile"]="Local file",
        ["MyMusic"] = "My music",
        ["EmptyLibrary"] = "No music has been downloaded for this collection yet.",
        ["SelectTrack"] = "Select a collection or tap a track.",
        ["NextTrackEmpty"] = "Next track: –",
        ["NextTrack"] = "Next track: {0}",
        ["Ready"] = "Ready",
        ["PlaybackSettings"] = "Playback settings",
        ["TimerHelp"] = "The selected duration starts with playback. Changing it while playing restarts the timer.",
        ["VolumeHelp"] = "The phone's hardware buttons additionally control Android media volume.",
        ["About"] = "About Open Sleep Music",
        ["AboutIntro"] = "Free sounds for a peaceful night — without ads, tracking, accounts or mandatory cloud services.",
        ["AboutOffline"] = "After downloading, the music library works offline. Unavailable or invalid source files are skipped without blocking an entire collection.",
        ["AboutLicenses"] = "The app is MIT licensed. Each recording retains its own license shown in track information. These details document the source's license and provenance; they are not an independent legal guarantee by the project.",
        ["AboutBackground"] = "Background playback and system media controls are supported on mobile devices.",
        ["Repository"] = "Open project on GitHub",
        ["TrackInfoSubtitle"] = "Source, license and local file",
        ["Instrumentation"] = "Instrumentation",
        ["LicenseName"] = "License",
        ["LicenseHelp"] = "The license information comes from the documented source page for this specific recording.",
        ["AddFavorite"] = "☆ Add favorite",
        ["BlockTrack"] = "Block track",
        ["CloseTrackList"] = "Close track list",
        ["Menu"] = "Menu",
        ["PreviousTrack"] = "Previous track",
        ["NextTrackAction"] = "Next track",
        ["PlayPause"] = "Play / pause",
        ["Pause"] = "Pause",
        ["ManageCollection"] = "Manage collection",
        ["FeedbackTitle"] = "Catalog feedback",
        ["FeedbackHelp"] = "Optionally explain why a track is particularly suitable or unsuitable. You will see a summary before sharing in the next step.",
        ["Continue"] = "Continue",
        ["Cancel"] = "Cancel",
        ["OptionalComment"] = "Optional comment",
        ["NoFeedback"] = "No feedback available",
        ["NoFeedbackHelp"] = "No favorites or blocks have been set and no comment has been entered.",
        ["PrepareEmail"] = "Prepare feedback email?",
        ["FeedbackSummary"] = "Recipient: {0}\n\nIncludes {1} favorite(s), {2} block(s), app version and platform{3} No device identifier, playback history or file paths are included. You review and send the message yourself in your email app.",
        ["WithComment"] = " as well as your comment.",
        ["OpenEmail"] = "Open email app",
        ["NoEmailApp"] = "No email app available",
        ["NoEmailAppHelp"] = "Please set up an email app and try again. App support: {0}. No feedback has been sent.",
        ["FeedbackSubject"] = "Open Sleep Music – Catalog feedback",
        ["FeedbackBody"] = "My catalog feedback is in the attached JSON file.",
        ["EmailOpened"] = "The email draft has been opened. Please review and send it in your email app.",
        ["EmailFailed"] = "Could not open email draft",
        ["EmailFailedHelp"] = "Please check your email app and try again. App support: {0}. The app has not sent any feedback automatically.",
        ["Shuffle"] = "Shuffle",
        ["InOrder"] = "In order",
        ["RepeatTrack"] = "Repeat track",
        ["RepeatCollection"] = "Repeat collection",
        ["PausedAt"] = " · paused at {0}",
        ["CanResume"] = "“{0}” can resume at {1}.",
        ["DownloadFolder"] = "Download folder: {0}",
        ["Minutes"] = "{0} min",
        ["MinutesLong"] = "{0} minutes",
        ["MinutesRemaining"] = "{0} min remaining",
        ["SecondsRemaining"] = "{0} sec remaining"
    };

    public static AppLanguage SelectedLanguage { get; private set; } = AppLanguage.System;
    public static bool IsGerman { get; private set; }

    public static void Apply(AppLanguage language)
    {
        SelectedLanguage = language;
        IsGerman = ResolveCulture(language, SystemCulture).TwoLetterISOLanguageName == "de";
        var culture = CultureInfo.GetCultureInfo(IsGerman ? "de" : "en");
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
    }

    internal static CultureInfo ResolveCulture(AppLanguage language, CultureInfo systemCulture) =>
        CultureInfo.GetCultureInfo(language == AppLanguage.German ||
            language == AppLanguage.System && systemCulture.TwoLetterISOLanguageName == "de" ? "de" : "en");

    internal static IEnumerable<string> Keys(AppLanguage language) =>
        (language == AppLanguage.German ? De : En).Keys;

    public static string Format(string key, params object[] arguments) =>
        string.Format(CultureInfo.CurrentCulture, Get(key), arguments);

    public static string TimerRemaining(TimeSpan remaining)
    {
        var seconds = Math.Max(0, Math.Ceiling(remaining.TotalSeconds));
        return remaining <= TimeSpan.FromMinutes(1)
            ? Format("SecondsRemaining", seconds)
            : Format("MinutesRemaining", Math.Ceiling(remaining.TotalMinutes));
    }

    public static string Get(string key) => (IsGerman ? De : En).TryGetValue(key, out var value) ? value : key;
    public static string Pick(string german, string english) => IsGerman ? german : english;

    public static string InstrumentName(string tag) => tag switch
    {
        "piano" => Pick("Klavier", "Piano"),
        "harp" => Pick("Harfe", "Harp"),
        "guitar" => Pick("Gitarre", "Guitar"),
        "recorder" => Pick("Blockflöte", "Recorder"),
        "pan-flute" => Pick("Panflöte", "Pan flute"),
        "flute" => Pick("Flöte", "Flute"),
        "violin" => Pick("Violine", "Violin"),
        "viola" => Pick("Bratsche", "Viola"),
        "cello" => Pick("Cello", "Cello"),
        "double-bass" => Pick("Kontrabass", "Double bass"),
        "clarinet" => Pick("Klarinette", "Clarinet"),
        "horn" => Pick("Horn", "Horn"),
        "bassoon" => Pick("Fagott", "Bassoon"),
        "saxophone" => Pick("Saxofon", "Saxophone"),
        "strings" => Pick("Streicher", "Strings"),
        "orchestra" => Pick("Orchester", "Orchestra"),
        "continuo" => "Continuo",
        "music-box" => Pick("Spieluhr", "Music box"),
        "instrumental" => Pick("Instrumental", "Instrumental"),
        "nature" => Pick("Naturklänge", "Nature sounds"),
        "birds" => Pick("Vögel", "Birds"),
        "crickets" => Pick("Grillen", "Crickets"),
        "insects" => Pick("Insekten", "Insects"),
        "cowbells" => Pick("Kuhglocken", "Cowbells"),
        "night" => Pick("Nacht", "Night"),
        "mountain" => Pick("Berge", "Mountains"),
        "jungle" => Pick("Dschungel", "Jungle"),
        "savanna" => Pick("Savanne", "Savanna"),
        "waves" => Pick("Wellen", "Waves"),
        _ => tag
    };

    public static string WorldName(string id, string fallback) => IsGerman ? fallback : id switch
    {
        "preselection" => "Preselection",
        "pre-qualify" => "Preview pre-qualify",
        "pre-qualify-licenses" => "Pre-Qualify – Do not use",
        "quiet-classics" => "Quiet classics", "rain" => "Gentle rain", "forest" => "Forest",
        "waves" => "Water & waves", "fireplace" => "Fireplace", "lullabies" => "Lullabies for little ones", _ => fallback
    };

    public static string WorldDescription(string id, string fallback) => IsGerman ? fallback : id switch
    {
        "preselection" => "Download candidates and audition offline. Rights and suitability reviews pending. Preview versions only.",
        "pre-qualify" => "External sources for further selection and technical review. Check each recording's rights status. Preview versions only.",
        "pre-qualify-licenses" => "Promising recordings with missing or conflicting permission. Do not use; source-page references only. Preview versions only.",
        "quiet-classics" => "Nocturnes, mazurkas and gentle classical miniatures.",
        "rain" => "Light to steady rain without selected thunder peaks.",
        "forest" => "Long forest and rainforest recordings with wind, water and birds.",
        "waves" => "Calm ocean, shore, waves and a babbling brook.",
        "fireplace" => "Quiet crackling of a fireplace.",
        "lullabies" => "Gentle piano, music-box and instrumental pieces without vocals.", _ => fallback
    };
}
