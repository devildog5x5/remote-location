#!/bin/bash

# iOS Development Setup Script for macOS
# This script helps set up the development environment for iOS development

echo "iOS Development Setup Script"
echo "============================"
echo ""

# Check if running on macOS
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "❌ Error: This script must be run on macOS"
    echo "iOS development requires macOS with Xcode"
    exit 1
fi

echo "✓ Running on macOS"
echo ""

# Check for .NET SDK
echo "Checking .NET SDK..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo "  ✓ .NET SDK found: $DOTNET_VERSION"
    
    if [[ $DOTNET_VERSION == 9.* ]]; then
        echo "  ✓ .NET 9.0 SDK detected"
    else
        echo "  ⚠ Warning: .NET 9.0 recommended (found $DOTNET_VERSION)"
        echo "    Install from: https://dotnet.microsoft.com/download"
    fi
else
    echo "  ✗ .NET SDK not found"
    echo "    Install from: https://dotnet.microsoft.com/download"
    echo ""
    echo "    Or use Homebrew:"
    echo "    brew install --cask dotnet"
    exit 1
fi

echo ""

# Check for Xcode
echo "Checking Xcode..."
if command -v xcodebuild &> /dev/null; then
    XCODE_VERSION=$(xcodebuild -version 2>&1 | head -n 1)
    echo "  ✓ Xcode found: $XCODE_VERSION"
    
    # Check if command line tools are installed
    if xcode-select -p &> /dev/null; then
        echo "  ✓ Xcode Command Line Tools installed"
    else
        echo "  ⚠ Installing Xcode Command Line Tools..."
        xcode-select --install
    fi
else
    echo "  ✗ Xcode not found"
    echo "    Install from Mac App Store"
    exit 1
fi

echo ""

# Check for CocoaPods (optional but recommended)
echo "Checking CocoaPods..."
if command -v pod &> /dev/null; then
    POD_VERSION=$(pod --version)
    echo "  ✓ CocoaPods found: $POD_VERSION"
else
    echo "  ℹ CocoaPods not found (optional)"
    echo "    Install with: sudo gem install cocoapods"
fi

echo ""

# Restore NuGet packages
echo "Restoring NuGet packages..."
if dotnet restore IPManagementInterface.Mobile.sln; then
    echo "  ✓ Packages restored successfully"
else
    echo "  ✗ Package restore failed"
    exit 1
fi

echo ""

# Build the solution
echo "Building solution..."
if dotnet build IPManagementInterface.Mobile.sln --configuration Debug; then
    echo "  ✓ Build successful"
else
    echo "  ✗ Build failed"
    exit 1
fi

echo ""
echo "✓ Setup complete!"
echo ""
echo "Next steps:"
echo "  1. Open IPManagementInterface.Mobile.sln in Visual Studio for Mac"
echo "  2. Select IPManagementInterface.iOS as the startup project"
echo "  3. Choose a simulator or connected device"
echo "  4. Press F5 to build and run"
echo ""

