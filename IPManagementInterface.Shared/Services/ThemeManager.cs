using Microsoft.Maui.Controls;

namespace IPManagementInterface.Shared.Services
{
    public enum ThemeType 
    { 
        Light, 
        Dark, 
        USMC, 
        OliveDrab, 
        Ocean, 
        Sunset 
    }

    public class ThemeManager
    {
        private const string ThemePreferenceKey = "SelectedTheme";

        public ThemeType GetSavedTheme()
        {
            var themeName = Preferences.Get(ThemePreferenceKey, "Light");
            return Enum.TryParse<ThemeType>(themeName, out var theme) ? theme : ThemeType.Light;
        }

        public void SaveTheme(ThemeType theme)
        {
            Preferences.Set(ThemePreferenceKey, theme.ToString());
            ApplyTheme(theme);
        }

        public void ApplyTheme(ThemeType theme)
        {
            var resources = Application.Current!.Resources;
            
            // Remove existing theme colors
            resources.Remove("PrimaryColor");
            resources.Remove("AccentColor");
            resources.Remove("PageBackgroundColor");
            resources.Remove("SurfaceColor");
            resources.Remove("PrimaryTextColor");
            resources.Remove("SecondaryTextColor");
            resources.Remove("SuccessColor");
            resources.Remove("ErrorColor");
            resources.Remove("WarningColor");

            // Apply theme colors
            switch (theme)
            {
                case ThemeType.Light:
                    resources["PrimaryColor"] = Color.FromArgb("#1976D2");
                    resources["AccentColor"] = Color.FromArgb("#FF6F00");
                    resources["PageBackgroundColor"] = Color.FromArgb("#F5F7FA");
                    resources["SurfaceColor"] = Color.FromArgb("#FFFFFF");
                    resources["PrimaryTextColor"] = Color.FromArgb("#1A1F2E");
                    resources["SecondaryTextColor"] = Color.FromArgb("#5A6C7D");
                    resources["SuccessColor"] = Color.FromArgb("#4CAF50");
                    resources["ErrorColor"] = Color.FromArgb("#F44336");
                    resources["WarningColor"] = Color.FromArgb("#FFC107");
                    break;

                case ThemeType.Dark:
                    resources["PrimaryColor"] = Color.FromArgb("#90CAF9");
                    resources["AccentColor"] = Color.FromArgb("#FFB74D");
                    resources["PageBackgroundColor"] = Color.FromArgb("#121212");
                    resources["SurfaceColor"] = Color.FromArgb("#1E1E1E");
                    resources["PrimaryTextColor"] = Color.FromArgb("#FFFFFF");
                    resources["SecondaryTextColor"] = Color.FromArgb("#B0B0B0");
                    resources["SuccessColor"] = Color.FromArgb("#66BB6A");
                    resources["ErrorColor"] = Color.FromArgb("#EF5350");
                    resources["WarningColor"] = Color.FromArgb("#FFA726");
                    break;

                case ThemeType.USMC:
                    resources["PrimaryColor"] = Color.FromArgb("#C8102E"); // USMC Red
                    resources["AccentColor"] = Color.FromArgb("#FFD700"); // Gold
                    resources["PageBackgroundColor"] = Color.FromArgb("#F5F5F0");
                    resources["SurfaceColor"] = Color.FromArgb("#FFFFFF");
                    resources["PrimaryTextColor"] = Color.FromArgb("#1A1A1A");
                    resources["SecondaryTextColor"] = Color.FromArgb("#4A4A4A");
                    resources["SuccessColor"] = Color.FromArgb("#4CAF50");
                    resources["ErrorColor"] = Color.FromArgb("#C8102E");
                    resources["WarningColor"] = Color.FromArgb("#FFD700");
                    break;

                case ThemeType.OliveDrab:
                    resources["PrimaryColor"] = Color.FromArgb("#3D5A3D"); // Dark green
                    resources["AccentColor"] = Color.FromArgb("#6B8E6B"); // Medium green
                    resources["PageBackgroundColor"] = Color.FromArgb("#F0F4F0");
                    resources["SurfaceColor"] = Color.FromArgb("#FFFFFF");
                    resources["PrimaryTextColor"] = Color.FromArgb("#1A2E1A");
                    resources["SecondaryTextColor"] = Color.FromArgb("#4A6A4A");
                    resources["SuccessColor"] = Color.FromArgb("#6B8E6B");
                    resources["ErrorColor"] = Color.FromArgb("#B85450");
                    resources["WarningColor"] = Color.FromArgb("#8B6F3D");
                    break;

                case ThemeType.Ocean:
                    resources["PrimaryColor"] = Color.FromArgb("#006994"); // Deep blue
                    resources["AccentColor"] = Color.FromArgb("#4A90A4"); // Teal
                    resources["PageBackgroundColor"] = Color.FromArgb("#F0F7FA");
                    resources["SurfaceColor"] = Color.FromArgb("#FFFFFF");
                    resources["PrimaryTextColor"] = Color.FromArgb("#003366");
                    resources["SecondaryTextColor"] = Color.FromArgb("#336699");
                    resources["SuccessColor"] = Color.FromArgb("#4A90A4");
                    resources["ErrorColor"] = Color.FromArgb("#CC6666");
                    resources["WarningColor"] = Color.FromArgb("#FFB84D");
                    break;

                case ThemeType.Sunset:
                    resources["PrimaryColor"] = Color.FromArgb("#FF6B35"); // Orange
                    resources["AccentColor"] = Color.FromArgb("#8B4C9F"); // Purple
                    resources["PageBackgroundColor"] = Color.FromArgb("#FFF5F0");
                    resources["SurfaceColor"] = Color.FromArgb("#FFFFFF");
                    resources["PrimaryTextColor"] = Color.FromArgb("#2E1A1F");
                    resources["SecondaryTextColor"] = Color.FromArgb("#6A4A5A");
                    resources["SuccessColor"] = Color.FromArgb("#FF6B35");
                    resources["ErrorColor"] = Color.FromArgb("#CC4C66");
                    resources["WarningColor"] = Color.FromArgb("#FFB84D");
                    break;
            }
        }
    }
}
