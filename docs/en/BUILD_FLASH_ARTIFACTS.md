# Build, Artifacts and Flashing

## PlatformIO projects

A PlatformIO project is identified by a `platformio.ini` file in the selected root directory.

ESP Flash Studio reads the declared environments and invokes the local PlatformIO Core for the selected environment.

## Arduino projects

Arduino CLI mode expects a valid Arduino sketch layout. The primary `.ino` file must exist according to Arduino CLI project rules.

## Build output vs published artifacts

PlatformIO generates technical build output beneath `.pio/build/<environment>`. ESP Flash Studio treats this as an internal build-system directory.

After a successful build, firmware intended for flashing/distribution is copied to:

```text
<project>/artifacts/<environment>/latest/
```

A timestamped snapshot is also created under:

```text
<project>/artifacts/<environment>/history/YYYYMMDD_HHMMSS/
```

## Flash layout

When PlatformIO generates a `flash_args` file, ESP Flash Studio parses it to determine the actual offset-to-binary mapping. This is preferred over static assumptions because ESP32 variants and project partition/layout settings can differ.

If generated layout metadata is unavailable, compatibility fallbacks may be used. Review the Flash grid before writing a device.

## Firmware manifest

The published artifact set includes a human-readable TXT manifest and a JSON manifest. Metadata includes relevant firmware files, offsets, sizes and SHA-256 hashes.

SHA-256 allows a firmware package to be checked for accidental modification after build/export.

## ZIP package

A ZIP package is generated for successful PlatformIO builds. It is intended to make distribution of one known build simpler while preserving the associated manifest.

## Backup before flash

When enabled, ESP Flash Studio asks esptool to read device flash before programming. The backup is stored under the project's artifact area.

Backup success depends on the device configuration. Secure boot, flash encryption and other security features can affect readback behavior and the usefulness of a raw flash image.

## Verify after flash

When enabled, a verification pass is performed after programming using the same selected binary set and offsets.

Verification is useful for catching communication/write errors but does not replace application-level functional validation after reboot.

## Recommended release workflow

1. Clean build.
2. Inspect the detected flash layout.
3. Review generated manifests.
4. Validate SHA-256 if the package will be redistributed.
5. Program a test board.
6. Run verify-after-flash.
7. Perform a functional firmware test.
8. Distribute the generated ZIP artifact.
