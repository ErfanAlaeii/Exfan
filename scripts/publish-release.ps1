# Publishes a self-contained win-x64 Release build to dist/app/
param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "src\ExcelCreator\ExcelCreator.csproj"
$distApp = Join-Path $root "dist\app"
$distRoot = Join-Path $root "dist"

function Find-DotNet {
    $candidates = @(
        "$env:ProgramFiles\dotnet\dotnet.exe",
        "${env:ProgramFiles(x86)}\dotnet\dotnet.exe",
        "$env:LOCALAPPDATA\Microsoft\dotnet\dotnet.exe"
    )
    foreach ($path in $candidates) {
        if (Test-Path $path) { return $path }
    }
    $inPath = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($inPath) { return $inPath.Source }
    return $null
}

$iconScript = Join-Path $root "scripts\create-icon.ps1"
if (Test-Path $iconScript) {
    & powershell -ExecutionPolicy Bypass -File $iconScript
}

$dotnet = Find-DotNet
if (-not $dotnet) {
    Write-Error @"
.NET 8 SDK not found.
Install from: https://dotnet.microsoft.com/download/dotnet/8.0
Then re-run: .\scripts\publish-release.ps1
"@
}

Write-Host "Using dotnet: $dotnet"
& $dotnet --version

# Stop a running portable copy so dist\app can be deleted fully.
Get-Process -Name "ExcelCreator" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

if (Test-Path $distApp) {
    Remove-Item $distApp -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $distRoot | Out-Null

Write-Host "Cleaning build outputs (removes stale copied templates) ..."
& $dotnet clean $project -c $Configuration -v q
if ($LASTEXITCODE -ne 0) { throw "dotnet clean failed" }

Write-Host "Publishing $project ..."
& $dotnet publish $project `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishReadyToRun=true `
    -p:Version=$Version `
    -p:InformationalVersion=$Version `
    -o $distApp

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

# Verify payload
$exe = Join-Path $distApp "ExcelCreator.exe"
$templates = Join-Path $distApp "Templates"
if (-not (Test-Path $exe)) { throw "Missing published executable: $exe" }
if (-not (Test-Path $templates)) { throw "Missing Templates folder in publish output" }

$sourceTemplates = Get-ChildItem (Join-Path $root "src\ExcelCreator\Templates\*.json")
$publishedTemplates = Get-ChildItem (Join-Path $templates "*.json")
if ($publishedTemplates.Count -ne $sourceTemplates.Count) {
    $names = $publishedTemplates.Name -join ", "
    throw "Template mismatch after publish. Source=$($sourceTemplates.Count) Published=$($publishedTemplates.Count). Files=$names. Close ExcelCreator.exe and rebuild."
}

# Portable ZIP for IT distribution without installer
$zipName = "ExcelCreator-$Version-win-x64-portable.zip"
$zipPath = Join-Path $distRoot $zipName
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path (Join-Path $distApp "*") -DestinationPath $zipPath -Force

# Manifest for inventory / deployment tools
$manifest = [ordered]@{
    product     = "ExcelCreator"
    displayName = "Excel Creator"
    version     = $Version
    platform    = "win-x64"
    selfContained = $true
    publishedAt = (Get-Date).ToString("o")
    files       = @{
        executable = "ExcelCreator.exe"
        portableZip = $zipName
    }
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $distRoot "manifest.json") -Encoding UTF8

Write-Host ""
Write-Host "Publish complete." -ForegroundColor Green
Write-Host "  App folder : $distApp"
Write-Host "  Portable   : $zipPath"
Write-Host "  Manifest   : $(Join-Path $distRoot 'manifest.json')"
