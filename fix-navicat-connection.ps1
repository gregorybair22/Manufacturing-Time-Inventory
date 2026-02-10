# Fix Navicat Connection Script
# This script will help diagnose and fix the LocalDB connection issue

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Navicat Connection Fix Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check LocalDB status
Write-Host "Step 1: Checking LocalDB status..." -ForegroundColor Yellow
$localdbInfo = sqllocaldb info MSSQLLocalDB 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "LocalDB instance not found. Creating..." -ForegroundColor Yellow
    sqllocaldb create MSSQLLocalDB
    Start-Sleep -Seconds 2
}

Write-Host "Starting LocalDB..." -ForegroundColor Yellow
sqllocaldb start MSSQLLocalDB
Start-Sleep -Seconds 3

$status = sqllocaldb info MSSQLLocalDB
if ($status -match "State: Running") {
    Write-Host "✓ LocalDB is running" -ForegroundColor Green
} else {
    Write-Host "✗ LocalDB failed to start" -ForegroundColor Red
    Write-Host "Status: $status" -ForegroundColor Red
    exit
}

Write-Host ""

# Step 2: Check if database exists
Write-Host "Step 2: Checking if database exists..." -ForegroundColor Yellow

$query = "SELECT name FROM sys.databases WHERE name = 'ManufacturingTimeTracking'"
$result = sqlcmd -S "(localdb)\mssqllocaldb" -E -Q $query -h -1 2>&1

if ($result -match "ManufacturingTimeTracking") {
    Write-Host "✓ Database 'ManufacturingTimeTracking' exists" -ForegroundColor Green
} else {
    Write-Host "✗ Database 'ManufacturingTimeTracking' does NOT exist" -ForegroundColor Red
    Write-Host ""
    Write-Host "Creating database..." -ForegroundColor Yellow
    
    $createDbQuery = @"
CREATE DATABASE ManufacturingTimeTracking;
GO
"@
    
    sqlcmd -S "(localdb)\mssqllocaldb" -E -Q $createDbQuery 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database created successfully" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to create database" -ForegroundColor Red
        Write-Host "You may need to run your application first to create the database." -ForegroundColor Yellow
    }
}

Write-Host ""

# Step 3: Test connection
Write-Host "Step 3: Testing connection..." -ForegroundColor Yellow

$testQuery = "SELECT @@VERSION"
$testResult = sqlcmd -S "(localdb)\mssqllocaldb" -d "ManufacturingTimeTracking" -E -Q $testQuery -h -1 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Connection test successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Connection Settings for Navicat:" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Host: (localdb)\mssqllocaldb" -ForegroundColor White
    Write-Host "Initial Database: ManufacturingTimeTracking" -ForegroundColor White
    Write-Host "Authentication: Windows Authentication" -ForegroundColor White
    Write-Host "Username: (leave blank)" -ForegroundColor White
    Write-Host "Password: (leave blank)" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "✗ Connection test failed" -ForegroundColor Red
    Write-Host "Error: $testResult" -ForegroundColor Red
    Write-Host ""
    Write-Host "Try these solutions:" -ForegroundColor Yellow
    Write-Host "1. Run your application first to create the database" -ForegroundColor White
    Write-Host "2. Check if SQL Server services are running" -ForegroundColor White
    Write-Host "3. Try connecting to 'master' database first" -ForegroundColor White
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
