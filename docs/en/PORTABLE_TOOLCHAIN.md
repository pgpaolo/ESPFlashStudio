# Portable Toolchain

ESP Flash Studio is designed to work with a toolchain stored beneath the application directory.

## Layout

```text
<app>/Tools/
├─ Python/
├─ PlatformIO/
├─ ArduinoCli/
├─ Esptool/
└─ Git/
```

The repository does **not** include third-party binaries. They are downloaded from their upstream sources by the bootstrap workflow.

## PlatformIO

PlatformIO Core is installed into an application-local environment. The application sets `PLATFORMIO_CORE_DIR` for PlatformIO subprocesses so packages, platforms and metadata remain associated with the portable application runtime rather than relying on a user-global PlatformIO directory.

Typical runtime locations are resolved from `AppContext.BaseDirectory` through `AppPaths.vb`.

## Python

A portable Python runtime is used for PlatformIO bootstrap and related Python-based tooling. It is not registered system-wide and no permanent system PATH modification is required.

## Arduino CLI

Arduino CLI is stored as an application-local executable and can be used for Arduino-style sketch projects.

## esptool

ESP Flash Studio can use the esptool Python module available within the local PlatformIO environment and/or the application-local standalone tool where supported by the workflow.

## Git / MinGit

A local Git runtime can be used by PlatformIO for dependencies that reference Git repositories.

## Corporate networks

TLS-inspecting proxies may require special handling because portable Python certificate trust can differ from the Windows certificate store. ESP Flash Studio scopes compatibility environment variables to spawned processes only.

For managed environments, the preferred solution is to provide the organization CA trust chain to the relevant Python/requests tooling rather than broadly disabling TLS verification.

## Deployment

Once the toolchain has been populated, copying the complete application directory to another compatible Windows system preserves the intended portable structure. Package caches and toolchains can be large, so copying only the executable without `Tools/` does not carry the local toolchain with it.
