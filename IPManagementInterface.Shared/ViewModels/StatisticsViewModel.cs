using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using IPManagementInterface.Shared.Models;
using IPManagementInterface.Shared.Services;

namespace IPManagementInterface.Shared.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly DeviceManagerService _deviceManager;

    [ObservableProperty]
    private int totalDevices;

    [ObservableProperty]
    private int onlineDevices;

    [ObservableProperty]
    private int offlineDevices;

    [ObservableProperty]
    private int unknownDevices;

    [ObservableProperty]
    private int cameraCount;

    [ObservableProperty]
    private int cameraOnlineCount;

    [ObservableProperty]
    private int networkCount;

    [ObservableProperty]
    private int networkOnlineCount;

    [ObservableProperty]
    private int serverCount;

    [ObservableProperty]
    private int serverOnlineCount;

    [ObservableProperty]
    private int otherCount;

    [ObservableProperty]
    private int otherOnlineCount;

    public StatisticsViewModel(DeviceManagerService deviceManager)
    {
        _deviceManager = deviceManager;
        LoadStatistics();
    }

    public void LoadStatistics()
    {
        var devices = _deviceManager.Devices.ToList();
        
        TotalDevices = devices.Count;
        OnlineDevices = devices.Count(d => d.Status == DeviceStatus.Online);
        OfflineDevices = devices.Count(d => d.Status == DeviceStatus.Offline);
        UnknownDevices = devices.Count(d => d.Status == DeviceStatus.Unknown);

        // By category
        var cameras = devices.Where(d => d.DeviceType == Models.DeviceType.Camera).ToList();
        CameraCount = cameras.Count;
        CameraOnlineCount = cameras.Count(d => d.Status == DeviceStatus.Online);

        var network = devices.Where(d => d.DeviceType == Models.DeviceType.Router || 
                                          d.DeviceType == Models.DeviceType.Switch || 
                                          d.DeviceType == Models.DeviceType.AccessPoint).ToList();
        NetworkCount = network.Count;
        NetworkOnlineCount = network.Count(d => d.Status == DeviceStatus.Online);

        var servers = devices.Where(d => d.DeviceType == Models.DeviceType.Server || 
                                         d.DeviceType == Models.DeviceType.Printer).ToList();
        ServerCount = servers.Count;
        ServerOnlineCount = servers.Count(d => d.Status == DeviceStatus.Online);

        var other = devices.Where(d => d.DeviceType == Models.DeviceType.Other).ToList();
        OtherCount = other.Count;
        OtherOnlineCount = other.Count(d => d.Status == DeviceStatus.Online);
    }
}

