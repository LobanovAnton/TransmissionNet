using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TransmissionNet.MainApp;
using TransmissionNet.Settings;

namespace TransmissionNet;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldSuppressExceptionsInBehaviors(true);
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ionicons.ttf", "Ionicons");
            });
        
        builder.Services.AddSingleton<IWindowCreator, WindowCreator>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        _ = SettingEntries.LoadEntries(SettingsType.All);
        
        return builder.Build();
    }
}