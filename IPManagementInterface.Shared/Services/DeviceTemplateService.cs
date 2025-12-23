using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPManagementInterface.Shared.Models;
using Newtonsoft.Json;

namespace IPManagementInterface.Shared.Services
{
    public class DeviceTemplateService
    {
        private readonly string _templatesPath;
        private List<DeviceTemplate> _templates = new();

        public DeviceTemplateService()
        {
            _templatesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "device_templates.json"
            );
            LoadTemplates();
            InitializeDefaultTemplates();
        }

        public List<DeviceTemplate> GetTemplates()
        {
            return _templates.ToList();
        }

        public DeviceTemplate? GetTemplate(string name)
        {
            return _templates.FirstOrDefault(t => t.Name == name);
        }

        public void AddTemplate(DeviceTemplate template)
        {
            if (!_templates.Any(t => t.Name == template.Name))
            {
                _templates.Add(template);
                SaveTemplates();
            }
        }

        public IoTDevice CreateDeviceFromTemplate(DeviceTemplate template, string ipAddress, string? name = null)
        {
            var device = new IoTDevice
            {
                Name = name ?? $"{template.Name} {ipAddress}",
                IpAddress = ipAddress,
                Port = template.DefaultPort,
                Protocol = template.DefaultProtocol,
                DeviceType = template.DeviceType,
                Group = template.DefaultGroup,
                Tags = new List<string>(template.DefaultTags),
                CustomProperties = new Dictionary<string, string>(template.DefaultCustomProperties),
                Manufacturer = template.DefaultManufacturer,
                Model = template.DefaultModel
            };

            return device;
        }

        private void InitializeDefaultTemplates()
        {
            if (_templates.Any()) return;

            _templates.AddRange(new[]
            {
                new DeviceTemplate
                {
                    Name = "Generic IP Camera",
                    DeviceType = Models.DeviceType.Camera,
                    DefaultPort = 80,
                    DefaultProtocol = "http",
                    DefaultTags = new List<string> { "camera", "surveillance" }
                },
                new DeviceTemplate
                {
                    Name = "Network Router",
                    DeviceType = Models.DeviceType.Router,
                    DefaultPort = 80,
                    DefaultProtocol = "http",
                    DefaultTags = new List<string> { "network", "router" }
                },
                new DeviceTemplate
                {
                    Name = "Web Server",
                    DeviceType = Models.DeviceType.Server,
                    DefaultPort = 80,
                    DefaultProtocol = "http",
                    DefaultTags = new List<string> { "server", "web" }
                }
            });

            SaveTemplates();
        }

        private void LoadTemplates()
        {
            try
            {
                if (File.Exists(_templatesPath))
                {
                    var json = File.ReadAllText(_templatesPath);
                    var templates = JsonConvert.DeserializeObject<List<DeviceTemplate>>(json);
                    if (templates != null)
                    {
                        _templates = templates;
                    }
                }
            }
            catch { }
        }

        private void SaveTemplates()
        {
            try
            {
                var directory = Path.GetDirectoryName(_templatesPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(_templates, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_templatesPath, json);
            }
            catch { }
        }
    }
}
