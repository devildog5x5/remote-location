# IP Management Interface - Downloads

Download ready-to-use executables and installers for the IP Management Dashboard application.

> **Note:** GitHub releases are created automatically. If download links don't work, the release may still be in progress. Check the [releases page](https://github.com/devildog5x5/remote-location/releases) for the latest version.

## 🎯 Recommended: MSI Installer

**[Download MSI Installer](https://github.com/devildog5x5/remote-location/releases/latest/download/IPManagementInterface-Setup.msi)**

**Size:** Varies | **Version:** 1.0.0

The MSI installer is the **recommended option** for Windows installation:

- ✅ **Professional Installation** - Standard Windows MSI installer
- ✅ **Easy Setup** - Guided installation wizard
- ✅ **Clean Uninstall** - Proper removal through Windows Settings
- ✅ **Desktop Shortcuts** - Optional desktop and Start Menu shortcuts
- ✅ **System Integration** - Properly registered Windows application

**Requirements:**
- Windows 7 SP1 or later (Windows 7, 8.1, 10, 11)
- .NET 8.0 Runtime (included in self-contained builds or install separately)

**Installation Notes:**
- Standard Windows installer experience
- Follow the installation wizard
- Optional: Create desktop shortcuts

---

## 💻 Standalone Executable

### Self-Contained Build (Recommended)

**[Download Self-Contained Executable](https://github.com/devildog5x5/remote-location/releases/latest/download/IPManagementInterface.exe)**

**Size:** ~73 MB | **Version:** 1.0.0

A self-contained executable that includes everything needed to run:

- ✅ **No Installation Required** - Just download and run
- ✅ **Includes .NET Runtime** - No need to install .NET separately
- ✅ **Works on Older Windows** - Runs on Windows 7 SP1 and later
- ✅ **Portable** - Can run from any location
- ✅ **Single File** - Everything bundled in one executable

**Requirements:**
- Windows 7 SP1 or later (Windows 7, 8.1, 10, 11)
- No additional software needed - everything is included

**Usage:**
- Simply double-click `IPManagementInterface.exe` to launch
- No installation, no admin rights required
- Perfect for quick deployment or testing

**Note:** This is the self-contained build created by `PublishForDistribution.ps1`

---

## 📦 Portable Package

**[Download Portable Package](https://github.com/devildog5x5/remote-location/releases/latest/download/IPManagementInterface-Installer-v1.0.0.zip)**

**Size:** Varies | **Version:** 1.0.0

A portable package containing the application and setup scripts:

- ✅ **Complete Package** - Includes executable and setup scripts
- ✅ **Portable Deployment** - Extract and run from any location
- ✅ **Setup Scripts Included** - Batch files for easy setup
- ✅ **Uninstall Script** - Included uninstall batch file

**Contents:**
- `IPManagementInterface.exe` - Main application executable
- `README.txt` - Usage instructions
- `Setup.bat` - Setup script (optional)
- `Uninstall.bat` - Uninstall script

**Requirements:**
- Windows 7 SP1 or later
- .NET 8.0 Runtime (unless using self-contained build)

---

## 📋 Build Options

| Option | Type | Size | .NET Runtime | Best For |
|--------|------|------|--------------|----------|
| **MSI Installer** | Installer | Medium | Required* | Production deployment |
| **Self-Contained Executable** | Standalone | ~73 MB | Included | Older Windows, portable use |
| **Portable Package** | ZIP Archive | Medium | Required* | Manual deployment |

\* Can use self-contained build which includes runtime

---

## 🚀 Quick Start Guide

### Option 1: Standard Installation
**→ Download the [MSI Installer](#-recommended-msi-installer)**
- Professional Windows installation
- System integration
- Easy uninstallation

### Option 2: Portable Use
**→ Download the [Self-Contained Executable](#self-contained-build-recommended)**
- No installation needed
- Works on Windows 7 SP1+
- Perfect for USB drives or quick deployment

### Option 3: Manual Deployment
**→ Download the [Portable Package](#-portable-package)**
- Extract and customize
- Includes setup scripts
- Full control over deployment

---

## 📦 What's Included

All versions include:
- ✅ Dashboard interface with tabbed device organization
- ✅ Device management (add, remove, edit devices)
- ✅ Multi-protocol support (HTTP/HTTPS, custom ports)
- ✅ Smart device discovery with network scanning
- ✅ Real-time status monitoring
- ✅ 6 beautiful themes (Light, Dark, USMC, Olive Drab, Ocean, Sunset)
- ✅ Device statistics and reporting
- ✅ Persistent device storage
- ✅ Modern, colorful UI

---

## 🎨 Available Platforms

- **Windows Desktop** - Full-featured WPF application (this download page)
- **iOS** - .NET MAUI application (see [README_iOS_SETUP.md](README_iOS_SETUP.md))
- **Android** - .NET MAUI application (see [README_ANDROID.md](README_ANDROID.md))

---

## 🔗 Additional Resources

- **Repository:** [https://github.com/devildog5x5/remote-location](https://github.com/devildog5x5/remote-location)
- **Documentation:** See [README.md](README.md) for detailed usage instructions
- **iOS Setup:** See [README_iOS_SETUP.md](README_iOS_SETUP.md) for iOS build instructions
- **Android Setup:** See [README_ANDROID.md](README_ANDROID.md) for Android build instructions
- **Deployment Guide:** See [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) for deployment information
- **Issues:** Report bugs or request features on GitHub Issues (check repository URL)

---

## 📝 Installation Instructions

### MSI Installer
1. Download `IPManagementInterface-Setup.msi`
2. Double-click to run the installer (admin rights may be required)
3. Follow the installation wizard
4. Launch from desktop shortcut or Start Menu

### Self-Contained Executable
1. Download `IPManagementInterface.exe`
2. Double-click to run
3. No installation required!

### Portable Package
1. Download `IPManagementInterface-Installer-v1.0.0.zip`
2. Extract to desired location
3. Run `IPManagementInterface.exe` directly
4. Optional: Run `Setup.bat` for additional setup

---

## ❓ Troubleshooting

**Q: Which version should I use?**  
A: For most users, the MSI Installer is recommended. For portable use or older Windows versions, use the Self-Contained Executable.

**Q: Do I need to install .NET?**  
A: The Self-Contained Executable includes .NET and doesn't require separate installation. The MSI installer may require .NET 8.0 Runtime.

**Q: Will it work on Windows 7?**  
A: Yes! The Self-Contained Executable works on Windows 7 SP1 and later.

**Q: Can I run it from a USB drive?**  
A: Yes! Both the Self-Contained Executable and Portable Package are fully portable.

---

## 🔧 Building from Source

If you prefer to build from source, see the [README.md](README.md) for detailed build instructions.

**Quick build commands:**
```powershell
# Self-contained build (recommended)
.\PublishForDistribution.ps1 -Configuration Release

# Standard build
dotnet build IPManagementInterface.sln --configuration Release

# MSI Installer
.\BuildWiXInstaller.ps1
```

---

**Last Updated:** Version 1.0.0

