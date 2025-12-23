param(
    [string]$Configuration = "Debug",
    [string]$Target = "build"
)

Write-Host "Building Android App..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "Target: $Target" -ForegroundColor Cyan

$projectPath = "IPManagementInterface.Android\IPManagementInterface.Android.csproj"

if ($Target -eq "clean") {
    Write-Host "Cleaning Android project..." -ForegroundColor Yellow
    dotnet clean $projectPath --configuration $Configuration
    exit $LASTEXITCODE
}

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore $projectPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "Package restore failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Build the project
Write-Host "Building Android project..." -ForegroundColor Yellow
dotnet build $projectPath --configuration $Configuration

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild successful!" -ForegroundColor Green
    Write-Host "APK location: IPManagementInterface.Android\bin\$Configuration\net9.0-android\" -ForegroundColor Cyan
    
    # Check for APK
    $apkPath = Get-ChildItem -Path "IPManagementInterface.Android\bin\$Configuration\net9.0-android" -Filter "*.apk" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    
    if ($apkPath) {
        Write-Host "APK found: $($apkPath.FullName)" -ForegroundColor Green
        Write-Host "`nTo install on device:" -ForegroundColor Yellow
        Write-Host "  adb install `"$($apkPath.FullName)`"" -ForegroundColor White
    } else {
        Write-Host "`nNote: APK not found. Run 'dotnet publish' to create APK for distribution." -ForegroundColor Yellow
    }
} else {
    Write-Host "`nBuild failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

