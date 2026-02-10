# PowerShell script to start ngrok tunnel for HTTPS
# Make sure ngrok is installed and in your PATH, or update the path below

Write-Host "Starting ngrok tunnel for Manufacturing Time Tracking (HTTPS)..." -ForegroundColor Green
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

# Start ngrok tunnel for HTTPS (port 7245)
Write-Host "Starting ngrok tunnel on port 7245 (HTTPS)..." -ForegroundColor Yellow

if ($customDomain) {
    Write-Host "Using custom domain: $customDomain" -ForegroundColor Cyan
    Write-Host "The public URL will be: https://$customDomain" -ForegroundColor Green
} else {
    Write-Host "Using random domain (free account)" -ForegroundColor Yellow
    Write-Host "The public URL will be displayed below. Share this URL with the other laptop." -ForegroundColor Cyan
}
Write-Host ""
Write-Host "To change domain, edit: ngrok-config.txt" -ForegroundColor Gray
Write-Host "Press Ctrl+C to stop the tunnel" -ForegroundColor Yellow
Write-Host ""

# Start ngrok with or without custom domain
if ($customDomain) {
    ngrok http 7245 --domain=$customDomain
} else {
    ngrok http 7245
}
