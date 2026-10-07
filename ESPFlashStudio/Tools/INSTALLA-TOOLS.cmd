@echo off
setlocal
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Install-Tools.ps1"
if errorlevel 1 (
  echo.
  echo ERRORE durante l'installazione dei tool.
  pause
)
