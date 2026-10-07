ESP FLASH STUDIO - TOOL LOCALI
==============================

Esegui:
    INSTALLA-TOOLS.cmd

Lo script popola questa cartella con:
  Python      -> Tools\Python\python.exe
  PlatformIO  -> Tools\PlatformIO\penv\Scripts\pio.exe
  Arduino CLI -> Tools\ArduinoCli\arduino-cli.exe
  esptool     -> Tools\Esptool\esptool.exe
  MinGit      -> Tools\Git\cmd\git.exe

I download avvengono dai repository ufficiali dei rispettivi progetti.
Per Arduino CLI, esptool e MinGit viene verificato anche SHA-256.

Nota: i toolchain ESP32/ESP8266 e i framework vengono poi gestiti da PlatformIO
in base al platformio.ini del progetto, evitando di includere gigabyte inutili.
