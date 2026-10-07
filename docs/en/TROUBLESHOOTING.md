# Troubleshooting

## PlatformIO local executable not found

Confirm that the tool bootstrap completed and that the application-local PlatformIO environment contains its executable under `Tools/PlatformIO`.

Do not rely on a separately installed global `pio.exe` when portable mode is intended.

## TLS / certificate errors during bootstrap

Typical message:

```text
CERTIFICATE_VERIFY_FAILED
self-signed certificate in certificate chain
```

This commonly occurs on networks that perform HTTPS/TLS inspection.

Recommended order of resolution:

1. Use the organization-provided CA certificate chain with Python/requests where possible.
2. Confirm proxy settings and HTTPS reachability.
3. Use ESP Flash Studio's scoped bootstrap compatibility handling only when required.

The application does not need to change the global Windows certificate configuration.

## `truststore` bootstrap error

Older/partial bootstrap attempts can leave an incomplete PlatformIO virtual environment. Remove the incomplete application-local PlatformIO environment and re-run the current bootstrap implementation.

## Arduino CLI reports that the main sketch is missing

The selected folder is probably a PlatformIO project or does not contain a correctly named Arduino `.ino` sketch. For PlatformIO projects, select PlatformIO mode and ensure `platformio.ini` exists in the selected project root.

## `No module named esptool`

The local PlatformIO/Python environment is incomplete. Complete or repair the integrated tool installation before chip detection or raw flashing.

## Wrong COM port / port busy

Close serial monitors, IDE terminals or other programs that may have the selected COM port open. Refresh the port list and retry.

## Build succeeds but no firmware appears in `artifacts`

Check the log for artifact publishing messages and verify the selected PlatformIO environment. The application publishes from the corresponding `.pio/build/<environment>` output after a successful build.

## Flash layout looks wrong

Do not flash. Check the generated PlatformIO `flash_args`, the selected environment, board definition and partition configuration. ESP Flash Studio should prefer generated layout metadata when available.

## Character encoding looks incorrect in logs

Subprocess output is expected to use UTF-8 where possible. Some external tools can still emit text using a Windows code page. Include the raw failing command/output when reporting the issue.

## Crash logs

Unhandled application errors are written beneath the portable application data directory:

```text
Data/Logs/Crash/
```

Review the latest crash file together with the application log when reporting a defect.
