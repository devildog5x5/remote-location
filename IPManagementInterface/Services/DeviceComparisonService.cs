using System;
using System.Collections.Generic;
using System.Linq;
using IPManagementInterface.Models;

namespace IPManagementInterface.Services
{
    public class DeviceComparisonService
    {
        public DeviceComparison CompareDevices(IEnumerable<IoTDevice> devices)
        {
            var deviceList = devices.ToList();
            if (!deviceList.Any())
                return new DeviceComparison();

            return new DeviceComparison
            {
                TotalDevices = deviceList.Count,
                CommonManufacturers = deviceList
                    .Where(d => !string.IsNullOrEmpty(d.Manufacturer))
                    .GroupBy(d => d.Manufacturer)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key!, g => g.Count()),
                CommonPorts = deviceList
                    .GroupBy(d => d.Port)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),
                StatusDistribution = deviceList
                    .GroupBy(d => d.Status)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TypeDistribution = deviceList
                    .GroupBy(d => d.DeviceType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                AverageUptime = deviceList.Any() 
                    ? TimeSpan.FromSeconds(deviceList.Average(d => d.TotalUptime.TotalSeconds))
                    : TimeSpan.Zero,
                MostCommonGroup = deviceList
                    .Where(d => !string.IsNullOrEmpty(d.Group))
                    .GroupBy(d => d.Group)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key
            };
        }

        public List<DeviceDifference> FindDifferences(IoTDevice device1, IoTDevice device2)
        {
            var differences = new List<DeviceDifference>();

            if (device1.Name != device2.Name)
                differences.Add(new DeviceDifference { Property = "Name", Value1 = device1.Name, Value2 = device2.Name });

            if (device1.IpAddress != device2.IpAddress)
                differences.Add(new DeviceDifference { Property = "IP Address", Value1 = device1.IpAddress, Value2 = device2.IpAddress });

            if (device1.Port != device2.Port)
                differences.Add(new DeviceDifference { Property = "Port", Value1 = device1.Port.ToString(), Value2 = device2.Port.ToString() });

            if (device1.Protocol != device2.Protocol)
                differences.Add(new DeviceDifference { Property = "Protocol", Value1 = device1.Protocol, Value2 = device2.Protocol });

            if (device1.Status != device2.Status)
                differences.Add(new DeviceDifference { Property = "Status", Value1 = device1.Status.ToString(), Value2 = device2.Status.ToString() });

            if (device1.DeviceType != device2.DeviceType)
                differences.Add(new DeviceDifference { Property = "Device Type", Value1 = device1.DeviceType.ToString(), Value2 = device2.DeviceType.ToString() });

            return differences;
        }
    }

    public class DeviceComparison
    {
        public int TotalDevices { get; set; }
        public Dictionary<string, int> CommonManufacturers { get; set; } = new();
        public Dictionary<int, int> CommonPorts { get; set; } = new();
        public Dictionary<DeviceStatus, int> StatusDistribution { get; set; } = new();
        public Dictionary<DeviceType, int> TypeDistribution { get; set; } = new();
        public TimeSpan AverageUptime { get; set; }
        public string? MostCommonGroup { get; set; }
    }

    public class DeviceDifference
    {
        public string Property { get; set; } = string.Empty;
        public string Value1 { get; set; } = string.Empty;
        public string Value2 { get; set; } = string.Empty;
    }
}
