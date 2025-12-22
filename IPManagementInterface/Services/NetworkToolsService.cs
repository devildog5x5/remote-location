using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IPManagementInterface.Services
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
            var tasks = ports.Select(port => ScanPortAsync(ipAddress, port, timeoutMs));
            var portResults = await Task.WhenAll(tasks);
            results.AddRange(portResults);
            return results;
        }

        private async Task<PortScanResult> ScanPortAsync(string ipAddress, int port, int timeoutMs)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(ipAddress, port);
                var timeoutTask = Task.Delay(timeoutMs);

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                
                if (completedTask == connectTask && client.Connected)
                {
                    return new PortScanResult { Port = port, IsOpen = true };
                }
            }
            catch { }

            return new PortScanResult { Port = port, IsOpen = false };
        }

        public async Task<List<string>> TracerouteAsync(string ipAddress, int maxHops = 30)
        {
            var hops = new List<string>();
            
            for (int ttl = 1; ttl <= maxHops; ttl++)
            {
                try
                {
                    using var ping = new Ping();
                    var options = new PingOptions(ttl, true);
                    var reply = await ping.SendPingAsync(ipAddress, 3000, new byte[32], options);
                    
                    if (reply.Status == IPStatus.Success)
                    {
                        hops.Add(reply.Address.ToString());
                        break;
                    }
                    else if (reply.Status == IPStatus.TtlExpired)
                    {
                        hops.Add(reply.Address.ToString());
                    }
                }
                catch
                {
                    break;
                }
            }

            return hops;
        }

        public string? GetMacAddress(string ipAddress)
        {
            try
            {
                var arpTable = GetArpTable();
                return arpTable.FirstOrDefault(entry => entry.IpAddress == ipAddress)?.MacAddress;
            }
            catch
            {
                return null;
            }
        }

        private List<ArpEntry> GetArpTable()
        {
            var entries = new List<ArpEntry>();
            
            try
            {
                var arpOutput = RunArpCommand();
                var lines = arpOutput.Split('\n');
                
                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        entries.Add(new ArpEntry
                        {
                            IpAddress = parts[0],
                            MacAddress = parts[1]
                        });
                    }
                }
            }
            catch { }

            return entries;
        }

        private string RunArpCommand()
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "arp",
                    Arguments = "-a",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
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

    public class ArpEntry
    {
        public string IpAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
    }
}
