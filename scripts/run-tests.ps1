# Run unit and performance tests with optional coverage.
param(
    [switch]$PerformanceOnly,
    [switch]$UnitOnly,
    [switch]$Coverage
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $root "tests\ExcelCreator.Tests\ExcelCreator.Tests.csproj"

$filter = ""
if ($PerformanceOnly) { $filter = '--filter "Category=Performance"' }
elseif ($UnitOnly) { $filter = '--filter "Category!=Performance"' }

$coverageArg = ""
if ($Coverage) {
    $coverageArg = '--collect:"XPlat Code Coverage" --results-directory (Join-Path $root "TestResults")'
}

Write-Host "Running tests..." -ForegroundColor Cyan
Push-Location $root
try {
    $cmd = "dotnet test `"$testProject`" -c Release --verbosity normal $filter $coverageArg"
    Invoke-Expression $cmd
    if ($LASTEXITCODE -ne 0) { throw "Tests failed with exit code $LASTEXITCODE" }
    Write-Host "All tests passed." -ForegroundColor Green
}
finally {
    Pop-Location
}
