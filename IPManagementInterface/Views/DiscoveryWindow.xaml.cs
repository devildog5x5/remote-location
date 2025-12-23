using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using IPManagementInterface.Models;
using IPManagementInterface.Services;

namespace IPManagementInterface.Views
{
    public partial class DiscoveryWindow : Window
    {
        private readonly DeviceDiscoveryService _discoveryService;
        private readonly DeviceManagerService _deviceManager;
        private readonly DeviceHistoryService _historyService;
        private CancellationTokenSource? _cancellationTokenSource;
        private List<IoTDevice> _discoveredDevices = new();
        private DispatcherTimer? _updateTimer;

        public DiscoveryWindow(
            DeviceDiscoveryService discoveryService,
            DeviceManagerService deviceManager,
            DeviceHistoryService historyService)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            _discoveryService = discoveryService;
            _deviceManager = deviceManager;
            _historyService = historyService;

            // Subscribe to discovery events
            _discoveryService.DeviceDiscovered += OnDeviceDiscovered;
            _discoveryService.ProgressUpdated += OnProgressUpdated;

            // Setup update timer
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        private void OnDeviceDiscovered(object? sender, IoTDevice device)
        {
            Dispatcher.Invoke(() =>
            {
                _discoveredDevices.Add(device);
                RecentDiscoveriesListBox.Items.Insert(0, $"{device.Name} ({device.IpAddress}:{device.Port})");
                if (RecentDiscoveriesListBox.Items.Count > 20)
                {
                    RecentDiscoveriesListBox.Items.RemoveAt(RecentDiscoveriesListBox.Items.Count - 1);
                }
            });
        }

        private void OnProgressUpdated(object? sender, DiscoveryProgress progress)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateProgress(progress);
            });
        }

        private void UpdateProgress(DiscoveryProgress progress)
        {
            StatusTextBlock.Text = progress.StatusMessage;
            
            if (progress.TotalHosts > 0)
            {
                var percentage = (double)progress.ScannedHosts / progress.TotalHosts * 100;
                ProgressBar.Value = percentage;
            }

            ScannedTextBlock.Text = progress.ScannedHosts.ToString();
            FoundTextBlock.Text = progress.FoundDevices.ToString();
            CurrentIpTextBlock.Text = progress.CurrentIp;
            ElapsedTimeTextBlock.Text = progress.ElapsedTime.ToString(@"hh\:mm\:ss");
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            // Keep UI responsive
        }

        private async void StartDiscoveryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _discoveredDevices.Clear();
                RecentDiscoveriesListBox.Items.Clear();
                _cancellationTokenSource = new CancellationTokenSource();

                // Parse IP range
                var startIpParts = StartIpTextBox.Text.Split('.');
                var endIpParts = EndIpTextBox.Text.Split('.');
                
                if (startIpParts.Length != 4 || endIpParts.Length != 4)
                {
                    MessageBox.Show("Invalid IP address format. Use format: 192.168.1.1", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var startHost = int.Parse(startIpParts[3]);
                var endHost = int.Parse(endIpParts[3]);

                // Parse ports
                var ports = PortsTextBox.Text.Split(',')
                    .Select(p => int.TryParse(p.Trim(), out var port) ? port : 0)
                    .Where(p => p > 0)
                    .ToList();

                if (!ports.Any())
                {
                    MessageBox.Show("Please specify at least one port to scan.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get protocols
                var protocols = new List<string>();
                if (HttpCheckBox.IsChecked == true) protocols.Add("http");
                if (HttpsCheckBox.IsChecked == true) protocols.Add("https");

                if (!protocols.Any())
                {
                    MessageBox.Show("Please select at least one protocol (HTTP or HTTPS).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                StartDiscoveryButton.IsEnabled = false;
                StopDiscoveryButton.IsEnabled = true;

                var progress = new Progress<DiscoveryProgress>(UpdateProgress);
                
                var discovered = await _discoveryService.DiscoverDevicesAsync(
                    deviceType: null,
                    group: null,
                    startHost: startHost,
                    endHost: endHost,
                    customPorts: ports,
                    progress: progress,
                    cancellationToken: _cancellationTokenSource.Token);

                _discoveredDevices = discovered;

                StatusTextBlock.Text = $"Discovery complete! Found {discovered.Count} device(s)";
                ProgressBar.Value = 100;
            }
            catch (OperationCanceledException)
            {
                StatusTextBlock.Text = "Discovery cancelled by user.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Discovery error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
            finally
            {
                StartDiscoveryButton.IsEnabled = true;
                StopDiscoveryButton.IsEnabled = false;
            }
        }

        private void StopDiscoveryButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            StatusTextBlock.Text = "Stopping discovery...";
        }

        private void AddAllButton_Click(object sender, RoutedEventArgs e)
        {
            int added = 0;
            foreach (var device in _discoveredDevices)
            {
                var existing = _deviceManager.Devices.FirstOrDefault(
                    d => d.IpAddress == device.IpAddress && d.Port == device.Port);
                if (existing == null)
                {
                    device.FirstSeen = DateTime.Now;
                    _deviceManager.AddDevice(device);
                    _historyService.AddDeviceDiscovered(device.IpAddress, device.DeviceType);
                    added++;
                }
            }

            MessageBox.Show($"Added {added} new device(s) to the dashboard.", "Devices Added", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _updateTimer?.Stop();
            Close();
        }
    }
}
