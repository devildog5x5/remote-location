using System;
using System.Collections.Generic;
using System.Linq;
using IPManagementInterface.Models;

namespace IPManagementInterface.Services
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
                    .ToDictionary(g => g.Key, g => g.Count()),
                
                ByGroup = deviceList.Where(d => !string.IsNullOrEmpty(d.Group))
                    .GroupBy(d => d.Group!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                
                AverageUptime = deviceList.Any() 
                    ? TimeSpan.FromSeconds(deviceList.Average(d => d.TotalUptime.TotalSeconds))
                    : TimeSpan.Zero,
                
                DevicesByStatus = new Dictionary<DeviceStatus, int>
                {
                    { DeviceStatus.Online, deviceList.Count(d => d.Status == DeviceStatus.Online) },
                    { DeviceStatus.Offline, deviceList.Count(d => d.Status == DeviceStatus.Offline) },
                    { DeviceStatus.Unknown, deviceList.Count(d => d.Status == DeviceStatus.Unknown) }
                }
            };
        }

        public List<DeviceUptimeReport> GetUptimeReport(IEnumerable<IoTDevice> devices)
        {
            return devices.Select(d => new DeviceUptimeReport
            {
                DeviceName = d.Name,
                IpAddress = d.IpAddress,
                TotalUptime = d.TotalUptime,
                UptimePercentage = d.FirstSeen != DateTime.MinValue 
                    ? (d.TotalUptime.TotalSeconds / (DateTime.Now - d.FirstSeen).TotalSeconds) * 100 
                    : 0,
                FirstSeen = d.FirstSeen,
                LastSeen = d.LastSeen,
                Status = d.Status
            }).ToList();
        }
    }

    public class DeviceStatistics
    {
        public int TotalDevices { get; set; }
        public int OnlineDevices { get; set; }
        public int OfflineDevices { get; set; }
        public int UnknownDevices { get; set; }
        public Dictionary<DeviceType, int> ByType { get; set; } = new();
        public Dictionary<string, int> ByGroup { get; set; } = new();
        public TimeSpan AverageUptime { get; set; }
        public Dictionary<DeviceStatus, int> DevicesByStatus { get; set; } = new();
    }

    public class DeviceUptimeReport
    {
        public string DeviceName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public TimeSpan TotalUptime { get; set; }
        public double UptimePercentage { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public DeviceStatus Status { get; set; }
    }
}
