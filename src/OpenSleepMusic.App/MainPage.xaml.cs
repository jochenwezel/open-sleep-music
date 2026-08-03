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
        finally
        {
            button.IsEnabled = true;
        }
    }

    private async void OnOpenFolderClicked(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(_downloadRoot);
        await Launcher.Default.OpenAsync(new OpenFileRequest(
            "Open Sleep Music",
            new ReadOnlyFile(_downloadRoot)));
    }
}
