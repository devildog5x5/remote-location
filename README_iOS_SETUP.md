# iOS App Setup Guide

Complete guide for setting up and building the IP Management Interface iOS app.

## Prerequisites

### Required (macOS only)
- **macOS** (iOS development requires macOS)
- **Xcode 14.0 or later** (download from Mac App Store)
- **.NET 9.0 SDK** (download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download))

### Recommended
- **Visual Studio 2022 for Mac** or **Visual Studio Code** with C# extension
- **Apple Developer Account** (for device deployment and App Store distribution)

## Quick Setup

### Option 1: Automated Setup (macOS)

1. Make the setup script executable:
   ```bash
   chmod +x SetupIOS.sh
   ```

2. Run the setup script:
   ```bash
   ./SetupIOS.sh
   ```

The script will:
- Check for .NET SDK
- Verify Xcode installation
- Restore NuGet packages
- Build the solution

### Option 2: Manual Setup

#### 1. Install .NET SDK

Download and install .NET 9.0 SDK from:
https://dotnet.microsoft.com/download

Or using Homebrew:
```bash
brew install --cask dotnet
```

Verify installation:
```bash
dotnet --version
```

#### 2. Install Xcode

1. Open Mac App Store
2. Search for "Xcode"
3. Install Xcode (large download, ~12GB)
4. Open Xcode once to accept license agreement
5. Install Command Line Tools (if prompted):
   ```bash
   xcode-select --install
   ```

#### 3. Verify Requirements (Windows/macOS)

Run the requirements checker:
```powershell
.\CheckIOSRequirements.ps1
```

This will verify:
- .NET SDK installation
- Project files
- Xcode (on macOS)
- Visual Studio (optional)

## Building the App

### Using Visual Studio 2022 for Mac

1. **Open the solution:**
   ```
   File → Open → IPManagementInterface.Mobile.sln
   ```

2. **Set startup project:**
   - Right-click `IPManagementInterface.iOS` → Set as Startup Project

3. **Select target:**
   - Choose a simulator (iPhone 15, iPad, etc.) or connected device
   - Note: Device deployment requires Apple Developer account

4. **Build and run:**
   - Press `F5` or click Run

### Using Command Line (macOS)

#### Build for Debug:
```powershell
.\BuildIOS.ps1 -Configuration Debug
```

#### Build for Release:
```powershell
.\BuildIOS.ps1 -Configuration Release
```

#### Clean build artifacts:
```powershell
.\BuildIOS.ps1 -Target clean
```

Or using dotnet CLI directly:
```bash
# Restore packages
dotnet restore IPManagementInterface.Mobile.sln

# Build
dotnet build IPManagementInterface.iOS/IPManagementInterface.iOS.csproj --configuration Debug

# Clean
dotnet clean IPManagementInterface.iOS/IPManagementInterface.iOS.csproj --configuration Debug
```

## Development Workflow

### Windows Users

If you're developing on Windows:
- You can edit code and build the shared project
- For iOS-specific builds, you need:
  - A Mac (physical or cloud-based like MacStadium)
  - Remote connection to Mac for building
  - Or use Visual Studio's Mac build agent

### Project Structure

```
IPManagementInterface.Mobile.sln          # Solution file
├── IPManagementInterface.Shared/          # Shared code (Views, ViewModels, Services)
│   ├── Views/                             # All UI pages
│   ├── ViewModels/                        # ViewModels
│   ├── Services/                          # Business logic
│   └── Models/                            # Data models
└── IPManagementInterface.iOS/             # iOS-specific project
    ├── Program.cs                         # Entry point
    ├── AppDelegate.cs                     # App delegate
    └── Platforms/iOS/
        └── Info.plist                     # iOS configuration
```

## Network Permissions

The app requires local network access for device discovery. iOS will prompt users for permission on first launch.

Permissions are configured in:
- `IPManagementInterface.iOS/Platforms/iOS/Info.plist`
- `IPManagementInterface.Shared/Info.plist`

## Troubleshooting

### Build Errors

**Error: "Xcode not found"**
- Ensure Xcode is installed from Mac App Store
- Run `xcode-select --install` to install Command Line Tools
- Verify: `xcodebuild -version`

**Error: ".NET SDK not found"**
- Install .NET 9.0 SDK from dotnet.microsoft.com
- Verify: `dotnet --version`

**Error: "NuGet package restore failed"**
- Check internet connection
- Run: `dotnet restore IPManagementInterface.Mobile.sln --force`

**Error: "Code signing" errors**
- Configure signing in Xcode
- Open workspace in Xcode: `open IPManagementInterface.iOS/IPManagementInterface.iOS.csproj`
- Select project → Signing & Capabilities → Configure Team

### Simulator Issues

**Simulator not starting:**
- Open Simulator app directly: `open -a Simulator`
- Or from Xcode: Xcode → Open Developer Tool → Simulator

**App crashes on launch:**
- Check Console.app for crash logs
- Verify Info.plist has required permissions
- Check that all NuGet packages restored correctly

### Device Deployment

**Requirements for physical device:**
1. Apple Developer account (free or paid)
2. Device registered in Apple Developer portal
3. Provisioning profile configured in Xcode
4. Device connected via USB
5. Trust computer on device when prompted

**Free Apple Developer account:**
- Limited to 7 days per app
- Requires re-installation after expiration
- Good for testing

**Paid Apple Developer account ($99/year):**
- No expiration
- App Store distribution
- TestFlight beta testing

## Distribution

### TestFlight (Beta Testing)

1. Build archive in Visual Studio or Xcode
2. Upload to App Store Connect
3. Add testers in TestFlight section
4. Testers receive email invitation

### App Store

1. Configure App Store Connect listing
2. Build release archive
3. Upload via Visual Studio or Xcode
4. Submit for review
5. Release to App Store

## Useful Commands

```bash
# Check .NET SDK version
dotnet --version

# List installed .NET runtimes
dotnet --list-runtimes

# Restore packages
dotnet restore IPManagementInterface.Mobile.sln

# Build solution
dotnet build IPManagementInterface.Mobile.sln

# Clean solution
dotnet clean IPManagementInterface.Mobile.sln

# Check for updates
dotnet --list-sdks
```

## Additional Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [iOS Development Guide](https://developer.apple.com/ios/)
- [Xcode Documentation](https://developer.apple.com/documentation/xcode)
- [Apple Developer Portal](https://developer.apple.com/)

## Getting Help

If you encounter issues:
1. Check the troubleshooting section above
2. Review error messages in Visual Studio/Xcode output
3. Check Console.app for system logs
4. Verify all prerequisites are installed correctly

