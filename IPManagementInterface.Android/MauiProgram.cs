using Microsoft.Extensions.Logging;
using IPManagementInterface.Shared;

namespace IPManagementInterface.Android;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return Shared.MauiProgram.CreateMauiApp();
    }
}

