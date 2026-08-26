using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.App;

public partial class AboutPage : ContentPage
{
    private static readonly Uri RepositoryUri = new("https://github.com/jochenwezel/open-sleep-music");

    public AboutPage()
    {
        InitializeComponent();
        VersionLabel.Text = $"Version {AppInfo.Current.VersionString}";
        var trackCount = BuiltInCatalog.SleepWorlds.Sum(world => world.Tracks.Count);
        CatalogLabel.Text = $"Der integrierte Katalog umfasst {trackCount} Titel mit rund {BuiltInCatalog.TotalDuration.TotalHours:0.#} Stunden Musik und Naturklängen.";
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
