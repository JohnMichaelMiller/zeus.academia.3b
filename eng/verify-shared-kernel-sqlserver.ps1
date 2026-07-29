param(
  [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$solution = Join-Path $repoRoot "zeus.academia.3b.sln"
$testProject = Join-Path $repoRoot "tests/Features/SharedKernel/Foundation/Zeus.Academia.Tests.Features.SharedKernel.Foundation.csproj"

if (-not (Test-Path $solution)) {
  throw "Solution file not found: $solution"
}

if (-not (Test-Path $testProject)) {
  throw "Test project file not found: $testProject"
}

if ([string]::IsNullOrWhiteSpace($env:ZEUS_SQLSERVER_CONNECTION)) {
  Write-Host "ZEUS_SQLSERVER_CONNECTION is not set. Falling back to (localdb)\\MSSQLLocalDB." -ForegroundColor Yellow
}

Write-Host "Restoring solution..." -ForegroundColor Cyan
dotnet restore $solution

Write-Host "Running focused Shared Kernel tests..." -ForegroundColor Cyan
dotnet test $testProject `
  --configuration $Configuration `
  --filter "FullyQualifiedName~Zeus.Academia.Tests.Features.SharedKernel.Foundation" `
  --logger "console;verbosity=normal"

Write-Host "Shared Kernel SQL Server verification completed." -ForegroundColor Green
