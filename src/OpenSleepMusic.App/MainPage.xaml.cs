using System.Diagnostics;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;
using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.App;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly LocalLibraryScanner _libraryScanner = new();
    private readonly SleepTimer _sleepTimer = new();
    private IReadOnlyList<LocalLibraryTrack> _library = [];
    private LocalLibraryTrack? _currentTrack;
    private bool _isSeeking;
    private int _consecutivePlaybackFailures;
    private const int MaximumAutomaticFailureSkips = 3;
    private readonly string _downloadRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
        "Open Sleep Music");

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "OpenSleepMusic/0.1 (+https://github.com/jochenwezel/open-sleep-music)");
        WorldsView.ItemsSource = BuiltInCatalog.SleepWorlds;
        SleepTimerPicker.ItemsSource = new[] { "Aus", "15 Minuten", "30 Minuten", "45 Minuten", "60 Minuten", "90 Minuten" };
        SleepTimerPicker.SelectedIndex = 0;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), UpdatePlaybackStatus);
        Loaded += async (_, _) => await RefreshLibraryAsync();
    }

    private async void OnDownloadClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: SleepWorld sleepWorld } button)
        {
            return;
        }

        button.IsEnabled = false;
        DownloadProgress.Progress = 0;
        StatusLabel.Text = $"{sleepWorld.Name} wird vorbereitet …";

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
            var result = await downloader.DownloadAsync(sleepWorld, _downloadRoot, progress);
            StatusLabel.Text = result.AvailableCount == 0
                ? "Derzeit sind keine Titel verfügbar. Bitte später erneut versuchen."
                : $"{sleepWorld.Name}: {result.AvailableCount} Titel sind offline verfügbar.";
            await RefreshLibraryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusLabel.Text = "Download angehalten.";
        }
        catch (Exception)
        {
            // Never let an unexpected batch-level failure escape an async UI event handler.
            // Individual media errors are already logged and skipped by SleepWorldDownloader.
            StatusLabel.Text = "Der Download wurde unerwartet beendet. Andere Sammlungen können weiter verwendet werden.";
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

    private async void OnOpenFolderClicked(object? sender, EventArgs e)
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
            // Exceptions must not escape an async void event handler and terminate the app.
            StatusLabel.Text = $"Download-Ordner: {_downloadRoot}";
        }
    }

    private async void OnRefreshLibraryClicked(object? sender, EventArgs e) => await RefreshLibraryAsync();

    private async Task RefreshLibraryAsync()
    {
        try
        {
            _library = await _libraryScanner.ScanAsync(_downloadRoot, BuiltInCatalog.SleepWorlds);
            LibraryView.ItemsSource = _library;
            LibraryEmptyLabel.IsVisible = _library.Count == 0;
            StatusLabel.Text = _library.Count == 0 ? "Noch keine gültigen Audiodateien vorhanden." : $"{_library.Count} Titel offline verfügbar.";
        }
        catch (Exception)
        {
            StatusLabel.Text = "Die lokale Musikbibliothek konnte nicht aktualisiert werden.";
        }
    }

    private void OnLibrarySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LocalLibraryTrack track)
            PlayTrack(track, userInitiated: true);
    }

    private void PlayTrack(LocalLibraryTrack track, bool userInitiated = false)
    {
        if (userInitiated)
            _consecutivePlaybackFailures = 0;
        _currentTrack = track;
        NowPlayingLabel.Text = track.Track.Title;
        NowPlayingDetailLabel.Text = $"{track.Track.Creator} · {track.SleepWorld.Name}";
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
            if (_library.Count > 0) PlayTrack(_library[0], userInitiated: true);
            return;
        }

        if (Player.CurrentState == MediaElementState.Playing)
        {
            Player.Pause();
            PlayPauseButton.Text = "▶";
        }
        else
        {
            Player.Play();
            PlayPauseButton.Text = "⏸";
        }
    }

    private void OnPreviousClicked(object? sender, EventArgs e) => MoveTrack(-1);
    private void OnNextClicked(object? sender, EventArgs e) => MoveTrack(1);

    private void MoveTrack(int offset)
    {
        if (_library.Count == 0) return;
        var index = _currentTrack is null ? 0 : _library.IndexOf(_currentTrack);
        if (index < 0) index = 0;
        PlayTrack(_library[(index + offset + _library.Count) % _library.Count]);
    }

    private void OnMediaEnded(object? sender, EventArgs e) => MoveTrack(1);

    private void OnMediaFailed(object? sender, MediaFailedEventArgs e)
    {
        _consecutivePlaybackFailures++;
        if (_consecutivePlaybackFailures >= Math.Min(MaximumAutomaticFailureSkips, _library.Count))
        {
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
            _consecutivePlaybackFailures = 0;
    }

    private void OnPositionDragStarted(object? sender, EventArgs e) => _isSeeking = true;

    private async void OnPositionDragCompleted(object? sender, EventArgs e)
    {
        if (Player.Duration > TimeSpan.Zero)
            await Player.SeekTo(TimeSpan.FromSeconds(PositionSlider.Value));
        _isSeeking = false;
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

    private bool UpdatePlaybackStatus()
    {
        if (!_isSeeking && Player.Duration > TimeSpan.Zero)
        {
            PositionSlider.Maximum = Player.Duration.TotalSeconds;
            PositionSlider.Value = Player.Position.TotalSeconds;
            TimeLabel.Text = $"{FormatTime(Player.Position)} / {FormatTime(Player.Duration)}";
        }

        if (_sleepTimer.ConsumeIfElapsed())
        {
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

    private static string FormatTime(TimeSpan value) => value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"m\:ss");
}

internal static class LibraryTrackListExtensions
{
    public static int IndexOf(this IReadOnlyList<LocalLibraryTrack> tracks, LocalLibraryTrack track)
    {
        for (var i = 0; i < tracks.Count; i++) if (tracks[i] == track) return i;
        return -1;
    }
}
