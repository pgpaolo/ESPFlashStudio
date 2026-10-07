# ESP Flash Studio

**ESP Flash Studio 0.9.0-beta** is a portable Windows desktop application written in **VB.NET / WinForms** for building, packaging, inspecting and flashing firmware for **ESP32** and **ESP8266** devices.

The project is designed around a simple principle: the toolchain can remain **local to the application directory**. PlatformIO, Python, Arduino CLI, esptool and Git can be bootstrapped into `Tools\` without requiring a system-wide installation or permanent PATH changes.

> Status: **Beta / release candidate**. Use a test board before relying on the application for production flashing workflows.

## Main features

- Portable local toolchain under `Tools\`.
- PlatformIO project detection from `platformio.ini`.
- Automatic PlatformIO environment discovery.
- PlatformIO build and upload workflows.
- Arduino CLI support for traditional Arduino sketches.
- ESP32 / ESP8266 raw BIN flashing through esptool.
- Device/chip detection.
- Flash erase support.
- PlatformIO `flash_args` parsing to obtain the actual generated flash layout when available.
- Firmware artifact publishing into a clean `artifacts\<environment>` tree.
- Versioned build history plus a `latest` snapshot.
- JSON and TXT firmware manifests.
- SHA-256 for published firmware binaries.
- Automatic ZIP firmware package after successful PlatformIO builds.
- Optional full-flash backup before programming.
- Optional verify-after-flash.
- Integrated structured/severity-aware log console.
- Portable settings and crash logs under `Data\`.
- Neutral enterprise-oriented WinForms UI.

## Repository layout

```text
ESPFlashStudio/
├─ ESPFlashStudio.slnx
├─ ESPFlashStudio/
│  ├─ ESPFlashStudio.vbproj
│  ├─ Program.vb
│  ├─ MainForm.vb
│  ├─ MainForm.Designer.vb
│  ├─ Models/
│  ├─ Services/
│  └─ Tools/
├─ docs/
├─ CHANGELOG.md
├─ LICENSE
├─ SECURITY.md
├─ THIRD_PARTY_NOTICES.md
└─ RELEASE_CHECKLIST.md
```

No machine-specific absolute paths are required by the source tree. Runtime locations are resolved relative to the application directory or to the project selected by the user.

## Portable runtime layout

After the tool bootstrap has completed, a deployed copy can look like this:

```text
ESPFlashStudio/
├─ ESPFlashStudio.exe
├─ Tools/
│  ├─ Python/
│  ├─ PlatformIO/
│  ├─ ArduinoCli/
│  ├─ Esptool/
│  └─ Git/
└─ Data/
   ├─ settings.json
   └─ Logs/
      └─ Crash/
```

The application resolves these paths from its own runtime directory and does not require the corresponding tools to be installed globally.

## Firmware artifact layout

For a PlatformIO environment named `myenv`, published firmware is stored beneath the selected firmware project:

```text
<firmware-project>/artifacts/myenv/
├─ latest/
│  ├─ firmware.bin
│  ├─ bootloader.bin
│  ├─ partitions.bin
│  ├─ ...
│  ├─ firmware-manifest.json
│  └─ firmware-manifest.txt
├─ history/
│  └─ YYYYMMDD_HHMMSS/
│     └─ ... immutable build snapshot ...
└─ ESPFlashStudio_myenv_YYYYMMDD_HHMMSS.zip
```

The Flash page uses the binaries published in `latest`. The history directory keeps timestamped build snapshots for traceability.

## Build requirements

- Windows 10/11 x64.
- A Visual Studio installation capable of targeting **.NET 10 Windows Desktop / WinForms**.
- .NET 10 SDK / Windows Desktop workload.
- Internet access is required only when downloading or updating local toolchain components/packages.

Build steps:

1. Clone or download this repository.
2. Open `ESPFlashStudio.slnx`.
3. Restore NuGet packages.
4. Build the solution.
5. Run the application.
6. Use **Install integrated tools** if the local toolchain is not already present.

## Toolchain bootstrap

ESP Flash Studio can bootstrap its local toolchain from upstream project sources. Downloaded third-party software is not committed to this repository.

The application and helper scripts keep the resulting toolchain beneath the application's `Tools\` directory. See [Portable Toolchain](docs/PORTABLE_TOOLCHAIN.md) for details.

## PlatformIO workflow

For a PlatformIO project:

1. Select the project root containing `platformio.ini`.
2. ESP Flash Studio discovers available environments.
3. Select the environment to build.
4. Build the project.
5. The application reads the generated PlatformIO flash layout when available.
6. Firmware binaries are copied into the artifact publishing tree.
7. A manifest and distributable ZIP package are generated.
8. Flash from the published artifact set.

See [Build, artifacts and flashing](docs/BUILD_FLASH_ARTIFACTS.md).

## Corporate TLS / HTTPS inspection

Some enterprise networks perform TLS inspection using an internal certificate authority. Portable Python environments do not always inherit the Windows certificate trust configuration in the same way as native Windows applications.

ESP Flash Studio contains a constrained bootstrap compatibility mode for PyPI access and PlatformIO package downloads. This behavior is scoped to subprocesses started by the application and does not modify global Windows settings.

For security-sensitive environments, using the organization's trusted CA chain is preferable to bypass-style compatibility options. See [Troubleshooting](docs/TROUBLESHOOTING.md).

## Safety notes

Flashing can erase device contents or make firmware temporarily unbootable. Verify the selected COM port, device family, flash layout and firmware package before programming.

Full flash backup and verification can be unavailable or behave differently when flash encryption, secure boot or other device security features are enabled.

## Documentation

- [Architecture](docs/ARCHITECTURE.md)
- [Portable Toolchain](docs/PORTABLE_TOOLCHAIN.md)
- [Build, artifacts and flashing](docs/BUILD_FLASH_ARTIFACTS.md)
- [Troubleshooting](docs/TROUBLESHOOTING.md)
- [Release checklist](RELEASE_CHECKLIST.md)
- [Third-party notices](THIRD_PARTY_NOTICES.md)
- [Security](SECURITY.md)
- [Changelog](CHANGELOG.md)

## License

ESP Flash Studio source code is released under the [MIT License](LICENSE).

Third-party tools downloaded or invoked by ESP Flash Studio remain subject to their respective upstream licenses.
