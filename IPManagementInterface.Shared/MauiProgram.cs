using Microsoft.Extensions.Logging;
using IPManagementInterface.Shared.ViewModels;
using IPManagementInterface.Shared.Services;
using IPManagementInterface.Shared.Views;

namespace IPManagementInterface.Shared;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        // Debug logging can be enabled via platform-specific configuration
        // builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.AddSingleton<DeviceCommunicationService>();
        builder.Services.AddSingleton<DeviceManagerService>();
        builder.Services.AddSingleton<NetworkToolsService>();
        builder.Services.AddSingleton<DeviceDiscoveryService>();
        builder.Services.AddSingleton<DeviceHistoryService>();
        builder.Services.AddSingleton<BulkOperationsService>();
        builder.Services.AddSingleton<ScheduledMonitoringService>();
        builder.Services.AddSingleton<ExportImportService>();
        builder.Services.AddSingleton<DeviceTemplateService>();
        builder.Services.AddSingleton<ReportingService>();
        builder.Services.AddSingleton<SecurityService>();
        builder.Services.AddSingleton<ThemeManager>();

        // Register ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<DiscoveryViewModel>();

        // Register Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DiscoveryPage>();
        builder.Services.AddTransient<DeviceDetailsPage>();
        builder.Services.AddTransient<SettingsPage>(sp => 
            new SettingsPage(sp.GetRequiredService<ThemeManager>()));

        return builder.Build();
    }
}
