using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IPManagementInterface.Shared.Models;

namespace IPManagementInterface.Shared.Services
{
    public class DeviceDiscoveryService
    {
        private readonly DeviceCommunicationService _communicationService;
        public event EventHandler<DiscoveryProgress>? ProgressUpdated;
        public event EventHandler<IoTDevice>? DeviceDiscovered;

        public DeviceDiscoveryService(DeviceCommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        public async Task<List<IoTDevice>> DiscoverDevicesAsync(
            Models.DeviceType? deviceType = null,
            string? group = null,
            int startHost = 1,
            int endHost = 254,
            IEnumerable<int>? customPorts = null,
            IProgress<DiscoveryProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var discoveredDevices = new List<IoTDevice>();
            var progressInfo = new DiscoveryProgress
            {
                StartTime = DateTime.Now,
                TotalHosts = (endHost - startHost + 1) * (customPorts?.Count() ?? 5) * 2,
                CurrentHost = 0
            };

            var baseIp = GetLocalNetworkBase();
            if (string.IsNullOrEmpty(baseIp))
                return discoveredDevices;

            var ports = customPorts?.ToList() ?? new List<int> { 80, 443, 8000, 8080, 8443 };
            var protocols = new List<string> { "http", "https" };
            var maxConcurrent = 20;
            var semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);
            var tasks = new List<Task>();
            int scannedCount = 0;
            object lockObject = new object();

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

                        await semaphore.WaitAsync(cancellationToken);
                        
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                progressInfo.CurrentIp = localIp;
                                progressInfo.StatusMessage = $"Scanning {localIp}:{localPort} ({localProtocol})...";
                                progress?.Report(progressInfo);
                                ProgressUpdated?.Invoke(this, progressInfo);

                                if (await _communicationService.CheckDeviceStatusAsync(localIp, localPort, localProtocol, cancellationToken))
                                {
                                    var device = await TryDiscoverDeviceAsync(localIp, localPort, localProtocol, deviceType, group, cancellationToken);
                                    if (device != null)
                                    {
                                        lock (discoveredDevices)
                                        {
                                            discoveredDevices.Add(device);
                                            progressInfo.FoundDevices = discoveredDevices.Count;
                                            progressInfo.RecentDiscoveries.Insert(0, $"{device.Name} ({device.IpAddress})");
                                            if (progressInfo.RecentDiscoveries.Count > 10)
                                            {
                                                progressInfo.RecentDiscoveries.RemoveAt(progressInfo.RecentDiscoveries.Count - 1);
                                            }
                                        }

                                        DeviceDiscovered?.Invoke(this, device);
                                    }
                                }
                            }
                            finally
                            {
                                lock (lockObject)
                                {
                                    scannedCount++;
                                    progressInfo.ScannedHosts = scannedCount;
                                    progressInfo.CurrentHost = scannedCount;
                                }
                                progress?.Report(progressInfo);
                                ProgressUpdated?.Invoke(this, progressInfo);
                                semaphore.Release();
                            }
                        }, cancellationToken));
                    }
                }
            }

            await Task.WhenAll(tasks);
            
            progressInfo.IsComplete = true;
            progressInfo.StatusMessage = $"Discovery complete! Found {discoveredDevices.Count} device(s)";
            progress?.Report(progressInfo);
            ProgressUpdated?.Invoke(this, progressInfo);
            
            return discoveredDevices;
        }

        private async Task<IoTDevice?> TryDiscoverDeviceAsync(
            string ipAddress,
            int port,
            string protocol,
            Models.DeviceType? filterType = null,
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

                var response = await _communicationService.SendRequestAsync(ipAddress, port, protocol, cancellationToken: cancellationToken);
                if (response != null)
                {
                    device = DetectDeviceType(device, response);
                }

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
            var fingerprint = FingerprintDevice(response);

            device.DeviceType = (Models.DeviceType)fingerprint.DeviceType;
            device.Name = fingerprint.Name ?? $"Device {device.IpAddress}";
            device.Manufacturer = fingerprint.Manufacturer;
            device.Model = fingerprint.Model;
            device.FirmwareVersion = fingerprint.FirmwareVersion;

            return device;
        }

        private DeviceFingerprint FingerprintDevice(string response)
        {
            var fingerprint = new DeviceFingerprint();

            if (response.Contains("camera") || response.Contains("ipcam") || response.Contains("webcam") || 
                response.Contains("axis") || response.Contains("hikvision") || response.Contains("dahua"))
            {
                fingerprint.DeviceType = Models.DeviceType.Camera;
                fingerprint.Name = "IP Camera";
                if (response.Contains("axis")) fingerprint.Manufacturer = "Axis";
                else if (response.Contains("hikvision")) fingerprint.Manufacturer = "Hikvision";
                else if (response.Contains("dahua")) fingerprint.Manufacturer = "Dahua";
            }
            else if (response.Contains("router") || response.Contains("gateway") || 
                     response.Contains("cisco") || response.Contains("netgear") || response.Contains("tp-link"))
            {
                fingerprint.DeviceType = Models.DeviceType.Router;
                fingerprint.Name = "Router";
                if (response.Contains("cisco")) fingerprint.Manufacturer = "Cisco";
                else if (response.Contains("netgear")) fingerprint.Manufacturer = "Netgear";
                else if (response.Contains("tp-link")) fingerprint.Manufacturer = "TP-Link";
            }
            else if (response.Contains("switch") || response.Contains("switching"))
            {
                fingerprint.DeviceType = Models.DeviceType.Switch;
                fingerprint.Name = "Network Switch";
            }
            else if (response.Contains("access point") || response.Contains("ap") || response.Contains("wireless"))
            {
                fingerprint.DeviceType = Models.DeviceType.AccessPoint;
                fingerprint.Name = "Access Point";
            }
            else if (response.Contains("server") || response.Contains("apache") || response.Contains("nginx") || 
                     response.Contains("iis") || response.Contains("tomcat"))
            {
                fingerprint.DeviceType = Models.DeviceType.Server;
                fingerprint.Name = "Server";
                if (response.Contains("apache")) fingerprint.Model = "Apache";
                else if (response.Contains("nginx")) fingerprint.Model = "Nginx";
                else if (response.Contains("iis")) fingerprint.Model = "IIS";
            }
            else if (response.Contains("printer") || response.Contains("hp") || response.Contains("canon") || 
                     response.Contains("epson") || response.Contains("brother"))
            {
                fingerprint.DeviceType = Models.DeviceType.Printer;
                fingerprint.Name = "Printer";
                if (response.Contains("hp")) fingerprint.Manufacturer = "HP";
                else if (response.Contains("canon")) fingerprint.Manufacturer = "Canon";
                else if (response.Contains("epson")) fingerprint.Manufacturer = "Epson";
                else if (response.Contains("brother")) fingerprint.Manufacturer = "Brother";
            }
            else
            {
                fingerprint.DeviceType = Models.DeviceType.Other;
                fingerprint.Name = "Unknown Device";
            }

            var firmwareMatch = Regex.Match(response, @"(?:firmware|version|v|ver)[\s:]*([\d.]+)", RegexOptions.IgnoreCase);
            if (firmwareMatch.Success)
            {
                fingerprint.FirmwareVersion = firmwareMatch.Groups[1].Value;
            }

            return fingerprint;
        }

        private class DeviceFingerprint
        {
            public Models.DeviceType DeviceType { get; set; }
            public string? Name { get; set; }
            public string? Manufacturer { get; set; }
            public string? Model { get; set; }
            public string? FirmwareVersion { get; set; }
        }

        private string GetLocalNetworkBase()
        {
            try
            {
                // On iOS, we'll use a simplified approach
                // In production, you might want to use Network.framework
                return "192.168.1";
            }
            catch { }

            return "192.168.1";
        }
    }
}
