using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

public partial class TrackDetailsPage : ContentPage
{
    private readonly LocalLibraryTrack _localTrack;
    private readonly AppStateStore _stateStore;
    private readonly AudioTrack? _backgroundTrack;
    internal string WorldId => _localTrack.SleepWorld.Id;

    public event EventHandler? PreferenceChanged;

    internal TrackDetailsPage(LocalLibraryTrack localTrack, AppStateStore stateStore,
        IReadOnlyList<SleepWorld> catalog)
    {
        InitializeComponent();
        _localTrack = localTrack;
        _stateStore = stateStore;
        _backgroundTrack = BackgroundAudioCatalog.Resolve(localTrack.Track, catalog);
        var track = localTrack.Track;
        TitleLabel.Text = track.Title;
        CreatorLabel.Text = track.Creator;
        WorldLabel.Text = AppText.WorldName(localTrack.SleepWorld.Id, localTrack.SleepWorld.Name);
        DurationLabel.Text = FormatDuration(TimeSpan.FromSeconds(track.PlaybackDurationSeconds));
        InstrumentationLabel.Text = track.Instrumentation is { Count: > 0 }
            ? string.Join(", ", track.Instrumentation.Select(AppText.InstrumentName))
            : AppText.Pick("Nicht angegeben", "Not specified");
        LicenseLabel.Text = track.License;
        if (localTrack.SleepWorld.Id == "preselection")
        {
            LicenseHelpLabel.Text = track.LicenseReviewStatus == "verified" ? AppText.Pick(
                "Vorauswahl: Die Quellenfreigabe der Aufnahme wurde geprüft. Die Prüfung der Schlaf-Eignung und die Freigabe für eine produktive Sammlung erfolgen separat.",
                "Preselection: The source grant for this recording has been reviewed. Sleep suitability and production admission are separate checks.") : AppText.Pick(
                "Vorauswahl: Die Lizenz der Aufnahme und ihre Schlaf-Eignung sind noch ungeprüft. Die Quellenangabe ist keine Freigabe für eine produktive Sammlung.",
                "Preselection: Recording rights and sleep suitability are unreviewed. The source declaration is not production approval.");
            LicenseLabel.Text += " · " + (track.LicenseReviewStatus == "verified"
                ? AppText.Pick("Lizenz geprüft", "License reviewed")
                : AppText.Pick("Prüfung offen", "Review pending"));
        }
        FileLabel.Text = localTrack.FilePath;
        SetBackgroundAttribution();
        UpdatePreferenceButtons();
    }

    private void SetBackgroundAttribution()
    {
        if (_backgroundTrack is not { } background) return;
        BackgroundSection.IsVisible = true;
        BackgroundHeadingLabel.Text = AppText.Pick("Hintergrundaufnahme", "Background recording");
        BackgroundTitleLabel.Text = background.Title;
        BackgroundCreatorTitleLabel.Text = AppText.Get("Creator");
        BackgroundCreatorLabel.Text = background.Creator;
        BackgroundLicenseTitleLabel.Text = AppText.Pick("Lizenz", "License");
        BackgroundLicenseLabel.Text = background.License;
        BackgroundSourceButton.Text = AppText.Pick("Quelle des Hintergrunds öffnen", "Open background source");
        BackgroundLicenseButton.Text = AppText.Pick("Lizenz des Hintergrunds öffnen", "Open background license");
        BackgroundUsageLabel.Text = AppText.IsGerman
            ? $"Separat mit {background.VolumeGain * 100:0.#} % Lautstärke im Originaltempo abgespielt; bei Bedarf wiederholt und am Titelende ausgeblendet."
            : $"Played separately at {background.VolumeGain * 100:0.#}% volume and original speed; repeated as needed and faded out at the end of the main track.";
        if (background.StartOffsetMilliseconds > 0 || background.EndOffsetMilliseconds > 0)
        {
            BackgroundUsageLabel.Text += AppText.IsGerman
                ? $" Am Anfang um {background.StartOffsetMilliseconds / 1000d:0.###} s und am Ende um {background.EndOffsetMilliseconds / 1000d:0.###} s gekürzt."
                : $" Trimmed by {background.StartOffsetMilliseconds / 1000d:0.###} s at the start and {background.EndOffsetMilliseconds / 1000d:0.###} s at the end.";
        }
        BackgroundLicenseHelpLabel.Text = AppText.Pick(
            "Diese Lizenz gilt für die Hintergrundaufnahme. Die Lizenz des Hauptstücks ist separat oben angegeben. Die Angaben stammen von der dokumentierten Quellseite der jeweiligen Aufnahme.",
            "This license applies to the background recording. The main track's license is listed separately above. The information comes from each recording's documented source page.");
        if (background.LicenseReviewStatus != "verified")
        {
            BackgroundLicenseLabel.Text += " · " + AppText.Pick("Prüfung offen", "Review pending");
            BackgroundLicenseHelpLabel.Text += AppText.Pick(
                " Die Aufnahme ist noch nicht zur Nutzung in einer produktiven Sammlung freigegeben.",
                " This recording is not yet approved for a production collection.");
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e) =>
        await Navigation.PopModalAsync();

    private async void OnSourceClicked(object? sender, EventArgs e) =>
        await OpenUriAsync(_localTrack.Track.SourcePageUri);

    private async void OnLicenseClicked(object? sender, EventArgs e) =>
        await OpenUriAsync(_localTrack.Track.LicenseUri);

    private async void OnBackgroundSourceClicked(object? sender, EventArgs e)
    {
        if (_backgroundTrack is { } background) await OpenUriAsync(background.SourcePageUri);
    }

    private async void OnBackgroundLicenseClicked(object? sender, EventArgs e)
    {
        if (_backgroundTrack is { } background) await OpenUriAsync(background.LicenseUri);
    }

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
        if (!await ChildSafety.RequireParentAsync(this)) return;
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
