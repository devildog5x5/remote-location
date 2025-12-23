# Development Tools Reference

This document describes all the build scripts and development tools available for the IP Management Interface project.

## Build Scripts

### Windows Build Tools

#### `PublishForDistribution.ps1`
Creates a self-contained Windows executable for distribution.

**Usage:**
```powershell
.\PublishForDistribution.ps1 -Configuration Release
```

**Output:** Self-contained executable in `IPManagementInterface\bin\Release\net8.0-windows\win-x64\publish\`

---

#### `BuildInstaller.ps1`
Creates a ZIP-based installer package with setup scripts.

**Usage:**
```powershell
.\BuildInstaller.ps1 -Configuration Release
```

**Output:** `IPManagementInterface-Installer-v1.0.0.zip` containing executable and setup scripts

---

#### `BuildWiXInstaller.ps1`
Creates a professional Windows Installer (MSI) package using WiX Toolset.

**Prerequisites:** WiX Toolset v3.11+ installed

**Usage:**
```powershell
.\BuildWiXInstaller.ps1 -Configuration Release
```

**Output:** `InstallerOutput\IPManagementInterface-Setup.msi`

**Features:**
- Start Menu shortcut
- Desktop shortcut
- Windows Installer integration
- Proper uninstall support

**See also:** [INSTALL_WIX.md](INSTALL_WIX.md)

---

### iOS Build Tools

#### `BuildIOS.ps1`
Builds the iOS app using .NET CLI.

**Usage:**
```powershell
# Build Debug
.\BuildIOS.ps1 -Configuration Debug

# Build Release
.\BuildIOS.ps1 -Configuration Release

# Clean build artifacts
.\BuildIOS.ps1 -Target clean

# Publish (requires macOS)
.\BuildIOS.ps1 -Target publish
```

**Note:** Actual iOS builds require macOS with Xcode. This script can restore packages and validate builds on Windows.

---

#### `SetupIOS.sh` (macOS only)
Automated setup script for iOS development on macOS.

**Prerequisites:** macOS only

**Usage:**
```bash
chmod +x SetupIOS.sh
./SetupIOS.sh
```

**What it does:**
- Checks .NET SDK installation
- Verifies Xcode installation
- Installs Command Line Tools if needed
- Restores NuGet packages
- Builds the solution

---

#### `CheckIOSRequirements.ps1`
Validates that all requirements for iOS development are met.

**Usage:**
```powershell
.\CheckIOSRequirements.ps1
```

**Checks:**
- .NET SDK installation and version
- Project files exist
- Xcode (on macOS)
- Visual Studio (optional)

---

### Utility Scripts

#### `CleanBuildArtifacts.ps1`
Removes build artifacts (bin/obj folders) to force a clean rebuild.

**Usage:**
```powershell
# Clean all projects
.\CleanBuildArtifacts.ps1

# Clean specific projects
.\CleanBuildArtifacts.ps1 -iOS
.\CleanBuildArtifacts.ps1 -Windows
.\CleanBuildArtifacts.ps1 -Shared

# Combine options
.\CleanBuildArtifacts.ps1 -iOS -Shared
```

**What it cleans:**
- `bin/` folders
- `obj/` folders
- `.vs/` folder (Visual Studio cache)

---

## Command Line Tools

### Dotnet CLI Commands

All projects can also be built using the standard `dotnet` CLI:

```powershell
# Restore packages
dotnet restore IPManagementInterface.sln
dotnet restore IPManagementInterface.Mobile.sln

# Build
dotnet build IPManagementInterface.sln --configuration Release
dotnet build IPManagementInterface.Mobile.sln --configuration Debug

# Clean
dotnet clean IPManagementInterface.sln
dotnet clean IPManagementInterface.Mobile.sln

# Publish (Windows)
dotnet publish IPManagementInterface\IPManagementInterface.csproj --configuration Release --runtime win-x64 --self-contained true

# List SDKs
dotnet --list-sdks

# Check version
dotnet --version
```

---

## Quick Reference

### Windows Development

| Task | Command |
|------|---------|
| Build for development | `dotnet build IPManagementInterface.sln` |
| Publish for distribution | `.\PublishForDistribution.ps1 -Configuration Release` |
| Create installer (ZIP) | `.\BuildInstaller.ps1 -Configuration Release` |
| Create installer (MSI) | `.\BuildWiXInstaller.ps1 -Configuration Release` |
| Clean build artifacts | `.\CleanBuildArtifacts.ps1 -Windows` |

### iOS Development

| Task | Command |
|------|---------|
| Check requirements | `.\CheckIOSRequirements.ps1` |
| Setup (macOS) | `./SetupIOS.sh` |
| Build | `.\BuildIOS.ps1 -Configuration Debug` |
| Clean | `.\BuildIOS.ps1 -Target clean` |
| Clean build artifacts | `.\CleanBuildArtifacts.ps1 -iOS` |

### General

| Task | Command |
|------|---------|
| Clean everything | `.\CleanBuildArtifacts.ps1` |
| Restore packages | `dotnet restore` |

---

## Project Structure

```
IPManagementInterface/
├── Build Scripts
│   ├── BuildInstaller.ps1          # ZIP installer
│   ├── BuildWiXInstaller.ps1       # MSI installer
│   ├── BuildIOS.ps1                # iOS build script
│   ├── PublishForDistribution.ps1  # Windows publish
│   ├── CleanBuildArtifacts.ps1     # Clean utility
│   └── CheckIOSRequirements.ps1    # Requirements checker
│
├── Setup Scripts
│   └── SetupIOS.sh                 # iOS setup (macOS)
│
├── Solutions
│   ├── IPManagementInterface.sln   # Windows solution
│   └── IPManagementInterface.Mobile.sln  # iOS/MAUI solution
│
└── Documentation
    ├── README.md                   # Main documentation
    ├── README_iOS_SETUP.md         # iOS setup guide
    ├── TOOLS.md                    # This file
    └── INSTALL_WIX.md              # WiX installer guide
```

---

## Troubleshooting

### Build Script Issues

**PowerShell execution policy:**
```powershell
# If scripts won't run, adjust execution policy (Administrator)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Script not found:**
- Ensure you're in the project root directory
- Check that the script file exists

**Build failures:**
- Run `dotnet restore` first
- Check error messages in the output
- Verify all prerequisites are installed

### iOS-Specific Issues

**"Xcode not found" (on macOS):**
- Install Xcode from Mac App Store
- Run `xcode-select --install`
- Verify: `xcodebuild -version`

**".NET SDK not found":**
- Install .NET 9.0 SDK from dotnet.microsoft.com
- Verify: `dotnet --version`

**See [README_iOS_SETUP.md](README_iOS_SETUP.md) for detailed iOS troubleshooting**

---

## Tips

1. **Always restore packages** after pulling changes:
   ```powershell
   dotnet restore
   ```

2. **Clean before major builds** if you encounter strange errors:
   ```powershell
   .\CleanBuildArtifacts.ps1
   ```

3. **Use Release configuration** for production builds:
   ```powershell
   .\PublishForDistribution.ps1 -Configuration Release
   ```

4. **Check requirements** before building iOS:
   ```powershell
   .\CheckIOSRequirements.ps1
   ```

5. **Verify WiX installation** before building MSI:
   - See [INSTALL_WIX.md](INSTALL_WIX.md)

---

## Contributing

When adding new build scripts or tools:

1. Add documentation to this file
2. Include usage examples
3. Add error handling
4. Use consistent parameter naming
5. Test on clean environment

