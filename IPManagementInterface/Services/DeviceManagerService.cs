using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IPManagementInterface.Models;
using Newtonsoft.Json;

namespace IPManagementInterface.Services
{
    public class DeviceManagerService
    {
        private readonly string _storagePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IPManagementInterface",
            "devices.json"
        );

        private ObservableCollection<IoTDevice> _devices;
        private readonly DeviceCommunicationService _communicationService;

        public DeviceManagerService(DeviceCommunicationService communicationService)
        {
            _communicationService = communicationService;
            _devices = new ObservableCollection<IoTDevice>();
            LoadDevices();
        }

        public ObservableCollection<IoTDevice> Devices => _devices;

        public void AddDevice(IoTDevice device)
        {
            if (!_devices.Any(d => d.IpAddress == device.IpAddress && d.Port == device.Port))
            {
                _devices.Add(device);
                SaveDevices();
            }
        }

        public void RemoveDevice(IoTDevice device)
        {
            _devices.Remove(device);
            SaveDevices();
        }

        public void UpdateDevice(IoTDevice device)
        {
            SaveDevices();
        }

        public async Task RefreshDeviceStatusAsync(IoTDevice device)
        {
            var isOnline = await _communicationService.CheckDeviceStatusAsync(
                device.IpAddress, device.Port, device.Protocol);
            
            device.Status = isOnline ? DeviceStatus.Online : DeviceStatus.Offline;
            if (isOnline)
            {
                device.LastSeen = DateTime.Now;
            }
        }

        public async Task RefreshAllDevicesAsync()
        {
            var tasks = _devices.Select(RefreshDeviceStatusAsync).ToArray();
            await Task.WhenAll(tasks);
        }

        private void LoadDevices()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    var json = File.ReadAllText(_storagePath);
                    var devices = JsonConvert.DeserializeObject<List<IoTDevice>>(json);
                    if (devices != null)
                    {
                        foreach (var device in devices)
                        {
                            _devices.Add(device);
                        }
                    }
                }
            }
            catch { }
        }

        private void SaveDevices()
        {
            try
            {
                var directory = Path.GetDirectoryName(_storagePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(_devices.ToList(), Formatting.Indented);
                File.WriteAllText(_storagePath, json);
            }
            catch { }
        }
    }
}
