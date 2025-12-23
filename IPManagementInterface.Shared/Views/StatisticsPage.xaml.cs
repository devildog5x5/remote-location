using IPManagementInterface.Shared.Services;
using IPManagementInterface.Shared.ViewModels;

namespace IPManagementInterface.Shared.Views;

public partial class StatisticsPage : ContentPage
{
    public StatisticsPage(DeviceManagerService deviceManager)
    {
        InitializeComponent();
        BindingContext = new StatisticsViewModel(deviceManager);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StatisticsViewModel viewModel)
        {
            viewModel.LoadStatistics();
        }
    }
}
