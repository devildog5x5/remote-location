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
                    .Where(d => d.Source != null)
                    .Where(d => {
                        var source = d.Source.ToString();
                        return source.Contains("Themes/Colors.xaml") ||
                               source.Contains("Themes/LightTheme.xaml") ||
                               source.Contains("Themes/DarkTheme.xaml") ||
                               source.Contains("Themes/USMCTheme.xaml") ||
                               source.Contains("Themes/OliveDrabTheme.xaml") ||
                               source.Contains("Themes/OceanTheme.xaml") ||
                               source.Contains("Themes/SunsetTheme.xaml");
                    })
                    .ToList();
                
                foreach (var dict in dictionariesToRemove)
                {
                    app.Resources.MergedDictionaries.Remove(dict);
                }

                // Small delay to ensure removal is complete
                System.Threading.Thread.Sleep(50);

                // Load theme-specific resources (which already include Colors.xaml via MergedDictionaries)
                var themeName = theme.ToString();
                ResourceDictionary themeDict = null;
                bool loaded = false;
                
                try
                {
                    // Try pack URI first
                    themeDict = new ResourceDictionary();
                    themeDict.Source = new Uri($"pack://application:,,,/Themes/{themeName}Theme.xaml", UriKind.Absolute);
                    app.Resources.MergedDictionaries.Insert(0, themeDict); // Insert at beginning to ensure it overrides defaults
                    loaded = true;
                    System.Diagnostics.Debug.WriteLine($"Successfully loaded {themeName}Theme.xaml using pack URI");
                }
                catch (Exception ex1)
                {
                    System.Diagnostics.Debug.WriteLine($"Pack URI failed: {ex1.Message}");
                    try
                    {
                        // Fallback to relative URI
                        themeDict = new ResourceDictionary();
                        themeDict.Source = new Uri($"/Themes/{themeName}Theme.xaml", UriKind.Relative);
                        app.Resources.MergedDictionaries.Insert(0, themeDict);
                        loaded = true;
                        System.Diagnostics.Debug.WriteLine($"Successfully loaded {themeName}Theme.xaml using relative URI");
                    }
                    catch (Exception ex2)
                    {
                        System.Diagnostics.Debug.WriteLine($"Relative URI failed: {ex2.Message}");
                        // Fallback to Light theme
                        try
                        {
                            themeDict = new ResourceDictionary();
                            themeDict.Source = new Uri("pack://application:,,,/Themes/LightTheme.xaml", UriKind.Absolute);
                            app.Resources.MergedDictionaries.Insert(0, themeDict);
                            loaded = true;
                        }
                        catch
                        {
                            try
                            {
                                themeDict = new ResourceDictionary();
                                themeDict.Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative);
                                app.Resources.MergedDictionaries.Insert(0, themeDict);
                                loaded = true;
                            }
                            catch (Exception ex3)
                            {
                                System.Diagnostics.Debug.WriteLine($"All theme loading attempts failed. Last error: {ex3.Message}");
                            }
                        }
                    }
                }

                // Force refresh of all windows to apply the new theme
                if (loaded)
                {
                    app.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Update all existing windows
                        foreach (Window window in app.Windows)
                        {
                            if (window != null && window.IsLoaded)
                            {
                                // Force resource refresh
                                var resources = window.Resources;
                                window.Resources = null;
                                window.Resources = resources;
                                
                                // Force visual refresh
                                window.UpdateLayout();
                                window.InvalidateVisual();
                            }
                        }
                        
                        // Also update the main application resources directly
                        app.Resources = app.Resources;
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme application error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
