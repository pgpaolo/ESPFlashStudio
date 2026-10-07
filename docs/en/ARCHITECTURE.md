# Architecture

ESP Flash Studio is a VB.NET WinForms application targeting .NET 10 for Windows.

## Main layers

### UI

- `MainForm.vb` contains application event handling and workflow orchestration.
- `MainForm.Designer.vb` contains the Visual Studio WinForms Designer-generated/control declarations.
- `UiTheme.vb` centralizes the visual theme.

### Models

- `AppSettings.vb` - persisted application configuration.
- `BoardProfile.vb` - board/profile metadata.
- `FlashItem.vb` - a firmware binary plus its flash offset and related metadata.

### Services

- `AppPaths.vb` - resolves application-local runtime paths.
- `ToolLocator.vb` - locates the portable local toolchain.
- `ToolBootstrapService.vb` - downloads/prepares local toolchain components.
- `ProjectDetector.vb` - identifies PlatformIO/Arduino project characteristics.
- `BuildService.vb` - build execution, flash layout extraction and artifact publishing.
- `FlashService.vb` - chip detection, erase, backup, write and verification workflows.
- `ProcessRunner.vb` - subprocess execution and output streaming.
- `SettingsService.vb` - settings persistence.

## Portability model

The application directory is the root of the local runtime environment. Tool paths are not based on developer-machine locations. Runtime data is intentionally separated from source code:

```text
<app>/Tools/
<app>/Data/
```

A selected firmware project remains external to the application. Its generated firmware publications are stored beneath that project in `artifacts/`.

## Build flow

```text
Select project
    ↓
Detect engine
    ↓
Discover PlatformIO environments / Arduino sketch
    ↓
Pre-flight checks
    ↓
Run build tool
    ↓
Locate generated binaries
    ↓
Read PlatformIO flash_args when available
    ↓
Publish latest + history
    ↓
Create SHA-256 manifest
    ↓
Create firmware ZIP
```

## Flash flow

```text
Select device / COM port
    ↓
Pre-flight checks
    ↓
Optional flash backup
    ↓
Write binary set using discovered offsets
    ↓
Optional verification
    ↓
Report result in structured log
```

## Design considerations

- Paths are resolved at runtime rather than hard-coded.
- PlatformIO-generated flash information is preferred to assumed offsets.
- Artifact publishing separates build-system internals from distributable firmware.
- Subprocess output is streamed to the application log for diagnostics.
- Portable tooling is favored over global installation dependencies.
