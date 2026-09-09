using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

public partial class AboutPage : ContentPage
{
    private static readonly Uri RepositoryUri = new("https://github.com/jochenwezel/open-sleep-music");

    public AboutPage()
    {
        InitializeComponent();
        VersionLabel.Text = $"Version {AppInfo.Current.VersionString}";
        var trackCount = BuiltInCatalog.SleepWorlds.Sum(world => world.Tracks.Count);
        CatalogLabel.Text = AppText.IsGerman ? $"Der integrierte Katalog umfasst {trackCount} Titel mit rund {BuiltInCatalog.TotalDuration.TotalHours:0.#} Stunden Musik und Naturklängen." : $"The built-in catalog contains {trackCount} tracks with about {BuiltInCatalog.TotalDuration.TotalHours:0.#} hours of music and natural sounds.";
        IntroLabel.Text = AppText.IsGerman ? "Kostenlose Klänge für eine ruhige Nacht – ohne Werbung, Tracking, Benutzerkonto oder Cloud-Zwang." : "Free sounds for a peaceful night — without ads, tracking, accounts or mandatory cloud services.";
        OfflineLabel.Text = AppText.IsGerman ? "Nach dem Herunterladen funktioniert die Musikbibliothek offline. Nicht mehr erreichbare oder ungültige Quelldateien werden übersprungen, ohne eine ganze Sammlung zu blockieren." : "After downloading, the music library works offline. Unavailable or invalid source files are skipped without blocking an entire collection.";
        LicenseInfoLabel.Text = AppText.IsGerman ? "Die App steht unter der MIT-Lizenz. Jede Aufnahme behält ihre eigene, in den Titelinformationen ausgewiesene Lizenz." : "The app is MIT licensed. Each recording retains its own license shown in track information.";
        BackgroundInfoLabel.Text = AppText.IsGerman ? "Hintergrundwiedergabe und System-Mediensteuerung werden auf Mobilgeräten unterstützt." : "Background playback and system media controls are supported on mobile devices.";
        RepositoryButton.Text = AppText.IsGerman ? "Projekt auf GitHub öffnen" : "Open project on GitHub";
    }

    private async void OnCloseClicked(object? sender, EventArgs e) =>
        await Navigation.PopModalAsync();

    private async void OnRepositoryClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!await Launcher.Default.OpenAsync(RepositoryUri))
            {
                StatusLabel.Text = RepositoryUri.AbsoluteUri;
            }
        }
        catch
        {
            StatusLabel.Text = RepositoryUri.AbsoluteUri;
        }
    }
}
