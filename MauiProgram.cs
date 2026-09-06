using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace OfflineMapApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        string[] args = Environment.GetCommandLineArgs();
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        if (args.Length > 1)
        {
            App.StartupPhotoPath = args[1];
        }

        return builder.Build();
    }
}	