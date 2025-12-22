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
copy /Y "%~dp0IPManagementInterface.exe" "%INSTALL_DIR%\IPManagementInterface.exe"

echo.
echo Creating Start Menu shortcut...
set START_MENU=%ProgramData%\Microsoft\Windows\Start Menu\Programs
if not exist "%START_MENU%" mkdir "%START_MENU%"

powershell -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%START_MENU%\IP Management Interface.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\IPManagementInterface.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.IconLocation = '%INSTALL_DIR%\IPManagementInterface.exe,0'; $Shortcut.Save()"

echo.
echo Creating Desktop shortcut...
set DESKTOP=%USERPROFILE%\Desktop
powershell -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%DESKTOP%\IP Management Interface.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\IPManagementInterface.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.IconLocation = '%INSTALL_DIR%\IPManagementInterface.exe,0'; $Shortcut.Save()"

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
