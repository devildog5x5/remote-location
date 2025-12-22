using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IPManagementInterface.ViewModels;
using IPManagementInterface.Models;
using IPManagementInterface.Views;
using Microsoft.Win32;

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

            // Setup multi-selection for ListBoxes
            SetupMultiSelection();
        }

        private void SetupMultiSelection()
        {
            // Handle selection changed for multi-select
            // Note: WPF ListBox doesn't directly support binding SelectedItems
            // We'll handle this via SelectionChanged events
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var menu = new ContextMenu();
            menu.Items.Add(CreateMenuItem("Export as CSV", () => _viewModel.ExportDevicesCommand.Execute("csv")));
            menu.Items.Add(CreateMenuItem("Export as JSON", () => _viewModel.ExportDevicesCommand.Execute("json")));
            menu.Items.Add(CreateMenuItem("Export as XML", () => _viewModel.ExportDevicesCommand.Execute("xml")));
            menu.IsOpen = true;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var menu = new ContextMenu();
            menu.Items.Add(CreateMenuItem("Import from CSV", () => _viewModel.ImportDevicesCommand.Execute("csv")));
            menu.Items.Add(CreateMenuItem("Import from JSON", () => _viewModel.ImportDevicesCommand.Execute("json")));
            menu.IsOpen = true;
        }

        private MenuItem CreateMenuItem(string header, Action action)
        {
            var item = new MenuItem { Header = header };
            item.Click += (s, e) => action();
            return item;
        }

        private void BulkGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new Views.InputDialog("Enter Group Name", "Group:");
            if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.ResponseText))
            {
                _viewModel.BulkAssignGroupCommand.Execute(inputDialog.ResponseText);
            }
        }
    }
}
