using System.Collections.Generic;

namespace IPManagementInterface.Shared.Models
{
    public class DeviceTemplate
    {
        public string Name { get; set; } = string.Empty;
        public Models.DeviceType DeviceType { get; set; }
        public int DefaultPort { get; set; } = 80;
        public string DefaultProtocol { get; set; } = "http";
        public string? DefaultGroup { get; set; }
        public List<string> DefaultTags { get; set; } = new();
        public Dictionary<string, string> DefaultCustomProperties { get; set; } = new();
        public string? DefaultManufacturer { get; set; }
        public string? DefaultModel { get; set; }
    }
}
