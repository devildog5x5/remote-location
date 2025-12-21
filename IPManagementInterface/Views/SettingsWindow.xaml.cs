using System;
using System.Windows;
using System.Windows.Controls;
using IPManagementInterface.ViewModels;

namespace IPManagementInterface.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly DashboardViewModel _mainViewModel;

        public SettingsWindow(DashboardViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            Owner = Application.Current.MainWindow;

            // Set the current theme as selected
            SetSelectedTheme(_mainViewModel.CurrentTheme);
        }

        private void SetSelectedTheme(ThemeType theme)
        {
            var themeName = theme.ToString();
            
            LightTheme.IsChecked = themeName == "Light";
            DarkTheme.IsChecked = themeName == "Dark";
            USMCTheme.IsChecked = themeName == "USMC";
            OliveDrabTheme.IsChecked = themeName == "OliveDrab";
            OceanTheme.IsChecked = themeName == "Ocean";
            SunsetTheme.IsChecked = themeName == "Sunset";
        }

        private void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Tag is string themeName)
            {
                if (Enum.TryParse<ThemeType>(themeName, out var theme))
                {
                    _mainViewModel.ChangeThemeCommand.Execute(theme);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
