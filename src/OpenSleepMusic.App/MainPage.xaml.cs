using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;
using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;
using OpenSleepMusic.App.Localization;
using OpenSleepMusic.App.Playback;

namespace OpenSleepMusic.App;

public partial class MainPage : ContentPage
{
    private const int MaximumAutomaticFailureSkips = 3;
    private static readonly int[] SleepTimerMinutes = [0, 15, 30, 45, 60, 90];
    private static readonly string[] SleepTimerLabels = ["Aus", "15 Minuten", "30 Minuten", "45 Minuten", "60 Minuten", "90 Minuten"];
    private static readonly string[] RepeatModeLabels = ["Themensammlung", "Einzeltitel"];
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
    private int _sleepTimerMinutes;
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
    private DateTimeOffset? _statusHideAtUtc;
    private bool _downloadInProgress;
#if !ANDROID
    private CancellationTokenSource? _sleepFadeCancellation;
#endif

    public MainPage()
    {
        InitializeComponent();
        HeaderSubtitle.Text = AppText.Get("Subtitle");
        WorldsTitleLabel.Text = AppText.Get("Collections");
        NowPlayingLabel.Text = AppText.Get("NoTrack");
        StatusLabel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(Label.Text)) return;
            StatusLabel.IsVisible = true;
            _statusHideAtUtc = _downloadInProgress ? null : DateTimeOffset.UtcNow.AddSeconds(7);
        };
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
        _sleepTimerMinutes = _initialState.SleepTimerMinutes;
        _restoringControls = true;
        SleepTimerPicker.ItemsSource = SleepTimerLabels;
        SleepTimerPicker.SelectedIndex = Math.Max(0, Array.IndexOf(SleepTimerMinutes, _sleepTimerMinutes));
        RepeatModePicker.ItemsSource = RepeatModeLabels;
        RepeatModePicker.SelectedIndex = _repeatMode == PlaybackRepeatMode.Track ? 1 : 0;
        VolumeSlider.Value = _initialState.Volume;
        AppVolumeBridge.Set(_initialState.Volume);
        AppVolumeBridge.Changed += OnAppVolumeBridgeChanged;
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
        Unloaded += (_, _) =>
        {
            AppVolumeBridge.Changed -= OnAppVolumeBridgeChanged;
#if ANDROID
            AndroidPlaybackBridge.StateChanged -= OnAndroidPlaybackStateChanged;
#endif
        };
        var volumeOverlay = new VolumeOverlayView();
        Grid.SetRowSpan(volumeOverlay, 4);
        RootGrid.Add(volumeOverlay);
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
        LibraryPanel.IsVisible = false;
        Grid.SetColumnSpan(WorldsPanel, narrow ? 1 : 2);

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

        PlayerOptionsGrid.IsVisible = false;
        VolumePanel.IsVisible = false;
        NextTrackLabel.IsVisible = false;
        SetRows(ContentGrid, GridLength.Star);
        Grid.SetRow(WorldsPanel, 0);
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
                PlayInitialTrack();
                await Navigation.PushModalAsync(new ImmersivePlayerPage(this));
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
                card.DisplayName,
                AppText.Get("Close"),
                AppText.Pick("Sammlung löschen", "Delete collection"),
                AppText.Pick("Titel anzeigen", "Show tracks"),
                AppText.Pick("Sammlung prüfen und reparieren", "Check and repair collection"));

            if (action == AppText.Pick("Titel anzeigen", "Show tracks"))
            {
                await OpenCollectionTracksAsync(card);
            }
            else if (action == AppText.Pick("Sammlung prüfen und reparieren", "Check and repair collection"))
            {
                await DownloadWorldAsync(card, button, isRepair: true);
            }
            else if (action == AppText.Pick("Sammlung löschen", "Delete collection"))
            {
                await DeleteWorldAsync(card);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = AppText.Pick("Die Sammlungsverwaltung konnte nicht geöffnet werden.", "Collection management could not be opened.");
        }
    }

    private async Task OpenCollectionTracksAsync(SleepWorldCard card)
    {
        var tracks = _library.Where(track => track.SleepWorld.Id == card.World.Id).ToArray();
        if (tracks.Length == 0)
        {
            StatusLabel.Text = AppText.Pick("Für diese Themensammlung ist noch keine Musik heruntergeladen.", "No music has been downloaded for this collection yet.");
            return;
        }
        var page = new CollectionTracksPage(card.DisplayName, tracks, _stateStore);
        page.PlayRequested += async (_, track) =>
        {
            PlayTrack(track, userInitiated: true);
            await Navigation.PushModalAsync(new ImmersivePlayerPage(this));
        };
        page.PreferenceChanged += (_, track) => ApplyTrackPreferences(track);
        await Navigation.PushModalAsync(page);
    }

    private async Task DownloadWorldAsync(SleepWorldCard card, Button button, bool isRepair)
    {
        _downloadInProgress = true;
        _statusHideAtUtc = null;
        StatusLabel.IsVisible = true;
        DownloadProgress.IsVisible = true;
        button.IsEnabled = false;
        DownloadProgress.Progress = 0;
        StatusLabel.Text = isRepair
            ? AppText.Pick($"{card.DisplayName} wird geprüft und repariert …", $"Checking and repairing {card.DisplayName} …")
            : AppText.Pick($"{card.DisplayName} wird vorbereitet …", $"Preparing {card.DisplayName} …");

        var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs", "downloads.jsonl");
        var downloader = new SleepWorldDownloader(_httpClient, new FileDownloadLogSink(logPath));
        var progress = new Progress<DownloadProgress>(value =>
        {
            DownloadProgress.Progress = value.Total == 0 ? 0 : (double)value.Completed / value.Total;
            if (!string.IsNullOrWhiteSpace(value.CurrentTitle))
            {
                StatusLabel.Text = isRepair
                    ? AppText.Pick($"Prüfe {value.CurrentTitle} …", $"Checking {value.CurrentTitle} …")
                    : AppText.Pick($"Lade {value.CurrentTitle} …", $"Downloading {value.CurrentTitle} …");
            }
        });

        try
        {
            var result = await downloader.DownloadAsync(card.World, _downloadRoot, progress);
            StatusLabel.Text = result.AvailableCount == 0
                ? AppText.Pick("Derzeit sind keine Titel verfügbar. Bitte später erneut versuchen.", "No tracks are currently available. Please try again later.")
                : isRepair
                    ? AppText.Pick($"{card.DisplayName}: Prüfung abgeschlossen, {result.AvailableCount} Titel verfügbar.", $"{card.DisplayName}: check complete, {result.AvailableCount} tracks available.")
                    : AppText.Pick($"{card.DisplayName}: {result.AvailableCount} Titel sind offline verfügbar.", $"{card.DisplayName}: {result.AvailableCount} tracks available offline.");
            await RefreshLibraryAsync(preserveStatus: true);
        }
        catch (OperationCanceledException)
        {
            StatusLabel.Text = AppText.Pick("Download angehalten.", "Download stopped.");
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = AppText.Pick("Der Download wurde unerwartet beendet. Andere Sammlungen können weiter verwendet werden.", "The download ended unexpectedly. Other collections remain available.");
        }
        finally
        {
            _downloadInProgress = false;
            _statusHideAtUtc = DateTimeOffset.UtcNow.AddSeconds(7);
            button.IsEnabled = true;
        }
    }

    private async Task DeleteWorldAsync(SleepWorldCard card)
    {
        var confirmed = await DisplayAlertAsync(
            AppText.Pick("Sammlung löschen", "Delete collection"),
            AppText.Pick($"Alle heruntergeladenen Dateien aus „{card.DisplayName}“ werden gelöscht. Andere Themensammlungen bleiben erhalten.", $"All downloaded files in “{card.DisplayName}” will be deleted. Other collections remain available."),
            AppText.Pick("Löschen", "Delete"),
            AppText.Get("Close"));
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
            StatusLabel.Text = AppText.Pick($"{card.DisplayName} wurde aus der lokalen Bibliothek gelöscht.", $"{card.DisplayName} was deleted from the local library.");
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = AppText.Pick($"{card.DisplayName} konnte nicht vollständig gelöscht werden.", $"{card.DisplayName} could not be deleted completely.");
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
                    ? AppText.Pick("Noch keine gültigen Audiodateien vorhanden.", "No valid audio files available yet.")
                    : AppText.Pick($"{_library.Count} Titel offline verfügbar.", $"{_library.Count} tracks available offline.");
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = AppText.Pick("Die lokale Musikbibliothek konnte nicht aktualisiert werden.", "The local music library could not be refreshed.");
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
            ? AppText.Pick("Meine Musik", "My music")
            : $"{AppText.Pick("Meine Musik", "My music")} · {_selectedWorldCard.DisplayName}";
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
                StatusLabel.Text = AppText.Pick("Dieser Titel ist blockiert. Die Blockierung kann über ⓘ aufgehoben werden.", "This track is blocked. You can unblock it via ⓘ.");
                return;
            }
            if (!_visibleLibrary.Contains(item.LocalTrack))
            {
                StatusLabel.Text = AppText.Pick("Für diese Themensammlung werden derzeit nur Favoriten abgespielt.", "Only favorites are currently played for this collection.");
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
            PlayInitialTrack();
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
        StartConfiguredSleepTimerIfNeeded();

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
        Player.Volume = PlaybackVolume.ApplyGain(VolumeSlider.Value, track.Track.VolumeGain);
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
                PlayInitialTrack();
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
        if (_currentTrack is null && _visibleLibrary.Count > 0) PlayInitialTrack();
        else AndroidPlaybackBridge.Previous();
#else
        MoveTrack(-1, forceSequential: true);
#endif
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
#if ANDROID
        if (_currentTrack is null && _visibleLibrary.Count > 0) PlayInitialTrack();
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

    private void PlayInitialTrack()
    {
        if (_visibleLibrary.Count == 0)
        {
            return;
        }
        var index = _shuffleEnabled && _visibleLibrary.Count > 1
            ? PlaybackQueue.ChooseDifferent(_visibleLibrary.Count, 0, Random.Shared.Next(_visibleLibrary.Count - 1))
            : 0;
        PlayTrack(_visibleLibrary[index], userInitiated: true);
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
            StatusLabel.Text = AppText.Pick("Mehrere Titel konnten nicht wiedergegeben werden. Bitte einen anderen Titel auswählen.", "Several tracks could not be played. Please select another track.");
            return;
        }

        StatusLabel.Text = AppText.Pick("Dieser Titel konnte nicht wiedergegeben werden; der nächste Titel wird geöffnet.", "This track could not be played; opening the next track.");
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
        _sleepTimerMinutes = minutes;
        if (minutes == 0)
        {
            _sleepTimer.Cancel();
            _sleepTimerEndUtc = null;
            SleepTimerLabel.Text = "Aus";
        }
        else
        {
            if (_loadedTrackId is not null && _shouldContinuePlayback)
            {
                _sleepTimer.Start(TimeSpan.FromMinutes(minutes));
                _sleepTimerEndUtc = DateTimeOffset.UtcNow.AddMinutes(minutes);
                SleepTimerLabel.Text = $"noch {minutes} Min.";
            }
            else
            {
                _sleepTimer.Cancel();
                _sleepTimerEndUtc = null;
                SleepTimerLabel.Text = $"{minutes} Min.";
            }
        }
        _stateStore.SaveSleepTimer(minutes, _sleepTimerEndUtc);
#if ANDROID
        UpdateAndroidSettings();
#endif
    }

    private void RestoreSleepTimer(PersistedAppState state)
    {
        _sleepTimerMinutes = state.SleepTimerMinutes;
        var index = Array.IndexOf(SleepTimerMinutes, _sleepTimerMinutes);
        if (index < 0)
        {
            _sleepTimerMinutes = 60;
            index = Array.IndexOf(SleepTimerMinutes, _sleepTimerMinutes);
        }
        _restoringControls = true;
        SleepTimerPicker.SelectedIndex = index;
        _restoringControls = false;

        if (_sleepTimerMinutes <= 0)
        {
            _sleepTimer.Cancel();
            _sleepTimerEndUtc = null;
            SleepTimerLabel.Text = "Aus";
            return;
        }
        if (state.SleepTimerEndUtc is not { } timerEnd || timerEnd <= DateTimeOffset.UtcNow)
        {
            _sleepTimer.Cancel();
            _sleepTimerEndUtc = null;
            SleepTimerLabel.Text = $"{_sleepTimerMinutes} Min.";
            _stateStore.SaveSleepTimer(_sleepTimerMinutes, null);
            return;
        }
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
        Player.Volume = PlaybackVolume.ApplyGain(e.NewValue, _currentTrack?.Track.VolumeGain ?? 1);
#endif
        VolumeLabel.Text = $"{e.NewValue:P0}";
        if (!_restoringControls)
        {
            if (Math.Abs(AppVolumeBridge.Volume - e.NewValue) > .001) AppVolumeBridge.Set(e.NewValue);
            _stateStore.SaveVolume(e.NewValue);
#if ANDROID
            UpdateAndroidSettings();
#endif
        }
    }

    private void OnAppVolumeBridgeChanged(object? sender, double volume) => Dispatcher.Dispatch(() =>
    {
        if (Math.Abs(VolumeSlider.Value - volume) <= .001) return;
        VolumeSlider.Value = volume;
    });

    private async void OnMenuClicked(object? sender, EventArgs e)
    {
        try
        {
            var shuffleAction = _shuffleEnabled
                ? (AppText.IsGerman ? "✓ Zufallswiedergabe ausschalten" : "✓ Disable shuffle")
                : (AppText.IsGerman ? "Zufallswiedergabe einschalten" : "Enable shuffle");
            var refreshAction = AppText.IsGerman ? "Bibliothek aktualisieren" : "Refresh library";
            var settingsAction = AppText.IsGerman ? "Wiedergabe-Einstellungen" : "Playback settings";
            var feedbackAction = AppText.IsGerman ? "Katalog-Feedback teilen" : "Share catalog feedback";
            var folderAction = AppText.IsGerman ? "Download-Ordner öffnen" : "Open download folder";
            var aboutAction = AppText.IsGerman ? "Über Open Sleep Music" : "About Open Sleep Music";
            var action = await DisplayActionSheetAsync(
                AppText.IsGerman ? "Menü" : "Menu",
                AppText.Get("Close"),
                null,
                shuffleAction,
                refreshAction, settingsAction, feedbackAction, folderAction, aboutAction);

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
                    ? AppText.Pick("Zufallswiedergabe ist eingeschaltet.", "Shuffle is enabled.")
                    : AppText.Pick("Wiedergabe erfolgt in Listenreihenfolge.", "Tracks play in list order.");
            }
            else if (action == refreshAction)
            {
                await RefreshLibraryAsync();
            }
            else if (action == settingsAction)
            {
                await OpenPlayerSettingsAsync();
            }
            else if (action == feedbackAction)
            {
                await ShareCatalogFeedbackAsync();
            }
            else if (action == folderAction)
            {
                await OpenDownloadFolderAsync();
            }
            else if (action == aboutAction)
            {
                await Navigation.PushModalAsync(new AboutPage());
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = AppText.Pick("Das Menü konnte nicht geöffnet werden.", "The menu could not be opened.");
        }
    }

    private void OnPlayerSettingsChanged(object? sender, PlayerSettings settings)
    {
        var languageChanged = settings.Language != _stateStore.LoadLanguage();
        _stateStore.SaveLanguage(settings.Language);
        _stateStore.SaveReducedMotion(settings.ReducedMotion);
        VolumeSlider.Value = settings.Volume;
        RepeatModePicker.SelectedIndex = settings.RepeatMode == PlaybackRepeatMode.Track ? 1 : 0;
        SleepTimerPicker.SelectedIndex = Array.IndexOf(SleepTimerMinutes, settings.SleepTimerMinutes);
        if (languageChanged)
        {
            AppText.Apply(settings.Language);
            if (Application.Current?.Windows.FirstOrDefault() is { } window) window.Page = new AppShell();
        }
    }

    internal bool ReducedMotion => _stateStore.LoadReducedMotion();

    internal ImmersivePlayerState GetImmersiveState()
    {
#if ANDROID
        var snapshot = AndroidPlaybackBridge.Snapshot;
        var isPlaying = snapshot.IsPlaying;
        var position = snapshot.Position;
        var duration = snapshot.Duration;
#else
        var isPlaying = Player.CurrentState == MediaElementState.Playing;
        var position = Player.Position;
        var duration = Player.Duration;
#endif
        var worldId = _currentTrack?.SleepWorld.Id;
        var trackId = _currentTrack?.Track.Id;
        return new ImmersivePlayerState(
            trackId,
            worldId,
            _currentTrack?.Track.Title,
            isPlaying,
            worldId is not null && trackId is not null && _stateStore.IsFavorite(worldId, trackId),
            worldId is not null && trackId is not null && _stateStore.IsBlocked(worldId, trackId),
            _repeatMode == PlaybackRepeatMode.Track,
            _sleepTimer.IsActive ? SleepTimerDisplay.FormatRemaining(_sleepTimer.Remaining) : AppText.Get("Off"),
            position,
            duration);
    }

    internal void ImmersiveTogglePlayback() => OnPlayPauseClicked(null, EventArgs.Empty);
    internal void ImmersiveMove(int offset)
    {
        if (offset < 0) OnPreviousClicked(null, EventArgs.Empty); else OnNextClicked(null, EventArgs.Empty);
    }

    internal void ImmersiveSeek(double seconds)
    {
#if ANDROID
        AndroidPlaybackBridge.Seek(seconds);
#else
        _ = Player.SeekTo(TimeSpan.FromSeconds(Math.Max(0, seconds)));
#endif
    }

    internal void ImmersiveToggleFavorite()
    {
        if (_currentTrack is null) return;
        var worldId = _currentTrack.SleepWorld.Id;
        var trackId = _currentTrack.Track.Id;
        _stateStore.SetFavorite(worldId, trackId, !_stateStore.IsFavorite(worldId, trackId));
        ApplyTrackPreferences(_currentTrack);
    }

    internal void ImmersiveToggleBlocked()
    {
        if (_currentTrack is null) return;
        var track = _currentTrack;
        var worldId = track.SleepWorld.Id;
        var trackId = track.Track.Id;
        _stateStore.SetBlocked(worldId, trackId, !_stateStore.IsBlocked(worldId, trackId));
        ApplyTrackPreferences(track);
    }

    internal Task OpenCurrentTrackDetailsAsync() => _currentTrack is null ? Task.CompletedTask : OpenTrackDetailsAsync(_currentTrack);

    internal async Task OpenPlayerSettingsAsync()
    {
        var page = new PlayerSettingsPage(new PlayerSettings(VolumeSlider.Value, _repeatMode, _sleepTimerMinutes, _stateStore.LoadLanguage(), _stateStore.LoadReducedMotion()));
        page.SettingsChanged += OnPlayerSettingsChanged;
        await Navigation.PushModalAsync(page);
    }

    internal async Task ChooseRepeatModeAsync()
    {
        var collection = AppText.Get("Collection");
        var track = AppText.Get("Track");
        var selected = await DisplayActionSheetAsync(AppText.Get("Repeat"), AppText.Get("Close"), null, collection, track);
        if (selected == collection) RepeatModePicker.SelectedIndex = 0;
        if (selected == track) RepeatModePicker.SelectedIndex = 1;
    }

    internal async Task ChooseSleepTimerAsync()
    {
        var labels = new[] { AppText.Get("Off"), "15 min", "30 min", "45 min", "60 min", "90 min" };
        var selected = await DisplayActionSheetAsync(AppText.Get("SleepTimer"), AppText.Get("Close"), null, labels);
        var index = Array.IndexOf(labels, selected);
        if (index >= 0) SleepTimerPicker.SelectedIndex = index;
    }

    private void StartConfiguredSleepTimerIfNeeded()
    {
        if (_sleepTimerMinutes <= 0 || _sleepTimerEndUtc is not null)
        {
            return;
        }
        _sleepTimer.Start(TimeSpan.FromMinutes(_sleepTimerMinutes));
        _sleepTimerEndUtc = DateTimeOffset.UtcNow.AddMinutes(_sleepTimerMinutes);
        SleepTimerLabel.Text = $"noch {_sleepTimerMinutes} Min.";
        _stateStore.SaveSleepTimer(_sleepTimerMinutes, _sleepTimerEndUtc);
    }

    private async Task ShareCatalogFeedbackAsync()
    {
        var comment = await DisplayPromptAsync(
            "Katalog-Feedback",
            "Optional kannst du ergänzen, warum ein Titel besonders gut oder ungeeignet ist. Im nächsten Schritt siehst du vor dem Teilen eine Zusammenfassung.",
            "Weiter",
            "Abbrechen",
            "Optionaler Kommentar",
            maxLength: 500,
            keyboard: Keyboard.Text);
        if (comment is null)
        {
            return;
        }

        var ratings = BuiltInCatalog.SleepWorlds
            .SelectMany(world => world.Tracks.Select(track => new { World = world, Track = track }))
            .SelectMany(item =>
            {
                var state = _stateStore.IsBlocked(item.World.Id, item.Track.Id)
                    ? "blocked"
                    : _stateStore.IsFavorite(item.World.Id, item.Track.Id)
                        ? "favorite"
                        : null;
                return state is null
                    ? []
                    : new[] { new CatalogFeedbackEntry(item.World.Id, item.World.Name, item.Track.Id, item.Track.Title, state) };
            })
            .ToArray();

        if (ratings.Length == 0 && string.IsNullOrWhiteSpace(comment))
        {
            await DisplayAlertAsync(
                "Kein Feedback vorhanden",
                "Es wurden noch keine Favoriten oder Blockierungen gesetzt und kein Kommentar eingegeben.",
                "OK");
            return;
        }

        var favoriteCount = ratings.Count(item => item.State == "favorite");
        var blockedCount = ratings.Count(item => item.State == "blocked");
        var confirmed = await DisplayAlertAsync(
            "Feedback jetzt teilen?",
            $"Enthalten sind {favoriteCount} Favorit(en), {blockedCount} Blockierung(en), App-Version und Plattform{(string.IsNullOrWhiteSpace(comment) ? "." : " sowie dein Kommentar.")} Keine Gerätekennung, Abspielhistorie oder Dateipfade werden aufgenommen.",
            "Teilen",
            "Abbrechen");
        if (!confirmed)
        {
            return;
        }

        var report = new CatalogFeedbackReport(
            1,
            DateTimeOffset.UtcNow,
            AppInfo.Current.VersionString,
            DeviceInfo.Current.Platform.ToString(),
            BuiltInCatalog.SleepWorlds.Sum(world => world.Tracks.Count),
            ratings,
            string.IsNullOrWhiteSpace(comment) ? null : comment.Trim());
        var json = JsonSerializer.Serialize(report, FeedbackJsonContext.Default.CatalogFeedbackReport);
        var path = Path.Combine(FileSystem.CacheDirectory, $"open-sleep-music-feedback-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json");
        await File.WriteAllTextAsync(path, json);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Open Sleep Music – Katalog-Feedback",
            File = new ShareFile(path, "application/json")
        });
        StatusLabel.Text = "Der Feedback-Bericht wurde zum Teilen bereitgestellt.";
    }

    private void UpdateNowPlayingDetails(bool isRestored = false)
    {
        if (_currentTrack is null)
        {
            return;
        }

        var order = _shuffleEnabled ? "Zufällig" : "Reihenfolge";
        var repeat = _repeatMode == PlaybackRepeatMode.Track ? "Titel wiederholen" : "Themensammlung wiederholen";
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
        if (!_downloadInProgress && _statusHideAtUtc is { } hideAt && DateTimeOffset.UtcNow >= hideAt)
        {
            StatusLabel.IsVisible = false;
            DownloadProgress.IsVisible = false;
            _statusHideAtUtc = null;
        }
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
            SleepTimerLabel.Text = _sleepTimerMinutes <= 0 ? "Aus" : $"{_sleepTimerMinutes} Min.";
            _stateStore.SaveSleepTimer(_sleepTimerMinutes, null);
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
        NowPlayingLabel.Text = AppText.Get("NoTrack");
        NowPlayingDetailLabel.Text = AppText.Pick("Themensammlung auswählen oder einen Titel anklicken.", "Select a collection or tap a track.");
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
        var configuredVolume = PlaybackVolume.ApplyGain(VolumeSlider.Value, _currentTrack?.Track.VolumeGain ?? 1);
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
            Player.Volume = PlaybackVolume.ApplyGain(VolumeSlider.Value, _currentTrack?.Track.VolumeGain ?? 1);
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
            StatusLabel.Text = AppText.Pick("Dieser Titel konnte nicht wiedergegeben werden; der nächste Titel wird geöffnet.", "This track could not be played; opening the next track.");
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
    public string DisplayName => AppText.WorldName(World.Id, World.Name);
    public string DisplayDescription => AppText.WorldDescription(World.Id, World.Description);

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
        ? $"▶ {AppText.Get("Play")}"
        : DownloadedCount == 0
            ? AppText.Get("Download")
            : AppText.IsGerman ? $"Vervollständigen ({DownloadedCount}/{World.Tracks.Count})" : $"Complete ({DownloadedCount}/{World.Tracks.Count})";

    public string StatusText => DownloadedCount == 0
        ? (AppText.IsGerman ? "Nicht heruntergeladen" : "Not downloaded")
        : IsComplete
            ? (AppText.IsGerman ? $"Vollständig · {DownloadedCount} Titel · {FormatSize(SizeBytes)}" : $"Complete · {DownloadedCount} tracks · {FormatSize(SizeBytes)}")
            : (AppText.IsGerman ? $"{DownloadedCount} von {World.Tracks.Count} Titeln · {FormatSize(SizeBytes)}" : $"{DownloadedCount} of {World.Tracks.Count} tracks · {FormatSize(SizeBytes)}");

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
