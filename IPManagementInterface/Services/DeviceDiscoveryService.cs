using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IPManagementInterface.Models;

namespace IPManagementInterface.Services
{
    public class DeviceDiscoveryService
    {
        private readonly DeviceCommunicationService _communicationService;
        private readonly NetworkToolsService? _networkTools;

        public DeviceDiscoveryService(DeviceCommunicationService communicationService, NetworkToolsService? networkTools = null)
        {
            _communicationService = communicationService;
            _networkTools = networkTools;
        }

        public async Task<List<IoTDevice>> DiscoverDevicesAsync(
            DeviceType? deviceType = null,
            string? group = null,
            int startHost = 1,
            int endHost = 254,
            IEnumerable<int>? customPorts = null,
            CancellationToken cancellationToken = default)
        {
            var discoveredDevices = new List<IoTDevice>();
            var tasks = new List<Task>();

            // Get local network base IP
            var baseIp = GetLocalNetworkBase();
            if (string.IsNullOrEmpty(baseIp))
                return discoveredDevices;

            // Use custom ports or default ports
            var ports = customPorts?.ToList() ?? new List<int> { 80, 443, 8000, 8080, 8443 };
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
                                    // Try to get MAC address
                                    if (_networkTools != null)
                                    {
                                        device.MacAddress = _networkTools.GetMacAddress(localIp);
                                    }

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

            // Enhanced device fingerprinting
            var fingerprint = FingerprintDevice(response);

            device.DeviceType = fingerprint.DeviceType;
            device.Name = fingerprint.Name ?? $"Device {device.IpAddress}";
            device.Manufacturer = fingerprint.Manufacturer;
            device.Model = fingerprint.Model;
            device.FirmwareVersion = fingerprint.FirmwareVersion;

            return device;
        }

        private DeviceFingerprint FingerprintDevice(string response)
        {
            var fingerprint = new DeviceFingerprint();

            // Camera detection
            if (response.Contains("camera") || response.Contains("ipcam") || response.Contains("webcam") || 
                response.Contains("axis") || response.Contains("hikvision") || response.Contains("dahua"))
            {
                fingerprint.DeviceType = DeviceType.Camera;
                fingerprint.Name = "IP Camera";
                if (response.Contains("axis")) fingerprint.Manufacturer = "Axis";
                else if (response.Contains("hikvision")) fingerprint.Manufacturer = "Hikvision";
                else if (response.Contains("dahua")) fingerprint.Manufacturer = "Dahua";
            }
            // Router detection
            else if (response.Contains("router") || response.Contains("gateway") || 
                     response.Contains("cisco") || response.Contains("netgear") || response.Contains("tp-link"))
            {
                fingerprint.DeviceType = DeviceType.Router;
                fingerprint.Name = "Router";
                if (response.Contains("cisco")) fingerprint.Manufacturer = "Cisco";
                else if (response.Contains("netgear")) fingerprint.Manufacturer = "Netgear";
                else if (response.Contains("tp-link")) fingerprint.Manufacturer = "TP-Link";
            }
            // Switch detection
            else if (response.Contains("switch") || response.Contains("switching"))
            {
                fingerprint.DeviceType = DeviceType.Switch;
                fingerprint.Name = "Network Switch";
            }
            // Access Point detection
            else if (response.Contains("access point") || response.Contains("ap") || response.Contains("wireless"))
            {
                fingerprint.DeviceType = DeviceType.AccessPoint;
                fingerprint.Name = "Access Point";
            }
            // Server detection
            else if (response.Contains("server") || response.Contains("apache") || response.Contains("nginx") || 
                     response.Contains("iis") || response.Contains("tomcat"))
            {
                fingerprint.DeviceType = DeviceType.Server;
                fingerprint.Name = "Server";
                if (response.Contains("apache")) fingerprint.Model = "Apache";
                else if (response.Contains("nginx")) fingerprint.Model = "Nginx";
                else if (response.Contains("iis")) fingerprint.Model = "IIS";
            }
            // Printer detection
            else if (response.Contains("printer") || response.Contains("hp") || response.Contains("canon") || 
                     response.Contains("epson") || response.Contains("brother"))
            {
                fingerprint.DeviceType = DeviceType.Printer;
                fingerprint.Name = "Printer";
                if (response.Contains("hp")) fingerprint.Manufacturer = "HP";
                else if (response.Contains("canon")) fingerprint.Manufacturer = "Canon";
                else if (response.Contains("epson")) fingerprint.Manufacturer = "Epson";
                else if (response.Contains("brother")) fingerprint.Manufacturer = "Brother";
            }
            else
            {
                fingerprint.DeviceType = DeviceType.Other;
                fingerprint.Name = "Unknown Device";
            }

            // Try to extract firmware version
            var firmwareMatch = Regex.Match(response, @"(?:firmware|version|v|ver)[\s:]*([\d.]+)", RegexOptions.IgnoreCase);
            if (firmwareMatch.Success)
            {
                fingerprint.FirmwareVersion = firmwareMatch.Groups[1].Value;
            }

            return fingerprint;
        }

        private class DeviceFingerprint
        {
            public DeviceType DeviceType { get; set; }
            public string? Name { get; set; }
            public string? Manufacturer { get; set; }
            public string? Model { get; set; }
            public string? FirmwareVersion { get; set; }
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
