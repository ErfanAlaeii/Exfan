# Enterprise deployment — Excel Creator (سازنده اکسل خالق: عرفان)

This guide is for **IT / build engineers** who produce the installer and roll it out to users.

## Deliverables (what users receive)

| Artifact | Path after build | Use case |
|----------|------------------|----------|
| **Setup EXE (recommended)** | `dist/installer/ExcelCreator-Setup-1.0.0-win-x64.exe` | Standard install: Program Files, Start menu, uninstaller |
| **Portable ZIP** | `dist/ExcelCreator-1.0.0-win-x64-portable.zip` | USB / locked-down PCs without installer rights |
| **Manifest** | `dist/manifest.json` | SCCM / Intune inventory metadata |

End-user guide (Persian): `docs/INSTALL-fa.txt` (copied into install folder).

## Build machine requirements

1. **Windows 10/11 x64**
2. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** — to compile and publish
3. **[Inno Setup 6](https://jrsoftware.org/isdl.php)** — to build `Setup.exe` (optional; ZIP still works without it)

## One-command release build

```powershell
cd C:\Users\P30LAPTOP\Desktop\excelcreator
.\scripts\build-installer.ps1
```

Steps performed:

1. `scripts\create-icon.ps1` — application icon
2. `scripts\publish-release.ps1` — self-contained **win-x64** publish to `dist\app\` (no .NET runtime required on target PCs)
3. Inno Setup — packages `dist\app\` into `dist\installer\ExcelCreator-Setup-*.exe`

Publish only (no installer):

```powershell
.\scripts\publish-release.ps1
```

Rebuild installer after publish:

```powershell
.\scripts\build-installer.ps1 -SkipPublish
```

Bump version:

```powershell
.\scripts\build-installer.ps1 -Version 1.1.0
```

Also update `installer/ExcelCreator.iss` `#define MyAppVersion` and `src/ExcelCreator/ExcelCreator.csproj` `<Version>`.

## Target PC requirements (users)

- Windows 10/11 **64-bit**
- **No .NET installation** required (self-contained)
- **Microsoft Excel** optional — only to open generated `.xlsx` files
- ~150–200 MB disk space after install

## Enterprise rollout options

### A. Manual / email

Distribute `ExcelCreator-Setup-1.0.0-win-x64.exe`. Users run as administrator (or per-machine install with elevated rights).

### B. SCCM / ConfigMgr

1. Deploy the Setup EXE as an **application**
2. Install command (silent):

   ```text
   ExcelCreator-Setup-1.0.0-win-x64.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-
   ```

3. Detection: file exists `%ProgramFiles%\Excel Creator\ExcelCreator.exe`  
   (folder name is English `Excel Creator` in Inno `DefaultDirName`)
4. Uninstall: `%ProgramFiles%\Excel Creator\unins000.exe` /VERYSILENT

### C. Microsoft Intune (Win32 app)

1. Wrap `ExcelCreator-Setup-1.0.0-win-x64.exe` with [Microsoft Win32 Content Prep Tool](https://github.com/Microsoft/Microsoft-Desktop-Optimization-Package)
2. Install command: same silent switches as above
3. Detection rule: file `ExcelCreator.exe` under install directory

### D. Portable (no admin)

Extract `ExcelCreator-1.0.0-win-x64-portable.zip` to `C:\Tools\ExcelCreator\` and shortcut `ExcelCreator.exe`.

## Code signing (recommended for enterprise)

Unsigned EXE may trigger SmartScreen. Sign both:

- `dist\installer\ExcelCreator-Setup-*.exe`
- `dist\app\ExcelCreator.exe`

Example (adjust certificate thumbprint):

```powershell
signtool sign /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 `
  /sha1 YOUR_CERT_THUMBPRINT `
  dist\installer\ExcelCreator-Setup-1.0.0-win-x64.exe
```

## Customization after install

| Item | Location |
|------|----------|
| User settings | `%AppData%\ExcelCreator\settings.json` |
| Templates (JSON) | `{InstallDir}\Templates\` |
| Install guide | `{InstallDir}\docs\INSTALL-fa.txt` |

Add company templates by placing JSON files in `Templates\` and restarting the app.

## Troubleshooting

| Issue | Action |
|-------|--------|
| Build: `dotnet` not found | Install .NET 8 SDK, restart terminal |
| Build: no Setup EXE | Install Inno Setup 6, rerun `build-installer.ps1` |
| App won't start | Ensure win-x64; install [VC++ Redistributable](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist) if needed |
| Templates missing | Reinstall; verify `Templates` folder next to `ExcelCreator.exe` |

## Security notes

- App runs **locally**; no cloud dependency
- Generated files are saved where the user chooses
- Install requires **admin** for per-machine Program Files install (`PrivilegesRequired=admin` in Inno script)

For per-user install without admin, change Inno `PrivilegesRequired` to `lowest` and rebuild (install path will be under `%LocalAppData%`).
