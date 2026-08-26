using Microsoft.Extensions.DependencyInjection;

namespace OpenSleepMusic.App;

public partial class App : Application
{
	public App()
	{
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
