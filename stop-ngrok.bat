@echo off
REM Stop any running ngrok tunnel (frees your domain for next time)
echo Stopping ngrok...
taskkill /IM ngrok.exe /F 2>nul
if errorlevel 1 (
    echo No ngrok process was running.
) else (
    echo Ngrok stopped. Your domain is now free.
)
echo.
pause
