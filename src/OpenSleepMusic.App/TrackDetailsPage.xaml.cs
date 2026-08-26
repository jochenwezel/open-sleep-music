using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.App;

public partial class TrackDetailsPage : ContentPage
{
    private readonly LocalLibraryTrack _localTrack;

    public TrackDetailsPage(LocalLibraryTrack localTrack)
    {
        InitializeComponent();
        _localTrack = localTrack;
        var track = localTrack.Track;
        TitleLabel.Text = track.Title;
        CreatorLabel.Text = track.Creator;
        WorldLabel.Text = localTrack.SleepWorld.Name;
        DurationLabel.Text = FormatDuration(TimeSpan.FromSeconds(track.DurationSeconds));
        LicenseLabel.Text = track.License;
        FileLabel.Text = localTrack.FilePath;
    }

    private async void OnCloseClicked(object? sender, EventArgs e) =>
        await Navigation.PopModalAsync();

    private async void OnSourceClicked(object? sender, EventArgs e) =>
        await OpenUriAsync(_localTrack.Track.SourcePageUri);

    private async void OnLicenseClicked(object? sender, EventArgs e) =>
        await OpenUriAsync(_localTrack.Track.LicenseUri);

    private async Task OpenUriAsync(Uri uri)
    {
        try
        {
            if (!await Launcher.Default.OpenAsync(uri))
            {
                StatusLabel.Text = uri.AbsoluteUri;
            }
        }
        catch
        {
            StatusLabel.Text = uri.AbsoluteUri;
        }
    }

    private static string FormatDuration(TimeSpan value) =>
        value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"m\:ss");
}
