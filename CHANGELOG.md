# Changelog

## 0.9.0-beta

- Added PlatformIO `flash_args` parsing for real firmware offsets.
- Added versioned artifact history and `latest` publishing.
- Added SHA-256 and size metadata in JSON and text manifests.
- Added automatic firmware ZIP packaging.
- Added optional flash backup and verify-after-flash.
- Added build/flash pre-flight logging.
- Added project/toolchain/artifact readiness dashboard.
- Added global crash logging under portable `Data/Logs/Crash`.
- Refined neutral enterprise UI and severity-aware logging.
- Added portable PlatformIO/Python/Arduino CLI/esptool/Git toolchain support.
- Added corporate TLS bootstrap compatibility handling.
- Corrected VB.NET case-insensitive naming collisions involving `System.IO.File`, `System.IO.Path` and `SHA256`.

## 0.3.x

- Introduced the neutral enterprise theme.
- Added artifact publishing.
- Added integrated tool bootstrap and local tool detection.
