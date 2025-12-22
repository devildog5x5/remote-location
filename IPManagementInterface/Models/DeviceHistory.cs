using System;

namespace IPManagementInterface.Models
{
    public class DeviceHistory
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string DeviceIpAddress { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty; // StatusChange, Edit, Discovered, etc.
        public string Description { get; set; } = string.Empty;
        public DeviceStatus? OldStatus { get; set; }
        public DeviceStatus? NewStatus { get; set; }
        public string? Details { get; set; }
    }
}
