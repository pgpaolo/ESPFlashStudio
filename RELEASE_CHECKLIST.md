# Release checklist - 0.9.0-beta

1. Clean/rebuild solution in Visual Studio with zero errors/warnings relevant to the project.
2. Run portable tool installation from a clean `bin\Debug\net10.0-windows\Tools` directory.
3. Confirm `pio.exe`, Python and esptool are resolved only from the application tree.
4. Build a PlatformIO ESP32 project and verify `flash_args` offsets are reflected in the Flash grid.
5. Confirm `artifacts\<env>\latest`, `history\<timestamp>` and ZIP package are created.
6. Verify SHA-256 values in manifest against `certutil -hashfile <bin> SHA256`.
7. Test chip detection and erase on a disposable/test device.
8. Test flash with verify enabled.
9. Test backup-before-flash and verify the backup file is non-zero and readable.
10. Repeat on ESP8266 and, if available, ESP32-C3/S2/S3.
11. Test corporate proxy/TLS bootstrap on the intended enterprise network.
12. Test application after moving the whole folder to a different path.
13. Confirm crash logs and settings remain under the application `Data` folder.
14. Review third-party licenses before distributing bundled tool binaries.
