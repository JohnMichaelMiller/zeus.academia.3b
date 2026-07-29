<#
.SYNOPSIS
Resets a git branch and associated PR after review changes.

.DESCRIPTION
This script performs a complete reset of a feature branch and its pull request:
1. Commits any working tree changes on the feature branch
2. Merges the feature branch into the main branch
3. Closes the associated PR
4. Recreates the feature branch from the updated main branch

.PARAMETER PrNumber
The GitHub pull request number to manage.

.PARAMETER BranchName
The name of the feature branch to reset.

.PARAMETER MainBranch
The main branch name (default: "main").

.PARAMETER CommitMessage
The commit message for working tree changes (default: "chore: update guardrails after PR review").

.PARAMETER SkipMainPull
Skip pulling the main branch from origin.

.PARAMETER SkipRemoteCreate
Skip creating and pushing the new remote branch.
#>

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

# Verifies that a required command is available in PATH
function Assert-Command {
  param([Parameter(Mandatory = $true)][string]$Name)

  if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
    throw "Required command '$Name' is not available in PATH."
  }
}

# Executes a git command and throws on failure
function Invoke-Git {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  & git @Args
  if ($LASTEXITCODE -ne 0) {
    throw "git $($Args -join ' ') failed with exit code $LASTEXITCODE."
  }
}

# Executes a git command and returns output, or null on failure
function Get-GitOutput {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  $output = & git @Args 2>$null
  if ($LASTEXITCODE -ne 0) {
    return $null
  }

  return ($output | Out-String).Trim()
}

# Executes a gh (GitHub CLI) command and throws on failure
function Invoke-Gh {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  & gh @Args
  if ($LASTEXITCODE -ne 0) {
    throw "gh $($Args -join ' ') failed with exit code $LASTEXITCODE."
  }
}

# Executes a gh command and returns output, or null on failure
function Get-GhOutput {
  param([Parameter(Mandatory = $true)][string[]]$Args)

  $output = & gh @Args 2>$null
  if ($LASTEXITCODE -ne 0) {
    return $null
  }

  return ($output | Out-String).Trim()
}

# Ensure required tools are available
Assert-Command -Name "git"
Assert-Command -Name "gh"

# Get repository root and validate we're in a git repository
$repoRoot = Get-GitOutput @("rev-parse", "--show-toplevel")
if ([string]::IsNullOrWhiteSpace($repoRoot)) {
  throw "Current directory is not inside a git repository."
}

Set-Location -Path $repoRoot
Write-Host "Repository: $repoRoot" -ForegroundColor Cyan

# Fetch latest refs and clean up deleted branches
Write-Host "Fetching latest refs from origin..." -ForegroundColor Cyan
Invoke-Git @("fetch", "origin", "--prune")

# Switch to the feature branch, creating it locally if needed
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

# Commit any uncommitted changes on the feature branch
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

# Push feature branch changes to origin
Write-Host "Pushing $BranchName to origin..." -ForegroundColor Cyan
Invoke-Git @("push", "origin", $BranchName)

# Switch to main branch, creating it locally if needed
$mainExistsLocal = -not [string]::IsNullOrWhiteSpace((Get-GitOutput @("show-ref", "--verify", "refs/heads/$MainBranch")))
if ($mainExistsLocal) {
  Invoke-Git @("checkout", $MainBranch)
}
else {
  Write-Host "Creating local $MainBranch from origin/$MainBranch..." -ForegroundColor Cyan
  Invoke-Git @("checkout", "-b", $MainBranch, "origin/$MainBranch")
}

# Update main branch from origin if not skipped
if (-not $SkipMainPull) {
  Write-Host "Updating $MainBranch from origin..." -ForegroundColor Cyan
  Invoke-Git @("pull", "--ff-only", "origin", $MainBranch)
}

# Merge feature branch into main with explicit merge commit
Write-Host "Merging $BranchName into $MainBranch..." -ForegroundColor Cyan
Invoke-Git @("merge", "--no-ff", $BranchName, "-m", "merge: $BranchName into $MainBranch")

# Push merged main branch to origin
Write-Host "Pushing $MainBranch to origin..." -ForegroundColor Cyan
Invoke-Git @("push", "origin", $MainBranch)

# Check PR status and close if open
Write-Host "Checking PR #$PrNumber state..." -ForegroundColor Cyan
$prState = Get-GhOutput @("pr", "view", "$PrNumber", "--json", "state", "--jq", ".state")
if ([string]::IsNullOrWhiteSpace($prState)) {
  throw "Unable to read state for PR #$PrNumber."
}

if ($prState -eq "OPEN") {
  Write-Host "Closing PR #$PrNumber..." -ForegroundColor Cyan
  Invoke-Gh @("pr", "close", "$PrNumber", "--comment", "Closed by reset-branch automation after merge to $MainBranch.")
}
else {
  Write-Host "PR #$PrNumber is $prState. Skipping close." -ForegroundColor Yellow
}

# Delete the old feature branch locally
$branchExistsLocalAfterMerge = -not [string]::IsNullOrWhiteSpace((Get-GitOutput @("show-ref", "--verify", "refs/heads/$BranchName")))
if ($branchExistsLocalAfterMerge) {
  Write-Host "Deleting local branch $BranchName..." -ForegroundColor Cyan
  Invoke-Git @("branch", "-D", $BranchName)
}

# Delete the old feature branch from remote, handling if it's already gone
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

# Create a fresh feature branch from the updated main branch
Write-Host "Creating new local branch $BranchName from $MainBranch..." -ForegroundColor Cyan
Invoke-Git @("checkout", "-b", $BranchName, $MainBranch)

# Push the new feature branch to remote if not skipped
if (-not $SkipRemoteCreate) {
  Write-Host "Creating new remote branch $BranchName and setting upstream..." -ForegroundColor Cyan
  Invoke-Git @("push", "-u", "origin", $BranchName)
}

Write-Host "Completed. Current branch is now $BranchName." -ForegroundColor Green
