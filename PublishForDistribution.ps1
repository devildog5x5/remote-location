param(
    [string]$Configuration = "Release"
)

Write-Host "Publishing self-contained application for distribution..." -ForegroundColor Yellow

$projectPath = "IPManagementInterface\IPManagementInterface.csproj"
$publishPath = "IPManagementInterface\bin\$Configuration\net8.0-windows\win-x64\publish"

# Clean previous publish
if (Test-Path $publishPath) {
    Remove-Item $publishPath -Recurse -Force
    Write-Host "Cleaned previous publish output" -ForegroundColor Cyan
}

# Publish self-contained
Write-Host "`nPublishing self-contained application..." -ForegroundColor Yellow
dotnet publish $projectPath `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishReadyToRun=true `
    -p:PublishTrimmed=false

if ($LASTEXITCODE -eq 0) {
    $exePath = Join-Path $publishPath "IPManagementInterface.exe"
    if (Test-Path $exePath) {
        $fileInfo = Get-Item $exePath
        $sizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
        
        Write-Host "`nPublish successful!" -ForegroundColor Green
        Write-Host "  Executable: IPManagementInterface.exe" -ForegroundColor Cyan
        Write-Host "  Size: $sizeMB MB" -ForegroundColor Cyan
        Write-Host "`nThis executable is self-contained and will run on:" -ForegroundColor Yellow
        Write-Host "  - Windows 7 SP1 and later" -ForegroundColor White
        Write-Host "  - Windows 8.1 and later" -ForegroundColor White
        Write-Host "  - Windows 10 and later" -ForegroundColor White
        Write-Host "  - Windows 11" -ForegroundColor White
        Write-Host "`nNo .NET runtime installation required" -ForegroundColor Green
    } else {
        Write-Host "`nExecutable not found at expected location" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "`nPublish failed!" -ForegroundColor Red
    exit 1
}
