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
        var query = Shell.Current.CurrentState.Location.OriginalString;
        if (query.Contains("deviceId="))
        {
            var deviceId = query.Split('=')[1];
            Device = _deviceManager.Devices.FirstOrDefault(d => d.IpAddress == deviceId);
        }
        
        BindingContext = this;
    }

    private async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if (Device != null)
        {
            await _deviceManager.RefreshDeviceStatusAsync(Device);
            await DisplayAlert("Refresh", $"Device is {Device.Status}", "OK");
        }
    }

    private async void PingButton_Clicked(object sender, EventArgs e)
    {
        if (Device != null)
        {
            var result = await _networkTools.PingAsync(Device.IpAddress);
            await DisplayAlert("Ping", result.Success 
                ? $"Success - {result.RoundTripTime}ms" 
                : "Failed", "OK");
        }
    }
}
