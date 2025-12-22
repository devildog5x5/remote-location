using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IPManagementInterface.Models;
using Newtonsoft.Json;

namespace IPManagementInterface.Services
{
    public class ScheduledMonitoringService
    {
        private readonly DeviceManagerService _deviceManager;
        private readonly DeviceHistoryService _historyService;
        private readonly List<AlertRule> _alertRules = new();
        private Timer? _monitoringTimer;
        private bool _isMonitoring;
        private int _refreshIntervalMinutes = 5;

        public ScheduledMonitoringService(
            DeviceManagerService deviceManager,
            DeviceHistoryService historyService)
        {
            _deviceManager = deviceManager;
            _historyService = historyService;
            LoadAlertRules();
        }

        public int RefreshIntervalMinutes
        {
            get => _refreshIntervalMinutes;
            set
            {
                _refreshIntervalMinutes = value;
                if (_isMonitoring)
                {
                    StartMonitoring(_refreshIntervalMinutes);
                }
            }
        }

        public bool IsMonitoring
        {
            get => _isMonitoring;
            private set => _isMonitoring = value;
        }

        public void StartMonitoring(int intervalMinutes = 5)
        {
            if (_isMonitoring)
            {
                StopMonitoring();
            }

            _refreshIntervalMinutes = intervalMinutes;
            _isMonitoring = true;
            _monitoringTimer = new Timer(async _ => await PerformMonitoring(), null, 
                TimeSpan.Zero, 
                TimeSpan.FromMinutes(intervalMinutes));
        }

        public void StopMonitoring()
        {
            _isMonitoring = false;
            _monitoringTimer?.Dispose();
            _monitoringTimer = null;
        }

        private async Task PerformMonitoring()
        {
            if (!_isMonitoring) return;

            try
            {
                await _deviceManager.RefreshAllDevicesAsync();

                // Check alert rules
                foreach (var device in _deviceManager.Devices)
                {
                    CheckAlertRules(device);
                }
            }
            catch { }
        }

        private void CheckAlertRules(IoTDevice device)
        {
            foreach (var rule in _alertRules.Where(r => r.IsEnabled))
            {
                if (ShouldTriggerAlert(device, rule))
                {
                    TriggerAlert(device, rule);
                }
            }
        }

        private bool ShouldTriggerAlert(IoTDevice device, AlertRule rule)
        {
            // Check device filters
            if (!string.IsNullOrEmpty(rule.DeviceIpAddress) && device.IpAddress != rule.DeviceIpAddress)
                return false;

            if (!string.IsNullOrEmpty(rule.Group) && device.Group != rule.Group)
                return false;

            if (rule.DeviceType.HasValue && device.DeviceType != rule.DeviceType.Value)
                return false;

            // Check conditions
            switch (rule.Condition)
            {
                case AlertCondition.DeviceGoesOffline:
                    return device.Status == DeviceStatus.Offline;

                case AlertCondition.DeviceComesOnline:
                    return device.Status == DeviceStatus.Online;

                case AlertCondition.DeviceOfflineForDuration:
                    if (device.Status == DeviceStatus.Offline && device.LastOnlineTime.HasValue)
                    {
                        var offlineDuration = DateTime.Now - device.LastOnlineTime.Value;
                        return offlineDuration.TotalMinutes >= (rule.OfflineDurationMinutes ?? 5);
                    }
                    return false;

                default:
                    return false;
            }
        }

        private void TriggerAlert(IoTDevice device, AlertRule rule)
        {
            _historyService.AddEvent(new DeviceHistory
            {
                DeviceIpAddress = device.IpAddress,
                EventType = "Alert",
                Description = $"Alert triggered: {rule.Name}"
            });

            // Execute alert actions
            if (rule.Action == AlertAction.ShowNotification)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    // Show notification (can be enhanced with toast notifications)
                    System.Diagnostics.Debug.WriteLine($"ALERT: {rule.Name} - Device {device.Name} ({device.IpAddress})");
                });
            }
        }

        public void AddAlertRule(AlertRule rule)
        {
            _alertRules.Add(rule);
            SaveAlertRules();
        }

        public void RemoveAlertRule(AlertRule rule)
        {
            _alertRules.Remove(rule);
            SaveAlertRules();
        }

        public List<AlertRule> GetAlertRules()
        {
            return _alertRules.ToList();
        }

        private void LoadAlertRules()
        {
            // Load from storage (implement similar to DeviceManagerService)
        }

        private void SaveAlertRules()
        {
            // Save to storage (implement similar to DeviceManagerService)
        }
    }
}
