Write-Host "Checking iOS Development Requirements..." -ForegroundColor Yellow
Write-Host ""

$allChecksPassed = $true

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Cyan
$dotnetCmd = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnetCmd) {
    $version = dotnet --version
    Write-Host "  ✓ .NET SDK found: $version" -ForegroundColor Green
    
    # Check for .NET 9.0
    if ($version -match "^9\.") {
        Write-Host "  ✓ .NET 9.0 SDK detected" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Warning: .NET 9.0 recommended (found $version)" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ✗ .NET SDK not found" -ForegroundColor Red
    Write-Host "    Install from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    $allChecksPassed = $false
}

Write-Host ""

# Check solution file
Write-Host "Checking project files..." -ForegroundColor Cyan
if (Test-Path "IPManagementInterface.Mobile.sln") {
    Write-Host "  ✓ Solution file found" -ForegroundColor Green
} else {
    Write-Host "  ✗ Solution file not found" -ForegroundColor Red
    $allChecksPassed = $false
}

if (Test-Path "IPManagementInterface.iOS\IPManagementInterface.iOS.csproj") {
    Write-Host "  ✓ iOS project found" -ForegroundColor Green
} else {
    Write-Host "  ✗ iOS project not found" -ForegroundColor Red
    $allChecksPassed = $false
}

if (Test-Path "IPManagementInterface.Shared\IPManagementInterface.Shared.csproj") {
    Write-Host "  ✓ Shared project found" -ForegroundColor Green
} else {
    Write-Host "  ✗ Shared project not found" -ForegroundColor Red
    $allChecksPassed = $false
}

Write-Host ""

# Check for Visual Studio
Write-Host "Checking development tools..." -ForegroundColor Cyan

# Check if running on macOS (for actual iOS development)
if ($IsMacOS -or $env:OS -eq "Darwin") {
    Write-Host "  ✓ Running on macOS" -ForegroundColor Green
    
    # Check for Xcode
    $xcodePath = Get-Command xcodebuild -ErrorAction SilentlyContinue
    if ($xcodePath) {
        try {
            $xcodeVersion = xcodebuild -version 2>&1 | Select-Object -First 1
            Write-Host "  ✓ Xcode found: $xcodeVersion" -ForegroundColor Green
        } catch {
            Write-Host "  ⚠ Xcode command found but version check failed" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ⚠ Xcode not found (required for iOS builds)" -ForegroundColor Yellow
        Write-Host "    Install from Mac App Store" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ⚠ Not running on macOS" -ForegroundColor Yellow
    Write-Host "    iOS development requires macOS with Xcode" -ForegroundColor Yellow
    Write-Host "    You can still edit code on Windows, but building requires a Mac" -ForegroundColor Yellow
}

Write-Host ""

# Check for Visual Studio
$vsPath = "${env:ProgramFiles}\Microsoft Visual Studio\2022"
if (Test-Path $vsPath) {
    Write-Host "  ✓ Visual Studio 2022 found" -ForegroundColor Green
} else {
    Write-Host "  ℹ Visual Studio 2022 not found in default location" -ForegroundColor Gray
    Write-Host "    You can use Visual Studio Code or any IDE with .NET support" -ForegroundColor Gray
}

Write-Host ""

# Summary
if ($allChecksPassed) {
    Write-Host "✓ All basic requirements met!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To build the iOS app, run:" -ForegroundColor Cyan
    Write-Host "  .\BuildIOS.ps1 -Configuration Debug" -ForegroundColor White
} else {
    Write-Host "✗ Some requirements are missing" -ForegroundColor Red
    Write-Host "Please install the missing components before building." -ForegroundColor Yellow
    exit 1
}

