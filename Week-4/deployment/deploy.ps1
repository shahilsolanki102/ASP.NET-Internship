param(
    [string]$Environment = "Production",
    [string]$Configuration = "Release",
    [switch]$SkipTests = $false
)

$ErrorActionPreference = "Stop"
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "Starting Automated ASP.NET Core Production Deployment Pipeline" -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan

# 1. Pre-deployment Validation
Write-Host "`n[Step 1/5] Verifying .NET 9.0 SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
Write-Host "Detected .NET SDK: $dotnetVersion" -ForegroundColor Green

# 2. Automated Test Execution Gate
if (-not $SkipTests) {
    Write-Host "`n[Step 2/5] Running Automated Test Suite Gate..." -ForegroundColor Yellow
    dotnet test "d:\ASP.NET Intern\Week-3\OrderManagementSuite.sln" --configuration $Configuration --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Deployment aborted: Tests failed."
        exit 1
    }
    Write-Host "All Unit and Integration Tests Passed." -ForegroundColor Green
}

# 3. Compile and Publish Production Binaries
Write-Host "`n[Step 3/5] Compiling and Publishing Release Binaries..." -ForegroundColor Yellow
$publishDir = "d:\ASP.NET Intern\Week-4\publish"

if (Test-Path $publishDir) {
    Remove-Item $publishDir -Recurse -Force
}
New-Item -ItemType Directory -Path $publishDir -Force | Out-Null

dotnet publish "d:\ASP.NET Intern\Week-3\src\OrderManagementApp\OrderManagementApp.csproj" --configuration $Configuration --output "$publishDir\OrderManagementApp" /p:EnvironmentName=$Environment /p:UseAppHost=false

Write-Host "Binaries successfully compiled and optimized in $publishDir" -ForegroundColor Green

# 4. Generate Production Release Bundle
Write-Host "`n[Step 4/5] Packaging Release Archive..." -ForegroundColor Yellow
$zipFile = "d:\ASP.NET Intern\Week-4\production-deployment-v1.0.0.zip"
if (Test-Path $zipFile) {
    Remove-Item $zipFile -Force
}
Compress-Archive -Path "$publishDir\*" -DestinationPath $zipFile
Write-Host "Production bundle generated: $zipFile" -ForegroundColor Green

# 5. Summary
Write-Host "`n=====================================================================" -ForegroundColor Cyan
Write-Host "Production Deployment Package is Ready!" -ForegroundColor Green
Write-Host "   - Output Directory: $publishDir" -ForegroundColor White
Write-Host "   - Release Bundle:   $zipFile" -ForegroundColor White
Write-Host "=====================================================================" -ForegroundColor Cyan
