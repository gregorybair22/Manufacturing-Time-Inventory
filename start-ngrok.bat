@echo off
REM Run the ngrok PowerShell script (bypasses execution policy so it works on all Windows setups)
cd /d "%~dp0"
echo Starting ngrok tunnel (port 5173). Keep this window OPEN to use the public URL.
echo.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0start-ngrok.ps1"
echo.
echo Ngrok stopped. Press any key to close this window.
pause >nul
