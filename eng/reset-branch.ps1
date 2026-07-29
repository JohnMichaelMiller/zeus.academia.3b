param(
  [Parameter(Mandatory = $true)]
  [ValidateRange(1, [int]::MaxValue)]
  [int]$PrNumber,

  [Parameter(Mandatory = $true)]
  [ValidateNotNullOrEmpty()]
  [string]$BranchName,

  [string]$MainBranch = "main",
  [string]$CommitMessage = "chore: update guardrails after PR review",
  [switch]$SkipMainPull,
  [switch]$SkipRemoteCreate
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Assert-Command {
  param([Parameter(Mandatory = $true)][string]$Name)

  if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
    throw "Required command '$Name' is not available in PATH."
  }
}

function Invoke-Git {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  & git @Args
  if ($LASTEXITCODE -ne 0) {
    throw "git $($Args -join ' ') failed with exit code $LASTEXITCODE."
  }
}

function Get-GitOutput {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  $output = & git @Args 2>$null
  if ($LASTEXITCODE -ne 0) {
    return $null
  }

  return ($output | Out-String).Trim()
}

function Invoke-Gh {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  & gh @Args
  if ($LASTEXITCODE -ne 0) {
    throw "gh $($Args -join ' ') failed with exit code $LASTEXITCODE."
  }
}

Assert-Command -Name "git"
Assert-Command -Name "gh"

$repoRoot = Get-GitOutput @("rev-parse", "--show-toplevel")
if ([string]::IsNullOrWhiteSpace($repoRoot)) {
  throw "Current directory is not inside a git repository."
}

Set-Location -Path $repoRoot
Write-Host "Repository: $repoRoot" -ForegroundColor Cyan

Write-Host "Fetching latest refs from origin..." -ForegroundColor Cyan
Invoke-Git @("fetch", "origin", "--prune")

$currentBranch = Get-GitOutput @("rev-parse", "--abbrev-ref", "HEAD")
if ($currentBranch -ne $BranchName) {
  $branchExistsLocal = -not [string]::IsNullOrWhiteSpace((Get-GitOutput @("show-ref", "--verify", "refs/heads/$BranchName")))
  if ($branchExistsLocal) {
    Write-Host "Switching to local branch $BranchName..." -ForegroundColor Cyan
    Invoke-Git @("checkout", $BranchName)
  }
  else {
    $branchExistsRemote = -not [string]::IsNullOrWhiteSpace((Get-GitOutput @("ls-remote", "--heads", "origin", $BranchName)))
    if (-not $branchExistsRemote) {
      throw "Branch '$BranchName' not found locally or on origin."
    }

    Write-Host "Creating local branch $BranchName from origin/$BranchName..." -ForegroundColor Cyan
    Invoke-Git @("checkout", "-b", $BranchName, "origin/$BranchName")
  }
}

$workingTreeStatus = Get-GitOutput @("status", "--porcelain")
if (-not [string]::IsNullOrWhiteSpace($workingTreeStatus)) {
  Write-Host "Committing working tree changes on $BranchName..." -ForegroundColor Cyan
  Invoke-Git @("add", "-A")

  # Commit only if staged content exists after add.
  $stagedStatus = Get-GitOutput @("diff", "--cached", "--name-only")
  if (-not [string]::IsNullOrWhiteSpace($stagedStatus)) {
    Invoke-Git @("commit", "-m", $CommitMessage)
  }
}
else {
  Write-Host "No uncommitted changes found on $BranchName." -ForegroundColor Yellow
}

Write-Host "Pushing $BranchName to origin..." -ForegroundColor Cyan
Invoke-Git @("push", "origin", $BranchName)

$mainExistsLocal = -not [string]::IsNullOrWhiteSpace((Get-GitOutput @("show-ref", "--verify", "refs/heads/$MainBranch")))
if ($mainExistsLocal) {
  Invoke-Git @("checkout", $MainBranch)
}
else {
  Write-Host "Creating local $MainBranch from origin/$MainBranch..." -ForegroundColor Cyan
  Invoke-Git @("checkout", "-b", $MainBranch, "origin/$MainBranch")
}

if (-not $SkipMainPull) {
  Write-Host "Updating $MainBranch from origin..." -ForegroundColor Cyan
  Invoke-Git @("pull", "--ff-only", "origin", $MainBranch)
}

Write-Host "Merging $BranchName into $MainBranch..." -ForegroundColor Cyan
Invoke-Git @("merge", "--no-ff", $BranchName, "-m", "merge: $BranchName into $MainBranch")

Write-Host "Pushing $MainBranch to origin..." -ForegroundColor Cyan
Invoke-Git @("push", "origin", $MainBranch)

Write-Host "Closing PR #$PrNumber..." -ForegroundColor Cyan
Invoke-Gh @("pr", "close", "$PrNumber", "--comment", "Closed by reset-part-eight-branch automation after merge to $MainBranch.")

$branchExistsLocalAfterMerge = -not [string]::IsNullOrWhiteSpace((Get-GitOutput @("show-ref", "--verify", "refs/heads/$BranchName")))
if ($branchExistsLocalAfterMerge) {
  Write-Host "Deleting local branch $BranchName..." -ForegroundColor Cyan
  Invoke-Git @("branch", "-D", $BranchName)
}

Write-Host "Deleting remote branch $BranchName..." -ForegroundColor Cyan
$remoteDeleteResult = & git push origin --delete $BranchName 2>&1
if ($LASTEXITCODE -ne 0) {
  $remoteDeleteText = ($remoteDeleteResult | Out-String).Trim()
  if ($remoteDeleteText -match "remote ref does not exist" -or $remoteDeleteText -match "unable to delete") {
    Write-Host "Remote branch $BranchName was already absent." -ForegroundColor Yellow
  }
  else {
    throw "Failed deleting remote branch '$BranchName': $remoteDeleteText"
  }
}

Write-Host "Creating new local branch $BranchName from $MainBranch..." -ForegroundColor Cyan
Invoke-Git @("checkout", "-b", $BranchName, $MainBranch)

if (-not $SkipRemoteCreate) {
  Write-Host "Creating new remote branch $BranchName and setting upstream..." -ForegroundColor Cyan
  Invoke-Git @("push", "-u", "origin", $BranchName)
}

Write-Host "Completed. Current branch is now $BranchName." -ForegroundColor Green
