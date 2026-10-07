# Security policy

## Firmware flashing risk

ESP Flash Studio performs operations that can erase or rewrite microcontroller flash. Use the software only on devices you are authorized to manage.

Always verify the selected device, COM port, flash layout and firmware files before writing.

## Secure boot / flash encryption

ESP32 security features such as Secure Boot and Flash Encryption can change the behavior of backup, readback and verification operations. A raw backup from an encrypted device may not be suitable for restoration to another device.

ESP Flash Studio does not bypass device security mechanisms.

## Toolchain downloads

Third-party tools are downloaded from their upstream distribution sources. Some helper scripts verify published SHA-256 values for selected binary archives. PlatformIO itself subsequently downloads platform packages/toolchains as required by projects.

For controlled enterprise deployment, consider pre-validating and internally mirroring required third-party packages.

## Corporate TLS inspection

Compatibility handling for TLS-inspecting environments is scoped to child processes and is not intended as a replacement for proper CA trust management. Where possible, configure the enterprise CA chain explicitly.

## Sensitive data

The repository intentionally contains no developer-specific absolute paths, credentials or machine-specific configuration. Before attaching logs to issues, review them for project paths, Wi-Fi credentials, tokens or other information emitted by the firmware/build process.

## Reporting a vulnerability

Please open a GitHub issue only for non-sensitive security concerns. For a vulnerability that should not be disclosed publicly, use GitHub's private vulnerability reporting feature if enabled for the repository.
