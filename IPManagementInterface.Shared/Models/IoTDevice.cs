using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IPManagementInterface.Shared.Models
{
    public enum DeviceStatus
    {
        Online,
        Offline,
        Unknown
    }

    public enum DeviceType
    {
        Camera,
        Router,
        Switch,
        AccessPoint,
        Server,
        Printer,
        Other
    }

    public class IoTDevice : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _ipAddress = string.Empty;
        private int _port = 80;
        private string _protocol = "http";
        private DeviceType _deviceType = DeviceType.Other;
        private DeviceStatus _status = DeviceStatus.Unknown;
        private DateTime _lastSeen = DateTime.Now;
        private string? _manufacturer;
        private string? _model;
        private string? _firmwareVersion;
        private string? _group;
        private bool _isFavorite;
        private string? _notes;
        private string? _macAddress;
        private DateTime _firstSeen = DateTime.Now;
        private TimeSpan _totalUptime = TimeSpan.Zero;
        private DateTime? _lastOnlineTime;
        private Dictionary<string, string> _customProperties = new();
        private List<string> _tags = new();

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); }
        }

        public int Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        public string Protocol
        {
            get => _protocol;
            set { _protocol = value; OnPropertyChanged(); }
        }

        public DeviceType DeviceType
        {
            get => _deviceType;
            set { _deviceType = value; OnPropertyChanged(); }
        }

        public DeviceStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public DateTime LastSeen
        {
            get => _lastSeen;
            set { _lastSeen = value; OnPropertyChanged(); }
        }

        public string? Manufacturer
        {
            get => _manufacturer;
            set { _manufacturer = value; OnPropertyChanged(); }
        }

        public string? Model
        {
            get => _model;
            set { _model = value; OnPropertyChanged(); }
        }

        public string? FirmwareVersion
        {
            get => _firmwareVersion;
            set { _firmwareVersion = value; OnPropertyChanged(); }
        }

        public string? Group
        {
            get => _group;
            set { _group = value; OnPropertyChanged(); }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(); }
        }

        public string? Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public string? MacAddress
        {
            get => _macAddress;
            set { _macAddress = value; OnPropertyChanged(); }
        }

        public DateTime FirstSeen
        {
            get => _firstSeen;
            set { _firstSeen = value; OnPropertyChanged(); }
        }

        public TimeSpan TotalUptime
        {
            get => _totalUptime;
            set { _totalUptime = value; OnPropertyChanged(); }
        }

        public DateTime? LastOnlineTime
        {
            get => _lastOnlineTime;
            set { _lastOnlineTime = value; OnPropertyChanged(); }
        }

        public Dictionary<string, string> CustomProperties
        {
            get => _customProperties;
            set { _customProperties = value ?? new Dictionary<string, string>(); OnPropertyChanged(); }
        }

        public List<string> Tags
        {
            get => _tags;
            set { _tags = value ?? new List<string>(); OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
