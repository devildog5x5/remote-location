using Foundation;

namespace IPManagementInterface.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => Shared.MauiProgram.CreateMauiApp();
}
