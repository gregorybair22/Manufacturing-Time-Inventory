# PowerShell script to download and install ngrok
# Run this script as Administrator for best results

Write-Host "=== ngrok Installation Script ===" -ForegroundColor Green
Write-Host ""

# Check if ngrok is already installed
$ngrokInstalled = Get-Command ngrok -ErrorAction SilentlyContinue

if ($ngrokInstalled) {
    Write-Host "ngrok is already installed at: $($ngrokInstalled.Source)" -ForegroundColor Yellow
    Write-Host "You can skip the installation." -ForegroundColor Yellow
    Write-Host ""
    $continue = Read-Host "Do you want to reinstall? (y/N)"
    if ($continue -ne "y" -and $continue -ne "Y") {
        Write-Host "Installation cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Create ngrok directory in user's local folder
$ngrokDir = Join-Path $env:LOCALAPPDATA "ngrok"
$ngrokExe = Join-Path $ngrokDir "ngrok.exe"

Write-Host "Installing ngrok to: $ngrokDir" -ForegroundColor Cyan
Write-Host ""

# Create directory if it doesn't exist
if (-not (Test-Path $ngrokDir)) {
    New-Item -ItemType Directory -Path $ngrokDir -Force | Out-Null
    Write-Host "Created directory: $ngrokDir" -ForegroundColor Green
}

# Download ngrok
Write-Host "Downloading ngrok..." -ForegroundColor Yellow
$downloadUrl = "https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip"
$zipFile = Join-Path $env:TEMP "ngrok.zip"

try {
    Invoke-WebRequest -Uri $downloadUrl -OutFile $zipFile -UseBasicParsing
    Write-Host "Download completed." -ForegroundColor Green
} catch {
    Write-Host "ERROR: Failed to download ngrok." -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please download ngrok manually from: https://ngrok.com/download" -ForegroundColor Yellow
    Write-Host "Extract ngrok.exe to: $ngrokDir" -ForegroundColor Yellow
    exit 1
}

# Extract ngrok
Write-Host "Extracting ngrok..." -ForegroundColor Yellow
try {
    Expand-Archive -Path $zipFile -DestinationPath $ngrokDir -Force
    Write-Host "Extraction completed." -ForegroundColor Green
} catch {
    Write-Host "ERROR: Failed to extract ngrok." -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}

# Clean up zip file
Remove-Item $zipFile -Force -ErrorAction SilentlyContinue

# Verify installation
if (Test-Path $ngrokExe) {
    Write-Host ""
    Write-Host "ngrok installed successfully!" -ForegroundColor Green
    Write-Host "Location: $ngrokExe" -ForegroundColor Cyan
    Write-Host ""
    
    # Add to PATH for current session
    $env:Path += ";$ngrokDir"
    Write-Host "Added ngrok to PATH for current session." -ForegroundColor Yellow
    Write-Host ""
    
    # Check if ngrok is in system PATH
    $systemPath = [Environment]::GetEnvironmentVariable("Path", "User")
    if ($systemPath -notlike "*$ngrokDir*") {
        Write-Host "To add ngrok to your PATH permanently:" -ForegroundColor Yellow
        Write-Host "1. Press Win + X and select 'System'" -ForegroundColor Cyan
        Write-Host "2. Click 'Advanced system settings'" -ForegroundColor Cyan
        Write-Host "3. Click 'Environment Variables'" -ForegroundColor Cyan
        Write-Host "4. Under 'User variables', select 'Path' and click 'Edit'" -ForegroundColor Cyan
        Write-Host "5. Click 'New' and add: $ngrokDir" -ForegroundColor Cyan
        Write-Host "6. Click OK on all dialogs" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Or run this command in PowerShell (as Administrator):" -ForegroundColor Yellow
        Write-Host "[Environment]::SetEnvironmentVariable('Path', [Environment]::GetEnvironmentVariable('Path', 'User') + ';$ngrokDir', 'User')" -ForegroundColor Cyan
        Write-Host ""
        
        $addToPath = Read-Host "Do you want to add ngrok to PATH now? (Y/n)"
        if ($addToPath -ne "n" -and $addToPath -ne "N") {
            try {
                $currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
                if ($currentPath -notlike "*$ngrokDir*") {
                    [Environment]::SetEnvironmentVariable("Path", $currentPath + ";$ngrokDir", "User")
                    Write-Host "ngrok added to PATH successfully!" -ForegroundColor Green
                    Write-Host "You may need to restart PowerShell for the changes to take effect." -ForegroundColor Yellow
                }
            } catch {
                Write-Host "Could not add to PATH automatically. Please add it manually using the instructions above." -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "ngrok is already in your PATH." -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Green
    Write-Host "1. Get your authtoken from: https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor Cyan
    Write-Host "2. Run: ngrok config add-authtoken YOUR_TOKEN" -ForegroundColor Cyan
    Write-Host "3. Start your application" -ForegroundColor Cyan
    Write-Host "4. Run: .\start-ngrok.ps1" -ForegroundColor Cyan
    Write-Host ""
    
} else {
    Write-Host "ERROR: ngrok.exe not found after installation." -ForegroundColor Red
    exit 1
}
