$exePath = "IPManagementInterface\bin\Debug\net8.0-windows\IPManagementInterface.exe"

if (Test-Path $exePath) {
    Write-Host "Starting IP Management Interface..." -ForegroundColor Green
    Write-Host "Executable: $exePath" -ForegroundColor Cyan
    
    $fullPath = Resolve-Path $exePath
    $workingDir = Split-Path $fullPath -Parent
    
    Write-Host "Working Directory: $workingDir" -ForegroundColor Cyan
    
    # Start the process and wait a moment
    $process = Start-Process -FilePath $fullPath -WorkingDirectory $workingDir -PassThru -WindowStyle Normal
    
    Start-Sleep -Seconds 2
    
    # Check if it's still running
    $proc = Get-Process -Id $process.Id -ErrorAction SilentlyContinue
    if ($proc) {
        Write-Host "`nApplication is running (PID: $($process.Id))" -ForegroundColor Green
        if ($proc.MainWindowTitle) {
            Write-Host "Window Title: $($proc.MainWindowTitle)" -ForegroundColor Cyan
        } else {
            Write-Host "Window may still be initializing..." -ForegroundColor Yellow
        }
        Write-Host "`nCheck your screen - the window should be visible!" -ForegroundColor Green
        Write-Host "If you don't see it, check:" -ForegroundColor Yellow
        Write-Host "  - Taskbar for minimized window" -ForegroundColor White
        Write-Host "  - Alt+Tab to switch between windows" -ForegroundColor White
    } else {
        Write-Host "`nApplication exited immediately. There may be an error." -ForegroundColor Red
    }
} else {
    Write-Host "Executable not found at: $exePath" -ForegroundColor Red
    Write-Host "Please build the project first: dotnet build IPManagementInterface.sln" -ForegroundColor Yellow
}

