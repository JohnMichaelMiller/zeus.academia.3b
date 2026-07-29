param()

$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot '..\tests\Features\SharedKernel\Foundation\Zeus.Academia.Tests.Features.SharedKernel.Foundation.csproj'

Write-Host 'Running Shared Kernel SQL Server verification tests...'
dotnet test $project --configuration Debug