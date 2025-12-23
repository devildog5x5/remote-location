using System.Linq;
using IPManagementInterface.Shared.Models;
using IPManagementInterface.Shared.Services;

namespace IPManagementInterface.Shared.Views;

public partial class DeviceDetailsPage : ContentPage
{
    public IoTDevice? Device { get; set; }
    private readonly DeviceManagerService _deviceManager;
    private readonly NetworkToolsService _networkTools;

    public DeviceDetailsPage(DeviceManagerService deviceManager, NetworkToolsService networkTools)
    {
        InitializeComponent();
        _deviceManager = deviceManager;
        _networkTools = networkTools;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Get device from query parameters
        if (Shell.Current.CurrentState?.Location.OriginalString != null)
        {
            var query = Shell.Current.CurrentState.Location.OriginalString;
            if (query.Contains("deviceId="))
            {
                var deviceId = query.Split('=')[1];
                Device = _deviceManager.Devices.FirstOrDefault(d => d.IpAddress == deviceId);
            }
        }
        
        BindingContext = this;
    }

    private async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if (Device != null)
        {
            await _deviceManager.RefreshDeviceStatusAsync(Device);
            _deviceManager.UpdateDevice(Device);
            await DisplayAlert("Status Updated", $"Device is {Device.Status}", "OK");
            OnPropertyChanged(nameof(Device));
        }
    }

    private async void PingButton_Clicked(object sender, EventArgs e)
    {
        if (Device != null)
        {
            var result = await _networkTools.PingAsync(Device.IpAddress);
            await DisplayAlert("Ping Result", result.Success 
                ? $"Success - {result.RoundTripTime}ms" 
                : "Ping failed - device may be offline", "OK");
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (Device != null)
        {
            _deviceManager.UpdateDevice(Device);
            await DisplayAlert("Saved", "Device information has been saved", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (Device != null)
        {
            var confirm = await DisplayAlert("Delete Device", 
                $"Are you sure you want to delete {Device.Name}?", 
                "Delete", "Cancel");
            
            if (confirm)
            {
                _deviceManager.RemoveDevice(Device);
                await DisplayAlert("Deleted", "Device has been removed", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
