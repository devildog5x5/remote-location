namespace IPManagementInterface.Shared.Models
{
    public class DeviceCredentials
    {
        public string DeviceIpAddress { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseCredentials { get; set; }
    }
}
