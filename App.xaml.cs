using Microsoft.Extensions.DependencyInjection;

namespace OfflineMapApp;

public partial class App : Application
{
    public static string? StartupPhotoPath { get; set; }

    public App()
    {
        InitializeComponent();
    }

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());

	#if WINDOWS
		window.Created += (s, e) =>
		{
			var platformWindow = window.Handler?.PlatformView
				as Microsoft.UI.Xaml.Window;

			if (platformWindow != null)
			{
				var hwnd = WinRT.Interop.WindowNative
					.GetWindowHandle(platformWindow);

				var windowId = Microsoft.UI.Win32Interop
					.GetWindowIdFromWindow(hwnd);

				var appWindow = Microsoft.UI.Windowing.AppWindow
					.GetFromWindowId(windowId);

				if (appWindow.Presenter
					is Microsoft.UI.Windowing.OverlappedPresenter presenter)
				{
					presenter.Maximize();
				}
			}
		};
	#endif

		return window;
	}
}