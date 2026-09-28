<#
ai_generated: true
model: "anthropic/claude-opus-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-28-pr46-review-instruction-hardening"
prompt: "Review PR 46 review comments and improve instruction files and prompts; go ahead"
ai_log: "ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md"
source: ".github/instructions/ai-dev-process.instructions.md#mechanical-verification-gate"
#>
<#
.SYNOPSIS
  Mechanical pre-handoff checks for one vertical slice.
.EXAMPLE
  pwsh eng/verify-slice.ps1 -Feature Academics/RegisterAcademic
#>
param(
  [Parameter(Mandatory)]
  [string]$Feature,
  [string]$Configuration = "Debug",
  [string]$BaseRef = "origin/main",
  [switch]$SkipBuild,
  [switch]$SkipTests,
  [switch]$RunEfChecks
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$solution = Join-Path $repoRoot "zeus.academia.3b.sln"
$featureDir = Join-Path $repoRoot "src/features/$Feature"
$testDir = Join-Path $repoRoot "tests/Features/$Feature"
$hostDir = Join-Path $repoRoot "src/Zeus.Academia.Api"
$hostProject = Join-Path $hostDir "Zeus.Academia.Api.csproj"
$programFile = Join-Path $hostDir "Program.cs"
$appSettings = Join-Path $hostDir "appsettings.json"

$failures = [System.Collections.Generic.List[string]]::new()
function Add-Failure([string]$check, [string]$message) {
  $failures.Add("[$check] $message")
  Write-Host "  FAIL: $message" -ForegroundColor Red
}
function Write-Check([string]$name) { Write-Host "`n== $name" -ForegroundColor Cyan }

if (-not (Test-Path $featureDir)) { throw "Feature folder not found: $featureDir" }

$featureProject = Get-ChildItem $featureDir -Filter *.csproj | Select-Object -First 1
if (-not $featureProject) { throw "No .csproj found in $featureDir" }
$featureAssembly = [System.IO.Path]::GetFileNameWithoutExtension($featureProject.Name)

$sourceFiles = Get-ChildItem $featureDir -Recurse -Filter *.cs |
Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
$testFiles = @()
if (Test-Path $testDir) {
  $testFiles = Get-ChildItem $testDir -Recurse -Filter *.cs |
  Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
}
$testText = ($testFiles | ForEach-Object { Get-Content $_.FullName -Raw }) -join "`n"

Write-Check "Test project"
$testProject = $null
if (-not (Test-Path $testDir)) {
  Add-Failure "tests" "Test folder missing: tests/Features/$Feature"
}
else {
  $testProject = Get-ChildItem $testDir -Filter *.csproj | Select-Object -First 1
  if (-not $testProject) { Add-Failure "tests" "No test .csproj in tests/Features/$Feature" }
}

Write-Check "Host composition"
$programText = Get-Content $programFile -Raw
$hostProjectText = Get-Content $hostProject -Raw
if ($programText -match [regex]::Escape($featureAssembly)) {
  if ($hostProjectText -notmatch [regex]::Escape($featureProject.Name)) {
    Add-Failure "host" "Program.cs uses $featureAssembly but Zeus.Academia.Api.csproj has no ProjectReference to $($featureProject.Name)"
  }
}
$mapMethods = $sourceFiles | Select-String -Pattern 'public static \S+ (Map\w+Endpoints)\s*\(' |
ForEach-Object { $_.Matches[0].Groups[1].Value } | Sort-Object -Unique
foreach ($method in $mapMethods) {
  if ($programText -notmatch "\b$method\s*\(") {
    Add-Failure "host" "$method is defined but never called from Program.cs"
  }
}

Write-Check "Configuration safety"
if ((Get-Content $appSettings -Raw) -match '(?i)\(localdb\)') {
  Add-Failure "config" "appsettings.json contains a LocalDB connection string; move it to appsettings.Development.json or user secrets"
}

Write-Check "Nullability and Try* contracts"
foreach ($hit in ($sourceFiles | Select-String -Pattern '(=\s*null!|=\s*default!)' | Where-Object Line -notmatch '\bDbSet<')) {
  Add-Failure "nullability" "$($hit.Path | Resolve-Path -Relative):$($hit.LineNumber) uses a null-forgiving placeholder"
}
foreach ($hit in ($sourceFiles | Select-String -Pattern 'bool\s+Try\w+\s*\([^)]*\bout\s+string\s+\w+')) {
  Add-Failure "try-pattern" "$($hit.Path | Resolve-Path -Relative):$($hit.LineNumber) Try* method has a non-nullable 'out string'; use 'out string?' and assign null on failure"
}

Write-Check "Persistence ownership"
if ($Feature -notlike "SharedKernel/*") {
  foreach ($hit in ($sourceFiles | Select-String -Pattern '\bSharedKernelDbContext\b')) {
    Add-Failure "persistence" "$($hit.Path | Resolve-Path -Relative):$($hit.LineNumber) references SharedKernelDbContext; use the feature-local DbContext"
  }
}
$dbContexts = $sourceFiles | Select-String -Pattern 'class\s+(\w+)\s*(\([^)]*\))?\s*:\s*DbContext\b' |
ForEach-Object { $_.Matches[0].Groups[1].Value } | Sort-Object -Unique
$writesData = [bool]($sourceFiles | Select-String -Pattern '\bSaveChangesAsync\s*\(' -Quiet)
if ($writesData -and -not $dbContexts) {
  Add-Failure "persistence" "Feature calls SaveChangesAsync but owns no DbContext"
}
foreach ($context in $dbContexts) {
  $snapshot = $sourceFiles | Where-Object Name -eq "${context}ModelSnapshot.cs"
  $designers = $sourceFiles | Where-Object { $_.Name -like "*.Designer.cs" -and (Select-String -Path $_.FullName -Pattern "DbContext\(typeof\($context\)\)" -Quiet) }
  $contextFile = $sourceFiles | Where-Object { Select-String -Path $_.FullName -Pattern "class\s+$context\b" -Quiet } | Select-Object -First 1
  $mappingOnly = $contextFile -and (Select-String -Path $contextFile.FullName -Pattern 'ExcludeFromMigrations' -Quiet)
  if ($mappingOnly -and -not $snapshot -and -not $designers) {
    Write-Host "  note: $context is mapping-only (ExcludeFromMigrations); confirm owners in migration-ownership-matrix.md" -ForegroundColor Yellow
    continue
  }
  if (-not $snapshot) { Add-Failure "migrations" "$context has no ${context}ModelSnapshot.cs" }
  if (-not $designers) { Add-Failure "migrations" "$context has no migration Designer file" }
}
if ($dbContexts -and $testProject) {
  if ((Get-Content $testProject.FullName -Raw) -notmatch 'Microsoft\.EntityFrameworkCore\.SqlServer') {
    Add-Failure "sqlserver" "$($testProject.Name) lacks Microsoft.EntityFrameworkCore.SqlServer; InMemory tests are not persistence evidence"
  }
  if ($testText -notmatch 'MigrateAsync\s*\(') {
    Add-Failure "sqlserver" "No test applies migrations with Database.MigrateAsync()"
  }
  foreach ($hit in ($testFiles | Select-String -Pattern '\bEnsureCreated(Async)?\s*\(')) {
    Add-Failure "sqlserver" "$($hit.Path | Resolve-Path -Relative):$($hit.LineNumber) uses EnsureCreated for a migration-owned context"
  }
}

Write-Check "Validator tests"
foreach ($validator in ($sourceFiles | Where-Object Name -like "*Validator.cs")) {
  $expected = "$($validator.BaseName)Tests.cs"
  if (-not ($testFiles | Where-Object Name -eq $expected)) {
    Add-Failure "validator-tests" "$($validator.Name) has no $expected"
  }
}

Write-Check "Route tests for declared statuses"
$statusNames = @{
  200 = 'OK'; 201 = 'Created'; 204 = 'NoContent'; 400 = 'BadRequest'
  401 = 'Unauthorized'; 403 = 'Forbidden'; 404 = 'NotFound'; 409 = 'Conflict'; 422 = 'UnprocessableEntity'
}
$routeTestText = ($testFiles | Where-Object {
    Select-String -Path $_.FullName -Pattern 'WebApplicationFactory|TestServer' -Quiet
  } | ForEach-Object { Get-Content $_.FullName -Raw }) -join "`n"
foreach ($endpoint in ($sourceFiles | Where-Object { Select-String -Path $_.FullName -Pattern '\.Produces' -Quiet })) {
  $text = Get-Content $endpoint.FullName -Raw
  $codes = [System.Collections.Generic.HashSet[int]]::new()
  if ($text -match 'ProducesValidationProblem\s*\(') { [void]$codes.Add(400) }
  foreach ($m in [regex]::Matches($text, 'Produces(?:Problem)?(?:<[^>]+>)?\s*\(\s*(?:StatusCodes\.Status)?(\d{3})')) {
    [void]$codes.Add([int]$m.Groups[1].Value)
  }
  if ($codes.Count -gt 0 -and -not $routeTestText) {
    Add-Failure "route-tests" "$($endpoint.Name) declares statuses but no WebApplicationFactory/TestServer tests exist"
    continue
  }
  foreach ($code in $codes) {
    $name = $statusNames[$code]
    $pattern = "\b$code\b|StatusCodes\.Status$code"
    if ($name) { $pattern += "|HttpStatusCode\.$name\b" }
    if ($routeTestText -notmatch $pattern) {
      Add-Failure "route-tests" "$($endpoint.Name) declares $code but no route test asserts it"
    }
  }
}

Write-Check "Provenance timestamps in changed Markdown"
$changed = @()
try { $changed = git -C $repoRoot diff --name-only --diff-filter=ACMR "$BaseRef...HEAD" 2>$null } catch { }
foreach ($path in ($changed | Where-Object { $_ -like "*.md" })) {
  $full = Join-Path $repoRoot $path
  if (-not (Test-Path $full)) { continue }
  $head = (Get-Content $full -TotalCount 60) -join "`n"
  if ($head -notmatch '(?m)^ai_generated:\s*true') { continue }
  $started = [regex]::Match($head, '(?m)^started:\s*"?([^"\r\n]+)"?').Groups[1].Value
  $ended = [regex]::Match($head, '(?m)^ended:\s*"?([^"\r\n]+)"?').Groups[1].Value
  $total = [regex]::Match($head, '(?m)^total_duration:\s*"?([^"\r\n]+)"?').Groups[1].Value
  if (-not ($started -and $ended -and $total)) { continue }
  try {
    $span = [datetimeoffset]::Parse($ended) - [datetimeoffset]::Parse($started)
    # Allow one minute for rounded durations.
    if ($span -le [timespan]::Zero -or $span.Add([timespan]::FromMinutes(1)) -lt [timespan]::Parse($total)) {
      Add-Failure "provenance" "${path}: ended - started ($span) is inconsistent with total_duration $total"
    }
  }
  catch {
    Add-Failure "provenance" "${path}: unparseable started/ended/total_duration"
  }
}

if (-not $SkipBuild) {
  Write-Check "Solution build"
  dotnet build $solution --configuration $Configuration --nologo -v quiet
  if ($LASTEXITCODE -ne 0) { Add-Failure "build" "dotnet build failed for zeus.academia.3b.sln" }
}

if ($RunEfChecks -and $dbContexts) {
  Write-Check "EF migration discovery"
  foreach ($context in $dbContexts) {
    $output = dotnet ef migrations list --project $featureProject.FullName --startup-project $hostProject --context $context --no-build 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0 -or $output -match 'No migrations were found') {
      Add-Failure "ef" "${context}: migrations not discovered`n$output"
    }
  }
}

if (-not $SkipTests -and $testProject) {
  Write-Check "Feature tests"
  dotnet test $testProject.FullName --configuration $Configuration --nologo
  if ($LASTEXITCODE -ne 0) { Add-Failure "tests" "dotnet test failed for $($testProject.Name)" }
}

Write-Host ""
if ($failures.Count -gt 0) {
  Write-Host "verify-slice: $($failures.Count) check(s) failed for $Feature" -ForegroundColor Red
  $failures | ForEach-Object { Write-Host "  $_" }
  exit 1
}
Write-Host "verify-slice: all checks passed for $Feature" -ForegroundColor Green
