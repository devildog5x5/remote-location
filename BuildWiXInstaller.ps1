param(
    [string]$Configuration = "Release"
)

Write-Host "Building WiX Installer Package..." -ForegroundColor Yellow

$projectRoot = "."
$exePath = "IPManagementInterface\bin\$Configuration\net8.0-windows\win-x64\publish\IPManagementInterface.exe"
$wxsFile = "IPManagementInterface.Installer.wxs"
$outputDir = "InstallerOutput"
$msiName = "IPManagementInterface-Setup.msi"

# Check if WiX is installed
$wixPaths = @(
    "C:\Program Files (x86)\WiX Toolset v3.11\bin",
    "C:\Program Files\WiX Toolset v3.11\bin",
    "C:\Program Files (x86)\WiX Toolset v3.14\bin",
    "C:\Program Files\WiX Toolset v3.14\bin"
)

$wixBinPath = $null
foreach ($path in $wixPaths) {
    if (Test-Path $path) {
        $wixBinPath = $path
        Write-Host "Found WiX at: $wixBinPath" -ForegroundColor Green
        break
    }
}

if (-not $wixBinPath) {
    Write-Host "`nERROR: WiX Toolset not found!" -ForegroundColor Red
    Write-Host "Please install WiX Toolset v3.11 or later from:" -ForegroundColor Yellow
    Write-Host "https://wixtoolset.org/releases/" -ForegroundColor Cyan
    Write-Host "`nOr install via Chocolatey: choco install wixtoolset" -ForegroundColor Cyan
    exit 1
}

# Add WiX to PATH for this session
$env:Path = "$wixBinPath;$env:Path"

# Verify WiX tools are available
$candle = Get-Command candle -ErrorAction SilentlyContinue
$light = Get-Command light -ErrorAction SilentlyContinue

if (-not $candle -or -not $light) {
    Write-Host "`nERROR: WiX tools (candle.exe, light.exe) not found in PATH" -ForegroundColor Red
    exit 1
}

# Check if executable exists
if (-not (Test-Path $exePath)) {
    Write-Host "`nERROR: Executable not found at: $exePath" -ForegroundColor Red
    Write-Host "Please run PublishForDistribution.ps1 first to create the executable." -ForegroundColor Yellow
    exit 1
}

# Check if WXS file exists
if (-not (Test-Path $wxsFile)) {
    Write-Host "`nERROR: WiX source file not found: $wxsFile" -ForegroundColor Red
    exit 1
}

# Create output directory
if (Test-Path $outputDir) {
    Remove-Item $outputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

Write-Host "`nCompiling WiX source..." -ForegroundColor Cyan
$wixobjFile = Join-Path $outputDir "IPManagementInterface.Installer.wixobj"

# Compile WXS to WIXOBJ
$candleArgs = @(
    "-out", "`"$wixobjFile`"",
    "`"$wxsFile`""
)

$candleProcess = Start-Process -FilePath "candle.exe" -ArgumentList $candleArgs -Wait -NoNewWindow -PassThru

if ($candleProcess.ExitCode -ne 0) {
    Write-Host "`nERROR: WiX compilation failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Linking installer..." -ForegroundColor Cyan
$msiFile = Join-Path $outputDir $msiName

# Link WIXOBJ to MSI
$lightArgs = @(
    "-out", "`"$msiFile`"",
    "-ext", "WixUIExtension",
    "`"$wixobjFile`""
)

$lightProcess = Start-Process -FilePath "light.exe" -ArgumentList $lightArgs -Wait -NoNewWindow -PassThru

if ($lightProcess.ExitCode -ne 0) {
    Write-Host "`nERROR: WiX linking failed!" -ForegroundColor Red
    exit 1
}

# Get file info
$msiInfo = Get-Item $msiFile
$msiSizeMB = [math]::Round($msiInfo.Length / 1MB, 2)

Write-Host "`nWiX Installer created successfully!" -ForegroundColor Green
Write-Host "  Location: $msiFile" -ForegroundColor Cyan
Write-Host "  Size: $msiSizeMB MB" -ForegroundColor Cyan
Write-Host "`nThe installer includes:" -ForegroundColor Yellow
Write-Host "  - Application executable" -ForegroundColor White
Write-Host "  - Start Menu shortcut" -ForegroundColor White
Write-Host "  - Desktop shortcut" -ForegroundColor White
Write-Host "  - Proper Windows Installer integration" -ForegroundColor White
Write-Host "`nTo install: Double-click the MSI file" -ForegroundColor Green
