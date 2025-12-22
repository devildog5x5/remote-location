# Installing WiX Toolset

To build the Windows Installer (MSI) package, you need to install WiX Toolset.

## Option 1: Download and Install (Recommended)

1. Download WiX Toolset v3.11 or later from:
   https://wixtoolset.org/releases/

2. Run the installer and follow the setup wizard

3. After installation, run `BuildWiXInstaller.ps1` again

## Option 2: Install via Chocolatey (If you have Chocolatey)

```powershell
choco install wixtoolset
```

## Option 3: Install via winget (Windows Package Manager)

```powershell
winget install WiXToolset.WiXToolset
```

## After Installation

Once WiX is installed, run:

```powershell
.\BuildWiXInstaller.ps1 -Configuration Release
```

This will create `InstallerOutput\IPManagementInterface-Setup.msi` - a professional Windows Installer package.

## What the WiX Installer Provides

- ✅ Professional Windows Installer (MSI) package
- ✅ Start Menu shortcut
- ✅ Desktop shortcut
- ✅ Proper uninstall support (via Add/Remove Programs)
- ✅ Registry entries
- ✅ Standard Windows installation experience
