ESP FLASH STUDIO - LOCAL TOOLS
==============================

Run:
    INSTALLA-TOOLS.cmd

The bootstrap populates this directory with:
  Python      -> Tools\Python\python.exe
  PlatformIO  -> Tools\PlatformIO\penv\Scripts\pio.exe
  Arduino CLI -> Tools\ArduinoCli\arduino-cli.exe
  esptool     -> Tools\Esptool\esptool.exe
  MinGit      -> Tools\Git\cmd\git.exe

Downloads are fetched from the official upstream sources.

ESP32/ESP8266 toolchains and frameworks are then managed by PlatformIO according
to the selected project's platformio.ini, avoiding unnecessary multi-gigabyte bundles.

The application is designed to prefer these local tools rather than global
system installations.
