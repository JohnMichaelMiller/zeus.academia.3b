[CmdletBinding()]
param(
  [string]$Base = "main",
  [string]$Head = "",
  [string]$Title = "[AI] feat(shared-kernel): EP-0-1 Shared Kernel slice",
  [string]$BodyFile = "eng/pr-ep-0-1-shared-kernel.md",
  [string]$ChatId = "",
  [string]$Model = "openai/gpt-5.3-codex@unknown",
  [string]$Operator = "johnmillerATcodemag-com",
  [switch]$PrepareBody,
  [switch]$Push
)

$ErrorActionPreference = "Stop"

function Update-PrBodyPlaceholders {
  param(
    [Parameter(Mandatory = $true)]
    [string]$Path,
    [Parameter(Mandatory = $true)]
    [string]$ResolvedChatId,
    [Parameter(Mandatory = $true)]
    [string]$ResolvedModel,
    [Parameter(Mandatory = $true)]
    [string]$ResolvedOperator
  )

  if (-not (Test-Path $Path)) {
    throw "PR body file not found: $Path"
  }

  $datePath = (Get-Date).ToString("yyyy/MM/dd")
  $aiLog = "ai-logs/$datePath/$ResolvedChatId/conversation.md"
  $content = Get-Content -Path $Path -Raw

  $content = $content.Replace("update-with-current-chat-id", $ResolvedChatId)
  $content = $content.Replace("openai/gpt-5.3-codex@unknown", $ResolvedModel)
  $content = $content.Replace("johnmillerATcodemag-com", $ResolvedOperator)
  $content = $content.Replace("ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/conversation.md", $aiLog)

  Set-Content -Path $Path -Value $content -NoNewline
}

if ($null -eq (Get-Command git -ErrorAction SilentlyContinue)) {
  throw "Required command 'git' is not available on PATH."
}

if ($null -eq (Get-Command gh -ErrorAction SilentlyContinue)) {
  throw "Required command 'gh' is not available on PATH."
}

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
Set-Location $repoRoot

if ([string]::IsNullOrWhiteSpace($Head)) {
  $Head = (git rev-parse --abbrev-ref HEAD).Trim()
}

if ([string]::IsNullOrWhiteSpace($Head)) {
  throw "Could not determine current branch name."
}

if (-not (Test-Path $BodyFile)) {
  throw "PR body file not found: $BodyFile"
}

if ($PrepareBody) {
  $resolvedChatId = $ChatId

  if ([string]::IsNullOrWhiteSpace($resolvedChatId)) {
    $resolvedChatId = "$(Get-Date -Format 'yyyy-MM-dd')-ep-0-1-shared-kernel-pr"
  }

  Write-Host "Preparing PR body metadata placeholders..." -ForegroundColor Cyan
  Update-PrBodyPlaceholders -Path $BodyFile -ResolvedChatId $resolvedChatId -ResolvedModel $Model -ResolvedOperator $Operator
}

Write-Host "Using base branch: $Base" -ForegroundColor Cyan
Write-Host "Using head branch: $Head" -ForegroundColor Cyan
Write-Host "Using PR body file: $BodyFile" -ForegroundColor Cyan

if ($Push) {
  Write-Host "Pushing branch to origin..." -ForegroundColor Cyan
  git push -u origin $Head
}

Write-Host "Creating pull request..." -ForegroundColor Cyan
gh pr create --base $Base --head $Head --title $Title --body-file $BodyFile

Write-Host "Pull request URL:" -ForegroundColor Green
gh pr view --json url --jq .url
