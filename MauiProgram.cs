using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace OfflineMapApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var args = Environment.GetCommandLineArgs();

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular"
                );
            });

        if (args.Length > 1)
        {
            string photoPath = args[1]
                .Trim()
                .Trim('"');

            App.StartupPhotoPath = photoPath;
        }

        return builder.Build();
    }
}	