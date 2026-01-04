# IP Management Dashboard

A modern multi-platform dashboard application for managing IoT devices such as IP Cameras and other IP-based devices. Available for **Windows** (WPF) and **iOS** (.NET MAUI).

## Downloads

**[📥 Download Page →](DOWNLOAD.md)** - Complete download guide with all executables and installers

### Quick Downloads

- **[MSI Installer (Recommended)](https://github.com/devildog5x5/remote-location/releases/latest/download/IPManagementInterface-Setup.msi)** - Professional Windows installer
- **[Self-Contained Executable](https://github.com/devildog5x5/remote-location/releases/latest/download/IPManagementInterface.exe)** - Portable executable (works on Windows 7 SP1+)
- **[Portable Package](https://github.com/devildog5x5/remote-location/releases/latest/download/IPManagementInterface-Installer-v1.0.0.zip)** - ZIP archive with setup scripts

**Repository:** [https://github.com/devildog5x5/remote-location](https://github.com/devildog5x5/remote-location)

## Features

- **Dashboard Interface**: Tabbed view organized by device types (Cameras, Network, Servers, Other)
- **Device Management**: Add, remove, and manage multiple IoT devices
- **Multi-Protocol Support**: HTTP and HTTPS on standard and custom ports (80, 443, 8000, 8080, 8443, etc.)
- **Smart Device Discovery**: Scan network with filters by device type and/or group
- **Status Monitoring**: Real-time device status checking and monitoring
- **Multiple Themes**: Choose from 6 beautiful themes:
  - Light (default - welcoming and bright)
  - Dark
  - USMC (Red, Gold, and Blue)
  - Olive Drab (Military Green)
  - Ocean (Calming Blues and Teals)
  - Sunset (Warm Oranges and Purples)
- **Device Statistics**: View total, online, and offline device counts by category
- **Persistent Storage**: Devices are saved automatically
- **Colorful UI**: Vibrant, inviting interface with modern design

## Requirements

### Self-Contained Build (Recommended)
- **Windows 7 SP1 or later** (Windows 7, 8.1, 10, 11)
- **No .NET runtime installation required** - everything is bundled!

### Framework-Dependent Build
- .NET 8.0 Runtime
- Windows 7 SP1 or later

## Building

### Windows Application

#### For Development
1. Open `IPManagementInterface.sln` in Visual Studio 2022 or later
2. Restore NuGet packages
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

### iOS Application

#### Prerequisites
- macOS with Xcode 14.0+
- .NET 9.0 SDK
- Apple Developer account (for device deployment)

#### Quick Start
1. See [README_iOS_SETUP.md](README_iOS_SETUP.md) for detailed setup instructions
2. Open `IPManagementInterface.Mobile.sln` in Visual Studio for Mac
3. Set `IPManagementInterface.iOS` as startup project
4. Build and run (F5)

#### Build Scripts
```powershell
# Check requirements
.\CheckIOSRequirements.ps1

# Build iOS app
.\BuildIOS.ps1 -Configuration Debug

# Automated setup (macOS)
./SetupIOS.sh
```

### For Distribution (Self-Contained - Works on Older Windows)
Run the publish script to create a self-contained executable that works on Windows 7 SP1+:
```powershell
.\PublishForDistribution.ps1 -Configuration Release
```

This creates a single executable file (~73 MB) in `IPManagementInterface\bin\Release\net8.0-windows\win-x64\publish\` that includes the .NET runtime and will run on:
- Windows 7 SP1 and later
- Windows 8.1 and later  
- Windows 10 and later
- Windows 11

**No .NET installation required on target machines!**

## Usage

### Dashboard Tabs
The dashboard is organized into tabs:
- **All**: Shows all devices
- **📷 Cameras**: IP cameras and webcams
- **🌐 Network**: Routers, switches, and access points
- **🖥️ Servers**: Servers and printers
- **📦 Other**: All other device types

### Discovering Devices
1. Select a device type filter (optional) from the dropdown
2. Enter a group filter (optional) if you want to filter by device group
3. Click "🔍 Discover" button
4. The application will scan your local network for devices
5. Found devices will be automatically added to the appropriate tab

### Adding a Device Manually
1. Click "➕ Add Device" button
2. Fill in the device details in the right panel
3. The device will be automatically categorized into the appropriate tab

### Changing Themes
Use the theme selector dropdown in the header to switch between:
- Light
- Dark
- USMC
- Olive Drab
- Ocean
- Sunset

Your theme preference is saved and will be restored on next launch.

### Refreshing Device Status
- Click "🔄 Refresh" to check a single device's status
- Devices are automatically categorized when their type changes

## Project Structure

```
IPManagementInterface/
├── Models/              # Data models (IoTDevice, DeviceType, DeviceStatus)
├── Services/            # Business logic services
│   ├── DeviceCommunicationService.cs
│   ├── DeviceDiscoveryService.cs
│   └── DeviceManagerService.cs
├── ViewModels/          # MVVM ViewModels
│   ├── DashboardViewModel.cs
│   └── ThemeManager.cs
├── Views/               # XAML views
├── Themes/              # UI themes and styles
│   ├── Colors.xaml
│   ├── LightTheme.xaml
│   ├── DarkTheme.xaml
│   ├── USMCTheme.xaml
│   ├── OliveDrabTheme.xaml
│   ├── OceanTheme.xaml
│   └── SunsetTheme.xaml
├── Converters/          # Value converters
└── Resources/           # Icons and resources
    └── TestWrrior.ico   # Application icon
```

## Color Palettes

### Light Theme (Default)
- Background: #F5F7FA (Welcoming light gray)
- Primary: #2196F3 (Bright blue)
- Accent: #FF9800 (Vibrant orange)

### USMC Theme
- Primary: #C8102E (USMC Red)
- Accent: #FFD700 (Gold)
- Background: #F5F5F0 (Off-white)

### Olive Drab Theme
- Primary: #3D5A3D (Dark green)
- Accent: #6B8E6B (Medium green)
- Background: #F0F4F0 (Light green tint)

### Ocean Theme
- Primary: #006994 (Deep blue)
- Accent: #4A90A4 (Teal)
- Background: #F0F7FA (Light blue tint)

### Sunset Theme
- Primary: #FF6B35 (Orange)
- Accent: #8B4C9F (Purple)
- Background: #FFF5F0 (Warm light tint)

## License

This project is open source and available for use.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
