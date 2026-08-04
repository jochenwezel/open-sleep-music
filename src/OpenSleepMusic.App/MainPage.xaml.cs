using System.Diagnostics;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.App;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;
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
}
