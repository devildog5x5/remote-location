# Deployment Guide - Testing on Android and iOS

Complete step-by-step instructions for deploying the IP Management Interface app to physical devices for testing.

## Table of Contents
- [Android Deployment](#android-deployment)
- [iOS Deployment](#ios-deployment)
- [Troubleshooting](#troubleshooting)

---

## Android Deployment

### Prerequisites

1. **Android Device Requirements:**
   - Android 5.0 (API 21) or later
   - USB Debugging enabled
   - At least 100MB free storage

2. **Development Machine Requirements:**
   - Windows, macOS, or Linux
   - .NET 9.0 SDK installed
   - Android SDK installed (via Android Studio or Visual Studio)
   - USB drivers for your Android device (if on Windows)

### Step 1: Install Prerequisites

#### Install .NET 9.0 SDK
```powershell
# Download from: https://dotnet.microsoft.com/download
# Or verify installation:
dotnet --version
# Should show: 9.0.x or higher
```

#### Install Android SDK
**Option A: Via Android Studio (Recommended)**
1. Download [Android Studio](https://developer.android.com/studio)
2. Install Android Studio
3. Open Android Studio → SDK Manager
4. Install:
   - Android SDK Platform 35 (or latest)
   - Android SDK Build-Tools
   - Android SDK Platform-Tools

**Option B: Via Visual Studio Installer**
1. Open Visual Studio Installer
2. Modify → Individual Components
3. Check "Android SDK setup (API level 35)"
4. Install

#### Set Environment Variables (if needed)
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

### Step 2: Prepare Android Device

1. **Enable Developer Options:**
   - Go to Settings → About Phone
   - Tap "Build Number" 7 times
   - You'll see "You are now a developer!"

2. **Enable USB Debugging:**
   - Go to Settings → Developer Options
   - Enable "USB Debugging"
   - Enable "Install via USB" (if available)

3. **Connect Device:**
   - Connect Android device to computer via USB
   - On device, tap "Allow USB debugging" when prompted
   - Check "Always allow from this computer" (optional)

4. **Verify Connection:**
   ```powershell
   adb devices
   # Should show your device listed
   # Example output:
   # List of devices attached
   # ABC123XYZ    device
   ```

### Step 3: Build the Android App

#### Option A: Using PowerShell Script (Easiest)
```powershell
cd C:\Users\rober\Documents\GitHub\IPManagementInterface
.\BuildAndroid.ps1 -Configuration Debug
```

#### Option B: Using Command Line
```powershell
cd C:\Users\rober\Documents\GitHub\IPManagementInterface
dotnet restore IPManagementInterface.Mobile.sln
dotnet build IPManagementInterface.Android\IPManagementInterface.Android.csproj --configuration Debug
```

### Step 4: Deploy to Device

#### Option A: Direct Deploy via ADB
```powershell
# Find the APK file
$apkPath = Get-ChildItem -Path "IPManagementInterface.Android\bin\Debug\net9.0-android" -Filter "*.apk" -Recurse | Select-Object -First 1

# Install on device
adb install "$($apkPath.FullName)"
```

#### Option B: Deploy via Visual Studio
1. Open `IPManagementInterface.Mobile.sln` in Visual Studio
2. Set `IPManagementInterface.Android` as startup project
3. Select your connected device from the device dropdown
4. Press `F5` or click Run
5. App will build and deploy automatically

#### Option C: Transfer and Install Manually
1. **Build APK:**
   ```powershell
   .\PublishAndroid.ps1 -Configuration Release
   ```

2. **Find APK:**
   - Location: `IPManagementInterface.Android\bin\Release\net9.0-android\publish\`

3. **Transfer to Device:**
   - Copy APK to device via USB, email, cloud storage, etc.

4. **Install on Device:**
   - Open file manager on Android
   - Navigate to APK location
   - Tap APK file
   - Tap "Install"
   - Allow installation from unknown sources if prompted

### Step 5: Verify Installation

1. Check device home screen for "IP Management" app icon
2. Launch the app
3. Grant network permissions when prompted
4. Test device discovery and management features

---

## iOS Deployment

### Prerequisites

1. **macOS Computer Required:**
   - iOS development requires macOS
   - Cannot build iOS apps on Windows/Linux

2. **iOS Device Requirements:**
   - iPhone 15 or later (or any iOS 13.0+ device)
   - iOS 13.0 or later
   - At least 100MB free storage

3. **Development Machine Requirements:**
   - macOS (any recent version)
   - Xcode 14.0 or later
   - .NET 9.0 SDK
   - Apple Developer account (free or paid)

### Step 1: Install Prerequisites on macOS

#### Install .NET 9.0 SDK
```bash
# Download from: https://dotnet.microsoft.com/download
# Or using Homebrew:
brew install --cask dotnet

# Verify installation:
dotnet --version
# Should show: 9.0.x or higher
```

#### Install Xcode
1. Open Mac App Store
2. Search for "Xcode"
3. Install Xcode (large download, ~12GB)
4. Open Xcode once to accept license agreement
5. Install Command Line Tools:
   ```bash
   xcode-select --install
   ```

#### Verify Xcode Installation
```bash
xcodebuild -version
# Should show Xcode version
```

### Step 2: Set Up Apple Developer Account

#### Free Apple Developer Account (7-day limit)
1. No signup required for basic testing
2. Apps expire after 7 days
3. Good for initial testing

#### Paid Apple Developer Account ($99/year)
1. Sign up at [developer.apple.com](https://developer.apple.com)
2. Provides:
   - No expiration
   - App Store distribution
   - TestFlight beta testing
   - More device registrations

### Step 3: Prepare iOS Device

1. **Connect iPhone to Mac:**
   - Use USB cable
   - Trust computer when prompted on iPhone

2. **Enable Developer Mode (iOS 16+):**
   - Go to Settings → Privacy & Security
   - Enable "Developer Mode"
   - Restart device if prompted

3. **Trust Developer Certificate:**
   - After first install, go to Settings → General → VPN & Device Management
   - Tap your developer certificate
   - Tap "Trust"

### Step 4: Build the iOS App

#### Option A: Using PowerShell Script (macOS)
```bash
cd ~/Documents/GitHub/IPManagementInterface
./BuildIOS.ps1 -Configuration Debug
```

#### Option B: Using Command Line
```bash
cd ~/Documents/GitHub/IPManagementInterface
dotnet restore IPManagementInterface.Mobile.sln
dotnet build IPManagementInterface.iOS\IPManagementInterface.iOS.csproj --configuration Debug
```

#### Option C: Using Visual Studio for Mac
1. Open `IPManagementInterface.Mobile.sln` in Visual Studio for Mac
2. Set `IPManagementInterface.iOS` as startup project
3. Select your connected iPhone from device dropdown
4. Press `F5` or click Run
5. App will build and deploy automatically

### Step 5: Configure Code Signing

#### Automatic Signing (Easiest)
1. Open project in Visual Studio for Mac or Xcode
2. Select project → Signing & Capabilities
3. Check "Automatically manage signing"
4. Select your Apple Developer team
5. Xcode will create provisioning profile automatically

#### Manual Signing
1. Open project in Xcode:
   ```bash
   open IPManagementInterface.iOS/IPManagementInterface.iOS.csproj
   ```
2. Select project → Signing & Capabilities
3. Uncheck "Automatically manage signing"
4. Select Provisioning Profile
5. Select Signing Certificate

### Step 6: Deploy to Device

#### Option A: Via Visual Studio for Mac
1. Connect iPhone via USB
2. Select device from dropdown
3. Press `F5` or click Run
4. App builds and installs automatically

#### Option B: Via Xcode
1. Open project in Xcode:
   ```bash
   open IPManagementInterface.iOS/IPManagementInterface.iOS.csproj
   ```
2. Select your iPhone from device dropdown
3. Click Run button (▶️)
4. App builds and installs automatically

#### Option C: Via Command Line
```bash
# Build and deploy
dotnet build IPManagementInterface.iOS\IPManagementInterface.iOS.csproj --configuration Debug
# Then use Xcode or Visual Studio to deploy
```

### Step 7: Verify Installation

1. Check iPhone home screen for "IP Management" app icon
2. Launch the app
3. Grant network permissions when prompted
4. Test device discovery and management features

---

## Quick Reference

### Android
```powershell
# 1. Enable USB debugging on device
# 2. Connect device
adb devices

# 3. Build
.\BuildAndroid.ps1

# 4. Install
adb install "IPManagementInterface.Android\bin\Debug\net9.0-android\*.apk"
```

### iOS
```bash
# 1. Connect iPhone to Mac
# 2. Trust computer on iPhone
# 3. Build
./BuildIOS.ps1

# 4. Deploy via Visual Studio or Xcode (F5)
```

---

## Troubleshooting

### Android Issues

**Device not detected:**
```powershell
# Restart ADB
adb kill-server
adb start-server
adb devices
```

**Installation fails:**
- Enable "Install from unknown sources" in device settings
- Uninstall previous version: `adb uninstall com.devildog5x5.ipmanagementinterface`
- Check device storage space

**Build errors:**
- Verify Android SDK is installed: `adb version`
- Check .NET SDK: `dotnet --version`
- Restore packages: `dotnet restore IPManagementInterface.Mobile.sln`

### iOS Issues

**Code signing errors:**
- Verify Apple Developer account is signed in
- Check Signing & Capabilities in Xcode
- Ensure device is registered in Apple Developer portal

**Build fails:**
- Verify Xcode is installed: `xcodebuild -version`
- Check .NET SDK: `dotnet --version`
- Clean build: `dotnet clean IPManagementInterface.Mobile.sln`

**App won't launch:**
- Check Settings → General → VPN & Device Management
- Trust developer certificate
- Enable Developer Mode (iOS 16+)

**Network permissions:**
- Go to Settings → IP Management → Local Network
- Enable local network access

---

## Testing Checklist

### Android Testing
- [ ] App installs successfully
- [ ] App launches without crashes
- [ ] Network permissions granted
- [ ] Device discovery works
- [ ] Can add devices manually
- [ ] Can ping devices
- [ ] UI displays correctly
- [ ] Themes work properly

### iOS Testing
- [ ] App installs successfully
- [ ] App launches without crashes
- [ ] Network permissions granted
- [ ] Device discovery works
- [ ] Can add devices manually
- [ ] Can ping devices
- [ ] UI displays correctly
- [ ] Themes work properly
- [ ] Works in portrait and landscape

---

## Next Steps

After successful deployment:
1. Test all features thoroughly
2. Report any bugs or issues
3. For production release:
   - Build Release configuration
   - Sign with production certificates
   - Submit to app stores (Google Play / App Store)

---

## Support

For issues or questions:
- Check troubleshooting section above
- Review error messages in build output
- Check device logs: `adb logcat` (Android) or Console.app (iOS)
- Open an issue on GitHub repository

