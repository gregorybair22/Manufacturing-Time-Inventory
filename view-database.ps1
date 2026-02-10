# Quick Database Viewer Script
# This script helps you view data in the ManufacturingTimeTracking database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ManufacturingTimeTracking Database Viewer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if LocalDB is running
Write-Host "Checking LocalDB status..." -ForegroundColor Yellow
$localdbStatus = sqllocaldb info MSSQLLocalDB 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "LocalDB is not running. Starting LocalDB..." -ForegroundColor Yellow
    sqllocaldb start MSSQLLocalDB
    Start-Sleep -Seconds 2
}

Write-Host "LocalDB is running!" -ForegroundColor Green
Write-Host ""

# Connection string
$server = "(localdb)\mssqllocaldb"
$database = "ManufacturingTimeTracking"

Write-Host "Connecting to: $server" -ForegroundColor Cyan
Write-Host "Database: $database" -ForegroundColor Cyan
Write-Host ""

# Function to run SQL query
function Run-Query {
    param([string]$query)
    
    $result = sqlcmd -S $server -d $database -E -Q $query -W -s "," -h -1
    return $result
}

# Menu
Write-Host "Select an option:" -ForegroundColor Yellow
Write-Host "1. View all StepTemplates"
Write-Host "2. View all ProcessTemplates"
Write-Host "3. View all PhaseTemplates"
Write-Host "4. View all Materials"
Write-Host "5. View all Tools"
Write-Host "6. View Steps with their Phase and Process"
Write-Host "7. View Steps with Tools"
Write-Host "8. View Steps with Materials"
Write-Host "9. Custom SQL Query"
Write-Host "0. Exit"
Write-Host ""

$choice = Read-Host "Enter your choice (0-9)"

switch ($choice) {
    "1" {
        Write-Host "`nStepTemplates:" -ForegroundColor Green
        Run-Query "SELECT * FROM StepTemplates ORDER BY Id"
    }
    "2" {
        Write-Host "`nProcessTemplates:" -ForegroundColor Green
        Run-Query "SELECT * FROM ProcessTemplates ORDER BY Id"
    }
    "3" {
        Write-Host "`nPhaseTemplates:" -ForegroundColor Green
        Run-Query "SELECT * FROM PhaseTemplates ORDER BY ProcessTemplateId, SortOrder"
    }
    "4" {
        Write-Host "`nMaterials:" -ForegroundColor Green
        Run-Query "SELECT * FROM Materials ORDER BY Name"
    }
    "5" {
        Write-Host "`nTools:" -ForegroundColor Green
        Run-Query "SELECT * FROM Tools ORDER BY Name"
    }
    "6" {
        Write-Host "`nSteps with Phase and Process:" -ForegroundColor Green
        $query = @"
SELECT 
    st.Id,
    st.Title,
    st.Instructions,
    st.AllowSkip,
    pt.Name AS PhaseName,
    prt.Name AS ProcessTemplateName
FROM StepTemplates st
INNER JOIN PhaseTemplates pt ON st.PhaseTemplateId = pt.Id
INNER JOIN ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
ORDER BY prt.Name, pt.SortOrder, st.SortOrder
"@
        Run-Query $query
    }
    "7" {
        Write-Host "`nSteps with Tools:" -ForegroundColor Green
        $query = @"
SELECT 
    st.Id AS StepId,
    st.Title AS StepTitle,
    t.Name AS ToolName
FROM StepTemplates st
INNER JOIN StepTemplateTools stt ON st.Id = stt.StepTemplateId
INNER JOIN Tools t ON stt.ToolId = t.Id
ORDER BY st.Id
"@
        Run-Query $query
    }
    "8" {
        Write-Host "`nSteps with Materials:" -ForegroundColor Green
        $query = @"
SELECT 
    st.Id AS StepId,
    st.Title AS StepTitle,
    m.Name AS MaterialName,
    stm.Qty AS Quantity
FROM StepTemplates st
INNER JOIN StepTemplateMaterials stm ON st.Id = stm.StepTemplateId
INNER JOIN Materials m ON stm.MaterialId = m.Id
ORDER BY st.Id
"@
        Run-Query $query
    }
    "9" {
        Write-Host "`nEnter your SQL query (end with GO on a new line):" -ForegroundColor Yellow
        Write-Host "Example: SELECT * FROM StepTemplates WHERE Id = 1" -ForegroundColor Gray
        $customQuery = Read-Host "Query"
        Run-Query $customQuery
    }
    "0" {
        Write-Host "`nExiting..." -ForegroundColor Yellow
        exit
    }
    default {
        Write-Host "`nInvalid choice!" -ForegroundColor Red
    }
}

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
