namespace OpenSleepMusic.App;

internal sealed record PlayerSettings(double Volume, PlaybackRepeatMode RepeatMode, int SleepTimerMinutes);

public partial class PlayerSettingsPage : ContentPage
{
    private static readonly int[] TimerMinutes = [0, 15, 30, 45, 60, 90];
    private bool _initializing = true;

    internal event EventHandler<PlayerSettings>? SettingsChanged;

    internal PlayerSettingsPage(PlayerSettings settings)
    {
        InitializeComponent();
        TimerPicker.ItemsSource = new[] { "Aus", "15 Minuten", "30 Minuten", "45 Minuten", "60 Minuten", "90 Minuten" };
        RepeatPicker.ItemsSource = new[] { "Themensammlung", "Einzeltitel" };
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
            TimerMinutes[timerIndex]));
    }
}
