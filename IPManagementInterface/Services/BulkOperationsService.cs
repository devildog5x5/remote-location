using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPManagementInterface.Models;

namespace IPManagementInterface.Services
{
    public class BulkOperationsService
    {
        private readonly DeviceManagerService _deviceManager;

        public BulkOperationsService(DeviceManagerService deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public async Task BulkRefreshStatusAsync(IEnumerable<IoTDevice> devices)
        {
            var tasks = devices.Select(d => _deviceManager.RefreshDeviceStatusAsync(d));
            await Task.WhenAll(tasks);
        }

        public void BulkAssignGroup(IEnumerable<IoTDevice> devices, string group)
        {
            foreach (var device in devices)
            {
                device.Group = group;
                _deviceManager.UpdateDevice(device);
            }
        }

        public void BulkAssignTags(IEnumerable<IoTDevice> devices, IEnumerable<string> tags)
        {
            var tagList = tags.ToList();
            foreach (var device in devices)
            {
                foreach (var tag in tagList)
                {
                    if (!device.Tags.Contains(tag))
                    {
                        device.Tags.Add(tag);
                    }
                }
                _deviceManager.UpdateDevice(device);
            }
        }

        public void BulkRemoveTags(IEnumerable<IoTDevice> devices, IEnumerable<string> tags)
        {
            var tagList = tags.ToList();
            foreach (var device in devices)
            {
                foreach (var tag in tagList)
                {
                    device.Tags.Remove(tag);
                }
                _deviceManager.UpdateDevice(device);
            }
        }

        public void BulkDelete(IEnumerable<IoTDevice> devices)
        {
            foreach (var device in devices.ToList())
            {
                _deviceManager.RemoveDevice(device);
            }
        }

        public void BulkSetFavorite(IEnumerable<IoTDevice> devices, bool isFavorite)
        {
            foreach (var device in devices)
            {
                device.IsFavorite = isFavorite;
                _deviceManager.UpdateDevice(device);
            }
        }
    }
}
