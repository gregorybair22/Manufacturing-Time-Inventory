# PowerShell script to open firewall ports for the application
# Run this script as Administrator

Write-Host "Opening firewall ports for Manufacturing Time Tracking application..." -ForegroundColor Green

# Open HTTP port 5173
New-NetFirewallRule -DisplayName "Manufacturing Time Tracking HTTP" -Direction Inbound -LocalPort 5173 -Protocol TCP -Action Allow -ErrorAction SilentlyContinue
Write-Host "HTTP port 5173 opened" -ForegroundColor Yellow

# Open HTTPS port 7245
New-NetFirewallRule -DisplayName "Manufacturing Time Tracking HTTPS" -Direction Inbound -LocalPort 7245 -Protocol TCP -Action Allow -ErrorAction SilentlyContinue
Write-Host "HTTPS port 7245 opened" -ForegroundColor Yellow

Write-Host "`nFirewall ports configured successfully!" -ForegroundColor Green
Write-Host "You can now access the application from other devices on your network." -ForegroundColor Green
Write-Host "`nTo find your IP address, run: ipconfig" -ForegroundColor Cyan
Write-Host "Then access the app using: http://[YOUR_IP]:5173 or https://[YOUR_IP]:7245" -ForegroundColor Cyan
