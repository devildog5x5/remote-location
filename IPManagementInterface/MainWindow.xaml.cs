using System;
using System.Windows;
using System.Windows.Controls;
using IPManagementInterface.ViewModels;
using IPManagementInterface.Models;

namespace IPManagementInterface
{
    public partial class MainWindow : Window
    {
        private readonly DashboardViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            // Set initial theme in selector
            var themeName = _viewModel.CurrentTheme.ToString();
            foreach (ComboBoxItem item in ThemeSelector.Items)
            {
                if (item.Tag?.ToString() == themeName)
                {
                    ThemeSelector.SelectedItem = item;
                    break;
                }
            }
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is ComboBoxItem item && item.Tag is string themeName)
            {
                if (Enum.TryParse<ViewModels.ThemeType>(themeName, out var theme))
                {
                    _viewModel.ChangeThemeCommand.Execute(theme);
                }
            }
        }

        private void DeviceTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceTypeFilter.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                if (string.IsNullOrEmpty(tag))
                {
                    _viewModel.DiscoveryFilterType = null;
                }
                else if (Enum.TryParse<DeviceType>(tag, out var deviceType))
                {
                    _viewModel.DiscoveryFilterType = deviceType;
                }
            }
        }
    }
}
