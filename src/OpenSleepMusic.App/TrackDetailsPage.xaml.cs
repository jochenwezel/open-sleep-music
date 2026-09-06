using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.App;

public partial class TrackDetailsPage : ContentPage
{
    private readonly LocalLibraryTrack _localTrack;
    private readonly AppStateStore _stateStore;

    public event EventHandler? PreferenceChanged;

    internal TrackDetailsPage(LocalLibraryTrack localTrack, AppStateStore stateStore)
    {
        InitializeComponent();
        _localTrack = localTrack;
        _stateStore = stateStore;
        var track = localTrack.Track;
        TitleLabel.Text = track.Title;
        CreatorLabel.Text = track.Creator;
        WorldLabel.Text = localTrack.SleepWorld.Name;
        DurationLabel.Text = FormatDuration(TimeSpan.FromSeconds(track.DurationSeconds));
        LicenseLabel.Text = track.License;
        FileLabel.Text = localTrack.FilePath;
        UpdatePreferenceButtons();
    }

    private async void OnCloseClicked(object? sender, EventArgs e) =>
        await Navigation.PopModalAsync();

    private async void OnSourceClicked(object? sender, EventArgs e) =>
        await OpenUriAsync(_localTrack.Track.SourcePageUri);

    private async void OnLicenseClicked(object? sender, EventArgs e) =>
        await OpenUriAsync(_localTrack.Track.LicenseUri);

    private void OnBlockClicked(object? sender, EventArgs e)
    {
        var worldId = _localTrack.SleepWorld.Id;
        var trackId = _localTrack.Track.Id;
        _stateStore.SetBlocked(worldId, trackId, !_stateStore.IsBlocked(worldId, trackId));
        UpdatePreferenceButtons();
        PreferenceChanged?.Invoke(this, EventArgs.Empty);
        StatusLabel.Text = _stateStore.IsBlocked(worldId, trackId)
            ? "Dieser Titel wird bei der Wiedergabe übersprungen."
            : "Dieser Titel kann wieder abgespielt werden.";
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        var worldId = _localTrack.SleepWorld.Id;
        var trackId = _localTrack.Track.Id;
        _stateStore.SetFavorite(worldId, trackId, !_stateStore.IsFavorite(worldId, trackId));
        UpdatePreferenceButtons();
        PreferenceChanged?.Invoke(this, EventArgs.Empty);
        StatusLabel.Text = _stateStore.IsFavorite(worldId, trackId)
            ? "Als Favorit markiert. Diese Sammlung spielt bevorzugt ihre Favoriten."
            : "Favorit entfernt.";
    }

    private void UpdatePreferenceButtons()
    {
        var worldId = _localTrack.SleepWorld.Id;
        var trackId = _localTrack.Track.Id;
        FavoriteButton.Text = _stateStore.IsFavorite(worldId, trackId) ? "★ Favorit" : "☆ Favorisieren";
        BlockButton.Text = _stateStore.IsBlocked(worldId, trackId)
            ? "Blockierung aufheben"
            : "Titel blockieren";
    }

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
