using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IPManagementInterface.Models;
using Newtonsoft.Json;

namespace IPManagementInterface.Services
{
    public class ExportImportService
    {
        public void ExportToCsv(IEnumerable<IoTDevice> devices, string filePath)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Name,IP Address,Port,Protocol,Device Type,Status,Group,Manufacturer,Model,Firmware,Last Seen,Tags,Notes");

            foreach (var device in devices)
            {
                var tags = string.Join(";", device.Tags);
                csv.AppendLine($"{EscapeCsv(device.Name)},{device.IpAddress},{device.Port},{device.Protocol}," +
                    $"{device.DeviceType},{device.Status},{EscapeCsv(device.Group ?? "")}," +
                    $"{EscapeCsv(device.Manufacturer ?? "")},{EscapeCsv(device.Model ?? "")}," +
                    $"{EscapeCsv(device.FirmwareVersion ?? "")},{device.LastSeen:yyyy-MM-dd HH:mm:ss}," +
                    $"{EscapeCsv(tags)},{EscapeCsv(device.Notes ?? "")}");
            }

            File.WriteAllText(filePath, csv.ToString());
        }

        public void ExportToJson(IEnumerable<IoTDevice> devices, string filePath)
        {
            var json = JsonConvert.SerializeObject(devices.ToList(), Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void ExportToXml(IEnumerable<IoTDevice> devices, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<IoTDevice>));
            using var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, devices.ToList());
        }

        public List<IoTDevice> ImportFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var devices = JsonConvert.DeserializeObject<List<IoTDevice>>(json);
            return devices ?? new List<IoTDevice>();
        }

        public List<IoTDevice> ImportFromCsv(string filePath)
        {
            var devices = new List<IoTDevice>();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length < 2) return devices;

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = ParseCsvLine(lines[i]);
                if (parts.Length >= 6)
                {
                    var device = new IoTDevice
                    {
                        Name = parts[0],
                        IpAddress = parts[1],
                        Port = int.TryParse(parts[2], out var port) ? port : 80,
                        Protocol = parts[3],
                        DeviceType = Enum.TryParse<DeviceType>(parts[4], out var type) ? type : DeviceType.Other,
                        Status = Enum.TryParse<DeviceStatus>(parts[5], out var status) ? status : DeviceStatus.Unknown,
                        Group = parts.Length > 6 ? parts[6] : null,
                        Manufacturer = parts.Length > 7 ? parts[7] : null,
                        Model = parts.Length > 8 ? parts[8] : null,
                        FirmwareVersion = parts.Length > 9 ? parts[9] : null,
                        Tags = parts.Length > 11 && !string.IsNullOrEmpty(parts[11]) ? parts[11].Split(';').ToList() : new List<string>(),
                        Notes = parts.Length > 12 ? parts[12] : null
                    };

                    if (parts.Length > 10 && DateTime.TryParse(parts[10], out var lastSeen))
                    {
                        device.LastSeen = lastSeen;
                    }

                    devices.Add(device);
                }
            }

            return devices;
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        private string[] ParseCsvLine(string line)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            foreach (var c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            parts.Add(current.ToString());

            return parts.ToArray();
        }
    }
}
