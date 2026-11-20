# Tráfico de Fauna installer

The installer is built with [NSIS](https://nsis.sourceforge.io/) from `TraficoFaunaInstaller.nsi`.
Binaries are not versioned in the repository, so the following must be prepared before building it.

## Requirements

- [NSIS](https://sourceforge.net/projects/nsis/) 3.13 or later.
- Visual Studio (or MSBuild) with the .NET Framework 4.8.1 Developer Pack.
- Managed DirectX installed on the build PC: the `Microsoft.DirectX*.dll` files are copied from there to the
  output folder. It is installed with the same DirectX redistributable as in step 2.

## Steps

### 1. Build the game in Release

Restore the NuGet packages and build `Trafico.De.Fauna.slnx` in the `Release` configuration.

The `src\Trafico.De.Fauna\bin\Release` folder becomes the complete game folder (executable, DLLs, `Media` and
`Shaders`). The installer packages what it finds there.

### 2. Prepare the DirectX redistributable

1. Download *DirectX End-User Runtimes (June 2010)*:
   <https://www.microsoft.com/en-us/download/details.aspx?id=8109>
2. Run it and extract its contents to a temporary folder.
3. Copy these six files to `installer\DirectX\`:

   - `DXSETUP.exe`
   - `DSETUP.dll`
   - `dsetup32.dll`
   - `dxupdate.cab`
   - `dxdllreg_x86.cab`
   - `Apr2006_d3dx9_30_x86.cab`

The game only needs `d3dx9_30.dll`, so that `.cab` plus the setup files are enough.

### 3. Build the installer

From the `installer` folder:

```
"C:\Program Files (x86)\NSIS\makensis.exe" /INPUTCHARSET UTF8 TraficoFaunaInstaller.nsi
```

The result is `installer\TraficoFaunaInstaller-<version>.exe`. The version is taken from the compiled executable
(`AssemblyFileVersion` in `src\Trafico.De.Fauna\Properties\AssemblyInfo.cs`).

## What the installer does

- Installs per user in `%LOCALAPPDATA%\Programs\TraficoFauna`, without administrator rights.
- Offers to install the DirectX 9 D3DX library only if `d3dx9_30.dll` is missing; that step does ask for
  administrator rights.
- If .NET Framework 4.8.1 is missing, it warns and offers to open the download page; it is not bundled.
- Registers the uninstaller in "Add or remove programs"; uninstalling removes only what it installed.

## To do

### Code signing

Without a signature, Windows 11 Smart App Control may block the installer and the game, and SmartScreen shows the
unknown publisher warning. A code signing certificate issued by an authority trusted by Microsoft is required; a
self-signed one does not work.

- **Game binaries:** sign `Trafico.De.Fauna.exe` and the distributed DLLs after building and before generating
  the installer, for example with
  [SignTool](https://learn.microsoft.com/en-us/windows/win32/seccrypto/signtool).
- **Installer (NSIS):** sign `TraficoFaunaInstaller-<version>.exe` once generated.
- **Uninstaller (NSIS):** it is generated at install time, so it must be signed from the script. Since NSIS 3.08
  [`!uninstfinalize`](https://nsis.sourceforge.io/Signing_an_Uninstaller) can be used directly.

To investigate:

- [Code signing for Smart App Control](https://learn.microsoft.com/en-us/windows/apps/develop/smart-app-control/code-signing-for-smart-app-control):
  Microsoft's requirements (sign every binary, RSA signature).
- [SignPath Foundation](https://signpath.org/): free signing for open source projects; requires the build to be
  automated from the repository.

## References

### Downloads

- [DirectX End-User Runtimes (June 2010)](https://www.microsoft.com/en-us/download/details.aspx?id=8109):
  redistributable the six files in `installer\DirectX\` come from.
- [.NET Framework 4.8.1, web installer](https://go.microsoft.com/fwlink/?LinkId=2203304) (`NDP481-Web.exe`, 1.5 MB).
- [.NET Framework 4.8.1, offline installer](https://go.microsoft.com/fwlink/?linkid=2203305)
  (`NDP481-x86-x64-AllOS-ENU.exe`, 78 MB).

### Background on DirectX 9 and Managed DirectX

- [Where is the DirectX SDK?](https://learn.microsoft.com/en-us/windows/win32/directx-sdk--august-2009-):
  the DirectX SDK became part of the Windows SDK; Managed DirectX and D3DX were deprecated.
- [Where is the DirectX SDK (2021 Edition)?](https://walbourn.github.io/where-is-the-directx-sdk-2021-edition/):
  update of the above; the legacy installers are still available, re-signed.
- [DirectX and .NET](https://walbourn.github.io/directx-and-net/): status of Managed DirectX 1.1. It is 32-bit
  only, which is why the game is built as x86.
- [Not So Direct Setup](https://walbourn.github.io/not-so-direct-setup/): `DXSETUP` no longer installs DirectX
  but optional components; only the required `.cab` files should be included, which is what this installer does.
- [DXSETUP Update](https://walbourn.github.io/dxsetup-update/): fixed version of `DXSETUP` (April 2011) that fixes
  a silent mode bug. It is the one included in the current download.
