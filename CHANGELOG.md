# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added

- `CHANGELOG.md`.
- `LICENSE`.
- `D` key to toggle debug mode, only in Debug builds.
- GitHub Actions workflows.

### Changed

- Upgraded from .NET Framework 2.0 to 4.8.1.
- Migrated from the monolithic `TgcViewer` to the `TGC.Core` 2.5.1 NuGet package.
- Turned the game into an application instead of a DLL loaded by the viewer.
- Moved game code and assets to `src\Trafico.De.Fauna\` (previously `Examples\TraficoFauna\`).
- Migrated the solution from `.sln` to `.slnx`, with `x86` as its only platform.
- Split textures by sector; they were previously shared by the whole level.
- Changed game logic to run at a fixed rate, independent of the frame rate.
- Migrated the documentation from the original PDF to a Docsify website and updated it.
- Replaced `README.txt` with `README.md`.
- Reorganized the error window.
- Upgraded the installer from NSIS 2.46 to NSIS 3.13.
- Removed the administrator rights requirement from the installer, except for the optional DirectX step.
- Changed the default install folder to `%LOCALAPPDATA%\Programs\TraficoFauna` (previously `C:\TraficoFauna`).
- Reduced the installer size from 181 MB to 12 MB.
- Reduced the DirectX step to the missing DirectX 9 component instead of the full redistributable.
- Removed the bundled .NET Framework installer; the installer warns if it is missing.

### Fixed

- The character now moves correctly on modern PCs, since game logic no longer depends on the frame rate.
- Guards' field of view: they saw about 48 degrees to each side instead of the configured 45.

## [1.0.0.0] - 2012-04-18

Original version developed by Educ.ar S.E. Its source code was recovered and published in this repository on
2023-05-20.

[Unreleased]: https://github.com/tgc-utn/trafico-de-fauna/compare/1.0.0.0...HEAD
[1.0.0.0]: https://github.com/tgc-utn/trafico-de-fauna/releases/tag/1.0.0.0
