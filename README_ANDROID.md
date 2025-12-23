# Android App Setup Guide

Complete guide for setting up and building the IP Management Interface Android app.

## Prerequisites

### Required
- **Windows, macOS, or Linux** (Android development works on all platforms)
- **.NET 9.0 SDK** (download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download))
- **Android SDK** (included with Visual Studio or install via Android Studio)
- **Java Development Kit (JDK) 17 or later**

### Recommended
- **Visual Studio 2022** with Mobile development workload
- **Android Studio** (for Android SDK management and emulator)
- **Physical Android device** (Android 5.0 / API 21 or later) or Android Emulator

## Quick Setup

### Option 1: Using Visual Studio 2022

1. **Install Visual Studio 2022** with the "Mobile development with .NET" workload
2. **Open the solution:**
   ```
   File → Open → IPManagementInterface.Mobile.sln
   ```
3. **Set startup project:**
   - Right-click `IPManagementInterface.Android` → Set as Startup Project
4. **Select target:**
   - Choose an Android emulator or connected device
5. **Build and run:**
   - Press `F5` or click Run

### Option 2: Using Command Line

#### 1. Install .NET SDK

Download and install .NET 9.0 SDK from:
https://dotnet.microsoft.com/download

Verify installation:
```bash
dotnet --version
```

#### 2. Install Android SDK

**Option A: Via Android Studio**
1. Download [Android Studio](https://developer.android.com/studio)
2. Install Android Studio
3. Open Android Studio → SDK Manager
4. Install Android SDK Platform 35 (or latest)
5. Install Android SDK Build-Tools

**Option B: Via Visual Studio Installer**
1. Open Visual Studio Installer
2. Modify → Individual Components
3. Check "Android SDK setup (API level 35)"
4. Install

#### 3. Set Environment Variables (if needed)

**Windows:**
```powershell
# Add to System Environment Variables
ANDROID_HOME = C:\Users\<YourUser>\AppData\Local\Android\Sdk
# Add to PATH
%ANDROID_HOME%\platform-tools
%ANDROID_HOME%\tools
```

**macOS/Linux:**
```bash
export ANDROID_HOME=$HOME/Library/Android/sdk  # macOS
# or
export ANDROID_HOME=$HOME/Android/Sdk  # Linux

export PATH=$PATH:$ANDROID_HOME/platform-tools
export PATH=$PATH:$ANDROID_HOME/tools
```

## Building the App

### Using PowerShell Script

#### Build for Debug:
```powershell
.\BuildAndroid.ps1 -Configuration Debug
```

#### Build for Release:
```powershell
.\BuildAndroid.ps1 -Configuration Release
```

#### Publish APK for Distribution:
```powershell
.\PublishAndroid.ps1 -Configuration Release
```

### Using Command Line

#### Restore packages:
```bash
dotnet restore IPManagementInterface.Mobile.sln
```

#### Build:
```bash
dotnet build IPManagementInterface.Android\IPManagementInterface.Android.csproj --configuration Debug
```

#### Publish APK:
```bash
dotnet publish IPManagementInterface.Android\IPManagementInterface.Android.csproj --configuration Release -f net9.0-android
```

## Installing on Device

### Option 1: Via ADB (Android Debug Bridge)

1. **Enable Developer Options** on your Android device:
   - Go to Settings → About Phone
   - Tap "Build Number" 7 times
   - Go back to Settings → Developer Options
   - Enable "USB Debugging"

2. **Connect device via USB** and authorize debugging

3. **Install APK:**
   ```bash
   adb install path\to\IPManagementInterface.Android.apk
   ```

### Option 2: Transfer and Install

1. **Build APK** (see above)
2. **Transfer APK** to your Android device (via USB, email, cloud storage, etc.)
3. **On device:** Open file manager → Tap APK → Install
4. **Allow installation** from unknown sources if prompted

### Option 3: Via Visual Studio

1. Connect device or start emulator
2. Set `IPManagementInterface.Android` as startup project
3. Press `F5` to build and deploy

## Android Emulator Setup

### Using Android Studio

1. Open Android Studio
2. Tools → Device Manager
3. Create Virtual Device
4. Select device (e.g., Pixel 5)
5. Select system image (API 35 recommended)
6. Finish and start emulator

### Using Visual Studio

1. Visual Studio → Tools → Android → Android SDK Manager
2. Install Android SDK Platform
3. Tools → Android → Android Emulator Manager
4. Create new emulator
5. Start emulator

## Project Structure

```
IPManagementInterface.Mobile.sln          # Solution file
├── IPManagementInterface.Shared/          # Shared code (Views, ViewModels, Services)
│   ├── Views/                             # All UI pages
│   ├── ViewModels/                        # ViewModels
│   ├── Services/                          # Business logic
│   └── Models/                            # Data models
└── IPManagementInterface.Android/         # Android-specific project
    ├── MainActivity.cs                    # Main activity
    ├── MauiProgram.cs                     # MAUI app initialization
    └── IPManagementInterface.Android.csproj
```

## Permissions

The app requires the following Android permissions (automatically configured):
- `INTERNET` - For network communication
- `ACCESS_NETWORK_STATE` - To check network connectivity
- `ACCESS_WIFI_STATE` - To access WiFi information
- `CHANGE_WIFI_STATE` - To modify WiFi settings (if needed)

These are automatically included in the generated AndroidManifest.xml.

## Troubleshooting

### Build Errors

**Error: "Android SDK not found"**
- Install Android SDK via Android Studio or Visual Studio Installer
- Set `ANDROID_HOME` environment variable
- Verify SDK path in Visual Studio → Tools → Options → Xamarin → Android Settings

**Error: "Java not found"**
- Install JDK 17 or later
- Set `JAVA_HOME` environment variable
- Verify in Visual Studio → Tools → Options → Xamarin → Android Settings

**Error: "NuGet package restore failed"**
- Check internet connection
- Run: `dotnet restore IPManagementInterface.Mobile.sln --force`
- Clear NuGet cache: `dotnet nuget locals all --clear`

### Device Connection Issues

**Device not detected:**
- Enable USB Debugging on device
- Install USB drivers for your device
- Try different USB cable/port
- Restart ADB: `adb kill-server && adb start-server`

**Installation fails:**
- Enable "Install from unknown sources" in device settings
- Uninstall previous version if exists
- Check device storage space

### Emulator Issues

**Emulator won't start:**
- Enable virtualization in BIOS (Intel VT-x or AMD-V)
- Allocate more RAM to emulator
- Try different system image

**App crashes on emulator:**
- Check emulator logs in Visual Studio Output window
- Verify emulator has internet connection
- Try different API level

## Distribution

### Creating Signed APK

1. **Generate keystore:**
   ```bash
   keytool -genkey -v -keystore ipmanagement.keystore -alias ipmanagement -keyalg RSA -keysize 2048 -validity 10000
   ```

2. **Configure signing in project:**
   - Right-click `IPManagementInterface.Android` → Properties
   - Android Package Signing
   - Select keystore and configure

3. **Publish signed APK:**
   ```powershell
   .\PublishAndroid.ps1 -Configuration Release
   ```

### Google Play Store

1. Create Google Play Developer account ($25 one-time fee)
2. Create app listing in Google Play Console
3. Build signed AAB (Android App Bundle):
   ```bash
   dotnet publish -f net9.0-android -c Release /p:AndroidPackageFormat=aab
   ```
4. Upload AAB to Play Console
5. Submit for review

## System Requirements

### Minimum Android Version
- **Android 5.0 (API 21)** - Lollipop
- Supports 99%+ of active Android devices

### Target Android Version
- **Android 15 (API 35)** - Latest features and optimizations

### Device Requirements
- ARM64 or x86_64 processor
- 100MB+ storage space
- Internet connection for device discovery

## Testing

### Supported Devices
- ✅ iPhone 15 and newer (via iOS project)
- ✅ Android 5.0+ devices
- ✅ Android tablets
- ✅ Android TV (with limitations)

### Tested On
- Android 12+ (recommended)
- Android 10+ (fully supported)
- Android 5.0-9.0 (basic support)

## Additional Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [Android Development Guide](https://developer.android.com/)
- [Visual Studio Mobile Development](https://learn.microsoft.com/visualstudio/cross-platform/)
- [Android Studio](https://developer.android.com/studio)

## Getting Help

If you encounter issues:
1. Check the troubleshooting section above
2. Review error messages in Visual Studio output
3. Check Android logcat: `adb logcat`
4. Verify all prerequisites are installed correctly
5. Open an issue on the GitHub repository

## Version

Current Version: 1.0
Target Framework: .NET 9.0
Minimum Android: API 21 (Android 5.0)
Target Android: API 35 (Android 15)

