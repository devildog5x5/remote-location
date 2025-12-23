using IPManagementInterface.Shared.ViewModels;

namespace IPManagementInterface.Shared.Views;

public partial class DiscoveryPage : ContentPage
{
    private readonly DiscoveryViewModel _viewModel;

    public DiscoveryPage(DiscoveryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void StartStopButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is DiscoveryViewModel viewModel)
        {
            if (viewModel.IsDiscovering)
            {
                viewModel.StopDiscoveryCommand.Execute(null);
            }
            else
            {
                await viewModel.StartDiscoveryCommand.ExecuteAsync(null);
            }
        }
    }

    private async void AddAllButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is DiscoveryViewModel viewModel)
        {
            await viewModel.AddAllDevicesCommand.ExecuteAsync(null);
        }
    }
}
