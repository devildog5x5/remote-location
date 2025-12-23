using IPManagementInterface.Shared.Services;
using IPManagementInterface.Shared.ViewModels;

namespace IPManagementInterface.Shared.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(DeviceHistoryService historyService)
    {
        InitializeComponent();
        BindingContext = new HistoryViewModel(historyService);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HistoryViewModel viewModel)
        {
            viewModel.LoadHistory();
        }
    }
}
