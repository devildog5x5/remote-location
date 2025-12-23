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

    private void ApplyTheme(ThemeType theme, string themeName)
    {
        _themeManager.SaveTheme(theme);
        _themeManager.ApplyTheme(theme);
        DisplayAlert("Theme Changed", $"{themeName} theme applied", "OK");
    }

    private void LightTheme_Clicked(object sender, EventArgs e)
    {
        ApplyTheme(ThemeType.Light, "Light");
    }

    private void DarkTheme_Clicked(object sender, EventArgs e)
    {
        ApplyTheme(ThemeType.Dark, "Dark");
    }

    private void USMCTheme_Clicked(object sender, EventArgs e)
    {
        ApplyTheme(ThemeType.USMC, "USMC");
    }

    private void OliveDrabTheme_Clicked(object sender, EventArgs e)
    {
        ApplyTheme(ThemeType.OliveDrab, "Olive Drab");
    }

    private void OceanTheme_Clicked(object sender, EventArgs e)
    {
        ApplyTheme(ThemeType.Ocean, "Ocean");
    }

    private void SunsetTheme_Clicked(object sender, EventArgs e)
    {
        ApplyTheme(ThemeType.Sunset, "Sunset");
    }
}
