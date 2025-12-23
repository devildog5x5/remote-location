using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPManagementInterface.Shared.Models;
using Newtonsoft.Json;

namespace IPManagementInterface.Shared.Services
{
    public class DeviceHistoryService
    {
        private readonly string _historyPath;
        private List<DeviceHistory> _history = new();

        public DeviceHistoryService()
        {
            _historyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "device_history.json"
            );
            LoadHistory();
        }

        public void AddEvent(DeviceHistory historyEvent)
        {
            _history.Add(historyEvent);
            if (_history.Count > 1000)
            {
                _history = _history.OrderByDescending(h => h.Timestamp).Take(1000).ToList();
            }
            SaveHistory();
        }

        public void AddStatusChange(string deviceIp, DeviceStatus? oldStatus, DeviceStatus? newStatus)
        {
            AddEvent(new DeviceHistory
            {
                DeviceIpAddress = deviceIp,
                EventType = "StatusChange",
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Description = $"Status changed from {oldStatus} to {newStatus}"
            });
        }

        public void AddDeviceEdit(string deviceIp, string description)
        {
            AddEvent(new DeviceHistory
            {
                DeviceIpAddress = deviceIp,
                EventType = "Edit",
                Description = description
            });
        }

        public void AddDeviceDiscovered(string deviceIp, Models.DeviceType deviceType)
        {
            AddEvent(new DeviceHistory
            {
                DeviceIpAddress = deviceIp,
                EventType = "Discovered",
                Description = $"Discovered {deviceType} device"
            });
        }

        public List<DeviceHistory> GetHistoryForDevice(string deviceIp)
        {
            return _history
                .Where(h => h.DeviceIpAddress == deviceIp)
                .OrderByDescending(h => h.Timestamp)
                .ToList();
        }

        public List<DeviceHistory> GetRecentHistory(int count = 50)
        {
            return _history
                .OrderByDescending(h => h.Timestamp)
                .Take(count)
                .ToList();
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyPath))
                {
                    var json = File.ReadAllText(_historyPath);
                    var history = JsonConvert.DeserializeObject<List<DeviceHistory>>(json);
                    if (history != null)
                    {
                        _history = history;
                    }
                }
            }
            catch { }
        }

        private void SaveHistory()
        {
            try
            {
                var directory = Path.GetDirectoryName(_historyPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(_history, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_historyPath, json);
            }
            catch { }
        }
    }
}
