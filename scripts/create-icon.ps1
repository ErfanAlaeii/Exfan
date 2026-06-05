# Generates assets/app.ico for the application and installer.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$assetsDir = Join-Path $root 'assets'
$icoPath = Join-Path $assetsDir 'app.ico'
New-Item -ItemType Directory -Force -Path $assetsDir | Out-Null

Add-Type -AssemblyName System.Drawing

function New-ExfanBitmap {
    param(
        [int]$Size,
        [single]$FontSize
    )

    $bmp = New-Object System.Drawing.Bitmap $Size, $Size
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
    $g.Clear([System.Drawing.Color]::FromArgb(37, 99, 235))

    $brush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::White)
    $font = New-Object System.Drawing.Font ('Segoe UI', $FontSize, [System.Drawing.FontStyle]::Bold)
    $format = New-Object System.Drawing.StringFormat
    $format.Alignment = [System.Drawing.StringAlignment]::Center
    $format.LineAlignment = [System.Drawing.StringAlignment]::Center
    $rect = New-Object System.Drawing.RectangleF 0, 0, $Size, $Size
    $g.DrawString('Exfan', $font, $brush, $rect, $format)

    $g.Dispose()
    $font.Dispose()
    $brush.Dispose()

    return $bmp
}

# Primary high-resolution icon; Windows scales down for taskbar and shortcuts.
$size = 256
$bmp = New-ExfanBitmap -Size $size -FontSize 52
try {
    $icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon())
    $stream = [System.IO.File]::Create($icoPath)
    try {
        $icon.Save($stream)
    }
    finally {
        $stream.Close()
        $icon.Dispose()
    }
}
finally {
    $bmp.Dispose()
}

Write-Host "Created $icoPath"
