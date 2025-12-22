using System;
using System.Collections.Generic;
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
using IPManagementInterface.Views;

namespace IPManagementInterface.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DeviceManagerService _deviceManager;
        private readonly DeviceDiscoveryService _discoveryService;
        private readonly ThemeManager _themeManager;
        private readonly BulkOperationsService _bulkOperations;
        private readonly DeviceHistoryService _historyService;
        private readonly ScheduledMonitoringService _monitoringService;
        private readonly ExportImportService _exportImport;
        private readonly DeviceTemplateService _templateService;
        private readonly NetworkToolsService _networkTools;
        private readonly ReportingService _reportingService;
        private readonly SecurityService _securityService;

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
        private ObservableCollection<IoTDevice> selectedDevices = new();

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

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string? filterGroup;

        [ObservableProperty]
        private DeviceStatus? filterStatus;

        [ObservableProperty]
        private bool showFavoritesOnly;

        [ObservableProperty]
        private string? selectedTag;

        [ObservableProperty]
        private bool isMonitoring;

        [ObservableProperty]
        private int monitoringIntervalMinutes = 5;

        public DashboardViewModel()
        {
            var communicationService = new DeviceCommunicationService();
            _deviceManager = new DeviceManagerService(communicationService);
            _networkTools = new NetworkToolsService();
            _discoveryService = new DeviceDiscoveryService(communicationService, _networkTools);
            _themeManager = new ThemeManager();
            _historyService = new DeviceHistoryService();
            _bulkOperations = new BulkOperationsService(_deviceManager);
            _monitoringService = new ScheduledMonitoringService(_deviceManager, _historyService);
            _exportImport = new ExportImportService();
            _templateService = new DeviceTemplateService();
            _reportingService = new ReportingService();
            _securityService = new SecurityService();

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

            // Subscribe to property changes for filtering
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SearchText) || 
                    e.PropertyName == nameof(FilterGroup) || 
                    e.PropertyName == nameof(FilterStatus) ||
                    e.PropertyName == nameof(ShowFavoritesOnly) ||
                    e.PropertyName == nameof(SelectedTag))
                {
                    ApplyFilters();
                }
            };
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
                        device.FirstSeen = DateTime.Now;
                        _deviceManager.AddDevice(device);
                        _historyService.AddDeviceDiscovered(device.IpAddress, device.DeviceType);
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
                DeviceType = DeviceType.Other,
                FirstSeen = DateTime.Now
            };

            _deviceManager.AddDevice(newDevice);
            _historyService.AddDeviceDiscovered(newDevice.IpAddress, newDevice.DeviceType);
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
                var oldStatus = SelectedDevice.Status;
                await _deviceManager.RefreshDeviceStatusAsync(SelectedDevice);
                
                if (oldStatus != SelectedDevice.Status)
                {
                    _historyService.AddStatusChange(SelectedDevice.IpAddress, oldStatus, SelectedDevice.Status);
                }
                
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
                
                // Force property notification
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }

        private void ApplyFilters()
        {
            LoadDevices();
            
            // Update filter group dropdown
            var groups = _deviceManager.Devices
                .Where(d => !string.IsNullOrEmpty(d.Group))
                .Select(d => d.Group!)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        }

        private IEnumerable<IoTDevice> GetFilteredDevices(IEnumerable<IoTDevice> devices)
        {
            var filtered = devices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLowerInvariant();
                filtered = filtered.Where(d => 
                    d.Name.ToLowerInvariant().Contains(searchLower) ||
                    d.IpAddress.Contains(SearchText) ||
                    (d.Group?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    d.Tags.Any(t => t.ToLowerInvariant().Contains(searchLower)));
            }

            if (!string.IsNullOrEmpty(FilterGroup))
            {
                filtered = filtered.Where(d => d.Group == FilterGroup);
            }

            if (FilterStatus.HasValue)
            {
                filtered = filtered.Where(d => d.Status == FilterStatus.Value);
            }

            if (ShowFavoritesOnly)
            {
                filtered = filtered.Where(d => d.IsFavorite);
            }

            if (!string.IsNullOrEmpty(SelectedTag))
            {
                filtered = filtered.Where(d => d.Tags.Contains(SelectedTag));
            }

            return filtered;
        }

        [RelayCommand]
        private async Task BulkRefresh()
        {
            if (SelectedDevices.Count > 0)
            {
                foreach (var device in SelectedDevices)
                {
                    var oldStatus = device.Status;
                    await _deviceManager.RefreshDeviceStatusAsync(device);
                    if (oldStatus != device.Status)
                    {
                        _historyService.AddStatusChange(device.IpAddress, oldStatus, device.Status);
                    }
                }
                LoadDevices();
            }
        }

        [RelayCommand]
        private void BulkAssignGroup(string? group)
        {
            if (SelectedDevices.Count > 0 && !string.IsNullOrEmpty(group))
            {
                _bulkOperations.BulkAssignGroup(SelectedDevices, group);
                LoadDevices();
            }
        }

        [RelayCommand]
        private void BulkDelete()
        {
            if (SelectedDevices.Count > 0)
            {
                _bulkOperations.BulkDelete(SelectedDevices);
                SelectedDevices.Clear();
                LoadDevices();
            }
        }

        [RelayCommand]
        private void BulkSetFavorite(object? parameter)
        {
            if (SelectedDevices.Count > 0 && parameter is bool isFavorite)
            {
                _bulkOperations.BulkSetFavorite(SelectedDevices, isFavorite);
                LoadDevices();
            }
        }

        [RelayCommand]
        private void ExportDevices(string format)
        {
            var devices = GetFilteredDevices(_deviceManager.Devices).ToList();
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = format.ToLower() switch
                {
                    "csv" => "CSV files (*.csv)|*.csv",
                    "json" => "JSON files (*.json)|*.json",
                    "xml" => "XML files (*.xml)|*.xml",
                    _ => "All files (*.*)|*.*"
                }
            };

            if (dialog.ShowDialog() == true)
            {
                switch (format.ToLower())
                {
                    case "csv":
                        _exportImport.ExportToCsv(devices, dialog.FileName);
                        break;
                    case "json":
                        _exportImport.ExportToJson(devices, dialog.FileName);
                        break;
                    case "xml":
                        _exportImport.ExportToXml(devices, dialog.FileName);
                        break;
                }
            }
        }

        [RelayCommand]
        private void ImportDevices(string format)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = format.ToLower() switch
                {
                    "csv" => "CSV files (*.csv)|*.csv",
                    "json" => "JSON files (*.json)|*.json",
                    _ => "All files (*.*)|*.*"
                }
            };

            if (dialog.ShowDialog() == true)
            {
                List<IoTDevice> devices;
                switch (format.ToLower())
                {
                    case "csv":
                        devices = _exportImport.ImportFromCsv(dialog.FileName);
                        break;
                    case "json":
                        devices = _exportImport.ImportFromJson(dialog.FileName);
                        break;
                    default:
                        return;
                }

                foreach (var device in devices)
                {
                    _deviceManager.AddDevice(device);
                }

                LoadDevices();
            }
        }

        [RelayCommand]
        private async Task PingDevice()
        {
            if (SelectedDevice != null)
            {
                var result = await _networkTools.PingAsync(SelectedDevice.IpAddress);
                DiscoveryStatus = result.Success 
                    ? $"Ping successful - {result.RoundTripTime}ms" 
                    : "Ping failed";
            }
        }

        [RelayCommand]
        private async Task ScanPorts()
        {
            if (SelectedDevice != null)
            {
                var commonPorts = new[] { 80, 443, 8080, 8443, 22, 23, 21, 25, 53, 3389 };
                var results = await _networkTools.ScanPortsAsync(SelectedDevice.IpAddress, commonPorts);
                var openPorts = results.Where(r => r.IsOpen).Select(r => r.Port).ToList();
                DiscoveryStatus = openPorts.Any() 
                    ? $"Open ports: {string.Join(", ", openPorts)}" 
                    : "No open ports found";
            }
        }

        [RelayCommand]
        private void ToggleMonitoring()
        {
            if (IsMonitoring)
            {
                _monitoringService.StopMonitoring();
                IsMonitoring = false;
            }
            else
            {
                _monitoringService.StartMonitoring(MonitoringIntervalMinutes);
                IsMonitoring = true;
            }
        }

        [RelayCommand]
        private void ShowDeviceHistory()
        {
            // Will be implemented in UI
        }

        [RelayCommand]
        private void ShowStatistics()
        {
            // Will be implemented in UI
        }

        [RelayCommand]
        private void AddDeviceFromTemplate(DeviceTemplate? template)
        {
            if (template != null)
            {
                var device = _templateService.CreateDeviceFromTemplate(template, "192.168.1.1", $"New {template.Name}");
                _deviceManager.AddDevice(device);
                LoadDevices();
                SelectedDevice = device;
            }
        }
    }
}
