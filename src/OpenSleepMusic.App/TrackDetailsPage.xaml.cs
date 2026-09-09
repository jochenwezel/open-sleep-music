using OpenSleepMusic.Core.Library;
using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

public partial class TrackDetailsPage : ContentPage
{
    private readonly LocalLibraryTrack _localTrack;
    private readonly AppStateStore _stateStore;

    public event EventHandler? PreferenceChanged;

    internal TrackDetailsPage(LocalLibraryTrack localTrack, AppStateStore stateStore)
    {
        InitializeComponent();
        PageTitleLabel.Text = AppText.Get("TrackInfo");
        PageSubtitleLabel.Text = AppText.IsGerman ? "Quelle, Lizenz und lokale Datei" : "Source, license and local file";
        CreatorTitleLabel.Text = AppText.Get("Creator");
        WorldTitleLabel.Text = AppText.Get("Collection");
        DurationTitleLabel.Text = AppText.Get("Duration");
        LicenseTitleLabel.Text = AppText.IsGerman ? "Lizenz" : "License";
        FileTitleLabel.Text = AppText.Get("LocalFile");
        SourceButton.Text = AppText.Get("Source");
        LicenseButton.Text = AppText.Get("License");
        LicenseHelpLabel.Text = AppText.IsGerman ? "Die Lizenzangabe stammt von der dokumentierten Quellseite der konkreten Aufnahme." : "The license information comes from the documented source page for this specific recording.";
        _localTrack = localTrack;
        _stateStore = stateStore;
        var track = localTrack.Track;
        TitleLabel.Text = track.Title;
        CreatorLabel.Text = track.Creator;
        WorldLabel.Text = AppText.WorldName(localTrack.SleepWorld.Id, localTrack.SleepWorld.Name);
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
            ? AppText.Pick("Dieser Titel wird bei der Wiedergabe übersprungen.", "This track will be skipped during playback.")
            : AppText.Pick("Dieser Titel kann wieder abgespielt werden.", "This track can be played again.");
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        var worldId = _localTrack.SleepWorld.Id;
        var trackId = _localTrack.Track.Id;
        _stateStore.SetFavorite(worldId, trackId, !_stateStore.IsFavorite(worldId, trackId));
        UpdatePreferenceButtons();
        PreferenceChanged?.Invoke(this, EventArgs.Empty);
        StatusLabel.Text = _stateStore.IsFavorite(worldId, trackId)
            ? AppText.Pick("Als Favorit markiert. Diese Sammlung spielt bevorzugt ihre Favoriten.", "Marked as favorite. This collection now prefers its favorites.")
            : AppText.Pick("Favorit entfernt.", "Favorite removed.");
    }

    private void UpdatePreferenceButtons()
    {
        var worldId = _localTrack.SleepWorld.Id;
        var trackId = _localTrack.Track.Id;
        FavoriteButton.Text = _stateStore.IsFavorite(worldId, trackId)
            ? $"★ {AppText.Get("Favorite")}" : (AppText.IsGerman ? "☆ Favorisieren" : "☆ Add favorite");
        BlockButton.Text = _stateStore.IsBlocked(worldId, trackId) ? AppText.Get("Unblock") : AppText.Get("Block");
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
