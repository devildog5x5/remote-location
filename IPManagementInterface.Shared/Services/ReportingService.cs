using System;
using System.Collections.Generic;
using System.Linq;
using IPManagementInterface.Shared.Models;

namespace IPManagementInterface.Shared.Services
{
    public class ReportingService
    {
        public DeviceStatistics GetStatistics(IEnumerable<IoTDevice> devices)
        {
            var deviceList = devices.ToList();
            
            return new DeviceStatistics
            {
                TotalDevices = deviceList.Count,
                OnlineDevices = deviceList.Count(d => d.Status == DeviceStatus.Online),
                OfflineDevices = deviceList.Count(d => d.Status == DeviceStatus.Offline),
                UnknownDevices = deviceList.Count(d => d.Status == DeviceStatus.Unknown),
                
                ByType = deviceList.GroupBy(d => d.DeviceType)
                    .ToDictionary(g => (Models.DeviceType)g.Key, g => g.Count()),
                
                ByGroup = deviceList.Where(d => !string.IsNullOrEmpty(d.Group))
                    .GroupBy(d => d.Group!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                
                AverageUptime = deviceList.Any() 
                    ? TimeSpan.FromSeconds(deviceList.Average(d => d.TotalUptime.TotalSeconds))
                    : TimeSpan.Zero
            };
        }
    }

    public class DeviceStatistics
    {
        public int TotalDevices { get; set; }
        public int OnlineDevices { get; set; }
        public int OfflineDevices { get; set; }
        public int UnknownDevices { get; set; }
        public Dictionary<Models.DeviceType, int> ByType { get; set; } = new();
        public Dictionary<string, int> ByGroup { get; set; } = new();
        public TimeSpan AverageUptime { get; set; }
    }
}
