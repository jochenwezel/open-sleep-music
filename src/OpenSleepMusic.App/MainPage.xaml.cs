using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;
using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;
#if ANDROID
using OpenSleepMusic.App.Playback;
#endif

namespace OpenSleepMusic.App;

public partial class MainPage : ContentPage
{
    private const int MaximumAutomaticFailureSkips = 3;
    private static readonly int[] SleepTimerMinutes = [0, 15, 30, 45, 60, 90];
    private static readonly string[] SleepTimerLabels = ["Aus", "15 Minuten", "30 Minuten", "45 Minuten", "60 Minuten", "90 Minuten"];
    private static readonly string[] RepeatModeLabels = ["Schlafwelt", "Einzeltitel"];
    private readonly HttpClient _httpClient;
    private readonly LocalLibraryScanner _libraryScanner = new();
    private readonly LocalLibraryManager _libraryManager = new();
    private readonly AppStateStore _stateStore = new();
    private readonly SleepTimer _sleepTimer = new();
    private readonly MediaElement Player = new()
    {
        IsVisible = false,
        ShouldAutoPlay = false
    };
    private readonly IReadOnlyList<SleepWorldCard> _worldCards;
    private readonly string _downloadRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
        "Open Sleep Music");
    private readonly PersistedAppState _initialState;
    private IReadOnlyList<LocalLibraryTrack> _library = [];
    private IReadOnlyList<LocalLibraryTrack> _displayedLibrary = [];
    private IReadOnlyList<LocalLibraryTrack> _visibleLibrary = [];
    private SleepWorldCard? _selectedWorldCard;
    private LocalLibraryTrack? _currentTrack;
    private LocalLibraryTrack? _nextTrack;
    private PlaybackRepeatMode _repeatMode;
    private DateTimeOffset? _sleepTimerEndUtc;
    private DateTimeOffset _lastPlaybackSaveUtc = DateTimeOffset.MinValue;
    private string? _loadedTrackId;
    private double _resumePositionSeconds;
    private double _pendingSeekSeconds;
    private bool _playWhenMediaOpens;
    private bool _shuffleEnabled;
    private bool _isSeeking;
    private bool _shouldContinuePlayback;
    private bool _isTrackTransitioning;
    private bool _restoringControls;
    private bool _didRestorePlayback;
    private int _consecutivePlaybackFailures;
    private string? _responsiveLayoutMode;
#if !ANDROID
    private CancellationTokenSource? _sleepFadeCancellation;
#endif

    public MainPage()
    {
        InitializeComponent();
        SetPlaybackControlsEnabled(false);
#if !ANDROID
        Player.MediaOpened += OnMediaOpened;
        Player.MediaEnded += OnMediaEnded;
        Player.MediaFailed += OnMediaFailed;
        Player.StateChanged += OnPlayerStateChanged;
        RootGrid.Add(Player);
#endif
        _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "OpenSleepMusic/0.1 (+https://github.com/jochenwezel/open-sleep-music)");
        _worldCards = BuiltInCatalog.SleepWorlds.Select(world => new SleepWorldCard(world)).ToArray();
        WorldsView.ItemsSource = _worldCards;

        _initialState = _stateStore.Load();
        _shuffleEnabled = _initialState.ShuffleEnabled;
        _repeatMode = _initialState.RepeatMode;
        _restoringControls = true;
        SleepTimerPicker.ItemsSource = SleepTimerLabels;
        SleepTimerPicker.SelectedIndex = 0;
        RepeatModePicker.ItemsSource = RepeatModeLabels;
        RepeatModePicker.SelectedIndex = _repeatMode == PlaybackRepeatMode.Track ? 1 : 0;
        VolumeSlider.Value = _initialState.Volume;
        Player.Volume = _initialState.Volume;
        VolumeLabel.Text = $"{_initialState.Volume:P0}";
        _restoringControls = false;
        RestoreSleepTimer(_initialState);

#if ANDROID
        AndroidPlaybackBridge.StateChanged += OnAndroidPlaybackStateChanged;
#endif

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), UpdatePlaybackStatus);
        SizeChanged += (_, _) => ApplyResponsiveLayout(Width, Height);
        Loaded += async (_, _) => await RefreshLibraryAsync();
    }

    private void ApplyResponsiveLayout(double width, double height)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var phoneWidth = width < 720;
        var narrow = phoneWidth && width <= height;
        var compactLandscape = width > height && height < 560;
        var layoutMode = $"{phoneWidth}:{narrow}:{compactLandscape}";
        if (_responsiveLayoutMode == layoutMode)
        {
            return;
        }
        _responsiveLayoutMode = layoutMode;

        RootGrid.Padding = narrow || compactLandscape ? new Thickness(12) : new Thickness(28);
        RootGrid.RowSpacing = narrow || compactLandscape ? 8 : 14;
        HeaderTitle.FontSize = compactLandscape ? 16 : narrow ? 22 : 30;
        HeaderSubtitle.IsVisible = !phoneWidth && !compactLandscape;
        HeaderGrid.HeightRequest = compactLandscape ? 36 : -1;
        MenuButton.WidthRequest = compactLandscape ? 38 : 48;
        MenuButton.HeightRequest = compactLandscape ? 34 : 44;
        MenuButton.FontSize = compactLandscape ? 18 : 22;
        WorldsTitleLabel.IsVisible = !compactLandscape;
        LibraryTitleLabel.IsVisible = !compactLandscape;
        PlayerBorder.Padding = compactLandscape ? new Thickness(8) : new Thickness(12);

        if (narrow)
        {
            SetColumns(ContentGrid, GridLength.Star);
            SetRows(ContentGrid, GridLength.Star, GridLength.Star);
            ContentGrid.ColumnSpacing = 0;
            ContentGrid.RowSpacing = 12;
            Grid.SetRow(WorldsPanel, 0);
            Grid.SetColumn(WorldsPanel, 0);
            Grid.SetRow(LibraryPanel, 1);
            Grid.SetColumn(LibraryPanel, 0);
            WorldsItemsLayout.Span = 1;

            SetColumns(PlayerOptionsGrid, GridLength.Star);
            SetRows(PlayerOptionsGrid, GridLength.Auto, GridLength.Auto);
            PlayerOptionsGrid.ColumnSpacing = 0;
            PlayerOptionsGrid.RowSpacing = 4;
            Grid.SetRow(RepeatOptionsPanel, 1);
            Grid.SetColumn(RepeatOptionsPanel, 0);
        }
        else
        {
            SetColumns(ContentGrid, new GridLength(3, GridUnitType.Star), new GridLength(2, GridUnitType.Star));
            SetRows(ContentGrid, GridLength.Star);
            ContentGrid.ColumnSpacing = 18;
            ContentGrid.RowSpacing = 0;
            Grid.SetRow(WorldsPanel, 0);
            Grid.SetColumn(WorldsPanel, 0);
            Grid.SetRow(LibraryPanel, 0);
            Grid.SetColumn(LibraryPanel, 1);
            WorldsItemsLayout.Span = 2;

            SetColumns(PlayerOptionsGrid, GridLength.Star, GridLength.Star);
            SetRows(PlayerOptionsGrid, GridLength.Auto);
            PlayerOptionsGrid.ColumnSpacing = 18;
            PlayerOptionsGrid.RowSpacing = 0;
            Grid.SetRow(RepeatOptionsPanel, 0);
            Grid.SetColumn(RepeatOptionsPanel, 1);
        }

        PlayerOptionsGrid.IsVisible = !compactLandscape;
        VolumePanel.IsVisible = !compactLandscape;
        NextTrackLabel.IsVisible = !compactLandscape;
    }

    private static void SetColumns(Grid grid, params GridLength[] widths)
    {
        grid.ColumnDefinitions.Clear();
        foreach (var width in widths)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
        }
    }

    private static void SetRows(Grid grid, params GridLength[] heights)
    {
        grid.RowDefinitions.Clear();
        foreach (var height in heights)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = height });
        }
    }

    internal void HandleAppDeactivated() => PersistPlaybackSnapshot(force: true);

    internal void HandleAppResumed()
    {
#if ANDROID
        ApplyAndroidSnapshot(AndroidPlaybackBridge.Snapshot);
#else
        if (_shouldContinuePlayback
            && _loadedTrackId is not null
            && Player.CurrentState != MediaElementState.Playing)
        {
            Player.Play();
        }
#endif
    }

    private async void OnWorldActionClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: SleepWorldCard card } button)
        {
            return;
        }

        SelectWorld(card);
        if (card.IsComplete)
        {
            if (_visibleLibrary.Count > 0)
            {
                PlayTrack(_visibleLibrary[0], userInitiated: true);
            }
            return;
        }

        await DownloadWorldAsync(card, button, isRepair: false);
    }

    private async void OnWorldManageClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: SleepWorldCard card } button)
        {
            return;
        }

        SelectWorld(card);
        try
        {
            var action = await DisplayActionSheetAsync(
                card.World.Name,
                "Abbrechen",
                "Sammlung löschen",
                "Sammlung prüfen und reparieren");

            if (action == "Sammlung prüfen und reparieren")
            {
                await DownloadWorldAsync(card, button, isRepair: true);
            }
            else if (action == "Sammlung löschen")
            {
                await DeleteWorldAsync(card);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = "Die Sammlungsverwaltung konnte nicht geöffnet werden.";
        }
    }

    private async Task DownloadWorldAsync(SleepWorldCard card, Button button, bool isRepair)
    {
        button.IsEnabled = false;
        DownloadProgress.Progress = 0;
        StatusLabel.Text = isRepair
            ? $"{card.World.Name} wird geprüft und repariert …"
            : $"{card.World.Name} wird vorbereitet …";

        var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs", "downloads.jsonl");
        var downloader = new SleepWorldDownloader(_httpClient, new FileDownloadLogSink(logPath));
        var progress = new Progress<DownloadProgress>(value =>
        {
            DownloadProgress.Progress = value.Total == 0 ? 0 : (double)value.Completed / value.Total;
            if (!string.IsNullOrWhiteSpace(value.CurrentTitle))
            {
                StatusLabel.Text = isRepair
                    ? $"Prüfe {value.CurrentTitle} …"
                    : $"Lade {value.CurrentTitle} …";
            }
        });

        try
        {
            var result = await downloader.DownloadAsync(card.World, _downloadRoot, progress);
            StatusLabel.Text = result.AvailableCount == 0
                ? "Derzeit sind keine Titel verfügbar. Bitte später erneut versuchen."
                : isRepair
                    ? $"{card.World.Name}: Prüfung abgeschlossen, {result.AvailableCount} Titel verfügbar."
                    : $"{card.World.Name}: {result.AvailableCount} Titel sind offline verfügbar.";
            await RefreshLibraryAsync(preserveStatus: true);
        }
        catch (OperationCanceledException)
        {
            StatusLabel.Text = "Download angehalten.";
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = "Der Download wurde unerwartet beendet. Andere Sammlungen können weiter verwendet werden.";
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

    private async Task DeleteWorldAsync(SleepWorldCard card)
    {
        var confirmed = await DisplayAlertAsync(
            "Sammlung löschen",
            $"Alle heruntergeladenen Dateien aus „{card.World.Name}“ werden gelöscht. Andere Schlafwelten bleiben erhalten.",
            "Löschen",
            "Abbrechen");
        if (!confirmed)
        {
            return;
        }

        try
        {
            if (_currentTrack?.SleepWorld.Id == card.World.Id)
            {
                StopAndClearPlayback();
            }

            await _libraryManager.DeleteWorldAsync(_downloadRoot, card.World);
            await RefreshLibraryAsync(preserveStatus: true);
            StatusLabel.Text = $"{card.World.Name} wurde aus der lokalen Bibliothek gelöscht.";
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = $"{card.World.Name} konnte nicht vollständig gelöscht werden.";
        }
    }

    private void OnWorldSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SleepWorldCard card)
        {
            SelectWorld(card);
        }
    }

    private void SelectWorld(SleepWorldCard card, bool persist = true)
    {
        _selectedWorldCard = card;
        if (WorldsView.SelectedItem != card)
        {
            WorldsView.SelectedItem = card;
        }
        if (persist)
        {
            _stateStore.SaveSelectedWorld(card.World.Id);
        }
        ApplyLibraryFilter();
    }

    private async Task RefreshLibraryAsync(bool preserveStatus = false)
    {
        try
        {
            _library = await _libraryScanner.ScanAsync(_downloadRoot, BuiltInCatalog.SleepWorlds);
#if ANDROID
            if (_library.Count == 0 && AndroidPlaybackBridge.Snapshot.TrackId is not null)
            {
                AndroidPlaybackBridge.Stop();
            }
#endif
            foreach (var card in _worldCards)
            {
                var tracks = _library.Where(track => track.SleepWorld.Id == card.World.Id).ToArray();
                card.SetLibraryStatus(tracks.Length, tracks.Sum(track => new FileInfo(track.FilePath).Length));
            }

            var cardToSelect = _selectedWorldCard
                ?? _worldCards.FirstOrDefault(card => card.World.Id == _initialState.SelectedWorldId)
                ?? _worldCards.FirstOrDefault(card => card.HasDownloads)
                ?? _worldCards[0];
            SelectWorld(cardToSelect, persist: _didRestorePlayback);
            RestorePlaybackOnce();
#if ANDROID
            ApplyAndroidSnapshot(AndroidPlaybackBridge.Snapshot);
#endif

            if (!preserveStatus)
            {
                StatusLabel.Text = _library.Count == 0
                    ? "Noch keine gültigen Audiodateien vorhanden."
                    : $"{_library.Count} Titel offline verfügbar.";
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = "Die lokale Musikbibliothek konnte nicht aktualisiert werden.";
        }
    }

    private void RestorePlaybackOnce()
    {
        if (_didRestorePlayback)
        {
            return;
        }
        _didRestorePlayback = true;

        var saved = _stateStore.LoadPlayback();
        var track = saved is null
            ? null
            : _library.FirstOrDefault(candidate => candidate.Track.Id == saved.TrackId);
        if (track is null || saved is null)
        {
            return;
        }

        _currentTrack = track;
        _resumePositionSeconds = Math.Min(saved.PositionSeconds, Math.Max(0, track.Track.DurationSeconds - 1));
        PositionSlider.Maximum = Math.Max(1, track.Track.DurationSeconds);
        PositionSlider.Value = _resumePositionSeconds;
        TimeLabel.Text = $"{FormatTime(TimeSpan.FromSeconds(_resumePositionSeconds))} / {FormatTime(TimeSpan.FromSeconds(track.Track.DurationSeconds))}";
        NowPlayingLabel.Text = track.Track.Title;
        SelectWorld(_worldCards.First(card => card.World.Id == track.SleepWorld.Id));
        UpdateNowPlayingDetails(isRestored: true);
        UpdateNextTrack();
        StatusLabel.Text = $"„{track.Track.Title}“ kann bei {FormatTime(TimeSpan.FromSeconds(_resumePositionSeconds))} fortgesetzt werden.";
    }

    private void ApplyLibraryFilter()
    {
        _displayedLibrary = _selectedWorldCard is null
            ? []
            : _library.Where(track => track.SleepWorld.Id == _selectedWorldCard.World.Id).ToArray();
        var worldId = _selectedWorldCard?.World.Id;
        _visibleLibrary = worldId is null
            ? []
            : TrackPreferenceFilter.Apply(
                _displayedLibrary,
                trackId => _stateStore.IsFavorite(worldId, trackId),
                trackId => _stateStore.IsBlocked(worldId, trackId));
        LibraryView.ItemsSource = _displayedLibrary.Select(CreateTrackItem).ToArray();
        LibraryEmptyLabel.IsVisible = _displayedLibrary.Count == 0;
        LibraryTitleLabel.Text = _selectedWorldCard is null
            ? "Meine Musik"
            : $"Meine Musik · {_selectedWorldCard.World.Name}";
        SetPlaybackControlsEnabled(_visibleLibrary.Count > 0);
        UpdateNextTrack();
    }

    private void SetPlaybackControlsEnabled(bool enabled)
    {
        PreviousButton.IsEnabled = enabled;
        PlayPauseButton.IsEnabled = enabled;
        NextButton.IsEnabled = enabled;
        PositionSlider.IsEnabled = enabled;
    }

    private void OnLibrarySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LibraryTrackItem item)
        {
            LibraryView.SelectedItem = null;
            if (item.IsBlocked)
            {
                StatusLabel.Text = "Dieser Titel ist blockiert. Die Blockierung kann über ⓘ aufgehoben werden.";
                return;
            }
            if (!_visibleLibrary.Contains(item.LocalTrack))
            {
                StatusLabel.Text = "Für diese Schlafwelt werden derzeit nur Favoriten abgespielt.";
                return;
            }
            PlayTrack(item.LocalTrack, userInitiated: true);
        }
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: LibraryTrackItem item })
        {
            return;
        }
        var worldId = item.LocalTrack.SleepWorld.Id;
        var trackId = item.LocalTrack.Track.Id;
        _stateStore.SetFavorite(worldId, trackId, !item.IsFavorite);
        ApplyTrackPreferences(item.LocalTrack);
    }

    private async void OnTrackDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: LibraryTrackItem item })
        {
            await OpenTrackDetailsAsync(item.LocalTrack);
        }
    }

    private async void OnNowPlayingTapped(object? sender, TappedEventArgs e)
    {
        if (_currentTrack is not null)
        {
            await OpenTrackDetailsAsync(_currentTrack);
        }
    }

    private async Task OpenTrackDetailsAsync(LocalLibraryTrack track)
    {
        var page = new TrackDetailsPage(track, _stateStore);
        page.PreferenceChanged += (_, _) => ApplyTrackPreferences(track);
        await Navigation.PushModalAsync(page);
    }

    private LibraryTrackItem CreateTrackItem(LocalLibraryTrack track) => new(
        track,
        _stateStore.IsFavorite(track.SleepWorld.Id, track.Track.Id),
        _stateStore.IsBlocked(track.SleepWorld.Id, track.Track.Id));

    private void ApplyTrackPreferences(LocalLibraryTrack changedTrack)
    {
        var wasPlaying = _shouldContinuePlayback;
#if ANDROID
        var position = AndroidPlaybackBridge.Snapshot.Position.TotalSeconds;
#else
        var position = Player.Position.TotalSeconds;
#endif
        ApplyLibraryFilter();

        if (_currentTrack is null || _visibleLibrary.Contains(_currentTrack))
        {
#if ANDROID
            if (_currentTrack is not null && _loadedTrackId is not null)
            {
                PlayTrack(_currentTrack, startPositionSeconds: position);
                if (!wasPlaying) AndroidPlaybackBridge.Pause();
            }
#endif
            return;
        }

        if (_visibleLibrary.Count > 0)
        {
            PlayTrack(_visibleLibrary[0], userInitiated: true);
            if (!wasPlaying)
            {
#if ANDROID
                AndroidPlaybackBridge.Pause();
#else
                Player.Pause();
#endif
            }
        }
        else
        {
            StopAndClearPlayback();
        }
    }

    private void PlayTrack(LocalLibraryTrack track, bool userInitiated = false, double startPositionSeconds = 0)
    {
        if (userInitiated)
        {
            _consecutivePlaybackFailures = 0;
        }

        var card = _worldCards.FirstOrDefault(candidate => candidate.World.Id == track.SleepWorld.Id);
        if (card is not null && _selectedWorldCard != card)
        {
            SelectWorld(card);
        }

        _currentTrack = track;
        _resumePositionSeconds = 0;
        _shouldContinuePlayback = true;
        _isTrackTransitioning = true;
        _pendingSeekSeconds = Math.Max(0, startPositionSeconds);
        _playWhenMediaOpens = true;
        NowPlayingLabel.Text = track.Track.Title;
        UpdateNowPlayingDetails();
        Player.MetadataTitle = track.Track.Title;
        Player.MetadataArtist = track.Track.Creator;

#if ANDROID
        _loadedTrackId = track.Track.Id;
        AndroidPlaybackBridge.LoadAndPlay(
            _visibleLibrary,
            track,
            startPositionSeconds,
            _shuffleEnabled,
            _repeatMode == PlaybackRepeatMode.Track,
            VolumeSlider.Value,
            _sleepTimerEndUtc);
#else
        if (_loadedTrackId == track.Track.Id)
        {
            _ = SeekAndPlayAsync(_pendingSeekSeconds);
        }
        else
        {
            _loadedTrackId = track.Track.Id;
            Player.Source = MediaSource.FromFile(track.FilePath);
            Player.Play();
        }
#endif

        PlayPauseButton.Text = "⏸";
        UpdateNextTrack();
        _stateStore.SavePlayback(track.Track.Id, Math.Max(0, startPositionSeconds));
        _lastPlaybackSaveUtc = DateTimeOffset.UtcNow;
    }

    private async Task SeekAndPlayAsync(double positionSeconds)
    {
        try
        {
            if (positionSeconds > 0)
            {
                await Player.SeekTo(TimeSpan.FromSeconds(positionSeconds));
            }
            else
            {
                await Player.SeekTo(TimeSpan.Zero);
            }
            Player.Play();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            Player.Play();
        }
    }

    private async void OnMediaOpened(object? sender, EventArgs e)
    {
        var seekTo = _pendingSeekSeconds;
        _pendingSeekSeconds = 0;
        try
        {
            if (seekTo > 0)
            {
                await Player.SeekTo(TimeSpan.FromSeconds(seekTo));
            }
            if (_playWhenMediaOpens)
            {
                Player.Play();
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
        finally
        {
            _playWhenMediaOpens = false;
        }
    }

    private void OnPlayPauseClicked(object? sender, EventArgs e)
    {
        if (_currentTrack is null)
        {
            if (_visibleLibrary.Count > 0)
            {
                PlayTrack(_visibleLibrary[0], userInitiated: true);
            }
            return;
        }

        if (_loadedTrackId is null)
        {
            PlayTrack(_currentTrack, userInitiated: true, startPositionSeconds: _resumePositionSeconds);
        }
#if ANDROID
        else if (AndroidPlaybackBridge.Snapshot.IsPlaying)
        {
            _shouldContinuePlayback = false;
            AndroidPlaybackBridge.Pause();
            PlayPauseButton.Text = "▶";
            PersistPlaybackSnapshot(force: true);
        }
        else
        {
            _shouldContinuePlayback = true;
            AndroidPlaybackBridge.Play();
            PlayPauseButton.Text = "⏸";
        }
#else
        else if (Player.CurrentState == MediaElementState.Playing)
        {
            _shouldContinuePlayback = false;
            Player.Pause();
            PlayPauseButton.Text = "▶";
            PersistPlaybackSnapshot(force: true);
        }
        else
        {
            _shouldContinuePlayback = true;
            Player.Play();
            PlayPauseButton.Text = "⏸";
        }
#endif
    }

    private void OnPreviousClicked(object? sender, EventArgs e)
    {
#if ANDROID
        if (_currentTrack is null && _visibleLibrary.Count > 0) PlayTrack(_visibleLibrary[0], userInitiated: true);
        else AndroidPlaybackBridge.Previous();
#else
        MoveTrack(-1, forceSequential: true);
#endif
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
#if ANDROID
        if (_currentTrack is null && _visibleLibrary.Count > 0) PlayTrack(_visibleLibrary[0], userInitiated: true);
        else AndroidPlaybackBridge.Next();
#else
        MoveTrack(1, forceSequential: !_shuffleEnabled);
#endif
    }

    private void MoveTrack(int offset, bool forceSequential = false)
    {
        if (_visibleLibrary.Count == 0)
        {
            return;
        }

        LocalLibraryTrack nextTrack;
        if (_shuffleEnabled && !forceSequential && offset > 0 && _nextTrack is not null && _nextTrack != _currentTrack)
        {
            nextTrack = _nextTrack;
        }
        else
        {
            var currentIndex = _currentTrack is null ? -1 : _visibleLibrary.IndexOf(_currentTrack);
            if (currentIndex < 0)
            {
                currentIndex = offset < 0 ? 0 : -1;
            }
            var nextIndex = PlaybackQueue.MoveSequential(_visibleLibrary.Count, currentIndex, offset);
            nextTrack = _visibleLibrary[nextIndex];
        }

        PlayTrack(nextTrack);
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        if (!_shouldContinuePlayback)
        {
            return;
        }

        if (_repeatMode == PlaybackRepeatMode.Track && _currentTrack is not null)
        {
            PlayTrack(_currentTrack);
        }
        else
        {
            MoveTrack(1);
        }
    }

    private void OnMediaFailed(object? sender, MediaFailedEventArgs e)
    {
        _isTrackTransitioning = false;
        _consecutivePlaybackFailures++;
        if (_visibleLibrary.Count == 0
            || _consecutivePlaybackFailures >= Math.Min(MaximumAutomaticFailureSkips, _visibleLibrary.Count))
        {
            _shouldContinuePlayback = false;
            Player.Stop();
            PlayPauseButton.Text = "▶";
            StatusLabel.Text = "Mehrere Titel konnten nicht wiedergegeben werden. Bitte einen anderen Titel auswählen.";
            return;
        }

        StatusLabel.Text = "Dieser Titel konnte nicht wiedergegeben werden; der nächste Titel wird geöffnet.";
        MoveTrack(1);
    }

    private void OnPlayerStateChanged(object? sender, MediaStateChangedEventArgs e)
    {
        if (e.NewState == MediaElementState.Playing)
        {
            _isTrackTransitioning = false;
            _consecutivePlaybackFailures = 0;
            PlayPauseButton.Text = "⏸";
        }
        else if (e.NewState is MediaElementState.Paused or MediaElementState.Stopped)
        {
            PlayPauseButton.Text = "▶";
            if (!_isTrackTransitioning)
            {
                PersistPlaybackSnapshot(force: true);
            }
        }
    }

    private void OnPositionDragStarted(object? sender, EventArgs e) => _isSeeking = true;

    private async void OnPositionDragCompleted(object? sender, EventArgs e)
    {
        try
        {
#if ANDROID
            if (_loadedTrackId is not null)
            {
                AndroidPlaybackBridge.Seek(PositionSlider.Value);
                PersistPlaybackSnapshot(force: true);
            }
            else
            {
                _resumePositionSeconds = PositionSlider.Value;
            }
#else
            if (_loadedTrackId is not null && Player.Duration > TimeSpan.Zero)
            {
                await Player.SeekTo(TimeSpan.FromSeconds(PositionSlider.Value));
                PersistPlaybackSnapshot(force: true);
            }
            else
            {
                _resumePositionSeconds = PositionSlider.Value;
                PersistPlaybackSnapshot(force: true);
            }
#endif
        }
        finally
        {
            _isSeeking = false;
        }
    }

    private void OnSleepTimerChanged(object? sender, EventArgs e)
    {
        if (_restoringControls)
        {
            return;
        }

        var index = SleepTimerPicker.SelectedIndex;
        var minutes = index >= 0 && index < SleepTimerMinutes.Length ? SleepTimerMinutes[index] : 0;
        if (minutes == 0)
        {
            _sleepTimer.Cancel();
            _sleepTimerEndUtc = null;
            SleepTimerLabel.Text = "Aus";
        }
        else
        {
            _sleepTimer.Start(TimeSpan.FromMinutes(minutes));
            _sleepTimerEndUtc = DateTimeOffset.UtcNow.AddMinutes(minutes);
            SleepTimerLabel.Text = $"noch {minutes} Min.";
        }
        _stateStore.SaveSleepTimer(minutes, _sleepTimerEndUtc);
#if ANDROID
        UpdateAndroidSettings();
#endif
    }

    private void RestoreSleepTimer(PersistedAppState state)
    {
        if (state.SleepTimerMinutes <= 0
            || state.SleepTimerEndUtc is not { } timerEnd
            || timerEnd <= DateTimeOffset.UtcNow)
        {
            _stateStore.SaveSleepTimer(0, null);
            return;
        }

        var index = Array.IndexOf(SleepTimerMinutes, state.SleepTimerMinutes);
        if (index < 1)
        {
            _stateStore.SaveSleepTimer(0, null);
            return;
        }

        _restoringControls = true;
        SleepTimerPicker.SelectedIndex = index;
        _restoringControls = false;
        _sleepTimerEndUtc = timerEnd;
        _sleepTimer.Start(timerEnd - DateTimeOffset.UtcNow);
        SleepTimerLabel.Text = SleepTimerDisplay.FormatRemaining(_sleepTimer.Remaining);
    }

    private void OnRepeatModeChanged(object? sender, EventArgs e)
    {
        if (_restoringControls)
        {
            return;
        }
        _repeatMode = RepeatModePicker.SelectedIndex == 1
            ? PlaybackRepeatMode.Track
            : PlaybackRepeatMode.SleepWorld;
        _stateStore.SaveRepeatMode(_repeatMode);
#if ANDROID
        UpdateAndroidSettings();
#endif
        UpdateNowPlayingDetails();
        UpdateNextTrack();
    }

    private void OnVolumeChanged(object? sender, ValueChangedEventArgs e)
    {
#if !ANDROID
        Player.Volume = e.NewValue;
#endif
        VolumeLabel.Text = $"{e.NewValue:P0}";
        if (!_restoringControls)
        {
            _stateStore.SaveVolume(e.NewValue);
#if ANDROID
            UpdateAndroidSettings();
#endif
        }
    }

    private async void OnMenuClicked(object? sender, EventArgs e)
    {
        try
        {
            var shuffleAction = _shuffleEnabled
                ? "✓ Zufallswiedergabe ausschalten"
                : "Zufallswiedergabe einschalten";
            var action = await DisplayActionSheetAsync(
                "Menü",
                "Abbrechen",
                null,
                shuffleAction,
                "Bibliothek aktualisieren",
                "Download-Ordner öffnen",
                "Über Open Sleep Music");

            if (action == shuffleAction)
            {
                _shuffleEnabled = !_shuffleEnabled;
                _stateStore.SaveShuffle(_shuffleEnabled);
#if ANDROID
                UpdateAndroidSettings();
#endif
                UpdateNowPlayingDetails();
                UpdateNextTrack();
                StatusLabel.Text = _shuffleEnabled
                    ? "Zufallswiedergabe ist eingeschaltet."
                    : "Wiedergabe erfolgt in Listenreihenfolge.";
            }
            else if (action == "Bibliothek aktualisieren")
            {
                await RefreshLibraryAsync();
            }
            else if (action == "Download-Ordner öffnen")
            {
                await OpenDownloadFolderAsync();
            }
            else if (action == "Über Open Sleep Music")
            {
                await Navigation.PushModalAsync(new AboutPage());
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = "Das Menü konnte nicht geöffnet werden.";
        }
    }

    private void UpdateNowPlayingDetails(bool isRestored = false)
    {
        if (_currentTrack is null)
        {
            return;
        }

        var order = _shuffleEnabled ? "Zufällig" : "Reihenfolge";
        var repeat = _repeatMode == PlaybackRepeatMode.Track ? "Titel wiederholen" : "Schlafwelt wiederholen";
        var resume = isRestored && _resumePositionSeconds > 0
            ? $" · pausiert bei {FormatTime(TimeSpan.FromSeconds(_resumePositionSeconds))}"
            : string.Empty;
        NowPlayingDetailLabel.Text = $"{_currentTrack.Track.Creator} · {order} · {repeat}{resume}";
    }

    private void UpdateNextTrack()
    {
        _nextTrack = null;
        if (_currentTrack is null || _visibleLibrary.Count == 0)
        {
            NextTrackLabel.Text = "Nächster Titel: –";
            return;
        }

        if (_repeatMode == PlaybackRepeatMode.Track)
        {
            _nextTrack = _currentTrack;
        }
        else if (_shuffleEnabled && _visibleLibrary.Count > 1)
        {
            var currentIndex = _visibleLibrary.IndexOf(_currentTrack);
            var nextIndex = currentIndex < 0
                ? 0
                : PlaybackQueue.ChooseDifferent(
                    _visibleLibrary.Count,
                    currentIndex,
                    Random.Shared.Next(_visibleLibrary.Count - 1));
            _nextTrack = _visibleLibrary[nextIndex];
        }
        else
        {
            var currentIndex = _visibleLibrary.IndexOf(_currentTrack);
            var nextIndex = currentIndex < 0 ? 0 : (currentIndex + 1) % _visibleLibrary.Count;
            _nextTrack = _visibleLibrary[nextIndex];
        }

        NextTrackLabel.Text = $"Nächster Titel: {_nextTrack.Track.Title}";
    }

    private async Task OpenDownloadFolderAsync()
    {
        try
        {
            Directory.CreateDirectory(_downloadRoot);
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{_downloadRoot}\"",
                    UseShellExecute = true
                });
                return;
            }

            var opened = await Launcher.Default.OpenAsync(new Uri(_downloadRoot));
            if (!opened)
            {
                StatusLabel.Text = $"Download-Ordner: {_downloadRoot}";
            }
        }
        catch (Exception)
        {
            StatusLabel.Text = $"Download-Ordner: {_downloadRoot}";
        }
    }

    private bool UpdatePlaybackStatus()
    {
#if ANDROID
        var androidState = AndroidPlaybackBridge.Snapshot;
        if (!_isSeeking && androidState.TrackId is not null && androidState.Duration > TimeSpan.Zero)
        {
            PositionSlider.Maximum = androidState.Duration.TotalSeconds;
            PositionSlider.Value = androidState.Position.TotalSeconds;
            TimeLabel.Text = $"{FormatTime(androidState.Position)} / {FormatTime(androidState.Duration)}";
            PersistPlaybackSnapshot();
        }
#else
        if (!_isSeeking && _loadedTrackId is not null && Player.Duration > TimeSpan.Zero)
        {
            PositionSlider.Maximum = Player.Duration.TotalSeconds;
            PositionSlider.Value = Player.Position.TotalSeconds;
            TimeLabel.Text = $"{FormatTime(Player.Position)} / {FormatTime(Player.Duration)}";

            var reachedEnd = Player.Position >= Player.Duration - TimeSpan.FromMilliseconds(500);
            if (_shouldContinuePlayback
                && !_isTrackTransitioning
                && reachedEnd
                && Player.CurrentState != MediaElementState.Playing)
            {
                OnMediaEnded(Player, EventArgs.Empty);
            }

            PersistPlaybackSnapshot();
        }
#endif

        if (_sleepTimer.ConsumeIfElapsed())
        {
            _shouldContinuePlayback = false;
#if ANDROID
            // The Android playback service owns its fade-out while the app is backgrounded.
#else
            _ = FadeOutAndPauseDesktopAsync();
#endif
            PlayPauseButton.Text = "▶";
            _sleepTimerEndUtc = null;
            _restoringControls = true;
            SleepTimerPicker.SelectedIndex = 0;
            _restoringControls = false;
            SleepTimerLabel.Text = "Aus";
            _stateStore.SaveSleepTimer(0, null);
            PersistPlaybackSnapshot(force: true);
        }
        else if (_sleepTimer.IsActive)
        {
            SleepTimerLabel.Text = SleepTimerDisplay.FormatRemaining(_sleepTimer.Remaining);
        }
        return true;
    }

    private void PersistPlaybackSnapshot(bool force = false)
    {
        if (_currentTrack is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        if (!force && now - _lastPlaybackSaveUtc < TimeSpan.FromSeconds(5))
        {
            return;
        }

        var position = _loadedTrackId is null
            ? _resumePositionSeconds
#if ANDROID
            : AndroidPlaybackBridge.Snapshot.Position.TotalSeconds;
#else
            : Player.Position.TotalSeconds;
#endif
        _stateStore.SavePlayback(_currentTrack.Track.Id, position);
        _lastPlaybackSaveUtc = now;
    }

    private void StopAndClearPlayback()
    {
        _shouldContinuePlayback = false;
#if ANDROID
        AndroidPlaybackBridge.Stop();
#else
        Player.Stop();
        Player.Source = null;
#endif
        _loadedTrackId = null;
        _currentTrack = null;
        _nextTrack = null;
        _resumePositionSeconds = 0;
        NowPlayingLabel.Text = "Noch kein Titel ausgewählt";
        NowPlayingDetailLabel.Text = "Schlafwelt auswählen oder einen Titel anklicken.";
        NextTrackLabel.Text = "Nächster Titel: –";
        PositionSlider.Maximum = 1;
        PositionSlider.Value = 0;
        TimeLabel.Text = "0:00 / 0:00";
        PlayPauseButton.Text = "▶";
        _stateStore.ClearPlayback();
    }

#if !ANDROID
    private async Task FadeOutAndPauseDesktopAsync()
    {
        _sleepFadeCancellation?.Cancel();
        _sleepFadeCancellation?.Dispose();
        _sleepFadeCancellation = new CancellationTokenSource();
        var token = _sleepFadeCancellation.Token;
        var configuredVolume = VolumeSlider.Value;
        try
        {
            const int steps = 30;
            for (var step = 1; step <= steps; step++)
            {
                token.ThrowIfCancellationRequested();
                Player.Volume = SleepTimerDisplay.FadeVolume(configuredVolume, step / (double)steps);
                await Task.Delay(TimeSpan.FromMilliseconds(100), token);
            }
            Player.Pause();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Player.Volume = VolumeSlider.Value;
        }
    }
#endif

#if ANDROID
    private void UpdateAndroidSettings() => AndroidPlaybackBridge.UpdateSettings(
        _shuffleEnabled,
        _repeatMode == PlaybackRepeatMode.Track,
        VolumeSlider.Value,
        _sleepTimerEndUtc);

    private void OnAndroidPlaybackStateChanged(object? sender, PlaybackSnapshot snapshot) =>
        ApplyAndroidSnapshot(snapshot);

    private void ApplyAndroidSnapshot(PlaybackSnapshot snapshot)
    {
        if (snapshot.TrackId is null)
        {
            return;
        }
        var track = _library.FirstOrDefault(item => item.Track.Id == snapshot.TrackId);
        if (track is not null && _currentTrack?.Track.Id != snapshot.TrackId)
        {
            _currentTrack = track;
            _loadedTrackId = snapshot.TrackId;
            NowPlayingLabel.Text = track.Track.Title;
            UpdateNowPlayingDetails();
            UpdateNextTrack();
        }
        _shouldContinuePlayback = snapshot.IsPlaying;
        PlayPauseButton.Text = snapshot.IsPlaying ? "⏸" : "▶";
        if (snapshot.Error is not null)
        {
            StatusLabel.Text = "Dieser Titel konnte nicht wiedergegeben werden; der nächste Titel wird geöffnet.";
        }
    }
#endif

    private static string FormatTime(TimeSpan value) =>
        value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"m\:ss");
}

internal sealed class SleepWorldCard(SleepWorld world) : INotifyPropertyChanged
{
    private int _downloadedCount;
    private long _sizeBytes;

    public SleepWorld World { get; } = world;

    public int DownloadedCount
    {
        get => _downloadedCount;
        private set
        {
            if (_downloadedCount == value) return;
            _downloadedCount = value;
            NotifyStatusChanged();
        }
    }

    public long SizeBytes
    {
        get => _sizeBytes;
        private set
        {
            if (_sizeBytes == value) return;
            _sizeBytes = value;
            NotifyStatusChanged();
        }
    }

    public bool HasDownloads => DownloadedCount > 0;

    public bool IsComplete => DownloadedCount == World.Tracks.Count;

    public string ActionText => IsComplete
        ? "▶ Abspielen"
        : DownloadedCount == 0
            ? "Herunterladen"
            : $"Vervollständigen ({DownloadedCount}/{World.Tracks.Count})";

    public string StatusText => DownloadedCount == 0
        ? "Nicht heruntergeladen"
        : IsComplete
            ? $"Vollständig · {DownloadedCount} Titel · {FormatSize(SizeBytes)}"
            : $"{DownloadedCount} von {World.Tracks.Count} Titeln · {FormatSize(SizeBytes)}";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetLibraryStatus(int count, long sizeBytes)
    {
        DownloadedCount = count;
        SizeBytes = sizeBytes;
    }

    private void NotifyStatusChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(propertyName);
        OnPropertyChanged(nameof(HasDownloads));
        OnPropertyChanged(nameof(IsComplete));
        OnPropertyChanged(nameof(ActionText));
        OnPropertyChanged(nameof(StatusText));
    }

    private void OnPropertyChanged(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        var value = (double)bytes;
        var unit = 0;
        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }
        return $"{value:0.#} {units[unit]}";
    }
}

internal sealed record LibraryTrackItem(LocalLibraryTrack LocalTrack, bool IsFavorite, bool IsBlocked)
{
    public string FavoriteGlyph => IsFavorite ? "★" : "☆";
    public Color FavoriteColor => IsFavorite ? Color.FromArgb("#F4C95D") : Color.FromArgb("#9E9AAF");
    public double Opacity => IsBlocked ? 0.45 : 1;
}

internal static class LibraryTrackListExtensions
{
    public static int IndexOf(this IReadOnlyList<LocalLibraryTrack> tracks, LocalLibraryTrack track)
    {
        for (var index = 0; index < tracks.Count; index++)
        {
            if (tracks[index] == track)
            {
                return index;
            }
        }
        return -1;
    }
}
