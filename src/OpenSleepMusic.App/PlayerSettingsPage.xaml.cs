using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

internal sealed record PlayerSettings(double Volume, PlaybackRepeatMode RepeatMode, int SleepTimerMinutes, AppLanguage Language, bool ReducedMotion);

public partial class PlayerSettingsPage : ContentPage
{
    private static readonly int[] TimerMinutes = [0, 15, 30, 45, 60, 90];
    private bool _initializing = true;

    internal event EventHandler<PlayerSettings>? SettingsChanged;

    internal PlayerSettingsPage(PlayerSettings settings)
    {
        InitializeComponent();
        LanguagePicker.ItemsSource = new[] { AppText.Get("SystemLanguage"), AppText.Get("German"), AppText.Get("English") };
        LanguagePicker.SelectedIndex = (int)settings.Language;
        ReducedMotionSwitch.IsToggled = settings.ReducedMotion;
        LanguageLabel.Text = AppText.Get("Language");
        ReducedMotionLabel.Text = AppText.Get("ReducedMotion");
        PageTitleLabel.Text = AppText.IsGerman ? "Wiedergabe-Einstellungen" : "Playback settings";
        TimerTitleLabel.Text = AppText.Get("SleepTimer");
        TimerHelpLabel.Text = AppText.IsGerman ? "Die gewählte Dauer beginnt beim Start einer Wiedergabe. Eine Änderung während der Wiedergabe startet den Timer neu." : "The selected duration starts with playback. Changing it while playing restarts the timer.";
        RepeatTitleLabel.Text = AppText.Get("Repeat");
        VolumeTitleLabel.Text = AppText.Get("AppVolume");
        VolumeHelpLabel.Text = AppText.IsGerman ? "Die Hardwaretasten des Handys regeln zusätzlich die Android-Medienlautstärke." : "The phone's hardware buttons additionally control Android media volume.";
        TimerPicker.ItemsSource = new[] { AppText.Get("Off"), "15 min", "30 min", "45 min", "60 min", "90 min" };
        RepeatPicker.ItemsSource = new[] { AppText.Get("Collection"), AppText.Get("Track") };
        TimerPicker.SelectedIndex = Math.Max(0, Array.IndexOf(TimerMinutes, settings.SleepTimerMinutes));
        RepeatPicker.SelectedIndex = settings.RepeatMode == PlaybackRepeatMode.Track ? 1 : 0;
        VolumeSlider.Value = settings.Volume;
        VolumeValueLabel.Text = $"{settings.Volume:P0}";
        _initializing = false;
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();

    private void OnVolumeChanged(object? sender, ValueChangedEventArgs e)
    {
        VolumeValueLabel.Text = $"{e.NewValue:P0}";
        PublishSettings();
    }

    private void OnSettingChanged(object? sender, EventArgs e) => PublishSettings();

    private void PublishSettings()
    {
        if (_initializing) return;
        var timerIndex = Math.Clamp(TimerPicker.SelectedIndex, 0, TimerMinutes.Length - 1);
        SettingsChanged?.Invoke(this, new PlayerSettings(
            VolumeSlider.Value,
            RepeatPicker.SelectedIndex == 1 ? PlaybackRepeatMode.Track : PlaybackRepeatMode.SleepWorld,
            TimerMinutes[timerIndex],
            (AppLanguage)Math.Clamp(LanguagePicker.SelectedIndex, 0, 2),
            ReducedMotionSwitch.IsToggled));
    }
}
