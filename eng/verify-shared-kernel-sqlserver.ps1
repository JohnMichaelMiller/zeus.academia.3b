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
  $isRunningOnWindows = $false
  if ($PSVersionTable.PSVersion.Major -lt 6) {
    $isRunningOnWindows = $true
  }
  elseif ($null -ne $IsWindows) {
    $isRunningOnWindows = [bool]$IsWindows
  }
  else {
    $isRunningOnWindows = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)
  }

  if (-not $isRunningOnWindows) {
    throw "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because LocalDB is not available."
  }

  Write-Host "ZEUS_SQLSERVER_CONNECTION is not set. Falling back to (localdb)\\MSSQLLocalDB on Windows." -ForegroundColor Yellow
}

Write-Host "Restoring solution..." -ForegroundColor Cyan
dotnet restore $solution
if ($LASTEXITCODE -ne 0) {
  throw "dotnet restore failed with exit code $LASTEXITCODE."
}

Write-Host "Running focused Shared Kernel tests..." -ForegroundColor Cyan
dotnet test $testProject `
  --configuration $Configuration `
  --filter "FullyQualifiedName~Zeus.Academia.Tests.Features.SharedKernel.Foundation" `
  --logger "console;verbosity=normal"
if ($LASTEXITCODE -ne 0) {
  throw "dotnet test failed with exit code $LASTEXITCODE."
}

Write-Host "Shared Kernel SQL Server verification completed." -ForegroundColor Green
