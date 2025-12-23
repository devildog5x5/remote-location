using IPManagementInterface.Shared.ViewModels;
using IPManagementInterface.Shared.Models;

namespace IPManagementInterface.Shared.Views;

    public partial class MainPage : ContentPage
    {
        public MainPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    private async void SettingsButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SettingsPage");
    }

    private async void DiscoverButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DiscoveryPage");
    }

    private void AddDeviceButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is DashboardViewModel viewModel)
        {
            viewModel.AddDeviceCommand.Execute(null);
        }
    }

    private async void DeviceDetails_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is IoTDevice device)
        {
            await Shell.Current.GoToAsync($"//DeviceDetailsPage?deviceId={device.IpAddress}");
        }
    }
}
