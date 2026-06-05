# Full release pipeline: publish app + build Inno Setup installer (if available)
param(
    [string]$Version = "1.0.0",
    [switch]$SkipPublish
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$publishScript = Join-Path $root "scripts\publish-release.ps1"
$issFile = Join-Path $root "installer\ExcelCreator.iss"

function Find-InnoSetup {
    $paths = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
        "${env:LocalAppData}\Programs\Inno Setup 6\ISCC.exe"
    )
    foreach ($p in $paths) {
        if (Test-Path $p) { return $p }
    }
    return $null
}

if (-not $SkipPublish) {
    & powershell -ExecutionPolicy Bypass -File $publishScript -Version $Version
}

$iscc = Find-InnoSetup
if (-not $iscc) {
    Write-Warning @"
Inno Setup 6 not found — installer EXE was NOT built.
Install Inno Setup: https://jrsoftware.org/isdl.php
Then run: .\scripts\build-installer.ps1 -SkipPublish

Portable ZIP is ready in dist\ for distribution.
"@
    exit 0
}

$distInstaller = Join-Path $root "dist\installer"
New-Item -ItemType Directory -Force -Path $distInstaller | Out-Null

Write-Host "Building installer with: $iscc"
& $iscc "/DMyAppVersion=$Version" $issFile

if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup compilation failed"
}

$setup = Get-ChildItem (Join-Path $distInstaller "ExcelCreator-Setup-*.exe") | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host ""
Write-Host "Installer ready for delivery:" -ForegroundColor Green
Write-Host "  $($setup.FullName)"
Write-Host "  Size: $([math]::Round($setup.Length / 1MB, 2)) MB"
