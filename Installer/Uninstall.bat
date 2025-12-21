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
    del /F /Q "%INSTALL_DIR%\IPManagementInterface.exe"
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
