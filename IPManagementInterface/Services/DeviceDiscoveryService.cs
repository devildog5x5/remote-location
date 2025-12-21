using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using IPManagementInterface.Models;

namespace IPManagementInterface.Services
{
    public class DeviceDiscoveryService
    {
        private readonly DeviceCommunicationService _communicationService;

        public DeviceDiscoveryService(DeviceCommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        public async Task<List<IoTDevice>> DiscoverDevicesAsync(
            DeviceType? deviceType = null,
            string? group = null,
            int startHost = 1,
            int endHost = 254,
            CancellationToken cancellationToken = default)
        {
            var discoveredDevices = new List<IoTDevice>();
            var tasks = new List<Task>();

            // Get local network base IP
            var baseIp = GetLocalNetworkBase();
            if (string.IsNullOrEmpty(baseIp))
                return discoveredDevices;

            // Common ports to check
            var ports = new[] { 80, 443, 8000, 8080, 8443 };
            var protocols = new[] { "http", "https" };

            for (int host = startHost; host <= endHost; host++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var localHost = host;
                var localIp = $"{baseIp}.{localHost}";

                foreach (var port in ports)
                {
                    foreach (var protocol in protocols)
                    {
                        var localPort = port;
                        var localProtocol = protocol;

                        tasks.Add(Task.Run(async () =>
                        {
                            if (await _communicationService.CheckDeviceStatusAsync(localIp, localPort, localProtocol, cancellationToken))
                            {
                                var device = await TryDiscoverDeviceAsync(localIp, localPort, localProtocol, deviceType, group, cancellationToken);
                                if (device != null)
                                {
                                    lock (discoveredDevices)
                                    {
                                        discoveredDevices.Add(device);
                                    }
                                }
                            }
                        }, cancellationToken));
                    }
                }
            }

            await Task.WhenAll(tasks);
            return discoveredDevices;
        }

        private async Task<IoTDevice?> TryDiscoverDeviceAsync(
            string ipAddress,
            int port,
            string protocol,
            DeviceType? filterType = null,
            string? filterGroup = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var device = new IoTDevice
                {
                    IpAddress = ipAddress,
                    Port = port,
                    Protocol = protocol,
                    Status = DeviceStatus.Online,
                    LastSeen = DateTime.Now
                };

                // Try to detect device type from response
                var response = await _communicationService.SendRequestAsync(ipAddress, port, protocol, cancellationToken: cancellationToken);
                if (response != null)
                {
                    device = DetectDeviceType(device, response);
                }

                // Apply filters
                if (filterType.HasValue && device.DeviceType != filterType.Value)
                    return null;

                if (!string.IsNullOrEmpty(filterGroup) && device.Group != filterGroup)
                    return null;

                return device;
            }
            catch
            {
                return null;
            }
        }

        private IoTDevice DetectDeviceType(IoTDevice device, string response)
        {
            response = response.ToLowerInvariant();

            if (response.Contains("camera") || response.Contains("ipcam") || response.Contains("webcam"))
            {
                device.DeviceType = DeviceType.Camera;
                device.Name = $"Camera {device.IpAddress}";
            }
            else if (response.Contains("router") || response.Contains("gateway"))
            {
                device.DeviceType = DeviceType.Router;
                device.Name = $"Router {device.IpAddress}";
            }
            else if (response.Contains("switch"))
            {
                device.DeviceType = DeviceType.Switch;
                device.Name = $"Switch {device.IpAddress}";
            }
            else if (response.Contains("access point") || response.Contains("ap"))
            {
                device.DeviceType = DeviceType.AccessPoint;
                device.Name = $"Access Point {device.IpAddress}";
            }
            else if (response.Contains("server"))
            {
                device.DeviceType = DeviceType.Server;
                device.Name = $"Server {device.IpAddress}";
            }
            else if (response.Contains("printer"))
            {
                device.DeviceType = DeviceType.Printer;
                device.Name = $"Printer {device.IpAddress}";
            }
            else
            {
                device.DeviceType = DeviceType.Other;
                device.Name = $"Device {device.IpAddress}";
            }

            return device;
        }

        private string GetLocalNetworkBase()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        var ipParts = ip.ToString().Split('.');
                        if (ipParts.Length == 4)
                        {
                            return $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}";
                        }
                    }
                }
            }
            catch { }

            return "192.168.1";
        }
    }
}
