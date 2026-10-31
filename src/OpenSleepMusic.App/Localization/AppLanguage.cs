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
        ["ChildSafety"] = "Kinderschutz",
        ["ChildSafetySetup"] = "Kinderschutz einrichten",
        ["ParentArea"] = "Elternbereich öffnen",
        ["ParentAreaUnlocked"] = "Elternbereich (entsperrt)",
        ["EnterChildMode"] = "Zum Kinderschutz wechseln",
        ["DisableChildSafety"] = "Kinderschutz deaktivieren",
        ["ChangeParentPin"] = "Eltern-PIN ändern",
        ["ChildModeActive"] = "Kinderschutz aktiv · nur offline",
        ["ChooseChildCollections"] = "Sammlungen für Kinder auswählen",
        ["SaveChildCollections"] = "Auswahl speichern",
        ["ChildDownloadedTracks"] = "{0} Titel heruntergeladen",
        ["ChildCollectionsHelp"] = "Welche Sammlungen darf das Kind nutzen? Markiere die gewünschten Sammlungen. Im Kinderschutz erscheinen nur freigegebene Sammlungen mit heruntergeladenen Titeln. Andere Sammlungen bleiben gespeichert und sind im Elternbereich weiter verfügbar. Die Auswahl kannst du später dort ändern.\n\nIst beim frischen App-Start genau eine Sammlung verfügbar und abspielbar, öffnet und startet sie automatisch. Mit dem Zurück-Button gelangst du zur Sammlungsübersicht.",
        ["ChildCollectionsEmpty"] = "Noch keine freigegebene Sammlung mit heruntergeladenen Titeln vorhanden. Im Menü den Elternbereich öffnen, Musik herunterladen oder die Freigabe ändern.",
        ["ParentRequired"] = "Im Kinderschutz sind Internetabrufe, externe Links, Feedback und die Sammlungsverwaltung gesperrt. Zum Freigeben im Menü den Elternbereich öffnen und die PIN eingeben.",
        ["ChildSafetySetupHelp"] = "Du möchtest das Gerät zum selbstständigen Musikhören an ein Kind weitergeben? Der freiwillige Kinderschutz beschränkt die App auf bereits heruntergeladene Inhalte – auch wenn das Gerät mit dem Internet verbunden bleibt.\n\nWeiter möglich: Musik abspielen, Lautstärke regeln, Schlaf-Timer einstellen und Favoriten auswählen.\n\nGesperrt: neue Downloads, Katalog- und Bildaktualisierungen, externe Internetlinks, Feedback sowie Reparieren und Löschen von Sammlungen.\n\nLade die gewünschten Sammlungen vorher vollständig herunter. Nach der Aktivierung kannst du im Menü mit deiner Eltern-PIN den Elternbereich entsperren. Dort sind alle Funktionen wieder verfügbar. Beim Verlassen der App wird erneut gesperrt; im Menü kannst du jederzeit zum Kinderschutz zurückkehren.\n\nOhne aktivierten Kinderschutz bleibt die App vollständig nutzbar.",
        ["ParentUnlockHelp"] = "Gib deine Eltern-PIN ein. Im entsperrten Elternbereich sind Downloads, Aktualisierungen, Links und Feedback wieder verfügbar. Beim Verlassen der App wird erneut gesperrt. Zum bewussten Zurückgeben des Geräts im Menü wieder zum Kinderschutz wechseln.",
        ["PinRecoveryHelp"] = "Wähle eine PIN mit 6 bis 12 Ziffern, die das Kind nicht kennt. Merke sie dir gut: Es gibt keine PIN-Wiederherstellung per E-Mail. Eine vergessene PIN erfordert das Zurücksetzen der App-Daten; dabei gehen lokale Einstellungen und Downloads verloren.",
        ["PinFormat"] = "Bitte 6 bis 12 Ziffern eingeben.",
        ["ParentPin"] = "Eltern-PIN",
        ["NewParentPin"] = "Neue Eltern-PIN (6–12 Ziffern)",
        ["ConfirmParentPin"] = "Eltern-PIN wiederholen",
        ["EnableChildSafety"] = "Kinderschutz aktivieren",
        ["UnlockParentArea"] = "Elternbereich entsperren",
        ["SaveParentPin"] = "PIN speichern",
        ["PinMismatch"] = "Die beiden PIN-Eingaben stimmen nicht überein.",
        ["PinIncorrect"] = "Die PIN ist nicht richtig.",
        ["PinWait"] = "Zu viele Versuche. Bitte 30 Sekunden warten und erneut versuchen.",
        ["PinUnavailable"] = "Die gespeicherte PIN ist auf diesem Gerät nicht verfügbar. Der Kinderschutz bleibt aktiv. Prüfe, ob die App-Daten aus einer Gerätesicherung wiederhergestellt wurden.",
        ["PinStorageError"] = "Die PIN konnte nicht sicher gelesen oder gespeichert werden. Bitte erneut versuchen.",
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
        ["ChildSafety"] = "Child safety",
        ["ChildSafetySetup"] = "Set up child mode",
        ["ParentArea"] = "Open parent area",
        ["ParentAreaUnlocked"] = "Parent area (unlocked)",
        ["EnterChildMode"] = "Return to child mode",
        ["DisableChildSafety"] = "Disable child mode",
        ["ChangeParentPin"] = "Change parent PIN",
        ["ChildModeActive"] = "Child mode active · offline only",
        ["ChooseChildCollections"] = "Choose collections for children",
        ["SaveChildCollections"] = "Save selection",
        ["ChildDownloadedTracks"] = "{0} tracks downloaded",
        ["ChildCollectionsHelp"] = "Which collections may the child use? Select the collections you want. Child mode only shows allowed collections with downloaded tracks. Other collections remain stored and available in the parent area. You can change this selection there later.\n\nIf exactly one collection is available and playable on a fresh app launch, it opens and starts automatically. Use the Back button to return to the collection overview.",
        ["ChildCollectionsEmpty"] = "No allowed collections with downloaded tracks yet. Open the parent area from the menu to download music or change the allowed collections.",
        ["ParentRequired"] = "Child mode blocks internet requests, external links, feedback and collection management. To allow them, open the parent area from the menu and enter your PIN.",
        ["ChildSafetySetupHelp"] = "Want to hand the device to a child for independent listening? Optional child mode limits the app to downloaded content, even while the device remains connected to the internet.\n\nStill available: play music, adjust volume, set the sleep timer and select favorites.\n\nBlocked: new downloads, catalog and artwork updates, external web links, feedback, and repairing or deleting collections.\n\nFully download the collections you want before enabling this mode. Afterwards, unlock the parent area from the menu with your parent PIN to use all features again. Leaving the app locks it again; you can also return to child mode from the menu at any time.\n\nWithout child mode enabled, all app features remain available.",
        ["ParentUnlockHelp"] = "Enter your parent PIN. Downloads, updates, links and feedback are available in the unlocked parent area. Leaving the app locks it again. Before handing back the device, return to child mode from the menu.",
        ["PinRecoveryHelp"] = "Choose a PIN of 6 to 12 digits that the child does not know. Keep it safe: there is no email PIN recovery. A forgotten PIN requires resetting the app data, which removes local settings and downloads.",
        ["PinFormat"] = "Please enter 6 to 12 digits.",
        ["ParentPin"] = "Parent PIN",
        ["NewParentPin"] = "New parent PIN (6–12 digits)",
        ["ConfirmParentPin"] = "Repeat parent PIN",
        ["EnableChildSafety"] = "Enable child mode",
        ["UnlockParentArea"] = "Unlock parent area",
        ["SaveParentPin"] = "Save PIN",
        ["PinMismatch"] = "The two PIN entries do not match.",
        ["PinIncorrect"] = "Incorrect PIN.",
        ["PinWait"] = "Too many attempts. Please wait 30 seconds and try again.",
        ["PinUnavailable"] = "The saved PIN is unavailable on this device. Child mode stays enabled. Check whether the app data was restored from a device backup.",
        ["PinStorageError"] = "The PIN could not be read or stored securely. Please try again.",
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
