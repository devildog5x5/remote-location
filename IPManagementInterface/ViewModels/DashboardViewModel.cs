using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IPManagementInterface.Models;
using IPManagementInterface.Services;
using IPManagementInterface.ViewModels;

namespace IPManagementInterface.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DeviceManagerService _deviceManager;
        private readonly DeviceDiscoveryService _discoveryService;
        private readonly ThemeManager _themeManager;

        [ObservableProperty]
        private ObservableCollection<IoTDevice> allDevices;

        [ObservableProperty]
        private ObservableCollection<IoTDevice> cameraDevices;

        [ObservableProperty]
        private ObservableCollection<IoTDevice> networkDevices;

        [ObservableProperty]
        private ObservableCollection<IoTDevice> serverDevices;

        [ObservableProperty]
        private ObservableCollection<IoTDevice> otherDevices;

        [ObservableProperty]
        private IoTDevice? selectedDevice;

        [ObservableProperty]
        private int selectedTabIndex;

        [ObservableProperty]
        private bool isDiscovering;

        [ObservableProperty]
        private string discoveryStatus = string.Empty;

        [ObservableProperty]
        private DeviceType? discoveryFilterType;

        [ObservableProperty]
        private string discoveryFilterGroup = string.Empty;

        [ObservableProperty]
        private ThemeType currentTheme;

        public DashboardViewModel()
        {
            var communicationService = new DeviceCommunicationService();
            _deviceManager = new DeviceManagerService(communicationService);
            _discoveryService = new DeviceDiscoveryService(communicationService);
            _themeManager = new ThemeManager();

            AllDevices = new ObservableCollection<IoTDevice>();
            CameraDevices = new ObservableCollection<IoTDevice>();
            NetworkDevices = new ObservableCollection<IoTDevice>();
            ServerDevices = new ObservableCollection<IoTDevice>();
            OtherDevices = new ObservableCollection<IoTDevice>();

            // Load devices from manager
            LoadDevices();

            // Initialize theme
            CurrentTheme = _themeManager.GetSavedTheme();
            _themeManager.ApplyTheme(CurrentTheme);
        }

        private void LoadDevices()
        {
            AllDevices.Clear();
            CameraDevices.Clear();
            NetworkDevices.Clear();
            ServerDevices.Clear();
            OtherDevices.Clear();

            foreach (var device in _deviceManager.Devices)
            {
                AllDevices.Add(device);
                CategorizeDevice(device);
            }

            if (AllDevices.Count > 0 && SelectedDevice == null)
            {
                SelectedDevice = AllDevices.First();
            }
            
            // Subscribe to device property changes for persistence
            if (SelectedDevice != null)
            {
                SelectedDevice.PropertyChanged += (s, e) => _deviceManager.UpdateDevice(SelectedDevice);
            }
        }

        private void CategorizeDevice(IoTDevice device)
        {
            switch (device.DeviceType)
            {
                case DeviceType.Camera:
                    if (!CameraDevices.Contains(device))
                        CameraDevices.Add(device);
                    break;
                case DeviceType.Router:
                case DeviceType.Switch:
                case DeviceType.AccessPoint:
                    if (!NetworkDevices.Contains(device))
                        NetworkDevices.Add(device);
                    break;
                case DeviceType.Server:
                case DeviceType.Printer:
                    if (!ServerDevices.Contains(device))
                        ServerDevices.Add(device);
                    break;
                default:
                    if (!OtherDevices.Contains(device))
                        OtherDevices.Add(device);
                    break;
            }
        }

        [RelayCommand]
        private async Task DiscoverDevices()
        {
            if (IsDiscovering) return;

            IsDiscovering = true;
            DiscoveryStatus = "Scanning network...";

            try
            {
                var groupFilter = string.IsNullOrWhiteSpace(DiscoveryFilterGroup) ? null : DiscoveryFilterGroup;
                var discovered = await _discoveryService.DiscoverDevicesAsync(
                    DiscoveryFilterType,
                    groupFilter,
                    cancellationToken: CancellationToken.None);

                int addedCount = 0;
                foreach (var device in discovered)
                {
                    var existing = _deviceManager.Devices.FirstOrDefault(
                        d => d.IpAddress == device.IpAddress && d.Port == device.Port);
                    if (existing == null)
                    {
                        _deviceManager.AddDevice(device);
                        addedCount++;
                    }
                }

                LoadDevices();
                DiscoveryStatus = $"Found {discovered.Count} device(s), added {addedCount} new";
            }
            catch (Exception ex)
            {
                DiscoveryStatus = $"Error: {ex.Message}";
            }
            finally
            {
                IsDiscovering = false;
            }
        }

        [RelayCommand]
        private void AddDevice()
        {
            var newDevice = new IoTDevice
            {
                Name = "New Device",
                IpAddress = "192.168.1.1",
                Port = 80,
                Protocol = "http",
                DeviceType = DeviceType.Other
            };

            _deviceManager.AddDevice(newDevice);
            LoadDevices();
            SelectedDevice = newDevice;
            
            // Switch to appropriate tab
            if (newDevice.DeviceType == DeviceType.Camera)
                SelectedTabIndex = 1;
            else if (newDevice.DeviceType == DeviceType.Router || newDevice.DeviceType == DeviceType.Switch || newDevice.DeviceType == DeviceType.AccessPoint)
                SelectedTabIndex = 2;
            else if (newDevice.DeviceType == DeviceType.Server || newDevice.DeviceType == DeviceType.Printer)
                SelectedTabIndex = 3;
            else
                SelectedTabIndex = 4;
            
            // Update device manager when device properties change
            if (SelectedDevice != null)
            {
                SelectedDevice.PropertyChanged += (s, e) => 
                {
                    _deviceManager.UpdateDevice(SelectedDevice);
                    // Re-categorize if device type changed
                    if (e.PropertyName == nameof(IoTDevice.DeviceType))
                    {
                        LoadDevices();
                    }
                };
            }
        }

        [RelayCommand]
        private void RemoveDevice()
        {
            if (SelectedDevice != null)
            {
                _deviceManager.RemoveDevice(SelectedDevice);
                LoadDevices();
            }
        }

        [RelayCommand]
        private async Task RefreshDevice()
        {
            if (SelectedDevice != null)
            {
                await _deviceManager.RefreshDeviceStatusAsync(SelectedDevice);
                _deviceManager.UpdateDevice(SelectedDevice);
            }
        }

        [RelayCommand]
        private async Task RefreshAll()
        {
            await _deviceManager.RefreshAllDevicesAsync();
            LoadDevices();
        }

        [RelayCommand]
        private void ChangeTheme(object? themeObj)
        {
            if (themeObj is ThemeType theme)
            {
                CurrentTheme = theme;
                _themeManager.SaveTheme(theme);
                _themeManager.ApplyTheme(theme);
            }
        }
    }
}
