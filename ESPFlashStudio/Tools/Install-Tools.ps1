# ESP Flash Studio - popolamento tool locali (Windows x64)
# Scarica esclusivamente dai siti ufficiali dei progetti.
# Eseguire da PowerShell:  powershell -ExecutionPolicy Bypass -File .\Install-Tools.ps1

$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$ToolsRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$Cache = Join-Path $ToolsRoot '_cache'
New-Item -ItemType Directory -Force -Path $ToolsRoot,$Cache | Out-Null

$PythonUrl = 'https://dl.registry.platformio.org/download/platformio/tool/python-portable/1.31107.0/python-portable-windows_amd64-1.31107.0.tar.gz'
$ArduinoUrl = 'https://github.com/arduino/arduino-cli/releases/download/v1.5.1/arduino-cli_1.5.1_Windows_64bit.zip'
$ArduinoSha256 = 'FABE42E0EB04D00E776A66178299FF95A46C623DBC260F997E58FD514853DD40'
$PioInstallerUrl = 'https://raw.githubusercontent.com/platformio/platformio-core-installer/master/get-platformio.py'
$EsptoolUrl = 'https://github.com/espressif/esptool/releases/download/v5.4.0/esptool-v5.4.0-windows-amd64.zip'
$EsptoolSha256 = 'B7F6B9DD301A210B31F4829118C909C84AAE23107F9CA1FDC14CCF4D7384BE2E'
$MinGitUrl = 'https://github.com/git-for-windows/git/releases/download/v2.56.0.windows.2/MinGit-2.56.0.2-64-bit.zip'
$MinGitSha256 = 'DA35E72AA21C005A5A0D298CFBAE110BC1609A815730EA0DDE84B01A1B3CD3BE'

function Download-File([string]$Url, [string]$Destination) {
    Write-Host "Download: $Url" -ForegroundColor Cyan
    if (Get-Command Start-BitsTransfer -ErrorAction SilentlyContinue) {
        try { Start-BitsTransfer -Source $Url -Destination $Destination -ErrorAction Stop; return } catch {}
    }
    Invoke-WebRequest -Uri $Url -OutFile $Destination -UseBasicParsing
}

function Assert-Sha256([string]$Path, [string]$Expected) {
    $actual = (Get-FileHash -Algorithm SHA256 -Path $Path).Hash.ToUpperInvariant()
    if ($actual -ne $Expected.ToUpperInvariant()) {
        throw "SHA256 non valido per $Path`nAtteso: $Expected`nOttenuto: $actual"
    }
    Write-Host "SHA256 OK: $(Split-Path $Path -Leaf)" -ForegroundColor Green
}

function Reset-Dir([string]$Path) {
    if (Test-Path $Path) { Remove-Item -Recurse -Force $Path }
    New-Item -ItemType Directory -Force -Path $Path | Out-Null
}

Write-Host ''
Write-Host '=== ESP Flash Studio - Tools Bootstrap ===' -ForegroundColor Yellow
Write-Host "Destinazione: $ToolsRoot"

# 1. Python portabile PlatformIO
$pythonDir = Join-Path $ToolsRoot 'Python'
$pythonExe = Join-Path $pythonDir 'python.exe'
if (-not (Test-Path $pythonExe)) {
    $pyArc = Join-Path $Cache 'python-portable.tar.gz'
    Download-File $PythonUrl $pyArc
    Reset-Dir $pythonDir
    Write-Host 'Estrazione Python portabile...'
    & tar.exe -xzf $pyArc -C $pythonDir
    if ($LASTEXITCODE -ne 0) { throw 'Errore estrazione Python con tar.exe.' }
    if (-not (Test-Path $pythonExe)) {
        $found = Get-ChildItem $pythonDir -Recurse -Filter python.exe | Select-Object -First 1
        if ($found) {
            $src = Split-Path $found.FullName -Parent
            $tmp = Join-Path $ToolsRoot '_python_normalize'
            Reset-Dir $tmp
            Copy-Item "$src\*" $tmp -Recurse -Force
            Remove-Item -Recurse -Force $pythonDir
            Move-Item $tmp $pythonDir
        }
    }
    if (-not (Test-Path $pythonExe)) { throw "python.exe non trovato dopo l'estrazione." }
}
Write-Host "Python: $pythonExe" -ForegroundColor Green

# 2. Arduino CLI
$arduinoDir = Join-Path $ToolsRoot 'ArduinoCli'
$arduinoExe = Join-Path $arduinoDir 'arduino-cli.exe'
if (-not (Test-Path $arduinoExe)) {
    $arc = Join-Path $Cache 'arduino-cli.zip'
    Download-File $ArduinoUrl $arc
    Assert-Sha256 $arc $ArduinoSha256
    Reset-Dir $arduinoDir
    Expand-Archive -LiteralPath $arc -DestinationPath $arduinoDir -Force
    $found = Get-ChildItem $arduinoDir -Recurse -Filter arduino-cli.exe | Select-Object -First 1
    if (-not $found) { throw 'arduino-cli.exe non trovato.' }
    if ($found.FullName -ne $arduinoExe) { Copy-Item $found.FullName $arduinoExe -Force }
}
Write-Host "Arduino CLI: $arduinoExe" -ForegroundColor Green

# 3. PlatformIO Core locale
$pioRoot = Join-Path $ToolsRoot 'PlatformIO'
$pioExe = Join-Path $pioRoot 'penv\Scripts\pio.exe'
$bootstrapDir = Join-Path $ToolsRoot 'Bootstrap'
New-Item -ItemType Directory -Force -Path $bootstrapDir | Out-Null
$pioInstaller = Join-Path $bootstrapDir 'get-platformio.py'
if (-not (Test-Path $pioExe)) {
    Download-File $PioInstallerUrl $pioInstaller
    Write-Host 'Installazione PlatformIO Core locale...' -ForegroundColor Cyan
    $oldCoreDir = $env:PLATFORMIO_CORE_DIR
    try {
        $env:PLATFORMIO_CORE_DIR = $pioRoot
        & $pythonExe $pioInstaller
        if ($LASTEXITCODE -ne 0) { throw "Installer PlatformIO terminato con exit code $LASTEXITCODE" }
    } finally {
        $env:PLATFORMIO_CORE_DIR = $oldCoreDir
    }
}
if (-not (Test-Path $pioExe)) {
    $alt = Get-ChildItem $pioRoot -Recurse -Filter pio.exe -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($alt) { $pioExe = $alt.FullName }
}
if (-not (Test-Path $pioExe)) { throw 'pio.exe non trovato dopo installazione PlatformIO.' }
Write-Host "PlatformIO: $pioExe" -ForegroundColor Green

# 4. esptool standalone (non dipende da Python)
$espDir = Join-Path $ToolsRoot 'Esptool'
$espExe = Join-Path $espDir 'esptool.exe'
if (-not (Test-Path $espExe)) {
    $arc = Join-Path $Cache 'esptool.zip'
    Download-File $EsptoolUrl $arc
    Assert-Sha256 $arc $EsptoolSha256
    Reset-Dir $espDir
    Expand-Archive -LiteralPath $arc -DestinationPath $espDir -Force
    $found = Get-ChildItem $espDir -Recurse -Filter esptool.exe | Select-Object -First 1
    if (-not $found) { throw 'esptool.exe non trovato.' }
    if ($found.FullName -ne $espExe) {
        $src = Split-Path $found.FullName -Parent
        $tmp = Join-Path $ToolsRoot '_esptool_normalize'
        Reset-Dir $tmp
        Copy-Item "$src\*" $tmp -Recurse -Force
        Remove-Item -Recurse -Force $espDir
        Move-Item $tmp $espDir
    }
}
Write-Host "esptool: $espExe" -ForegroundColor Green

# 5. MinGit portabile - utile per lib_deps/repository Git
$gitDir = Join-Path $ToolsRoot 'Git'
$gitExe = Join-Path $gitDir 'cmd\git.exe'
if (-not (Test-Path $gitExe)) {
    $arc = Join-Path $Cache 'mingit.zip'
    Download-File $MinGitUrl $arc
    Assert-Sha256 $arc $MinGitSha256
    Reset-Dir $gitDir
    Expand-Archive -LiteralPath $arc -DestinationPath $gitDir -Force
}
if (Test-Path $gitExe) { Write-Host "Git: $gitExe" -ForegroundColor Green }

# Versioni
Write-Host ''
Write-Host '=== Verifica versioni ===' -ForegroundColor Yellow
try { & $pythonExe --version } catch {}
try { & $pioExe --version } catch {}
try { & $arduinoExe version } catch {}
try { & $espExe version } catch {}
try { & $gitExe --version } catch {}

# Manifest per l'app/diagnostica
$manifest = [ordered]@{
    generatedAt = (Get-Date).ToString('o')
    python = $pythonExe
    platformio = $pioExe
    arduinoCli = $arduinoExe
    esptool = $espExe
    git = $gitExe
}
$manifest | ConvertTo-Json | Set-Content -Encoding UTF8 (Join-Path $ToolsRoot 'tools-manifest.json')

# Cache rimossa per avere una cartella pulita
if (Test-Path $Cache) { Remove-Item -Recurse -Force $Cache }

Write-Host ''
Write-Host 'TUTTI I TOOL SONO PRONTI.' -ForegroundColor Green
Write-Host 'Puoi avviare ESPFlashStudio: userà i tool locali nella cartella Tools.' -ForegroundColor Green
Read-Host 'Premi INVIO per chiudere'
