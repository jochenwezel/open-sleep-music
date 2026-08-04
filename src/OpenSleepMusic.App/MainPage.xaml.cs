using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;
using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.App;

public partial class MainPage : ContentPage
{
    private const int MaximumAutomaticFailureSkips = 3;
    private readonly HttpClient _httpClient;
    private readonly LocalLibraryScanner _libraryScanner = new();
    private readonly SleepTimer _sleepTimer = new();
    private readonly IReadOnlyList<SleepWorldCard> _worldCards;
    private readonly string _downloadRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
        "Open Sleep Music");
    private IReadOnlyList<LocalLibraryTrack> _library = [];
    private IReadOnlyList<LocalLibraryTrack> _visibleLibrary = [];
    private SleepWorldCard? _selectedWorldCard;
    private LocalLibraryTrack? _currentTrack;
    private bool _shuffleEnabled;
    private bool _isSeeking;
    private bool _shouldContinuePlayback;
    private bool _isTrackTransitioning;
    private int _consecutivePlaybackFailures;

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "OpenSleepMusic/0.1 (+https://github.com/jochenwezel/open-sleep-music)");
        _worldCards = BuiltInCatalog.SleepWorlds.Select(world => new SleepWorldCard(world)).ToArray();
        WorldsView.ItemsSource = _worldCards;
        SleepTimerPicker.ItemsSource = new[] { "Aus", "15 Minuten", "30 Minuten", "45 Minuten", "60 Minuten", "90 Minuten" };
        SleepTimerPicker.SelectedIndex = 0;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), UpdatePlaybackStatus);
        Loaded += async (_, _) => await RefreshLibraryAsync();
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

        button.IsEnabled = false;
        DownloadProgress.Progress = 0;
        StatusLabel.Text = $"{card.World.Name} wird vorbereitet …";

        var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs", "downloads.jsonl");
        var downloader = new SleepWorldDownloader(_httpClient, new FileDownloadLogSink(logPath));
        var progress = new Progress<DownloadProgress>(value =>
        {
            DownloadProgress.Progress = value.Total == 0 ? 0 : (double)value.Completed / value.Total;
            if (!string.IsNullOrWhiteSpace(value.CurrentTitle))
            {
                StatusLabel.Text = $"Lade {value.CurrentTitle} …";
            }
        });

        try
        {
            var result = await downloader.DownloadAsync(card.World, _downloadRoot, progress);
            StatusLabel.Text = result.AvailableCount == 0
                ? "Derzeit sind keine Titel verfügbar. Bitte später erneut versuchen."
                : $"{card.World.Name}: {result.AvailableCount} Titel sind offline verfügbar.";
            await RefreshLibraryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusLabel.Text = "Download angehalten.";
        }
        catch (Exception)
        {
            StatusLabel.Text = "Der Download wurde unerwartet beendet. Andere Sammlungen können weiter verwendet werden.";
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

    private void OnWorldSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SleepWorldCard card)
        {
            SelectWorld(card);
        }
    }

    private void SelectWorld(SleepWorldCard card)
    {
        _selectedWorldCard = card;
        if (WorldsView.SelectedItem != card)
        {
            WorldsView.SelectedItem = card;
        }
        ApplyLibraryFilter();
    }

    private async Task RefreshLibraryAsync()
    {
        try
        {
            _library = await _libraryScanner.ScanAsync(_downloadRoot, BuiltInCatalog.SleepWorlds);
            foreach (var card in _worldCards)
            {
                card.SetDownloadedCount(_library.Count(track => track.SleepWorld.Id == card.World.Id));
            }

            var cardToSelect = _selectedWorldCard
                ?? _worldCards.FirstOrDefault(card => card.DownloadedCount > 0)
                ?? _worldCards[0];
            SelectWorld(cardToSelect);
            StatusLabel.Text = _library.Count == 0
                ? "Noch keine gültigen Audiodateien vorhanden."
                : $"{_library.Count} Titel offline verfügbar.";
        }
        catch (Exception)
        {
            StatusLabel.Text = "Die lokale Musikbibliothek konnte nicht aktualisiert werden.";
        }
    }

    private void ApplyLibraryFilter()
    {
        _visibleLibrary = _selectedWorldCard is null
            ? []
            : _library.Where(track => track.SleepWorld.Id == _selectedWorldCard.World.Id).ToArray();
        LibraryView.ItemsSource = _visibleLibrary;
        LibraryEmptyLabel.IsVisible = _visibleLibrary.Count == 0;
        LibraryTitleLabel.Text = _selectedWorldCard is null
            ? "Meine Musik"
            : $"Meine Musik · {_selectedWorldCard.World.Name}";
    }

    private void OnLibrarySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LocalLibraryTrack track)
        {
            PlayTrack(track, userInitiated: true);
        }
    }

    private void PlayTrack(LocalLibraryTrack track, bool userInitiated = false)
    {
        if (userInitiated)
        {
            _consecutivePlaybackFailures = 0;
        }

        _currentTrack = track;
        _shouldContinuePlayback = true;
        _isTrackTransitioning = true;
        NowPlayingLabel.Text = track.Track.Title;
        UpdateNowPlayingDetails();
        Player.MetadataTitle = track.Track.Title;
        Player.MetadataArtist = track.Track.Creator;
        Player.Source = MediaSource.FromFile(track.FilePath);
        Player.Play();
        PlayPauseButton.Text = "⏸";
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

        if (Player.CurrentState == MediaElementState.Playing)
        {
            _shouldContinuePlayback = false;
            Player.Pause();
            PlayPauseButton.Text = "▶";
        }
        else
        {
            _shouldContinuePlayback = true;
            Player.Play();
            PlayPauseButton.Text = "⏸";
        }
    }

    private void OnPreviousClicked(object? sender, EventArgs e) => MoveTrack(-1, forceSequential: true);

    private void OnNextClicked(object? sender, EventArgs e) => MoveTrack(1);

    private void MoveTrack(int offset, bool forceSequential = false)
    {
        if (_visibleLibrary.Count == 0)
        {
            return;
        }

        var currentIndex = _currentTrack is null ? -1 : _visibleLibrary.IndexOf(_currentTrack);
        int nextIndex;
        if (_shuffleEnabled && !forceSequential && _visibleLibrary.Count > 1)
        {
            nextIndex = Random.Shared.Next(_visibleLibrary.Count - 1);
            if (nextIndex >= currentIndex && currentIndex >= 0)
            {
                nextIndex++;
            }
        }
        else
        {
            if (currentIndex < 0)
            {
                currentIndex = offset < 0 ? 0 : -1;
            }
            nextIndex = (currentIndex + offset + _visibleLibrary.Count) % _visibleLibrary.Count;
        }

        PlayTrack(_visibleLibrary[nextIndex]);
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        if (_shouldContinuePlayback)
        {
            MoveTrack(1);
        }
    }

    private void OnMediaFailed(object? sender, MediaFailedEventArgs e)
    {
        _isTrackTransitioning = false;
        _consecutivePlaybackFailures++;
        if (_consecutivePlaybackFailures >= Math.Min(MaximumAutomaticFailureSkips, _visibleLibrary.Count))
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
        }
    }

    private void OnPositionDragStarted(object? sender, EventArgs e) => _isSeeking = true;

    private async void OnPositionDragCompleted(object? sender, EventArgs e)
    {
        try
        {
            if (Player.Duration > TimeSpan.Zero)
            {
                await Player.SeekTo(TimeSpan.FromSeconds(PositionSlider.Value));
            }
        }
        finally
        {
            _isSeeking = false;
        }
    }

    private void OnSleepTimerChanged(object? sender, EventArgs e)
    {
        int[] minutes = [0, 15, 30, 45, 60, 90];
        var index = SleepTimerPicker.SelectedIndex;
        if (index <= 0)
        {
            _sleepTimer.Cancel();
            SleepTimerLabel.Text = "Aus";
        }
        else
        {
            _sleepTimer.Start(TimeSpan.FromMinutes(minutes[index]));
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
                "Download-Ordner öffnen");

            if (action == shuffleAction)
            {
                _shuffleEnabled = !_shuffleEnabled;
                UpdateNowPlayingDetails();
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
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            StatusLabel.Text = "Das Menü konnte nicht geöffnet werden.";
        }
    }

    private void UpdateNowPlayingDetails()
    {
        if (_currentTrack is null)
        {
            return;
        }

        var mode = _shuffleEnabled ? "Zufällig" : "Reihenfolge";
        NowPlayingDetailLabel.Text = $"{_currentTrack.Track.Creator} · {_currentTrack.SleepWorld.Name} · {mode}";
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
        if (!_isSeeking && Player.Duration > TimeSpan.Zero)
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
                MoveTrack(1);
            }
        }

        if (_sleepTimer.ConsumeIfElapsed())
        {
            _shouldContinuePlayback = false;
            Player.Pause();
            PlayPauseButton.Text = "▶";
            SleepTimerPicker.SelectedIndex = 0;
        }
        else if (_sleepTimer.IsActive)
        {
            SleepTimerLabel.Text = $"noch {Math.Ceiling(_sleepTimer.Remaining.TotalMinutes):0} Min.";
        }
        return true;
    }

    private static string FormatTime(TimeSpan value) =>
        value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"m\:ss");
}

internal sealed class SleepWorldCard(SleepWorld world) : INotifyPropertyChanged
{
    private int _downloadedCount;

    public SleepWorld World { get; } = world;

    public int DownloadedCount
    {
        get => _downloadedCount;
        private set
        {
            if (_downloadedCount == value)
            {
                return;
            }
            _downloadedCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsComplete));
            OnPropertyChanged(nameof(ActionText));
        }
    }

    public bool IsComplete => DownloadedCount == World.Tracks.Count;

    public string ActionText => IsComplete
        ? "▶ Abspielen"
        : DownloadedCount == 0
            ? "Herunterladen"
            : $"Vervollständigen ({DownloadedCount}/{World.Tracks.Count})";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetDownloadedCount(int count) => DownloadedCount = count;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
