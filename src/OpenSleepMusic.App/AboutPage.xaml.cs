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
        var trackCount = BuiltInCatalog.SleepWorlds.SelectMany(world => world.Tracks).DistinctBy(track => track.Id).Count();
        CatalogLabel.Text = AppText.IsGerman ? $"Der integrierte Katalog umfasst {trackCount} Titel mit rund {BuiltInCatalog.TotalDuration.TotalHours:0.#} Stunden Musik und Naturklängen." : $"The built-in catalog contains {trackCount} tracks with about {BuiltInCatalog.TotalDuration.TotalHours:0.#} hours of music and natural sounds.";
    }

    private async void OnCloseClicked(object? sender, EventArgs e) =>
        await Navigation.PopModalAsync();

    private async void OnRepositoryClicked(object? sender, EventArgs e)
    {
        if (!await ChildSafety.RequireParentAsync(this)) return;
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
