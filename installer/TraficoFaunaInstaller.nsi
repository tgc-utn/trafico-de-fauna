; NSIS - Trafico de Fauna installer

!if 0x3013000 > "${NSIS_PACKEDVERSION}"
  !error "NSIS 3.13 or higher is required to build this installer!"
!endif

Unicode True
SetCompressor /SOLID lzma

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "FileFunc.nsh"

;--------------------------------
;Definitions

!define APP_NAME "Trafico de Fauna"
!define APP_ID "TraficoFauna"
!define APP_EXE "Trafico.De.Fauna.exe"
!define APP_PUBLISHER "TGC - UTN.BA - Educ.ar"
!define BUILD_DIR "..\src\Trafico.De.Fauna\bin\Release"
!define DIRECTX_DIR "DirectX"
!define UNINSTALL_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_ID}"

; Minimum registry "Release" value at which .NET Framework 4.8.1 is installed
!ifndef NETFX_481_RELEASE
  !define NETFX_481_RELEASE 533320
!endif
!define NETFX_481_URL "https://dotnet.microsoft.com/download/dotnet-framework/net481"

; The installer version is taken from the compiled executable
!getdllversion "${BUILD_DIR}\${APP_EXE}" APP_VERSION_
!define APP_VERSION "${APP_VERSION_1}.${APP_VERSION_2}.${APP_VERSION_3}.${APP_VERSION_4}"

;--------------------------------
;General

Name "${APP_NAME}"
OutFile "TraficoFaunaInstaller-${APP_VERSION}.exe"
InstallDir "$LOCALAPPDATA\Programs\${APP_ID}"
InstallDirRegKey HKCU "Software\${APP_ID}" "Install_Dir"
RequestExecutionLevel user

VIProductVersion "${APP_VERSION}"
VIAddVersionKey "ProductName" "${APP_NAME}"
VIAddVersionKey "CompanyName" "${APP_PUBLISHER}"
VIAddVersionKey "LegalCopyright" "(c) Educ.ar 2012"
VIAddVersionKey "FileDescription" "Instalador de ${APP_NAME}"
VIAddVersionKey "FileVersion" "${APP_VERSION}"
VIAddVersionKey "ProductVersion" "${APP_VERSION}"

;--------------------------------
;Interface

!define MUI_ABORTWARNING
!define MUI_ICON "..\src\Trafico.De.Fauna\Icon.ico"
!define MUI_UNICON "..\src\Trafico.De.Fauna\Icon.ico"
!define MUI_FINISHPAGE_RUN "$INSTDIR\${APP_EXE}"

;--------------------------------
;Pages

!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "Spanish"

;--------------------------------
;Installer sections

Section "${APP_NAME}" SecGame

  SectionIn RO
  SetOverwrite on
  SetOutPath "$INSTDIR"

  ;Executable and its configuration (required to load Managed DirectX on .NET 4)
  File "${BUILD_DIR}\${APP_EXE}"
  File "${BUILD_DIR}\${APP_EXE}.config"

  ;Dependencies
  File "${BUILD_DIR}\TGC.Core.dll"
  File "${BUILD_DIR}\BulletSharp.dll"
  File "${BUILD_DIR}\libbulletc.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.Direct3D.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.Direct3DX.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.DirectInput.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.DirectSound.dll"

  ;Assets and shaders
  File /r "${BUILD_DIR}\Media"
  File /r "${BUILD_DIR}\Shaders"

  ;Install folder
  WriteRegStr HKCU "Software\${APP_ID}" "Install_Dir" "$INSTDIR"

  ;Uninstaller and "Add or remove programs" entry
  WriteUninstaller "$INSTDIR\uninstall.exe"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "DisplayName" "${APP_NAME}"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "DisplayVersion" "${APP_VERSION}"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "Publisher" "${APP_PUBLISHER}"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "DisplayIcon" "$INSTDIR\${APP_EXE}"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNINSTALL_KEY}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKCU "${UNINSTALL_KEY}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
  WriteRegDWORD HKCU "${UNINSTALL_KEY}" "NoModify" 1
  WriteRegDWORD HKCU "${UNINSTALL_KEY}" "NoRepair" 1
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKCU "${UNINSTALL_KEY}" "EstimatedSize" "$0"

SectionEnd

Section "DirectX 9 D3DX" SecDirectX

  ;The game uses Managed DirectX, which requires d3dx9_30.dll. It is installed with the official redistributable,
  ;reduced to that component. When it runs, DXSETUP asks for administrator rights,
  ;so the rest of the installer does not need them.
  InitPluginsDir
  SetOutPath "$PLUGINSDIR\DirectX"
  File "${DIRECTX_DIR}\DXSETUP.exe"
  File "${DIRECTX_DIR}\DSETUP.dll"
  File "${DIRECTX_DIR}\dsetup32.dll"
  File "${DIRECTX_DIR}\dxupdate.cab"
  File "${DIRECTX_DIR}\dxdllreg_x86.cab"
  File "${DIRECTX_DIR}\Apr2006_d3dx9_30_x86.cab"

  DetailPrint "Instalando la biblioteca D3DX de DirectX 9..."
  ExecShellWait "" "$PLUGINSDIR\DirectX\DXSETUP.exe" "/silent"
  SetOutPath "$INSTDIR"

  ${IfNot} ${FileExists} "$SYSDIR\d3dx9_30.dll"
    MessageBox MB_OK|MB_ICONEXCLAMATION "No se pudo instalar la biblioteca D3DX de DirectX 9. El juego no va a funcionar hasta que se instale: volve a ejecutar este instalador y acepta el pedido de permisos de administrador."
  ${EndIf}

SectionEnd

Section "Acceso en el escritorio" SecDesktop

  SetOutPath "$INSTDIR"
  CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"

SectionEnd

Section "Accesos en menú Inicio" SecStartMenu

  SetOutPath "$INSTDIR"
  CreateDirectory "$SMPROGRAMS\${APP_NAME}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\Desinstalar ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

SectionEnd

;--------------------------------
;Component descriptions

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecGame} "Archivos del juego."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDirectX} "Biblioteca D3DX de DirectX 9, necesaria para el juego. Se marca sola si falta en esta PC. Pide permisos de administrador."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Crea un acceso directo en el escritorio."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenu} "Crea accesos directos en el menú Inicio."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Installer startup

Function .onInit

  ;DirectX: only offered if missing. On 64-bit Windows, $SYSDIR is SysWOW64 for this 32-bit installer,
  ;which is where the DLL must be because the game is x86.
  ${If} ${FileExists} "$SYSDIR\d3dx9_30.dll"
    SectionGetFlags ${SecDirectX} $0
    IntOp $0 $0 & ~${SF_SELECTED}
    SectionSetFlags ${SecDirectX} $0
  ${EndIf}

  ;.NET Framework 4.8.1: not bundled; a warning is shown if it is missing.
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  ${If} $0 < ${NETFX_481_RELEASE}
    MessageBox MB_YESNO|MB_ICONEXCLAMATION "${APP_NAME} necesita .NET Framework 4.8.1, que no esta instalado en esta PC.$\r$\n$\r$\nSe puede instalar el juego igual, pero no va a iniciar hasta que instales .NET Framework 4.8.1.$\r$\n$\r$\n¿Abrir la pagina de descarga de Microsoft?" /SD IDNO IDNO skipNetFx
      ExecShell "open" "${NETFX_481_URL}"
    skipNetFx:
  ${EndIf}

FunctionEnd

;--------------------------------
;Uninstaller

Section "Uninstall"

  ;Only delete what this installer installed
  Delete "$INSTDIR\${APP_EXE}"
  Delete "$INSTDIR\${APP_EXE}.config"
  Delete "$INSTDIR\TGC.Core.dll"
  Delete "$INSTDIR\BulletSharp.dll"
  Delete "$INSTDIR\libbulletc.dll"
  Delete "$INSTDIR\Microsoft.DirectX.dll"
  Delete "$INSTDIR\Microsoft.DirectX.Direct3D.dll"
  Delete "$INSTDIR\Microsoft.DirectX.Direct3DX.dll"
  Delete "$INSTDIR\Microsoft.DirectX.DirectInput.dll"
  Delete "$INSTDIR\Microsoft.DirectX.DirectSound.dll"
  RMDir /r "$INSTDIR\Media"
  RMDir /r "$INSTDIR\Shaders"
  Delete "$INSTDIR\uninstall.exe"
  RMDir "$INSTDIR"

  ;Shortcuts
  Delete "$DESKTOP\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\Desinstalar ${APP_NAME}.lnk"
  RMDir "$SMPROGRAMS\${APP_NAME}"

  ;Registry
  DeleteRegKey HKCU "${UNINSTALL_KEY}"
  DeleteRegKey HKCU "Software\${APP_ID}"

SectionEnd
