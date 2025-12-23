using IPManagementInterface.Shared.Services;

namespace IPManagementInterface.Shared.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ThemeManager _themeManager;

    public SettingsPage(ThemeManager themeManager)
    {
        InitializeComponent();
        _themeManager = themeManager;
    }

    private void LightTheme_Clicked(object sender, EventArgs e)
    {
        _themeManager.SaveTheme(ThemeManager.ThemeType.Light);
        _themeManager.ApplyTheme(ThemeManager.ThemeType.Light);
        DisplayAlert("Theme Changed", "Light theme applied", "OK");
    }

    private void DarkTheme_Clicked(object sender, EventArgs e)
    {
        _themeManager.SaveTheme(ThemeManager.ThemeType.Dark);
        _themeManager.ApplyTheme(ThemeManager.ThemeType.Dark);
        DisplayAlert("Theme Changed", "Dark theme applied", "OK");
    }
}
