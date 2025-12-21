param(
    [string]$Configuration = "Release"
)

Write-Host "Building Installer Package..." -ForegroundColor Yellow

$projectPath = "IPManagementInterface\IPManagementInterface.csproj"
$publishPath = "IPManagementInterface\bin\$Configuration\net8.0-windows\win-x64\publish"
$installerPath = "Installer"
$appName = "IPManagementInterface"
$version = "1.0.0"

# Ensure publish exists
if (-not (Test-Path $publishPath)) {
    Write-Host "`nPublished executable not found. Running PublishForDistribution.ps1 first..." -ForegroundColor Yellow
    & ".\PublishForDistribution.ps1" -Configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nPublish failed. Cannot create installer." -ForegroundColor Red
        exit 1
    }
}

$exePath = Join-Path $publishPath "$appName.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "`nExecutable not found at: $exePath" -ForegroundColor Red
    exit 1
}

# Create installer directory
if (Test-Path $installerPath) {
    Remove-Item $installerPath -Recurse -Force
}
New-Item -ItemType Directory -Path $installerPath | Out-Null

# Copy files to installer directory
Write-Host "`nCopying files to installer directory..." -ForegroundColor Cyan
Copy-Item $exePath -Destination $installerPath -Force
Write-Host "  Copied: $appName.exe" -ForegroundColor Green

# Create a simple setup script
$setupScript = @"
@echo off
echo ========================================
echo IP Management Interface Setup
echo ========================================
echo.
echo This will install IP Management Interface to:
echo   %ProgramFiles%\IPManagementInterface
echo.
pause

set INSTALL_DIR=%ProgramFiles%\IPManagementInterface

if not exist "%INSTALL_DIR%" (
    mkdir "%INSTALL_DIR%"
)

echo Installing files...
copy /Y "%~dp0$appName.exe" "%INSTALL_DIR%\$appName.exe"

echo.
echo Creating Start Menu shortcut...
set START_MENU=%ProgramData%\Microsoft\Windows\Start Menu\Programs
if not exist "%START_MENU%" mkdir "%START_MENU%"

powershell -Command "`$WshShell = New-Object -ComObject WScript.Shell; `$Shortcut = `$WshShell.CreateShortcut('%START_MENU%\IP Management Interface.lnk'); `$Shortcut.TargetPath = '%INSTALL_DIR%\$appName.exe'; `$Shortcut.WorkingDirectory = '%INSTALL_DIR%'; `$Shortcut.IconLocation = '%INSTALL_DIR%\$appName.exe,0'; `$Shortcut.Save()"

echo.
echo Creating Desktop shortcut...
set DESKTOP=%USERPROFILE%\Desktop
powershell -Command "`$WshShell = New-Object -ComObject WScript.Shell; `$Shortcut = `$WshShell.CreateShortcut('%DESKTOP%\IP Management Interface.lnk'); `$Shortcut.TargetPath = '%INSTALL_DIR%\$appName.exe'; `$Shortcut.WorkingDirectory = '%INSTALL_DIR%'; `$Shortcut.IconLocation = '%INSTALL_DIR%\$appName.exe,0'; `$Shortcut.Save()"

echo.
echo ========================================
echo Installation Complete!
echo ========================================
echo.
echo IP Management Interface has been installed to:
echo   %INSTALL_DIR%
echo.
echo Shortcuts have been created in:
echo   - Start Menu
echo   - Desktop
echo.
pause
"@

$setupScriptPath = Join-Path $installerPath "Setup.bat"
$setupScript | Out-File -FilePath $setupScriptPath -Encoding ASCII
Write-Host "  Created: Setup.bat" -ForegroundColor Green

# Create uninstall script
$uninstallScript = @"
@echo off
echo ========================================
echo IP Management Interface Uninstaller
echo ========================================
echo.
echo This will remove IP Management Interface from your system.
echo.
pause

set INSTALL_DIR=%ProgramFiles%\IPManagementInterface
set START_MENU=%ProgramData%\Microsoft\Windows\Start Menu\Programs
set DESKTOP=%USERPROFILE%\Desktop

echo Removing application files...
if exist "%INSTALL_DIR%" (
    del /F /Q "%INSTALL_DIR%\$appName.exe"
    rmdir "%INSTALL_DIR%"
)

echo Removing shortcuts...
if exist "%START_MENU%\IP Management Interface.lnk" (
    del /F /Q "%START_MENU%\IP Management Interface.lnk"
)

if exist "%DESKTOP%\IP Management Interface.lnk" (
    del /F /Q "%DESKTOP%\IP Management Interface.lnk"
)

echo.
echo ========================================
echo Uninstallation Complete!
echo ========================================
echo.
echo IP Management Interface has been removed from your system.
echo.
pause
"@

$uninstallScriptPath = Join-Path $installerPath "Uninstall.bat"
$uninstallScript | Out-File -FilePath $uninstallScriptPath -Encoding ASCII
Write-Host "  Created: Uninstall.bat" -ForegroundColor Green

# Create README
$readme = @"
IP Management Interface - Installer Package
===========================================

INSTALLATION INSTRUCTIONS:
--------------------------
1. Run Setup.bat as Administrator (Right-click > Run as Administrator)
2. Follow the on-screen instructions
3. The application will be installed to: %ProgramFiles%\IPManagementInterface
4. Shortcuts will be created in Start Menu and Desktop

UNINSTALLATION:
---------------
1. Run Uninstall.bat as Administrator
2. Follow the on-screen instructions

SYSTEM REQUIREMENTS:
--------------------
- Windows 7 SP1 or later (Windows 7, 8.1, 10, 11)
- No .NET runtime installation required (self-contained)

FEATURES:
---------
- Dashboard interface for managing IoT devices
- Device discovery and monitoring
- Multiple themes (Light, Dark, USMC, Olive Drab, Ocean, Sunset)
- HTTP/HTTPS support on standard and custom ports

For more information, see README.md in the source repository.
"@

$readmePath = Join-Path $installerPath "README.txt"
$readme | Out-File -FilePath $readmePath -Encoding UTF8
Write-Host "  Created: README.txt" -ForegroundColor Green

# Create ZIP archive
Write-Host "`nCreating ZIP archive..." -ForegroundColor Cyan
$zipPath = Join-Path "." "$appName-Installer-v$version.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($installerPath, $zipPath)

$zipInfo = Get-Item $zipPath
$zipSizeMB = [math]::Round($zipInfo.Length / 1MB, 2)

Write-Host "`nInstaller package created successfully!" -ForegroundColor Green
Write-Host "  Location: $zipPath" -ForegroundColor Cyan
Write-Host "  Size: $zipSizeMB MB" -ForegroundColor Cyan
Write-Host "`nInstaller directory: $installerPath" -ForegroundColor Yellow
Write-Host "`nTo distribute:" -ForegroundColor Yellow
Write-Host "  1. Share the ZIP file: $zipPath" -ForegroundColor White
Write-Host "  2. Users extract the ZIP" -ForegroundColor White
Write-Host "  3. Run Setup.bat as Administrator" -ForegroundColor White
