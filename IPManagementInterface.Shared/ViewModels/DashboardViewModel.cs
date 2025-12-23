using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IPManagementInterface.Shared.Models;
using IPManagementInterface.Shared.Services;

namespace IPManagementInterface.Shared.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DeviceManagerService _deviceManager;
    private readonly DeviceDiscoveryService _discoveryService;

    [ObservableProperty]
    private ObservableCollection<IoTDevice> allDevices = new();

    [ObservableProperty]
    private ObservableCollection<IoTDevice> cameraDevices = new();

    [ObservableProperty]
    private ObservableCollection<IoTDevice> networkDevices = new();

    [ObservableProperty]
    private ObservableCollection<IoTDevice> serverDevices = new();

    [ObservableProperty]
    private ObservableCollection<IoTDevice> otherDevices = new();

    [ObservableProperty]
    private ObservableCollection<IoTDevice> filteredDevices = new();

    [ObservableProperty]
    private IoTDevice? selectedDevice;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private int selectedCategoryIndex = 0; // 0=All, 1=Cameras, 2=Network, 3=Servers, 4=Other

    public DashboardViewModel(
        DeviceManagerService deviceManager,
        DeviceDiscoveryService discoveryService)
    {
        _deviceManager = deviceManager;
        _discoveryService = discoveryService;
        LoadDevices();
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
        
        ApplyFilters();
    }

    private void CategorizeDevice(IoTDevice device)
    {
        switch (device.DeviceType)
        {
            case Models.DeviceType.Camera:
                if (!CameraDevices.Contains(device))
                    CameraDevices.Add(device);
                break;
            case Models.DeviceType.Router:
            case Models.DeviceType.Switch:
            case Models.DeviceType.AccessPoint:
                if (!NetworkDevices.Contains(device))
                    NetworkDevices.Add(device);
                break;
            case Models.DeviceType.Server:
            case Models.DeviceType.Printer:
                if (!ServerDevices.Contains(device))
                    ServerDevices.Add(device);
                break;
            default:
                if (!OtherDevices.Contains(device))
                    OtherDevices.Add(device);
                break;
        }
    }

    private void ApplyFilters()
    {
        ObservableCollection<IoTDevice> sourceCollection = SelectedCategoryIndex switch
        {
            1 => CameraDevices,
            2 => NetworkDevices,
            3 => ServerDevices,
            4 => OtherDevices,
            _ => AllDevices
        };

        var filtered = sourceCollection.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(d =>
                d.Name.ToLowerInvariant().Contains(searchLower) ||
                d.IpAddress.Contains(SearchText) ||
                (d.Group?.ToLowerInvariant().Contains(searchLower) ?? false));
        }

        FilteredDevices.Clear();
        foreach (var device in filtered)
        {
            FilteredDevices.Add(device);
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedCategoryIndexChanged(int value)
    {
        ApplyFilters();
    }

    partial void OnIsRefreshingChanged(bool value)
    {
        if (!value)
        {
            // Refresh completed
        }
    }

    [RelayCommand]
    private async Task RefreshAll()
    {
        IsRefreshing = true;
        await _deviceManager.RefreshAllDevicesAsync();
        LoadDevices();
        IsRefreshing = false;
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
            DeviceType = Models.DeviceType.Other,
            FirstSeen = DateTime.Now
        };

        _deviceManager.AddDevice(newDevice);
        LoadDevices();
        SelectedDevice = newDevice;
    }

    [RelayCommand]
    private void RemoveDevice(IoTDevice device)
    {
        if (device != null)
        {
            _deviceManager.RemoveDevice(device);
            LoadDevices();
        }
    }
}
