# Instalador de Tráfico de Fauna

El instalador se genera con [NSIS](https://nsis.sourceforge.io/) a partir de `TraficoFaunaInstaller.nsi`.
En el repositorio no se versionan binarios, así que antes de generarlo hay que preparar lo siguiente.

## Requisitos

- [NSIS](https://sourceforge.net/projects/nsis/) 3.02 o superior.
- Visual Studio (o MSBuild) con .NET Framework 4.8.1 Developer Pack.
- Managed DirectX instalado en la PC donde se compila: las `Microsoft.DirectX*.dll` se copian desde ahí a la
  carpeta de salida. Se instala con el mismo redistribuible de DirectX del paso 2.

## Pasos

### 1. Compilar el juego en Release

Restaurar los paquetes NuGet y compilar `Trafico.De.Fauna.sln` en configuración `Release`.

La carpeta `Trafico.De.Fauna\bin\Release` queda como la carpeta completa del juego (ejecutable, DLLs, `Media` y
`Shaders`). El instalador empaqueta lo que encuentra ahí.

### 2. Preparar el redistribuible de DirectX

1. Descargar *DirectX End-User Runtimes (June 2010)*:
   <https://www.microsoft.com/en-us/download/details.aspx?id=8109>
2. Ejecutarlo y extraer su contenido en una carpeta temporal.
3. Copiar a `Installer\DirectX\` estos seis archivos:

   - `DXSETUP.exe`
   - `DSETUP.dll`
   - `dsetup32.dll`
   - `dxupdate.cab`
   - `dxdllreg_x86.cab`
   - `Apr2006_d3dx9_30_x86.cab`

El juego solo necesita `d3dx9_30.dll`, por eso alcanza con ese `.cab` y los archivos del setup.

### 3. Generar el instalador

Desde la carpeta `Installer`:

```
"C:\Program Files (x86)\NSIS\makensis.exe" /INPUTCHARSET UTF8 TraficoFaunaInstaller.nsi
```

El resultado es `Installer\TraficoFaunaInstaller-<versión>.exe`. La versión se toma del ejecutable compilado
(`AssemblyFileVersion` en `Trafico.De.Fauna\Properties\AssemblyInfo.cs`).

## Qué hace el instalador

- Instala por usuario en `%LOCALAPPDATA%\Programs\TraficoFauna`, sin permisos de administrador.
- Propone instalar la biblioteca D3DX de DirectX 9 solo si falta `d3dx9_30.dll`; ese paso sí pide permisos de administrador.
- Si falta .NET Framework 4.8.1 avisa y ofrece abrir la página de descarga; no lo incluye.
- No instala fuentes en Windows: el juego las carga desde `Media\Fonts`.
- Registra el desinstalador en "Agregar o quitar programas"; al desinstalar borra solo lo que instaló.

## Pendientes

### Firma de código

Sin firma, Smart App Control de Windows 11 puede bloquear el instalador y el juego, y SmartScreen muestra la
advertencia de editor desconocido. Hace falta un certificado de firma de código emitido por una autoridad
reconocida por Microsoft; uno autofirmado no sirve.

- **Binarios del juego (proyecto C#):** firmar `Trafico.De.Fauna.exe` y las DLL que se distribuyen después de
  compilar y antes de generar el instalador, por ejemplo con
  [SignTool](https://learn.microsoft.com/en-us/windows/win32/seccrypto/signtool).
- **Instalador (NSIS):** firmar `TraficoFaunaInstaller-<versión>.exe` una vez generado.
- **Desinstalador (NSIS):** se genera recién al instalar, así que hay que firmarlo desde el script. Ver
  [Signing an Uninstaller](https://nsis.sourceforge.io/Signing_an_Uninstaller); desde NSIS 3.08 se puede hacer
  directamente con `!uninstfinalize`.

Para investigar:

- [Code signing for Smart App Control](https://learn.microsoft.com/en-us/windows/apps/develop/smart-app-control/code-signing-for-smart-app-control):
  requisitos de Microsoft (firmar todos los binarios, firma RSA).
- [SignPath Foundation](https://signpath.org/): firma gratuita para proyectos de código abierto; exige que el
  build esté automatizado desde el repositorio.

## Referencias

### Descargas

- [DirectX End-User Runtimes (June 2010)](https://www.microsoft.com/en-us/download/details.aspx?id=8109):
  redistribuible de donde salen los seis archivos de `Installer\DirectX\`.
- [.NET Framework 4.8.1, instalador web](https://go.microsoft.com/fwlink/?LinkId=2203304) (`NDP481-Web.exe`, 1,5 MB).
- [.NET Framework 4.8.1, instalador offline](https://go.microsoft.com/fwlink/?linkid=2203305)
  (`NDP481-x86-x64-AllOS-ENU.exe`, 78 MB).

### NSIS

- [Documentación de NSIS](https://nsis.sourceforge.io/Main_Page).

### Contexto sobre DirectX 9 y Managed DirectX

- [Where is the DirectX SDK?](https://learn.microsoft.com/en-us/windows/win32/directx-sdk--august-2009-):
  el SDK de DirectX pasó a ser parte del Windows SDK; Managed DirectX y D3DX quedaron obsoletos.
- [Where is the DirectX SDK (2021 Edition)?](https://walbourn.github.io/where-is-the-directx-sdk-2021-edition/):
  actualización de lo anterior; los instaladores antiguos siguen disponibles, re-firmados.
- [DirectX and .NET](https://walbourn.github.io/directx-and-net/): estado de Managed DirectX 1.1. Es solo de
  32 bits, por eso el juego se compila como x86.
- [Not So Direct Setup](https://walbourn.github.io/not-so-direct-setup/): `DXSETUP` ya no instala DirectX sino
  componentes opcionales; conviene incluir solo los `.cab` necesarios, que es lo que hace este instalador.
- [DXSETUP Update](https://walbourn.github.io/dxsetup-update/): versión corregida de `DXSETUP` (abril 2011), que
  arregla un error del modo silencioso. Es la que trae la descarga actual.
