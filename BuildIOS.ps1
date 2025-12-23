param(
    [string]$Configuration = "Debug",
    [string]$Target = "build",
    [string]$DeviceId = ""
)

Write-Host "Building iOS App..." -ForegroundColor Yellow

$solutionPath = "IPManagementInterface.Mobile.sln"
$iosProjectPath = "IPManagementInterface.iOS\IPManagementInterface.iOS.csproj"

# Check if dotnet is available
$dotnetCmd = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnetCmd) {
    Write-Host "`nERROR: .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 9.0 SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Display .NET version
$dotnetVersion = dotnet --version
Write-Host "Using .NET SDK version: $dotnetVersion" -ForegroundColor Cyan

# Restore packages
Write-Host "`nRestoring NuGet packages..." -ForegroundColor Cyan
dotnet restore $solutionPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nPackage restore failed!" -ForegroundColor Red
    exit 1
}

switch ($Target.ToLower()) {
    "build" {
        Write-Host "`nBuilding iOS project..." -ForegroundColor Cyan
        dotnet build $iosProjectPath --configuration $Configuration
        if ($LASTEXITCODE -eq 0) {
            Write-Host "`nBuild successful!" -ForegroundColor Green
        } else {
            Write-Host "`nBuild failed!" -ForegroundColor Red
            exit 1
        }
    }
    "clean" {
        Write-Host "`nCleaning build artifacts..." -ForegroundColor Cyan
        dotnet clean $iosProjectPath --configuration $Configuration
        Write-Host "Clean completed!" -ForegroundColor Green
    }
    "publish" {
        Write-Host "`nPublishing iOS app..." -ForegroundColor Cyan
        Write-Host "Note: iOS publishing requires:" -ForegroundColor Yellow
        Write-Host "  - macOS with Xcode installed" -ForegroundColor White
        Write-Host "  - Apple Developer account" -ForegroundColor White
        Write-Host "  - Proper provisioning profiles configured" -ForegroundColor White
        Write-Host "`nFor iOS publishing, use Visual Studio or Xcode directly." -ForegroundColor Yellow
    }
    default {
        Write-Host "`nUnknown target: $Target" -ForegroundColor Red
        Write-Host "Valid targets: build, clean, publish" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "`nDone!" -ForegroundColor Green

