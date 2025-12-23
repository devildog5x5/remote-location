using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IPManagementInterface.Shared.Models;

namespace IPManagementInterface.Shared.Services
{
    public class ScheduledMonitoringService
    {
        private readonly DeviceManagerService _deviceManager;
        private readonly DeviceHistoryService _historyService;
        private Timer? _monitoringTimer;
        private bool _isMonitoring;
        private int _refreshIntervalMinutes = 5;

        public ScheduledMonitoringService(
            DeviceManagerService deviceManager,
            DeviceHistoryService historyService)
        {
            _deviceManager = deviceManager;
            _historyService = historyService;
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

        public bool IsMonitoring => _isMonitoring;

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
            }
            catch { }
        }
    }
}
