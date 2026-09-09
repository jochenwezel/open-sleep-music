using OpenSleepMusic.App.Localization;
using OpenSleepMusic.App.Playback;
using OpenSleepMusic.App.Visuals;

namespace OpenSleepMusic.App;

public partial class ImmersivePlayerPage : ContentPage
{
    private readonly MainPage _owner;
    private bool _seeking;
    private bool _animateBack;
    private PlaybackVisualTheme _theme = PlaybackVisualCatalog.For(null, null);

    internal ImmersivePlayerPage(MainPage owner)
    {
        InitializeComponent();
        _owner = owner;
        VolumeTitleLabel.Text = AppText.Get("Volume");
        VolumeSlider.Value = AppVolumeBridge.Volume;
        VolumeValueLabel.Text = $"{AppVolumeBridge.Volume:P0}";
        var overlay = new VolumeOverlayView();
        Scene.Add(overlay);
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(500), RefreshState);
        Loaded += (_, _) => StartAmbientAnimation();
        Unloaded += (_, _) => Scene.AbortAnimation("ambient");
        RefreshState();
    }

    private bool RefreshState()
    {
        if (!IsLoaded) return true;
        var state = _owner.GetImmersiveState();
        var theme = PlaybackVisualCatalog.For(state.WorldId, state.TrackId);
        if (theme != _theme)
        {
            _theme = theme;
            MotifImage.Source = theme.MotifAsset;
            BackgroundColor = Color.FromArgb(theme.StartColor);
        }
        TitleLabel.Text = state.Title ?? AppText.Get("NoTrack");
        FavoriteButton.Text = state.IsFavorite ? "★" : "☆";
        BlockButton.TextColor = state.IsBlocked ? Color.FromArgb("#FFB0C8") : Colors.White;
        PlayButton.Text = FloatingPlayButton.Text = state.IsPlaying ? "⏸" : "▶";
        RepeatButton.TextColor = state.RepeatTrack ? Color.FromArgb("#AFA7FF") : Color.FromArgb("#C9C5D8");
        TimerButton.Text = state.TimerText;
        if (!_seeking)
        {
            PositionSlider.Maximum = Math.Max(1, state.Duration.TotalSeconds);
            PositionSlider.Value = Math.Clamp(state.Position.TotalSeconds, 0, PositionSlider.Maximum);
        }
        PositionLabel.Text = FormatTime(state.Position);
        DurationLabel.Text = FormatTime(state.Duration);
        return true;
    }

    private void StartAmbientAnimation()
    {
        if (!IsLoaded || _owner.ReducedMotion) return;
        var from = Color.FromArgb(_animateBack ? _theme.EndColor : _theme.StartColor);
        var to = Color.FromArgb(_animateBack ? _theme.StartColor : _theme.EndColor);
        Scene.Animate("ambient", value => BackgroundColor = Interpolate(from, to, value), 16, 15000, Easing.SinInOut, (_, _) =>
        {
            _animateBack = !_animateBack;
            StartAmbientAnimation();
        });
    }

    private void OnSceneTapped(object? sender, TappedEventArgs e)
    {
        Controls.IsVisible = !Controls.IsVisible;
        FloatingPlayButton.IsVisible = !Controls.IsVisible;
    }

    private async void OnBackClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
    private void OnPlayClicked(object? sender, EventArgs e) { _owner.ImmersiveTogglePlayback(); RefreshState(); }
    private void OnPreviousClicked(object? sender, EventArgs e) => _owner.ImmersiveMove(-1);
    private void OnNextClicked(object? sender, EventArgs e) => _owner.ImmersiveMove(1);
    private void OnFavoriteClicked(object? sender, EventArgs e) { _owner.ImmersiveToggleFavorite(); RefreshState(); }
    private void OnBlockClicked(object? sender, EventArgs e) { _owner.ImmersiveToggleBlocked(); RefreshState(); }
    private void OnVolumeClicked(object? sender, EventArgs e) => VolumePanel.IsVisible = !VolumePanel.IsVisible;
    private async void OnTitleTapped(object? sender, TappedEventArgs e) => await _owner.OpenCurrentTrackDetailsAsync();
    private async void OnRepeatClicked(object? sender, EventArgs e) { await _owner.ChooseRepeatModeAsync(); RefreshState(); }
    private async void OnTimerClicked(object? sender, EventArgs e) { await _owner.ChooseSleepTimerAsync(); RefreshState(); }
    private void OnSeekStarted(object? sender, EventArgs e) => _seeking = true;
    private void OnSeekCompleted(object? sender, EventArgs e) { _seeking = false; _owner.ImmersiveSeek(PositionSlider.Value); }
    private void OnVolumeChanged(object? sender, ValueChangedEventArgs e)
    {
        VolumeValueLabel.Text = $"{e.NewValue:P0}";
        if (Math.Abs(AppVolumeBridge.Volume - e.NewValue) > .001) AppVolumeBridge.Set(e.NewValue);
    }

    private static Color Interpolate(Color a, Color b, double value) => Color.FromRgba(
        a.Red + (b.Red - a.Red) * value, a.Green + (b.Green - a.Green) * value,
        a.Blue + (b.Blue - a.Blue) * value, 1);
    private static string FormatTime(TimeSpan value) => value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"m\:ss");
}

internal sealed record ImmersivePlayerState(string? TrackId, string? WorldId, string? Title, bool IsPlaying, bool IsFavorite, bool IsBlocked, bool RepeatTrack, string TimerText, TimeSpan Position, TimeSpan Duration);
