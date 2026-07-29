param(
    [switch]$PrepareBody,
    [switch]$Push
)

$ErrorActionPreference = 'Stop'

$bodyFile = Join-Path $PSScriptRoot 'pr-ep-0-1-shared-kernel.md'

if ($PrepareBody -and -not (Test-Path $bodyFile)) {
    throw "PR body file not found: $bodyFile"
}

if ($Push) {
    git push -u origin Part-Eight
}

gh pr create --title '[AI] feat(shared-kernel): EP-0-1 Shared Kernel slice' --body-file $bodyFile --base main --head Part-Eight