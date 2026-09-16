using OpenSleepMusic.App.Localization;
using Microsoft.Maui.Controls.Shapes;

namespace OpenSleepMusic.App.Playback;

internal sealed class VolumeOverlayView : ContentView
{
    private readonly Slider _appSlider;
    private readonly Slider _systemSlider;
    private CancellationTokenSource? _hideCancellation;
    private bool _subscribed;

    public VolumeOverlayView()
    {
        IsVisible = false;
        InputTransparent = false;
        HorizontalOptions = LayoutOptions.End;
        VerticalOptions = LayoutOptions.Start;
        Margin = new Thickness(8, 58, 8, 0);
        ZIndex = 100;

        _appSlider = NewSlider();
        _appSlider.Value = AppVolumeBridge.Volume;
        _appSlider.ValueChanged += (_, args) =>
        {
            if (Math.Abs(AppVolumeBridge.Volume - args.NewValue) > .001) AppVolumeBridge.Set(args.NewValue);
        };
        _systemSlider = NewSlider();
        _systemSlider.Value = SystemVolumeSnapshot.Value;
        _systemSlider.IsEnabled = false;

        Content = new Border
        {
            BackgroundColor = Color.FromArgb("#F2171D31"),
            Stroke = Color.FromArgb("#596485"),
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Padding = 14,
            WidthRequest = 300,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    NewLabel(AppText.Get("AppVolume")), _appSlider,
                    NewLabel(AppText.Get("SystemVolume")), _systemSlider
                }
            }
        };

        Loaded += (_, _) => Subscribe();
        Unloaded += (_, _) => Unsubscribe();
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent is null) Unsubscribe(); else Subscribe();
    }

    private void Subscribe()
    {
        if (_subscribed) return;
        SystemVolumeSnapshot.Changed += OnSystemVolumeChanged;
        AppVolumeBridge.Changed += OnAppVolumeChanged;
        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;
        SystemVolumeSnapshot.Changed -= OnSystemVolumeChanged;
        AppVolumeBridge.Changed -= OnAppVolumeChanged;
        _hideCancellation?.Cancel();
        _subscribed = false;
    }

    private void OnAppVolumeChanged(object? sender, double volume) => Dispatcher.Dispatch(() => _appSlider.Value = volume);

    private void OnSystemVolumeChanged(object? sender, double volume) => Dispatcher.Dispatch(() =>
    {
        _systemSlider.Value = volume;
        Show();
    });

    public void Show()
    {
        _appSlider.Value = AppVolumeBridge.Volume;
        _systemSlider.Value = SystemVolumeSnapshot.Value;
        IsVisible = true;
        _hideCancellation?.Cancel();
        _hideCancellation?.Dispose();
        _hideCancellation = new CancellationTokenSource();
        _ = HideLaterAsync(_hideCancellation.Token);
    }

    private async Task HideLaterAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);
            Dispatcher.Dispatch(() => IsVisible = false);
        }
        catch (OperationCanceledException) { }
    }

    private static Slider NewSlider() => new() { Minimum = 0, Maximum = 1, MinimumTrackColor = Color.FromArgb("#8A7FFF") };
    private static Label NewLabel(string text) => new() { Text = text, TextColor = Color.FromArgb("#F7F3FF"), FontSize = 12 };
}
