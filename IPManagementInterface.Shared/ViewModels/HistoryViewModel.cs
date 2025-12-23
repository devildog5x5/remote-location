using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IPManagementInterface.Shared.Models;
using IPManagementInterface.Shared.Services;

namespace IPManagementInterface.Shared.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly DeviceHistoryService _historyService;

    [ObservableProperty]
    private ObservableCollection<DeviceHistory> historyItems = new();

    [ObservableProperty]
    private bool isRefreshing;

    public HistoryViewModel(DeviceHistoryService historyService)
    {
        _historyService = historyService;
        LoadHistory();
    }

    private void LoadHistory()
    {
        HistoryItems.Clear();
        var history = _historyService.GetRecentHistory(100);
        foreach (var item in history)
        {
            HistoryItems.Add(item);
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        IsRefreshing = true;
        LoadHistory();
        IsRefreshing = false;
    }
}

