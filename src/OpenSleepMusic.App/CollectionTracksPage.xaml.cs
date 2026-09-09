using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;
using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

public partial class CollectionTracksPage : ContentPage
{
    private readonly IReadOnlyList<LocalLibraryTrack> _tracks;
    private readonly AppStateStore _stateStore;

    internal event EventHandler<LocalLibraryTrack>? PlayRequested;
    internal event EventHandler<LocalLibraryTrack>? PreferenceChanged;

    internal CollectionTracksPage(string title, IReadOnlyList<LocalLibraryTrack> tracks, AppStateStore stateStore)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        _tracks = tracks;
        _stateStore = stateStore;
        RefreshItems();
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not LibraryTrackItem item) return;
        TracksView.SelectedItem = null;
        if (item.IsBlocked)
        {
            await DisplayAlertAsync(AppText.IsGerman ? "Titel blockiert" : "Track blocked", AppText.IsGerman ? "Die Blockierung kann über die Titelinformationen aufgehoben werden." : "You can unblock it in track information.", "OK");
            return;
        }
        var playable = PlayableTracks();
        if (!playable.Contains(item.LocalTrack))
        {
            await DisplayAlertAsync(AppText.IsGerman ? "Favoriten aktiv" : "Favorites active", AppText.IsGerman ? "Für diese Themensammlung werden derzeit nur Favoriten abgespielt." : "Only favorites are currently played for this collection.", "OK");
            return;
        }
        PlayRequested?.Invoke(this, item.LocalTrack);
        await Navigation.PopModalAsync();
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: LibraryTrackItem item }) return;
        var track = item.LocalTrack;
        _stateStore.SetFavorite(track.SleepWorld.Id, track.Track.Id, !item.IsFavorite);
        RefreshItems();
        PreferenceChanged?.Invoke(this, track);
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: LibraryTrackItem item }) return;
        var page = new TrackDetailsPage(item.LocalTrack, _stateStore);
        page.PreferenceChanged += (_, _) =>
        {
            RefreshItems();
            PreferenceChanged?.Invoke(this, item.LocalTrack);
        };
        await Navigation.PushModalAsync(page);
    }

    private IReadOnlyList<LocalLibraryTrack> PlayableTracks()
    {
        var worldId = _tracks.FirstOrDefault()?.SleepWorld.Id;
        return worldId is null ? [] : TrackPreferenceFilter.Apply(
            _tracks,
            id => _stateStore.IsFavorite(worldId, id),
            id => _stateStore.IsBlocked(worldId, id));
    }

    private void RefreshItems()
    {
        TracksView.ItemsSource = _tracks.Select(track => new LibraryTrackItem(
            track,
            _stateStore.IsFavorite(track.SleepWorld.Id, track.Track.Id),
            _stateStore.IsBlocked(track.SleepWorld.Id, track.Track.Id))).ToArray();
        var playable = PlayableTracks();
        var favorites = _tracks.Count(track => _stateStore.IsFavorite(track.SleepWorld.Id, track.Track.Id));
        SummaryLabel.Text = AppText.IsGerman
            ? $"{_tracks.Count} offline · {playable.Count} in Wiedergabe{(favorites > 0 ? " · nur Favoriten" : string.Empty)}"
            : $"{_tracks.Count} offline · {playable.Count} playable{(favorites > 0 ? " · favorites only" : string.Empty)}";
    }
}
