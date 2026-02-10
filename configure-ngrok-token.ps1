# PowerShell script to configure ngrok authtoken
# Usage: .\configure-ngrok-token.ps1 YOUR_AUTHTOKEN

param(
    [Parameter(Mandatory=$true)]
    [string]$AuthToken
)

Write-Host "Configuring ngrok authtoken..." -ForegroundColor Green
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
        Write-Host "ERROR: ngrok is not installed." -ForegroundColor Red
        Write-Host ""
        Write-Host "Please run: .\install-ngrok.ps1" -ForegroundColor Yellow
        exit 1
    } else {
        Write-Host "Found ngrok at: $ngrokExe" -ForegroundColor Yellow
        & $ngrokExe config add-authtoken $AuthToken
    }
} else {
    Write-Host "Using ngrok from: $($ngrokPath.Source)" -ForegroundColor Yellow
    ngrok config add-authtoken $AuthToken
}

Write-Host ""
Write-Host "ngrok authtoken configured successfully!" -ForegroundColor Green
Write-Host "You can now start ngrok using: .\start-ngrok.ps1" -ForegroundColor Cyan
