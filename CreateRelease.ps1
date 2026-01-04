# Script to create GitHub release for IP Management Interface
# Prerequisites: GitHub CLI (gh) must be installed and authenticated
# Run: gh auth login (if not already authenticated)

param(
    [string]$Tag = "v1.0.0",
    [string]$Title = "IP Management Interface v1.0.0"
)

Write-Host "=== Creating IP Management Interface GitHub Release ===" -ForegroundColor Cyan
Write-Host ""

# Check if GitHub CLI is available
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "Error: GitHub CLI (gh) is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Install from: https://cli.github.com/" -ForegroundColor Yellow
    exit 1
}

# Check authentication
Write-Host "Checking GitHub authentication..." -ForegroundColor Yellow
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Not authenticated with GitHub CLI" -ForegroundColor Red
    Write-Host "Run: gh auth login" -ForegroundColor Yellow
    exit 1
}

Write-Host "[OK] Authenticated" -ForegroundColor Green
Write-Host ""

# Check if files exist
$files = @(
    @{Path="InstallerOutput\IPManagementInterface-Setup.msi"; Name="IPManagementInterface-Setup.msi"},
    @{Path="IPManagementInterface-Installer-v1.0.0.zip"; Name="IPManagementInterface-Installer-v1.0.0.zip"},
    @{Path="IPManagementInterface\bin\Release\net8.0-windows\win-x64\publish\IPManagementInterface.exe"; Name="IPManagementInterface.exe"}
)

Write-Host "Checking files..." -ForegroundColor Yellow
$missingFiles = @()
foreach ($file in $files) {
    if (Test-Path $file.Path) {
        $size = [math]::Round((Get-Item $file.Path).Length / 1MB, 2)
        Write-Host "  [OK] $($file.Name) ($size MB)" -ForegroundColor Green
    } else {
        Write-Host "  [MISSING] $($file.Path) - NOT FOUND" -ForegroundColor Red
        $missingFiles += $file.Path
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host ""
    Write-Host "Warning: Some files are missing. Continuing with available files..." -ForegroundColor Yellow
    $files = $files | Where-Object { Test-Path $_.Path }
    if ($files.Count -eq 0) {
        Write-Host "Error: No files found to upload" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""

# Create release notes
$releaseNotes = @"
Initial release of IP Management Dashboard.

Features:
- MSI installer for professional Windows installation
- Self-contained executable (works on Windows 7 SP1+)
- Portable package with setup scripts
- Multi-platform support (Windows, iOS, Android)
- 6 beautiful themes
- Device discovery and management
- Real-time status monitoring

See DOWNLOAD.md for detailed download options and descriptions.
"@

# Create the release
Write-Host "Creating release: $Tag" -ForegroundColor Cyan
Write-Host "Title: $Title" -ForegroundColor Cyan
Write-Host "Uploading $($files.Count) files..." -ForegroundColor Yellow

$fileArgs = $files | ForEach-Object { $_.Path }
gh release create $Tag --title $Title --notes $releaseNotes $fileArgs 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "[SUCCESS] Release created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "View release at:" -ForegroundColor Cyan
    $repo = gh repo view --json nameWithOwner -q .nameWithOwner 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "https://github.com/$repo/releases/tag/$Tag" -ForegroundColor White
    }
} else {
    Write-Host ""
    Write-Host "[FAILED] Release creation failed" -ForegroundColor Red
    exit 1
}

