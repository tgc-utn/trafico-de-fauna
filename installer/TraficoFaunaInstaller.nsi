; NSIS - Instalador de Trafico de Fauna
;
; Requiere NSIS 3.02 o superior. Antes de compilar este script hay que compilar la solucion en Release:
; la carpeta src\Trafico.De.Fauna\bin\Release queda como la carpeta completa del juego y de ahi se empaqueta.
;
; Se instala por usuario y sin permisos de administrador. Solo el paso opcional de DirectX los pide.

Unicode True
SetCompressor /SOLID lzma

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "FileFunc.nsh"

;--------------------------------
;Definiciones

!define APP_NAME "Trafico de Fauna"
!define APP_ID "TraficoFauna"
!define APP_EXE "Trafico.De.Fauna.exe"
!define APP_PUBLISHER "TGC - UTN.BA - Educ.ar"
!define BUILD_DIR "..\src\Trafico.De.Fauna\bin\Release"
!define DIRECTX_DIR "DirectX"
!define UNINSTALL_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_ID}"

; Valor "Release" del registro a partir del cual esta instalado .NET Framework 4.8.1
!ifndef NETFX_481_RELEASE
  !define NETFX_481_RELEASE 533320
!endif
!define NETFX_481_URL "https://dotnet.microsoft.com/download/dotnet-framework/net481"

; La version del instalador se toma del ejecutable compilado
!getdllversion "${BUILD_DIR}\${APP_EXE}" APP_VERSION_
!define APP_VERSION "${APP_VERSION_1}.${APP_VERSION_2}.${APP_VERSION_3}"

;--------------------------------
;General

Name "${APP_NAME}"
OutFile "TraficoFaunaInstaller-${APP_VERSION}.exe"
InstallDir "$LOCALAPPDATA\Programs\${APP_ID}"
InstallDirRegKey HKCU "Software\${APP_ID}" "Install_Dir"
RequestExecutionLevel user

VIProductVersion "${APP_VERSION_1}.${APP_VERSION_2}.${APP_VERSION_3}.${APP_VERSION_4}"
VIAddVersionKey "ProductName" "${APP_NAME}"
VIAddVersionKey "CompanyName" "${APP_PUBLISHER}"
VIAddVersionKey "LegalCopyright" "(c) Educ.ar 2012"
VIAddVersionKey "FileDescription" "Instalador de ${APP_NAME}"
VIAddVersionKey "FileVersion" "${APP_VERSION}"
VIAddVersionKey "ProductVersion" "${APP_VERSION}"

;--------------------------------
;Interfaz

!define MUI_ABORTWARNING
!define MUI_ICON "..\src\Trafico.De.Fauna\Icon.ico"
!define MUI_UNICON "..\src\Trafico.De.Fauna\Icon.ico"
!define MUI_FINISHPAGE_RUN "$INSTDIR\${APP_EXE}"

;--------------------------------
;Paginas

!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Idiomas

!insertmacro MUI_LANGUAGE "Spanish"

;--------------------------------
;Secciones del instalador

Section "${APP_NAME}" SecGame

  SectionIn RO
  SetOverwrite on
  SetOutPath "$INSTDIR"

  ;Ejecutable y su configuracion (necesaria para cargar Managed DirectX en .NET 4)
  File "${BUILD_DIR}\${APP_EXE}"
  File "${BUILD_DIR}\${APP_EXE}.config"

  ;Dependencias
  File "${BUILD_DIR}\TGC.Core.dll"
  File "${BUILD_DIR}\BulletSharp.dll"
  File "${BUILD_DIR}\libbulletc.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.Direct3D.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.Direct3DX.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.DirectInput.dll"
  File "${BUILD_DIR}\Microsoft.DirectX.DirectSound.dll"

  ;Contenido del juego (incluye las fuentes, que se cargan desde Media\Fonts sin instalarlas en Windows)
  File /r /x "Thumbs.db" "${BUILD_DIR}\Media"
  File /r /x "Thumbs.db" "${BUILD_DIR}\Shaders"

  ;Carpeta de instalacion
  WriteRegStr HKCU "Software\${APP_ID}" "Install_Dir" "$INSTDIR"

  ;Desinstalador y entrada en "Agregar o quitar programas"
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

  ;El juego usa Managed DirectX, que necesita d3dx9_30.dll. Se instala con el redistribuible oficial,
  ;reducido a ese componente. DXSETUP pide permisos de administrador por si mismo.
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
;Descripciones de los componentes

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecGame} "Archivos del juego."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDirectX} "Biblioteca D3DX de DirectX 9, necesaria para el juego. Se marca sola si falta en esta PC. Pide permisos de administrador."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Crea un acceso directo en el escritorio."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenu} "Crea accesos directos en el menú Inicio."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Inicio del instalador

Function .onInit

  ;DirectX: solo se propone instalarlo si falta. En Windows de 64 bits $SYSDIR es SysWOW64 para este
  ;instalador de 32 bits, que es donde tiene que estar la DLL porque el juego es x86.
  ${If} ${FileExists} "$SYSDIR\d3dx9_30.dll"
    SectionGetFlags ${SecDirectX} $0
    IntOp $0 $0 & ~${SF_SELECTED}
    SectionSetFlags ${SecDirectX} $0
  ${EndIf}

  ;.NET Framework 4.8.1: no se empaqueta, se avisa si falta.
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  ${If} $0 < ${NETFX_481_RELEASE}
    MessageBox MB_YESNO|MB_ICONEXCLAMATION "${APP_NAME} necesita .NET Framework 4.8.1, que no esta instalado en esta PC.$\r$\n$\r$\nSe puede instalar el juego igual, pero no va a iniciar hasta que instales .NET Framework 4.8.1.$\r$\n$\r$\n¿Abrir la pagina de descarga de Microsoft?" /SD IDNO IDNO skipNetFx
      ExecShell "open" "${NETFX_481_URL}"
    skipNetFx:
  ${EndIf}

FunctionEnd

;--------------------------------
;Desinstalador

Section "Uninstall"

  ;Solo se borra lo que instalo este instalador
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

  ;Accesos directos
  Delete "$DESKTOP\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\Desinstalar ${APP_NAME}.lnk"
  RMDir "$SMPROGRAMS\${APP_NAME}"

  ;Registro
  DeleteRegKey HKCU "${UNINSTALL_KEY}"
  DeleteRegKey HKCU "Software\${APP_ID}"

SectionEnd
