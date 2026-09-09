using Microsoft.Extensions.DependencyInjection;

using OpenSleepMusic.App.Localization;

namespace OpenSleepMusic.App;

public partial class App : Application
{
	public App()
	{
		AppText.Apply(new AppStateStore().LoadLanguage());
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
		window.Deactivated += (_, _) => CurrentMainPage()?.HandleAppDeactivated();
		window.Stopped += (_, _) => CurrentMainPage()?.HandleAppDeactivated();
		window.Resumed += (_, _) => CurrentMainPage()?.HandleAppResumed();
		return window;
	}

	private static MainPage? CurrentMainPage() =>
		Shell.Current?.CurrentPage as MainPage;
}
