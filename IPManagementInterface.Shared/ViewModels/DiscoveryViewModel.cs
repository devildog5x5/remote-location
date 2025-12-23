using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IPManagementInterface.Shared.Models;
using IPManagementInterface.Shared.Services;

namespace IPManagementInterface.Shared.ViewModels;

    public partial class DiscoveryViewModel : ObservableObject
    {
        private readonly DeviceDiscoveryService _discoveryService;
        private readonly DeviceManagerService _deviceManager;
        private CancellationTokenSource? _cancellationTokenSource;

        [ObservableProperty]
        private string startIp = "192.168.1.1";

        [ObservableProperty]
        private string endIp = "192.168.1.254";

        [ObservableProperty]
        private string ports = "80,443,8000,8080,8443";

        [ObservableProperty]
        private bool useHttp = true;

        [ObservableProperty]
        private bool useHttps = true;

        [ObservableProperty]
        private bool isDiscovering;

        [ObservableProperty]
        private double progress;

        [ObservableProperty]
        private int scannedCount;

        [ObservableProperty]
        private int foundCount;

        [ObservableProperty]
        private string statusMessage = "Ready to start discovery";

        [ObservableProperty]
        private ObservableCollection<IoTDevice> discoveredDevices = new();

        public string StartStopButtonText => IsDiscovering ? "⏹️ Stop" : "🚀 Start";
        public Color StartStopButtonColor => IsDiscovering ? Color.FromArgb("#F44336") : Color.FromArgb("#4CAF50");
        public bool HasDiscoveredDevices => DiscoveredDevices.Count > 0;

        public DiscoveryViewModel(DeviceDiscoveryService discoveryService, DeviceManagerService deviceManager)
        {
            _discoveryService = discoveryService;
            _deviceManager = deviceManager;
            
            // Subscribe to discovery events
            _discoveryService.DeviceDiscovered += OnDeviceDiscovered;
            _discoveryService.ProgressUpdated += OnProgressUpdated;
        }

        private void OnDeviceDiscovered(object? sender, IoTDevice device)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DiscoveredDevices.Add(device);
                FoundCount = DiscoveredDevices.Count;
            });
        }

        private void OnProgressUpdated(object? sender, DiscoveryProgress progress)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ScannedCount = progress.ScannedHosts;
                FoundCount = progress.FoundDevices;
                StatusMessage = progress.StatusMessage;
                if (progress.TotalHosts > 0)
                {
                    Progress = (double)progress.ScannedHosts / progress.TotalHosts;
                }
            });
        }

    [RelayCommand]
    private async Task StartDiscovery()
    {
        if (IsDiscovering) return;

        _cancellationTokenSource = new CancellationTokenSource();
        IsDiscovering = true;
        DiscoveredDevices.Clear();
        StatusMessage = "Starting discovery...";
        ScannedCount = 0;
        FoundCount = 0;
        Progress = 0;

        try
        {
            var startParts = StartIp.Split('.');
            var endParts = EndIp.Split('.');
            
            if (startParts.Length != 4 || endParts.Length != 4)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Invalid IP address format", "OK");
                IsDiscovering = false;
                return;
            }

            var startHost = int.Parse(startParts[3]);
            var endHost = int.Parse(endParts[3]);
            var portList = Ports.Split(',')
                .Select(p => int.TryParse(p.Trim(), out var port) ? port : 0)
                .Where(p => p > 0)
                .ToList();

                var protocols = new List<string>();
                if (UseHttp) protocols.Add("http");
                if (UseHttps) protocols.Add("https");

                var progress = new Progress<DiscoveryProgress>(p => OnProgressUpdated(this, p));

                var devices = await _discoveryService.DiscoverDevicesAsync(
                    startHost: startHost,
                    endHost: endHost,
                    customPorts: portList,
                    progress: progress,
                    cancellationToken: _cancellationTokenSource.Token);

            DiscoveredDevices.Clear();
            foreach (var device in devices)
            {
                DiscoveredDevices.Add(device);
            }

            FoundCount = devices.Count;
            StatusMessage = $"Discovery complete! Found {devices.Count} device(s)";
            Progress = 1.0;
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsDiscovering = false;
        }
    }

    [RelayCommand]
    private void StopDiscovery()
    {
        _cancellationTokenSource?.Cancel();
        IsDiscovering = false;
        StatusMessage = "Discovery stopped";
    }

        [RelayCommand]
        private async Task AddAllDevices()
        {
            int added = 0;
            foreach (var device in DiscoveredDevices)
            {
                var existing = _deviceManager.Devices.FirstOrDefault(
                    d => d.IpAddress == device.IpAddress && d.Port == device.Port);
                if (existing == null)
                {
                    device.FirstSeen = System.DateTime.Now;
                    _deviceManager.AddDevice(device);
                    added++;
                }
            }

            await Application.Current!.MainPage!.DisplayAlert("Success", $"Added {added} new device(s)", "OK");
        }
}
