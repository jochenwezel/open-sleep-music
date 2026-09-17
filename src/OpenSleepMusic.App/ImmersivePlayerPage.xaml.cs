using OpenSleepMusic.App.Localization;
using OpenSleepMusic.App.Playback;
using OpenSleepMusic.App.Visuals;

namespace OpenSleepMusic.App;

public partial class ImmersivePlayerPage : ContentPage
{
    private readonly MainPage _owner;
    private readonly string _worldId;
    private bool _seeking;
    private bool _animateBack;
    private bool _fadeMotifBack;
    private readonly VolumeOverlayView _volumeOverlay;
    private readonly IDispatcherTimer _refreshTimer;
    private PlaybackVisualTheme _theme = PlaybackVisualCatalog.For(null, null);

    internal ImmersivePlayerPage(MainPage owner, string worldId)
    {
        InitializeComponent();
        _owner = owner;
        _worldId = worldId;
        _volumeOverlay = new VolumeOverlayView();
        Scene.Add(_volumeOverlay);
        _refreshTimer = Dispatcher.CreateTimer();
        _refreshTimer.Interval = TimeSpan.FromMilliseconds(500);
        _refreshTimer.IsRepeating = true;
        Loaded += (_, _) => StartRefreshing();
        Unloaded += (_, _) => StopRefreshing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartRefreshing();
    }

    protected override void OnDisappearing()
    {
        StopRefreshing();
        base.OnDisappearing();
    }

    private void StartRefreshing()
    {
        if (!IsLoaded || _refreshTimer.IsRunning) return;
        _refreshTimer.Tick += OnRefreshTick;
        _refreshTimer.Start();
        RefreshState();
        StartAmbientAnimations();
    }

    private void StopRefreshing()
    {
        _refreshTimer.Stop();
        _refreshTimer.Tick -= OnRefreshTick;
        StopAmbientAnimations();
    }

    private void OnRefreshTick(object? sender, EventArgs e) => RefreshState();

    private void RefreshState()
    {
        if (!IsLoaded) return;
        var state = _owner.GetImmersiveState(_worldId);
        var theme = PlaybackVisualCatalog.For(state.WorldId, state.TrackId);
        if (theme != _theme)
        {
            _theme = theme;
            MotifImage.Source = theme.MotifAsset;
            BackgroundColor = Color.FromArgb(theme.StartColor);
            MotifImage.Opacity = 0.88;
            if (IsLoaded)
            {
                StopAmbientAnimations();
                StartAmbientAnimations();
            }
        }
        TitleLabel.Text = state.Title ?? AppText.Get("NoTrack");
        FavoriteButton.IsEnabled = state.TrackId is not null;
        BlockButton.IsEnabled = state.TrackId is not null;
        PositionSlider.IsEnabled = state.TrackId is not null;
        FavoriteButton.Text = state.IsFavorite ? "★" : "☆";
        BlockButton.Text = state.IsBlocked ? "🚫" : "⊘";
        BlockButton.TextColor = state.IsBlocked ? Color.FromArgb("#FFB0C8") : Colors.White;
        SemanticProperties.SetDescription(
            BlockButton,
            state.IsBlocked
                ? AppText.Pick("Aktueller Titel ist blockiert", "Current track is blocked")
                : AppText.Pick("Aktuellen Titel blockieren", "Block current track"));
        PlayButton.Text = FloatingPlayButton.Text = state.IsPlaying ? "⏸" : "▶";
        RepeatButton.Text = state.RepeatTrack ? "🔂" : "🔁";
        RepeatButton.TextColor = state.RepeatTrack ? Color.FromArgb("#AFA7FF") : Color.FromArgb("#C9C5D8");
        SemanticProperties.SetDescription(
            RepeatButton,
            state.RepeatTrack
                ? AppText.Pick("Wiederholung: einzelner Titel", "Repeat: single track")
                : AppText.Pick("Wiederholung: Themensammlung", "Repeat: collection"));
        TimerButton.Text = "Zzz";
        TimerButton.TextColor = state.IsSleepTimerActive
            ? Color.FromArgb("#AFA7FF")
            : Color.FromArgb("#C9C5D8");
        SemanticProperties.SetDescription(
            TimerButton,
            AppText.Pick($"Schlaftimer: {state.TimerText}", $"Sleep timer: {state.TimerText}"));
        SemanticProperties.SetDescription(
            TrackListButton,
            AppText.Pick("Titelliste öffnen", "Open track list"));
        if (!_seeking)
        {
            PositionSlider.Maximum = Math.Max(1, state.Duration.TotalSeconds);
            PositionSlider.Value = Math.Clamp(state.Position.TotalSeconds, 0, PositionSlider.Maximum);
        }
        PositionLabel.Text = FormatTime(state.Position);
        DurationLabel.Text = FormatTime(state.Duration);
    }

    private void StartAmbientAnimations()
    {
        if (!IsLoaded || _owner.ReducedMotion) return;
        StartColorAnimation();
        StartMotifFade();
    }

    private void StartColorAnimation()
    {
        if (!IsLoaded || _owner.ReducedMotion) return;
        var from = Color.FromArgb(_animateBack ? _theme.EndColor : _theme.StartColor);
        var to = Color.FromArgb(_animateBack ? _theme.StartColor : _theme.EndColor);
        Scene.Animate("ambient-color", value => BackgroundColor = Interpolate(from, to, value), 50,
            (uint)_theme.ColorPhaseDuration.TotalMilliseconds, Easing.SinInOut, (_, cancelled) =>
        {
            if (cancelled) return;
            _animateBack = !_animateBack;
            StartColorAnimation();
        });
    }

    private void StartMotifFade()
    {
        if (!IsLoaded || _owner.ReducedMotion || _theme.MotifFadeDuration is not { } duration) return;
        var from = _fadeMotifBack ? _theme.MotifMinimumOpacity : 0.88;
        var to = _fadeMotifBack ? 0.88 : _theme.MotifMinimumOpacity;
        MotifImage.Animate("ambient-motif", value => MotifImage.Opacity = from + ((to - from) * value), 50,
            (uint)duration.TotalMilliseconds, Easing.SinInOut, (_, cancelled) =>
        {
            if (cancelled) return;
            _fadeMotifBack = !_fadeMotifBack;
            StartMotifFade();
        });
    }

    private void StopAmbientAnimations()
    {
        Scene.AbortAnimation("ambient-color");
        MotifImage.AbortAnimation("ambient-motif");
    }

    private void OnSceneTapped(object? sender, TappedEventArgs e)
    {
        Controls.IsVisible = !Controls.IsVisible;
        FloatingPlayButton.IsVisible = !Controls.IsVisible;
    }

    private async void OnBackClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
    private void OnPlayClicked(object? sender, EventArgs e) { _owner.ImmersiveTogglePlayback(_worldId); RefreshState(); }
    private void OnPreviousClicked(object? sender, EventArgs e) => _owner.ImmersiveMove(_worldId, -1);
    private void OnNextClicked(object? sender, EventArgs e) => _owner.ImmersiveMove(_worldId, 1);
    private void OnFavoriteClicked(object? sender, EventArgs e) { _owner.ImmersiveToggleFavorite(_worldId); RefreshState(); }
    private void OnBlockClicked(object? sender, EventArgs e) { _owner.ImmersiveToggleBlocked(_worldId); RefreshState(); }
    private void OnVolumeClicked(object? sender, EventArgs e) => _volumeOverlay.Show();
    private async void OnTrackListClicked(object? sender, EventArgs e) =>
        await _owner.OpenImmersiveCollectionTracksAsync(_worldId);
    private async void OnTitleTapped(object? sender, TappedEventArgs e) => await _owner.OpenCurrentTrackDetailsAsync(_worldId);
    private async void OnRepeatClicked(object? sender, EventArgs e) { await _owner.ChooseRepeatModeAsync(); RefreshState(); }
    private async void OnTimerClicked(object? sender, EventArgs e) { await _owner.ChooseSleepTimerAsync(); RefreshState(); }
    private void OnSeekStarted(object? sender, EventArgs e) => _seeking = true;
    private void OnSeekCompleted(object? sender, EventArgs e) { _seeking = false; _owner.ImmersiveSeek(_worldId, PositionSlider.Value); }
    private static Color Interpolate(Color a, Color b, double value) => Color.FromRgba(
        a.Red + (b.Red - a.Red) * value, a.Green + (b.Green - a.Green) * value,
        a.Blue + (b.Blue - a.Blue) * value, 1);
    private static string FormatTime(TimeSpan value) => value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"m\:ss");
}

internal sealed record ImmersivePlayerState(string? TrackId, string? WorldId, string? Title, bool IsPlaying, bool IsFavorite, bool IsBlocked, bool RepeatTrack, bool IsSleepTimerActive, string TimerText, TimeSpan Position, TimeSpan Duration);
