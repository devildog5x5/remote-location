using System;
using System.Windows;
using System.Windows.Controls;
using IPManagementInterface.ViewModels;
using IPManagementInterface.Models;
using IPManagementInterface.Views;

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
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_viewModel);
            settingsWindow.ShowDialog();
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
