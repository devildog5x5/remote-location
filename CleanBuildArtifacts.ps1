param(
    [switch]$All,
    [switch]$iOS,
    [switch]$Windows,
    [switch]$Shared
)

Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow

if (-not ($All -or $iOS -or $Windows -or $Shared)) {
    $All = $true
}

$cleaned = @()

if ($All -or $Windows) {
    Write-Host "`nCleaning Windows project..." -ForegroundColor Cyan
    if (Test-Path "IPManagementInterface\bin") {
        Remove-Item "IPManagementInterface\bin" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += "Windows bin"
    }
    if (Test-Path "IPManagementInterface\obj") {
        Remove-Item "IPManagementInterface\obj" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += "Windows obj"
    }
}

if ($All -or $iOS) {
    Write-Host "Cleaning iOS project..." -ForegroundColor Cyan
    if (Test-Path "IPManagementInterface.iOS\bin") {
        Remove-Item "IPManagementInterface.iOS\bin" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += "iOS bin"
    }
    if (Test-Path "IPManagementInterface.iOS\obj") {
        Remove-Item "IPManagementInterface.iOS\obj" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += "iOS obj"
    }
}

if ($All -or $Shared) {
    Write-Host "Cleaning Shared project..." -ForegroundColor Cyan
    if (Test-Path "IPManagementInterface.Shared\bin") {
        Remove-Item "IPManagementInterface.Shared\bin" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += "Shared bin"
    }
    if (Test-Path "IPManagementInterface.Shared\obj") {
        Remove-Item "IPManagementInterface.Shared\obj" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += "Shared obj"
    }
}

# Clean solution-level artifacts
if ($All) {
    Write-Host "Cleaning solution artifacts..." -ForegroundColor Cyan
    if (Test-Path ".vs") {
        Remove-Item ".vs" -Recurse -Force -ErrorAction SilentlyContinue
        $cleaned += ".vs"
    }
}

if ($cleaned.Count -gt 0) {
    Write-Host "`n✓ Cleaned:" -ForegroundColor Green
    foreach ($item in $cleaned) {
        Write-Host "  - $item" -ForegroundColor White
    }
} else {
    Write-Host "`nNo build artifacts found to clean." -ForegroundColor Gray
}

Write-Host "`nDone!" -ForegroundColor Green

