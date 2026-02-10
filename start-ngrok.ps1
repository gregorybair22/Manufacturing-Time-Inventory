# PowerShell script to start ngrok tunnel for the application
# Make sure ngrok is installed and in your PATH, or update the path below

Write-Host "Starting ngrok tunnel for Manufacturing Time Tracking..." -ForegroundColor Green
Write-Host ""

# Check if ngrok is available
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue

if (-not $ngrokPath) {
    # Try common installation locations
    $possiblePaths = @(
        "$env:LOCALAPPDATA\ngrok\ngrok.exe",
        "$env:ProgramFiles\ngrok\ngrok.exe",
        "$env:ProgramFiles(x86)\ngrok\ngrok.exe",
        "C:\ngrok\ngrok.exe"
    )
    
    $ngrokExe = $null
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $ngrokExe = $path
            break
        }
    }
    
    if (-not $ngrokExe) {
        Write-Host "ERROR: ngrok is not installed or not in your PATH." -ForegroundColor Red
        Write-Host ""
        Write-Host "To install ngrok automatically, run:" -ForegroundColor Yellow
        Write-Host "  .\install-ngrok.ps1" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Or install manually:" -ForegroundColor Yellow
        Write-Host "1. Download from: https://ngrok.com/download" -ForegroundColor Cyan
        Write-Host "2. Extract ngrok.exe to a folder in your PATH" -ForegroundColor Cyan
        Write-Host "3. Sign up for a free account at https://ngrok.com and get your authtoken" -ForegroundColor Cyan
        Write-Host "4. Run: ngrok config add-authtoken YOUR_TOKEN" -ForegroundColor Cyan
        Write-Host ""
        exit 1
    } else {
        Write-Host "Found ngrok at: $ngrokExe" -ForegroundColor Yellow
        $env:Path += ";$(Split-Path $ngrokExe -Parent)"
        $ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
    }
}

# Read custom domain from config file
$configFile = Join-Path $PSScriptRoot "ngrok-config.txt"
$customDomain = $null

if (Test-Path $configFile) {
    $configContent = Get-Content $configFile | Where-Object { $_ -match '^DOMAIN=(.+)$' }
    if ($configContent) {
        $customDomain = ($configContent -split '=')[1].Trim()
        if ([string]::IsNullOrWhiteSpace($customDomain)) {
            $customDomain = $null
        }
    }
}

# Start ngrok tunnel for HTTP (port 5173)
Write-Host "Starting ngrok tunnel on port 5173 (HTTP)..." -ForegroundColor Yellow

if ($customDomain) {
    Write-Host "Using custom domain: $customDomain" -ForegroundColor Cyan
    Write-Host "The public URL will be: https://$customDomain" -ForegroundColor Green
    # Stop any existing ngrok so this domain is free (fixes ERR_NGROK_334 "endpoint already online")
    $existing = Get-Process -Name ngrok -ErrorAction SilentlyContinue
    if ($existing) {
        Write-Host "Stopping existing ngrok tunnel so this domain can be used..." -ForegroundColor Yellow
        $existing | Stop-Process -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
    }
} else {
    Write-Host "Using random domain (free account)" -ForegroundColor Yellow
    Write-Host "The public URL will be displayed below. Share this URL with the other laptop." -ForegroundColor Cyan
}
Write-Host ""
Write-Host "WHERE TO FIND YOUR ADDRESS:" -ForegroundColor Green
Write-Host "  1. Look in the ngrok output below for the line: Forwarding  https://xxxx.ngrok-free.app -> ..." -ForegroundColor White
Write-Host "  2. Or open this in your browser to see the public URL: http://127.0.0.1:4040" -ForegroundColor Cyan
Write-Host ""
Write-Host "To change domain, edit: ngrok-config.txt" -ForegroundColor Gray
Write-Host "Press Ctrl+C to stop the tunnel" -ForegroundColor Yellow
Write-Host "If you see ERR_NGROK_3200 (endpoint offline): keep this window OPEN and use the URL shown below (free URLs change each run)." -ForegroundColor Gray
Write-Host "If you see ERR_NGROK_334 (already online): the script will stop existing ngrok first when using a custom domain." -ForegroundColor Gray
Write-Host ""

# Start ngrok with or without custom domain
if ($customDomain) {
    ngrok http 5173 --domain=$customDomain
} else {
    ngrok http 5173
}
