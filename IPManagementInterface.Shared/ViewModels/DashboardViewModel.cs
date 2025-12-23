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
    private ObservableCollection<IoTDevice> filteredDevices = new();

    [ObservableProperty]
    private IoTDevice? selectedDevice;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

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
        foreach (var device in _deviceManager.Devices)
        {
            AllDevices.Add(device);
        }
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = AllDevices.AsEnumerable();

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
            FirstSeen = System.DateTime.Now
        };

        _deviceManager.AddDevice(newDevice);
        LoadDevices();
        SelectedDevice = newDevice;
    }
}
