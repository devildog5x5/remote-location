# IP Management Interface - iOS App

This is the iOS version of the IP Management Interface application, built using .NET MAUI (Multi-platform App UI) to share code with the Windows version.

## Features

- **Device Discovery**: Scan your local network for IoT devices
- **Device Management**: View, add, edit, and remove devices
- **Real-time Status**: Check device online/offline status
- **Search & Filter**: Quickly find devices by name, IP, or type
- **Device Details**: View comprehensive device information
- **Statistics**: View device statistics and analytics
- **History**: Track device activity and status changes

## Requirements

- iOS 13.0 or later
- iPhone or iPad
- Xcode 14.0 or later (for development)
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio for Mac (for development)

## Building for iOS

### Prerequisites

1. Install Xcode from the Mac App Store
2. Install .NET 8.0 SDK
3. Install Visual Studio 2022 with iOS development tools

### Build Steps

1. Open the solution in Visual Studio
2. Select the iOS project as the startup project
3. Connect your iPhone/iPad or select a simulator
4. Build and run (F5)

### For Distribution

1. Configure your Apple Developer account in Xcode
2. Set up provisioning profiles
3. Archive the app in Visual Studio
4. Distribute through App Store or TestFlight

## Network Permissions

The app requires local network access to discover devices. iOS will prompt the user for permission on first launch.

## Architecture

- **Shared Code**: Models, Services, ViewModels in `IPManagementInterface.Shared`
- **iOS-Specific**: Platform-specific implementations in `IPManagementInterface.iOS`
- **MAUI**: Cross-platform UI using .NET MAUI

## Notes

- Network discovery on iOS has some limitations compared to Windows
- Some advanced features may be simplified for mobile
- The app uses the same backend services as the Windows version
