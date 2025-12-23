using System.Collections.Generic;

namespace IPManagementInterface.Models
{
    public class DiscoveryProfile
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StartIp { get; set; } = "192.168.1.1";
        public string EndIp { get; set; } = "192.168.1.254";
        public List<int> Ports { get; set; } = new() { 80, 443, 8000, 8080, 8443 };
        public List<string> Protocols { get; set; } = new() { "http", "https" };
        public DeviceType? FilterType { get; set; }
        public string? FilterGroup { get; set; }
        public int TimeoutMs { get; set; } = 2000;
        public int MaxConcurrentScans { get; set; } = 50;
    }
}
