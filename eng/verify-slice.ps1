<#
ai_generated: true
model: "anthropic/claude-opus-5.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-29-pr48-review-instruction-hardening"
prompt: "Review PR 48 review comments and improve instruction files and prompts; go ahead"
ai_log: "ai-logs/2026/09/29/2026-09-29-pr48-review-instruction-hardening/conversation.md"
source: ".github/instructions/ai-dev-process.instructions.md#mechanical-verification-gate"
#>
<#
.SYNOPSIS
  Mechanical pre-handoff checks for one vertical slice.
.EXAMPLE
  pwsh eng/verify-slice.ps1 -AllChangedFeatures
.EXAMPLE
  pwsh eng/verify-slice.ps1 -Feature Academics/RegisterAcademic
.EXAMPLE
  pwsh eng/verify-slice.ps1 -Features Academics/RegisterAcademic,SharedKernel/Foundation
#>
[CmdletBinding(DefaultParameterSetName = "Single")]
param(
  [Parameter(Mandatory, ParameterSetName = "Single")]
  [string]$Feature,
  [Parameter(Mandatory, ParameterSetName = "All")]
  [switch]$AllChangedFeatures,
  [Parameter(Mandatory, ParameterSetName = "Explicit")]
  [string[]]$Features,
  [string]$Configuration = "Debug",
  [string]$BaseRef = "origin/main",
  [switch]$SkipBuild,
  [switch]$SkipTests,
  [switch]$SkipEfChecks,
  # Internal: set by the -AllChangedFeatures dispatcher after it has run repo-level checks once.
  [Parameter(ParameterSetName = "Single")]
  [switch]$SkipRepoChecks
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

# A local branch literally named "origin/main" would otherwise shadow the remote-tracking ref.
$remoteRef = "refs/remotes/$BaseRef"
git -C $repoRoot rev-parse --verify --quiet $remoteRef *> $null
if ($LASTEXITCODE -eq 0) { $BaseRef = $remoteRef }

$failures = [System.Collections.Generic.List[string]]::new()
function Add-Failure([string]$check, [string]$message) {
  $failures.Add("[$check] $message")
  Write-Host "  FAIL: $message" -ForegroundColor Red
}
function Write-Check([string]$name) { Write-Host "`n== $name" -ForegroundColor Cyan }

function Get-ChangedPaths {
  $paths = @()
  $paths += git -C $repoRoot diff --name-only --diff-filter=ACMR "$BaseRef...HEAD" 2>$null
  $paths += git -C $repoRoot diff --name-only --diff-filter=ACMR HEAD 2>$null
  $paths += git -C $repoRoot ls-files --others --exclude-standard 2>$null
  $paths | Where-Object { $_ } | Sort-Object -Unique
}

function Test-InsideTry([string]$text, [int]$index) {
  foreach ($try in [regex]::Matches($text.Substring(0, $index), '\btry\s*\{')) {
    $depth = 0
    for ($i = $try.Index + $try.Length - 1; $i -lt $index; $i++) {
      if ($text[$i] -eq '{') { $depth++ }
      elseif ($text[$i] -eq '}') { $depth--; if ($depth -eq 0) { break } }
    }
    if ($depth -gt 0) { return $true }
  }
  return $false
}

function Invoke-RepoChecks([string[]]$changedPaths) {
  Write-Check "Solution integrity"
  $slnLines = Get-Content $solution
  if (-not ($slnLines | Select-Object -First 2 | Where-Object { $_ -match '^\W*Microsoft Visual Studio Solution File' })) {
    Add-Failure "solution" "zeus.academia.3b.sln header is not in the first two lines"
  }
  $declared = @($slnLines | Select-String -Pattern '^Project\("\{[^}]+\}"\)\s*=\s*"[^"]*",\s*"([^"]+\.csproj)"' |
    ForEach-Object { $_.Matches[0].Groups[1].Value.Replace('\', '/') })
  foreach ($dup in ($declared | Group-Object | Where-Object Count -gt 1)) {
    Add-Failure "solution" "$($dup.Name) is declared $($dup.Count) times"
  }
  $projects = Get-ChildItem (Join-Path $repoRoot "src"), (Join-Path $repoRoot "tests") -Recurse -Filter *.csproj |
  Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
  foreach ($project in $projects) {
    $relative = [System.IO.Path]::GetRelativePath($repoRoot, $project.FullName).Replace('\', '/')
    if ($declared -notcontains $relative) { Add-Failure "solution" "$relative is not declared in zeus.academia.3b.sln" }
  }

  Write-Check "Migration ownership matrix artifacts"
  $matrixPath = Join-Path $repoRoot "src/models/workflows/migration-ownership-matrix.md"
  $matrixText = Get-Content $matrixPath -Raw
  foreach ($match in [regex]::Matches(
      $matrixText,
      '(?m)^\|\s*[^|]+\|\s*(?<context>\w+DbContext)\s*\|[^|]+\|\s*`(?<migration>\d+_\w+)`\s*\|')) {
    $context = $match.Groups['context'].Value
    $migration = $match.Groups['migration'].Value
    $migrationFile = Get-ChildItem (Join-Path $repoRoot "src/features") -Recurse -Filter "${migration}.cs" |
    Where-Object { $_.Name -notlike "*.Designer.cs" }
    $designerFile = Get-ChildItem (Join-Path $repoRoot "src/features") -Recurse -Filter "${migration}.Designer.cs"
    $snapshotFile = Get-ChildItem (Join-Path $repoRoot "src/features") -Recurse -Filter "${context}ModelSnapshot.cs"

    if (-not $migrationFile) { Add-Failure "migration-matrix" "$migration for $context has no migration class" }
    if (-not $designerFile) { Add-Failure "migration-matrix" "$migration for $context has no Designer file" }
    elseif (-not (Select-String -Path $designerFile.FullName -Pattern "DbContext\(typeof\($context\)\)" -Quiet)) {
      Add-Failure "migration-matrix" "$migration Designer does not target $context"
    }
    if (-not $snapshotFile) { Add-Failure "migration-matrix" "$migration for $context has no ${context}ModelSnapshot.cs" }
  }

  Write-Check "verify-slice.ps1 parameters cited in guidance"
  $scriptParams = (Get-Command $PSCommandPath).Parameters.Keys
  $guidance = @(Get-ChildItem (Join-Path $repoRoot ".github") -Recurse -Filter *.md) + @(Get-Item (Join-Path $repoRoot "README.md"))
  foreach ($doc in $guidance) {
    $text = Get-Content $doc.FullName -Raw
    foreach ($m in [regex]::Matches($text, 'verify-slice\.ps1((?:[ \t]+-\w+(?:[ \t]+[^\s`\-][^\s`]*)?)*)')) {
      foreach ($p in [regex]::Matches($m.Groups[1].Value, '(?<=\s)-(\w+)')) {
        if ($scriptParams -notcontains $p.Groups[1].Value) {
          Add-Failure "doc-refs" "$([System.IO.Path]::GetRelativePath($repoRoot, $doc.FullName)): verify-slice.ps1 has no -$($p.Groups[1].Value) parameter"
        }
      }
    }
  }

  Write-Check "Documented paths in changed Markdown"
  # Standards files and their generator prompts cite illustrative paths by design.
  $illustrative = '^(\.github/instructions/|\.github/prompts/create-|ai-logs/)'
  foreach ($path in ($changedPaths | Where-Object { $_ -like "*.md" -and $_ -notmatch $illustrative })) {
    $full = Join-Path $repoRoot $path
    if (-not (Test-Path $full)) { continue }
    $text = Get-Content $full -Raw
    $dir = Split-Path $full -Parent
    $refs = @()
    $refs += [regex]::Matches($text, '`((?:src|tests|eng|docs|\.github)/[^`\s]+)`') | ForEach-Object { $_.Groups[1].Value }
    $refs += [regex]::Matches($text, '\]\(([^)\s]+)\)') | ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -notmatch '^(https?:|mailto:|#)' }
    foreach ($ref in ($refs | Sort-Object -Unique)) {
      # Placeholders, globs, and elided paths are templates, not references.
      if ($ref -match '[<>*{}$]|\.\.\.') { continue }
      $target = [uri]::UnescapeDataString((($ref -split '[#?]')[0] -replace ':\d+(-\d+)?$', ''))
      if (-not $target) { continue }
      if (-not ((Test-Path (Join-Path $dir $target)) -or (Test-Path (Join-Path $repoRoot $target)))) {
        Add-Failure "doc-refs" "${path}: '$ref' does not exist"
      }
    }
  }

  Write-Check "Provenance timestamps in changed Markdown"
  foreach ($path in ($changedPaths | Where-Object { $_ -like "*.md" })) {
    $full = Join-Path $repoRoot $path
    if (-not (Test-Path $full)) { continue }
    $head = (Get-Content $full -TotalCount 60) -join "`n"
    if ($head -notmatch '(?m)^ai_generated:\s*true') { continue }
    $taskColumn = -1
    foreach ($line in ($head -split "`n")) {
      if ($line -match '^(\s*)- task:') { $taskColumn = $Matches[1].Length + 2 }
      elseif ($taskColumn -ge 0 -and $line -match '^(\s*)duration:' -and $Matches[1].Length -ne $taskColumn) {
        Add-Failure "provenance" "${path}: task_durations 'duration:' is not aligned with its 'task:' key (invalid YAML)"
        break
      }
    }
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
}

if ($AllChangedFeatures -or $Features) {
  $changedPaths = @(Get-ChangedPaths)
  $detectedFeatures = @($changedPaths | ForEach-Object {
      if ($_ -match '^(?:src/features|tests/Features)/([^/]+)/([^/]+)/') { "$($Matches[1])/$($Matches[2])" }
    } | Sort-Object -Unique | Where-Object {
      $dir = Join-Path $repoRoot "src/features/$_"
      (Test-Path $dir) -and (Get-ChildItem $dir -Filter *.csproj)
    })
  $explicitFeatures = @($Features | ForEach-Object { $_ -split ',' } |
    ForEach-Object { $_.Trim() } | Where-Object { $_ } | Sort-Object -Unique)
  foreach ($explicitFeature in $explicitFeatures) {
    $explicitDir = Join-Path $repoRoot "src/features/$explicitFeature"
    if (-not (Test-Path $explicitDir) -or -not (Get-ChildItem $explicitDir -Filter *.csproj)) {
      Add-Failure "manifest" "Explicit feature '$explicitFeature' does not exist on the current branch"
    }
  }
  $featuresToVerify = @(($detectedFeatures + $explicitFeatures) | Sort-Object -Unique)
  Write-Host "Changed features: $(if ($detectedFeatures) { $detectedFeatures -join ', ' } else { '(none)' })"
  if ($Features) {
    Write-Host "Manifest features: $(if ($explicitFeatures) { $explicitFeatures -join ', ' } else { '(none)' })"
    if (-not $explicitFeatures) { Add-Failure "manifest" "-Features resolved to an empty feature set" }
  }

  Invoke-RepoChecks $changedPaths
  $failedFeatures = [System.Collections.Generic.List[string]]::new()
  foreach ($changedFeature in $featuresToVerify) {
    Write-Host "`n#### $changedFeature" -ForegroundColor Magenta
    $childArgs = @{
      Feature = $changedFeature; Configuration = $Configuration; BaseRef = $BaseRef
      SkipBuild = $true; SkipRepoChecks = $true; SkipTests = $SkipTests; SkipEfChecks = $SkipEfChecks
    }
    try { & $PSCommandPath @childArgs }
    catch { Write-Host "  $_" -ForegroundColor Red; $global:LASTEXITCODE = 1 }
    if ($LASTEXITCODE -ne 0) { $failedFeatures.Add($changedFeature) }
  }

  Write-Host ""
  if ($failures.Count -gt 0 -or $failedFeatures.Count -gt 0) {
    Write-Host "verify-slice: FAILED" -ForegroundColor Red
    $failures | ForEach-Object { Write-Host "  $_" }
    $failedFeatures | ForEach-Object { Write-Host "  [feature] $_ failed; see its section above" }
    exit 1
  }
  $scopeLabel = if ($Features) { "explicit/changed" } else { "changed" }
  Write-Host "verify-slice: all checks passed for $($featuresToVerify.Count) $scopeLabel feature(s)" -ForegroundColor Green
  exit 0
}

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
if ($programText -match '(?i)\(localdb\)') {
  if ($programText -notmatch '(?s)IsDevelopment\(\)\s*&&\s*OperatingSystem\.IsWindows\(\)\)\s*\{[^}]*\(localdb\)') {
    Add-Failure "config" "Program.cs LocalDB fallback must be guarded by builder.Environment.IsDevelopment() (and OperatingSystem.IsWindows()); unconditional LocalDB fallback can silently mask missing production configuration"
  }
}
if ($programText -match 'Database\.MigrateAsync\s*\(\s*\)') {
  if ($programText -notmatch [regex]::Escape('args.Contains("--migrate"')) {
    Add-Failure "config" "Program.cs applies migrations without gating on an explicit '--migrate' pipeline switch; startup migration must not run unconditionally"
  }
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
$migrationContexts = [System.Collections.Generic.List[string]]::new()
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
  $migrationContexts.Add($context)
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

Write-Check "Claim concurrency"
foreach ($file in ($sourceFiles | Where-Object { $_.FullName -notmatch '[\\/]Migrations[\\/]' })) {
  $text = Get-Content $file.FullName -Raw
  $relative = Resolve-Path -Relative $file.FullName
  foreach ($call in [regex]::Matches($text, '\.ExecuteUpdateAsync\s*\(')) {
    if (-not (Test-InsideTry $text $call.Index)) {
      Add-Failure "concurrency" "${relative}: ExecuteUpdateAsync runs outside try/catch; a unique-index violation from the claim escapes as 500"
    }
    if ($text -notmatch '\bBeginTransactionAsync\s*\(' -or
      $text -notmatch '\bCommitAsync\s*\(' -or
      $text -notmatch '\bRollbackAsync\s*\(') {
      Add-Failure "concurrency" "${relative}: atomic claim must begin, commit, and roll back an explicit transaction"
    }
    $assignment = [regex]::Match(
      $text,
      '(?s)\bvar\s+(?<result>\w+)\s*=\s*await(?:(?!;).)*?\.ExecuteUpdateAsync\s*\(')
    if (-not $assignment.Success) {
      Add-Failure "concurrency" "${relative}: ExecuteUpdateAsync claim must capture the affected-row count"
    }
    else {
      $result = [regex]::Escape($assignment.Groups['result'].Value)
      if ($text -notmatch "\b$result\s*==\s*0\b") {
        Add-Failure "concurrency" "${relative}: zero affected rows must translate the lost claim to a conflict"
      }
    }
  }
  if ($text -match '\bSaveChangesAsync\s*\(' -and
    $text -match '\.\s*(Assign|Claim|Reserve|Allocate)\w*\s*\(' -and
    $text -notmatch 'ExecuteUpdateAsync|DbUpdateConcurrencyException') {
    Add-Failure "concurrency" "${relative}: claim is a tracked mutation + SaveChangesAsync with no ExecuteUpdateAsync predicate or concurrency token"
  }
}
$hasAtomicClaim = [bool]($sourceFiles | Select-String -Pattern '\.ExecuteUpdateAsync\s*\(' | Select-Object -First 1)
if ($hasAtomicClaim) {
  if ($testText -notmatch '\bTask\.WhenAll\s*\(' -or $testText -notmatch 'SqlServer') {
    Add-Failure "concurrency" "Atomic claim has no concurrent SQL Server test using simultaneous claimant tasks"
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
$routeTestFiles = @($testFiles | Where-Object {
    Select-String -Path $_.FullName -Pattern 'WebApplicationFactory|TestServer' -Quiet
  })
$routeTestText = ($routeTestFiles | ForEach-Object { Get-Content $_.FullName -Raw }) -join "`n"
foreach ($endpoint in ($sourceFiles | Where-Object { Select-String -Path $_.FullName -Pattern '\.Produces' -Quiet })) {
  $text = Get-Content $endpoint.FullName -Raw
  $endpointStem = $endpoint.BaseName -replace 'Endpoints?$', ''
  $endpointRouteText = ($routeTestFiles | Where-Object {
      $_.BaseName -match [regex]::Escape($endpointStem) -or
      (Get-Content $_.FullName -Raw) -match [regex]::Escape($endpointStem)
    } | ForEach-Object { Get-Content $_.FullName -Raw }) -join "`n"
  $codes = [System.Collections.Generic.HashSet[int]]::new()
  if ($text -match 'ProducesValidationProblem\s*\(') { [void]$codes.Add(400) }
  foreach ($m in [regex]::Matches($text, 'Produces(?:Problem)?(?:<[^>]+>)?\s*\(\s*(?:StatusCodes\.Status)?(\d{3})')) {
    [void]$codes.Add([int]$m.Groups[1].Value)
  }
  if ($codes.Count -gt 0 -and -not $endpointRouteText) {
    Add-Failure "route-tests" "$($endpoint.Name) declares statuses but has no attributable WebApplicationFactory/TestServer route tests"
    continue
  }
  foreach ($code in $codes) {
    $name = $statusNames[$code]
    $pattern = "\b$code\b|StatusCodes\.Status$code"
    if ($name) { $pattern += "|HttpStatusCode\.$name\b" }
    if ($endpointRouteText -notmatch $pattern) {
      Add-Failure "route-tests" "$($endpoint.Name) declares $code but no route test asserts it"
    }
  }
}

$advertisesValidation = [bool]($sourceFiles | Select-String -Pattern 'ProducesValidationProblem\s*\(' -Quiet)
if ($advertisesValidation -and $routeTestText) {
  foreach ($validator in ($sourceFiles | Where-Object Name -like "*Validator.cs")) {
    $validatorStem = $validator.BaseName -replace '(Command|Query)?Validator$', ''
    $validatorRouteText = ($routeTestFiles | Where-Object {
        $_.BaseName -match [regex]::Escape($validatorStem) -or
        (Get-Content $_.FullName -Raw) -match [regex]::Escape($validatorStem)
      } | ForEach-Object { Get-Content $_.FullName -Raw }) -join "`n"
    $properties = Select-String -Path $validator.FullName -Pattern 'RuleFor(?:Each)?\(\s*\w+\s*=>\s*\w+\.(\w+)' -AllMatches |
    ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
    foreach ($property in $properties) {
      $key = $property.Substring(0, 1).ToLowerInvariant() + $property.Substring(1)
      if ($validatorRouteText -notmatch "`"(?:[\w\[\]]+\.)*$key(?:[\.\[][^`"]*)?`"") {
        Add-Failure "route-tests" "$($validator.Name) validates $property but no route test asserts a '$key' validation error key"
      }
    }
  }
}

if (-not $SkipRepoChecks) { Invoke-RepoChecks @(Get-ChangedPaths) }

if (-not $SkipEfChecks -and $migrationContexts.Count -gt 0) {
  Write-Check "EF migration discovery and model drift"
  $efArgs = @("--project", $featureProject.FullName, "--startup-project", $hostProject, "--no-build", "--configuration", $Configuration)
  foreach ($context in $migrationContexts) {
    $output = dotnet ef migrations list @efArgs --context $context --no-connect 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0 -or $output -match 'No migrations were found') {
      Add-Failure "ef" "${context}: migrations not discovered`n$output"
      continue
    }
    $output = dotnet ef migrations has-pending-model-changes @efArgs --context $context 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) {
      Add-Failure "ef" "${context}: model differs from the snapshot; regenerate the migration instead of hand-editing it"
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
exit 0
