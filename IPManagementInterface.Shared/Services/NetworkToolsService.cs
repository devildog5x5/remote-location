using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace IPManagementInterface.Shared.Services
{
    public class NetworkToolsService
    {
        public async Task<PingResult> PingAsync(string ipAddress, int timeoutMs = 3000)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ipAddress, timeoutMs);
                
                return new PingResult
                {
                    Success = reply.Status == IPStatus.Success,
                    RoundTripTime = reply.RoundtripTime,
                    Status = reply.Status.ToString()
                };
            }
            catch
            {
                return new PingResult { Success = false, Status = "Failed" };
            }
        }

        public async Task<List<PortScanResult>> ScanPortsAsync(string ipAddress, IEnumerable<int> ports, int timeoutMs = 2000)
        {
            var results = new List<PortScanResult>();
            // Simplified port scanning for mobile
            // Full TCP scanning may not be available on iOS
            foreach (var port in ports)
            {
                var result = await ScanPortAsync(ipAddress, port, timeoutMs);
                results.Add(result);
            }
            return results;
        }

        private async Task<PortScanResult> ScanPortAsync(string ipAddress, int port, int timeoutMs)
        {
            // Simplified implementation for mobile
            // iOS has restrictions on raw socket access
            // Use HTTP check instead
            try
            {
                using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromMilliseconds(timeoutMs) };
                var response = await client.GetAsync($"http://{ipAddress}:{port}");
                return new PortScanResult { Port = port, IsOpen = response.IsSuccessStatusCode };
            }
            catch
            {
                return new PortScanResult { Port = port, IsOpen = false };
            }
        }

        public string? GetMacAddress(string ipAddress)
        {
            // iOS doesn't allow direct ARP table access
            // This would need platform-specific implementation
            return null;
        }
    }

    public class PingResult
    {
        public bool Success { get; set; }
        public long RoundTripTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PortScanResult
    {
        public int Port { get; set; }
        public bool IsOpen { get; set; }
    }
}
