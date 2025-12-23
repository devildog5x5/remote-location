using System;
using System.Collections.Generic;

namespace IPManagementInterface.Models
{
    public class DiscoveryProgress
    {
        public int TotalHosts { get; set; }
        public int ScannedHosts { get; set; }
        public int FoundDevices { get; set; }
        public int CurrentHost { get; set; }
        public string CurrentIp { get; set; } = string.Empty;
        public List<string> RecentDiscoveries { get; set; } = new();
        public DateTime StartTime { get; set; }
        public TimeSpan ElapsedTime => DateTime.Now - StartTime;
        public bool IsComplete { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }
}
