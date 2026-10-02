using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;
using OpenSleepMusic.App.Localization;
using OpenSleepMusic.App.Navigation;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OpenSleepMusic.App;

public partial class CollectionTracksPage : ContentPage
{
    private readonly IReadOnlyList<LocalLibraryTrack> _tracks;
    private readonly AppStateStore _stateStore;

    private readonly Func<LocalLibraryTrack, Task> _playRequested;
    private readonly ModalPlaybackNavigation _playbackNavigation = new();
    private readonly IReadOnlyList<CollectionTrackListItem> _items;
    private readonly string? _worldId;
    internal event EventHandler<LocalLibraryTrack>? PreferenceChanged;
    internal event EventHandler? PlaybackFilterChanged;

    internal CollectionTracksPage(string title, IReadOnlyList<LocalLibraryTrack> tracks, AppStateStore stateStore,
        Func<LocalLibraryTrack, Task> playRequested)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        _tracks = tracks;
        _stateStore = stateStore;
        _playRequested = playRequested;
        _worldId = tracks.FirstOrDefault()?.SleepWorld.Id;
        _items = tracks.Select(track => new CollectionTrackListItem(track)).ToArray();
        TracksView.ItemsSource = _items;
        SemanticProperties.SetDescription(CloseButton, AppText.Pick("Titelliste schließen", "Close track list"));
        RefreshItemStates();
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        if (!_playbackNavigation.IsOpening) await Navigation.PopModalAsync();
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_playbackNavigation.IsOpening || e.CurrentSelection.FirstOrDefault() is not CollectionTrackListItem item) return;
        TracksView.SelectedItem = null;
        if (item.IsBlocked)
        {
            await DisplayAlertAsync(AppText.IsGerman ? "Titel blockiert" : "Track blocked", AppText.IsGerman ? "Die Blockierung kann über die Titelinformationen aufgehoben werden." : "You can unblock it in track information.", "OK");
            return;
        }
        try
        {
            await _playbackNavigation.OpenAsync(
                () => Task.CompletedTask,
                () => _playRequested(item.LocalTrack));
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            await Shell.Current.DisplayAlertAsync(AppText.Get("Play"),
                AppText.Pick("Der Player konnte nicht geöffnet werden. Bitte erneut versuchen.",
                    "The player could not be opened. Please try again."), AppText.Get("Close"));
        }
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: CollectionTrackListItem item }) return;
        var track = item.LocalTrack;
        _stateStore.SetFavorite(track.SleepWorld.Id, track.Track.Id, !item.IsFavorite);
        RefreshItemStates();
        PreferenceChanged?.Invoke(this, track);
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (_playbackNavigation.IsOpening) return;
        if (sender is not Button { CommandParameter: CollectionTrackListItem item }) return;
        var page = new TrackDetailsPage(item.LocalTrack, _stateStore);
        page.PreferenceChanged += (_, _) =>
        {
            RefreshItemStates();
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
            id => _stateStore.IsBlocked(worldId, id),
            _stateStore.IsFavoritesOnly(worldId));
    }

    private void OnFavoritesModeClicked(object? sender, EventArgs e)
    {
        if (_worldId is null) return;
        _stateStore.SetFavoritesOnly(_worldId, !_stateStore.IsFavoritesOnly(_worldId));
        RefreshItemStates();
        PlaybackFilterChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshItemStates()
    {
        foreach (var item in _items)
        {
            item.Update(
                _stateStore.IsFavorite(item.LocalTrack.SleepWorld.Id, item.LocalTrack.Track.Id),
                _stateStore.IsBlocked(item.LocalTrack.SleepWorld.Id, item.LocalTrack.Track.Id));
        }
        var playable = PlayableTracks();
        var favorites = _tracks.Count(track => _stateStore.IsFavorite(track.SleepWorld.Id, track.Track.Id));
        var favoritesOnly = _worldId is not null && _stateStore.IsFavoritesOnly(_worldId);
        FavoritesModeButton.Text = favoritesOnly
            ? AppText.Pick("★ Nur Favoriten abspielen", "★ Play favorites only")
            : AppText.Pick("☆ Alle Titel abspielen", "☆ Play all tracks");
        SummaryLabel.Text = AppText.IsGerman
            ? $"{_tracks.Count} offline · {favorites} Favoriten · {playable.Count} in Wiedergabe"
            : $"{_tracks.Count} offline · {favorites} favorites · {playable.Count} playable";
    }
}

internal sealed class CollectionTrackListItem(LocalLibraryTrack localTrack) : INotifyPropertyChanged
{
    public LocalLibraryTrack LocalTrack { get; } = localTrack;
    public bool IsFavorite { get; private set; }
    public bool IsBlocked { get; private set; }
    public string FavoriteGlyph => IsFavorite ? "★" : "☆";
    public Color FavoriteColor => IsFavorite ? Color.FromArgb("#F4C95D") : Color.FromArgb("#9E9AAF");
    public double Opacity => IsBlocked ? 0.42 : 1;
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Update(bool isFavorite, bool isBlocked)
    {
        IsFavorite = isFavorite;
        IsBlocked = isBlocked;
        OnPropertyChanged(nameof(IsFavorite));
        OnPropertyChanged(nameof(IsBlocked));
        OnPropertyChanged(nameof(FavoriteGlyph));
        OnPropertyChanged(nameof(FavoriteColor));
        OnPropertyChanged(nameof(Opacity));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
