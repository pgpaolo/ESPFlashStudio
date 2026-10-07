# Contributing

Contributions and bug reports are welcome.

## Development requirements

- Windows
- Visual Studio with .NET 10 Windows Desktop / WinForms support
- VB.NET familiarity
- An ESP32/ESP8266 test device for flash-related changes

## Before submitting changes

1. Build with `Option Strict On` and `Option Explicit On` without introducing new compiler errors.
2. Keep runtime paths portable; do not commit absolute user/machine paths.
3. Do not commit downloaded third-party toolchains or firmware binaries.
4. Test PlatformIO project detection and at least one build workflow.
5. For flashing changes, test on disposable/development hardware.
6. Update documentation when behavior or artifact layout changes.

## Coding notes

VB.NET identifiers are case-insensitive. Avoid local variables or methods whose names shadow framework types such as `File`, `Path` or `SHA256`.

Prefer fully qualified framework types in code paths where naming ambiguity is possible.
