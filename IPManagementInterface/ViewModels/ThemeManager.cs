using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace IPManagementInterface.ViewModels
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
        private const string RegistryKey = @"HKEY_CURRENT_USER\Software\IPManagementInterface";
        private const string ThemeKey = "Theme";

        public ThemeType GetSavedTheme()
        {
            try
            {
                var themeValue = Registry.GetValue(RegistryKey, ThemeKey, "Light")?.ToString() ?? "Light";
                return Enum.TryParse<ThemeType>(themeValue, out var theme) ? theme : ThemeType.Light;
            }
            catch
            {
                return ThemeType.Light;
            }
        }

        public void SaveTheme(ThemeType theme)
        {
            try
            {
                Registry.SetValue(RegistryKey, ThemeKey, theme.ToString());
            }
            catch { }
        }

        public void ApplyTheme(ThemeType theme)
        {
            var app = Application.Current;
            if (app == null) return;

            try
            {
                // Clear existing theme dictionaries
                var dictionariesToRemove = app.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.ToString().Contains("Themes/"))
                    .ToList();
                
                foreach (var dict in dictionariesToRemove)
                {
                    app.Resources.MergedDictionaries.Remove(dict);
                }

                // Load base colors
                try
                {
                    app.Resources.MergedDictionaries.Add(
                        new ResourceDictionary { Source = new Uri("/Themes/Colors.xaml", UriKind.Relative) }
                    );
                }
                catch { }

                // Load theme-specific resources
                try
                {
                    var themeName = theme.ToString();
                    app.Resources.MergedDictionaries.Add(
                        new ResourceDictionary { Source = new Uri($"/Themes/{themeName}Theme.xaml", UriKind.Relative) }
                    );
                }
                catch
                {
                    // Fallback to Light theme
                    try
                    {
                        app.Resources.MergedDictionaries.Add(
                            new ResourceDictionary { Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative) }
                        );
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme application error: {ex.Message}");
            }
        }
    }
}
