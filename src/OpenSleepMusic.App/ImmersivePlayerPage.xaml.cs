using OpenSleepMusic.App.Localization;
using OpenSleepMusic.App.Playback;
using OpenSleepMusic.App.Visuals;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.App;

public partial class ImmersivePlayerPage : ContentPage
{
    private readonly MainPage _owner;
    private readonly string _worldId;
    private bool _seeking;

    internal string WorldId => _worldId;
    private bool _animateBack;
    private bool _fadeMotifBack;
    private bool _driftSongMotifBack;
    private int _motifTransitionVersion;
    private int _songMotifTransitionVersion;
    private string? _artworkRequestKey;
    private CancellationTokenSource? _artworkCancellation;
    private readonly VolumeOverlayView _volumeOverlay;
    private readonly IDispatcherTimer _refreshTimer;
    private PlaybackVisualTheme? _theme;

    internal ImmersivePlayerPage(MainPage owner, string worldId)
    {
        InitializeComponent();
        _owner = owner;
        _worldId = worldId;
        ApplyTheme(PlaybackVisualCatalog.For(worldId, null));
        _volumeOverlay = new VolumeOverlayView();
        Scene.Add(_volumeOverlay);
        _refreshTimer = Dispatcher.CreateTimer();
        _refreshTimer.Interval = TimeSpan.FromMilliseconds(500);
        _refreshTimer.IsRepeating = true;
        Loaded += (_, _) => StartRefreshing();
        Unloaded += (_, _) => StopRefreshing();
        SizeChanged += (_, _) => ApplySongMotifLayout();
    }

    private void ApplySongMotifLayout()
    {
        if (Width <= 0 || Height <= 0) return;
        var landscape = Width > Height;
        TopBar.RowDefinitions.Clear();
        if (landscape)
        {
            TopBar.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetRow(TitleLabel, 0);
            Grid.SetColumn(TitleLabel, 2);
            Grid.SetColumnSpan(TitleLabel, 1);
            TopBar.RowSpacing = 0;
        }
        else
        {
            TopBar.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            TopBar.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetRow(TitleLabel, 1);
            Grid.SetColumn(TitleLabel, 0);
            Grid.SetColumnSpan(TitleLabel, 6);
            TopBar.RowSpacing = 8;
        }
        var motifSize = landscape ? Math.Min(190d, Height * 0.48d) : Math.Min(280d, Width * 0.68d);
        SongMotifImage.WidthRequest = motifSize;
        SongMotifImage.HeightRequest = motifSize;
        SongMotifImage.Margin = landscape
            ? new Thickness(18, 12, 68, 96)
            : new Thickness(24, 24, 36, 122);
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
            ApplyTheme(theme, animateMotif: _theme is not null && IsLoaded);
            if (IsLoaded)
            {
                StopAmbientAnimations();
                StartAmbientAnimations();
            }
        }
        RequestArtwork(state.Track, theme.MotifAsset);
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
        PlayButton.Text = FloatingPlayButton.Text = state.IsPlaying ? "Ⅱ" : "▶︎";
        RepeatButton.Text = state.RepeatTrack ? "↻¹" : "↻";
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

    private void ApplyTheme(PlaybackVisualTheme theme, bool animateMotif = false)
    {
        var motifChanged = _theme?.MotifAsset != theme.MotifAsset;
        _theme = theme;
        Scene.BackgroundColor = Color.FromArgb(theme.StartColor);
        BackgroundColor = Color.FromArgb(theme.StartColor);
        if (animateMotif && motifChanged)
        {
            _ = TransitionMotifAsync(ImageSource.FromFile(theme.MotifAsset), ++_motifTransitionVersion);
        }
        else
        {
            MotifImage.Source = ImageSource.FromFile(theme.MotifAsset);
            MotifImage.Opacity = 0.88;
        }
    }

    private async Task TransitionMotifAsync(ImageSource source, int version)
    {
        await MotifImage.FadeToAsync(0, 250, Easing.SinInOut);
        if (version != _motifTransitionVersion || !IsLoaded) return;
        MotifImage.Source = source;
        await MotifImage.FadeToAsync(0.88, 650, Easing.SinInOut);
    }

    private void RequestArtwork(AudioTrack? track, string fallbackAsset)
    {
        var key = track is null
            ? $"fallback:{fallbackAsset}"
            : $"{track.Id}:{track.ArtworkSha256}:{track.SongMotifSha256}";
        if (_artworkRequestKey == key) return;
        _artworkRequestKey = key;
        _artworkCancellation?.Cancel();
        _artworkCancellation?.Dispose();
        _artworkCancellation = new CancellationTokenSource();
        if (track is null)
        {
            _ = TransitionMotifAsync(ImageSource.FromFile(fallbackAsset), ++_motifTransitionVersion);
            _ = TransitionSongMotifAsync(null, ++_songMotifTransitionVersion);
            return;
        }
        _ = LoadArtworkAsync(track, fallbackAsset, key, _artworkCancellation.Token);
    }

    private async Task LoadArtworkAsync(AudioTrack track, string fallbackAsset, string key, CancellationToken cancellationToken)
    {
        try
        {
            var artwork = await _owner.GetArtworkAsync(track, cancellationToken);
            if (cancellationToken.IsCancellationRequested || key != _artworkRequestKey) return;
            var backgroundSource = artwork.BackgroundPath is null
                ? ImageSource.FromFile(fallbackAsset)
                : await LoadCachedImageSourceAsync(artwork.BackgroundPath, cancellationToken);
            var songMotifSource = artwork.SongMotifPath is null
                ? null
                : await LoadCachedImageSourceAsync(artwork.SongMotifPath, cancellationToken);
            if (cancellationToken.IsCancellationRequested || key != _artworkRequestKey) return;
            await Task.WhenAll(
                TransitionMotifAsync(backgroundSource, ++_motifTransitionVersion),
                TransitionSongMotifAsync(songMotifSource, ++_songMotifTransitionVersion));
        }
        catch (OperationCanceledException) { }
    }

    private async Task TransitionSongMotifAsync(ImageSource? source, int version)
    {
        await SongMotifImage.FadeToAsync(0, 250, Easing.SinInOut);
        if (version != _songMotifTransitionVersion || !IsLoaded) return;
        SongMotifImage.Source = source;
        SongMotifImage.IsVisible = source is not null;
        if (source is not null)
        {
            await SongMotifImage.FadeToAsync(0.78, 650, Easing.SinInOut);
            if (version == _songMotifTransitionVersion) StartSongMotifDrift();
        }
    }

    private static async Task<ImageSource> LoadCachedImageSourceAsync(string path, CancellationToken cancellationToken)
    {
        var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
        return ImageSource.FromStream(() => new MemoryStream(bytes, writable: false));
    }

    private void StartAmbientAnimations()
    {
        if (!IsLoaded || _owner.ReducedMotion) return;
        StartColorAnimation();
        StartMotifFade();
        StartSongMotifDrift();
    }

    private void StartColorAnimation()
    {
        if (!IsLoaded || _owner.ReducedMotion) return;
        if (_theme is null) return;
        var from = Color.FromArgb(_animateBack ? _theme.EndColor : _theme.StartColor);
        var to = Color.FromArgb(_animateBack ? _theme.StartColor : _theme.EndColor);
        Scene.Animate("ambient-color", value => Scene.BackgroundColor = Interpolate(from, to, value), 50,
            (uint)_theme.ColorPhaseDuration.TotalMilliseconds, Easing.SinInOut, (_, cancelled) =>
        {
            if (cancelled) return;
            _animateBack = !_animateBack;
            StartColorAnimation();
        });
    }

    private void StartMotifFade()
    {
        if (!IsLoaded || _owner.ReducedMotion || _theme?.MotifFadeDuration is not { } duration) return;
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
        SongMotifImage.AbortAnimation("ambient-song-motif");
        SongMotifImage.TranslationY = 0;
    }

    private void StartSongMotifDrift()
    {
        SongMotifImage.AbortAnimation("ambient-song-motif");
        if (!IsLoaded || _owner.ReducedMotion || !SongMotifImage.IsVisible) return;
        var from = _driftSongMotifBack ? -3d : 3d;
        var to = _driftSongMotifBack ? 3d : -3d;
        SongMotifImage.Animate(
            "ambient-song-motif",
            value => SongMotifImage.TranslationY = from + ((to - from) * value),
            50,
            12_000,
            Easing.SinInOut,
            (_, cancelled) =>
            {
                if (cancelled) return;
                _driftSongMotifBack = !_driftSongMotifBack;
                StartSongMotifDrift();
            });
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

internal sealed record ImmersivePlayerState(string? TrackId, string? WorldId, string? Title, bool IsPlaying, bool IsFavorite, bool IsBlocked, bool RepeatTrack, bool IsSleepTimerActive, string TimerText, TimeSpan Position, TimeSpan Duration, AudioTrack? Track);

internal sealed record PlaybackArtworkPaths(string? BackgroundPath, string? SongMotifPath);
