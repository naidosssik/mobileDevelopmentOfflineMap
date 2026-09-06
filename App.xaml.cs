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
        return new Window(new AppShell());
    }
}