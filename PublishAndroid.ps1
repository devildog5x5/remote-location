param(
    [string]$Configuration = "Release"
)

Write-Host "Publishing Android App for Distribution..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan

$projectPath = "IPManagementInterface.Android\IPManagementInterface.Android.csproj"
$outputPath = "IPManagementInterface.Android\bin\$Configuration\net9.0-android\publish"

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore $projectPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "Package restore failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Publish the project
Write-Host "Publishing Android project..." -ForegroundColor Yellow
dotnet publish $projectPath --configuration $Configuration -f net9.0-android -c $Configuration

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nPublish successful!" -ForegroundColor Green
    
    # Find APK
    $apkPath = Get-ChildItem -Path $outputPath -Filter "*.apk" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    
    if ($apkPath) {
        Write-Host "APK location: $($apkPath.FullName)" -ForegroundColor Green
        Write-Host "`nTo install on device:" -ForegroundColor Yellow
        Write-Host "  adb install `"$($apkPath.FullName)`"" -ForegroundColor White
        Write-Host "`nOr transfer the APK to your device and install manually." -ForegroundColor Cyan
    } else {
        Write-Host "`nAPK not found in expected location. Check: $outputPath" -ForegroundColor Yellow
    }
} else {
    Write-Host "`nPublish failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

