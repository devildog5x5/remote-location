using System;

namespace IPManagementInterface.Models
{
    public class AlertRule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? DeviceIpAddress { get; set; } // null = all devices
        public string? Group { get; set; } // null = all groups
        public DeviceType? DeviceType { get; set; } // null = all types
        public AlertCondition Condition { get; set; }
        public AlertAction Action { get; set; }
        public bool IsEnabled { get; set; } = true;
        public int? OfflineDurationMinutes { get; set; } // For offline alerts
    }

    public enum AlertCondition
    {
        DeviceGoesOffline,
        DeviceComesOnline,
        DeviceOfflineForDuration,
        StatusChange
    }

    public enum AlertAction
    {
        ShowNotification,
        PlaySound,
        LogEvent
    }
}
