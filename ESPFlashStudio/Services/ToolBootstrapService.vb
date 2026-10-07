Imports System.Formats.Tar
Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Diagnostics

Namespace ESPFlashStudio.Services
    Public Class ToolBootstrapService
        Public Event Progress(message As String)

        Private Const PythonPortableUrl As String = "https://dl.registry.platformio.org/download/platformio/tool/python-portable/1.31107.0/python-portable-windows_amd64-1.31107.0.tar.gz"
        Private Const ArduinoCliUrl As String = "https://github.com/arduino/arduino-cli/releases/download/v1.5.1/arduino-cli_1.5.1_Windows_64bit.zip"
        Private Const PlatformIoInstallerUrl As String = "https://raw.githubusercontent.com/platformio/platformio-core-installer/master/get-platformio.py"

        Public ReadOnly Property ToolsRoot As String
            Get
                Return AppPaths.ToolsRoot
            End Get
        End Property

        Public ReadOnly Property BundledPython As String
            Get
                Return AppPaths.PortablePython
            End Get
        End Property

        Public ReadOnly Property BundledArduinoCli As String
            Get
                Return AppPaths.ArduinoCliExe
            End Get
        End Property

        Public ReadOnly Property BundledPio As String
            Get
                Return AppPaths.PlatformIoExe
            End Get
        End Property

        Public ReadOnly Property BundledPioPython As String
            Get
                Return AppPaths.PlatformIoPython
            End Get
        End Property

        Public Async Function InstallAllAsync(ct As Threading.CancellationToken) As Task
            Directory.CreateDirectory(ToolsRoot)
            Await EnsurePythonAsync(ct)
            Await EnsureArduinoCliAsync(ct)
            Await EnsurePlatformIoAsync(ct)
            Await EnsureEsptoolAsync(ct)
            RaiseEvent Progress("Strumenti integrati pronti.")
        End Function

        Private Async Function EnsurePythonAsync(ct As Threading.CancellationToken) As Task
            If File.Exists(BundledPython) Then
                RaiseEvent Progress("Python portabile già presente.")
                Return
            End If

            RaiseEvent Progress("Download Python portabile...")
            Dim archive = Path.Combine(ToolsRoot, "python-portable.tar.gz")
            Await DownloadFileAsync(PythonPortableUrl, archive, ct)

            Dim pyDir = Path.Combine(ToolsRoot, "Python")
            If Directory.Exists(pyDir) Then Directory.Delete(pyDir, True)
            Directory.CreateDirectory(pyDir)

            RaiseEvent Progress("Estrazione Python portabile...")
            Using fs = File.OpenRead(archive)
                Using gz As New GZipStream(fs, CompressionMode.Decompress)
                    TarFile.ExtractToDirectory(gz, pyDir, True)
                End Using
            End Using
            File.Delete(archive)

            If Not File.Exists(BundledPython) Then
                Throw New FileNotFoundException("Python portabile estratto ma python.exe non è stato trovato.", BundledPython)
            End If
        End Function

        Private Async Function EnsureArduinoCliAsync(ct As Threading.CancellationToken) As Task
            If File.Exists(BundledArduinoCli) Then
                RaiseEvent Progress("Arduino CLI già presente.")
                Return
            End If

            RaiseEvent Progress("Download Arduino CLI 1.5.1...")
            Dim archive = Path.Combine(ToolsRoot, "arduino-cli.zip")
            Await DownloadFileAsync(ArduinoCliUrl, archive, ct)

            Dim arduinoDir = Path.Combine(ToolsRoot, "ArduinoCli")
            If Directory.Exists(arduinoDir) Then Directory.Delete(arduinoDir, True)
            Directory.CreateDirectory(arduinoDir)
            ZipFile.ExtractToDirectory(archive, arduinoDir, True)
            File.Delete(archive)

            If Not File.Exists(BundledArduinoCli) Then
                Throw New FileNotFoundException("Arduino CLI estratto ma arduino-cli.exe non è stato trovato.", BundledArduinoCli)
            End If
        End Function

        Private Async Function EnsurePlatformIoAsync(ct As Threading.CancellationToken) As Task
            If File.Exists(BundledPio) Then
                RaiseEvent Progress("PlatformIO Core già presente.")
                Return
            End If

            If Not File.Exists(BundledPython) Then Throw New FileNotFoundException("Python portabile non disponibile.")

            Dim bootstrapDir = Path.Combine(ToolsRoot, "Bootstrap")
            Directory.CreateDirectory(bootstrapDir)
            Dim installer = Path.Combine(bootstrapDir, "get-platformio.py")
            RaiseEvent Progress("Download installer PlatformIO Core...")
            Await DownloadFileAsync(PlatformIoInstallerUrl, installer, ct)

            RaiseEvent Progress("Installazione PlatformIO Core locale...")
            Dim psi As New ProcessStartInfo(BundledPython) With {
                .WorkingDirectory = bootstrapDir,
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            psi.ArgumentList.Add(installer)
            psi.Environment("PLATFORMIO_CORE_DIR") = AppPaths.PlatformIoRoot
            psi.Environment("PIP_TRUSTED_HOST") = "pypi.org files.pythonhosted.org"
            psi.Environment("PIP_INDEX_URL") = "https://pypi.org/simple"
            psi.Environment("PIP_DISABLE_PIP_VERSION_CHECK") = "1"
            psi.Environment("PYTHONNOUSERSITE") = "1"
            RaiseEvent Progress("Bootstrap TLS v2: trusted-host PyPI attivo; truststore NON utilizzato.")
            Await RunProcessAsync(psi, ct)

            If Not File.Exists(BundledPio) Then
                Throw New FileNotFoundException("L'installazione di PlatformIO è terminata ma pio.exe non è stato trovato.", BundledPio)
            End If
        End Function

        Private Async Function EnsureEsptoolAsync(ct As Threading.CancellationToken) As Task
            If Not File.Exists(BundledPioPython) Then Return

            RaiseEvent Progress("Verifica/installazione esptool...")
            Dim checkPsi As New ProcessStartInfo(BundledPioPython) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            checkPsi.ArgumentList.Add("-c")
            checkPsi.ArgumentList.Add("import esptool")
            If Await RunProcessAsync(checkPsi, ct, False) = 0 Then Return

            Dim installPsi As New ProcessStartInfo(BundledPioPython) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            installPsi.ArgumentList.Add("-m")
            installPsi.ArgumentList.Add("pip")
            installPsi.ArgumentList.Add("install")
            installPsi.ArgumentList.Add("-U")
            installPsi.ArgumentList.Add("esptool")
            installPsi.Environment("PIP_TRUSTED_HOST") = "pypi.org files.pythonhosted.org"
            installPsi.Environment("PIP_INDEX_URL") = "https://pypi.org/simple"
            installPsi.Environment("PIP_DISABLE_PIP_VERSION_CHECK") = "1"
            installPsi.Environment("PYTHONNOUSERSITE") = "1"
            Await RunProcessAsync(installPsi, ct)
        End Function

        Private Async Function DownloadFileAsync(url As String, destination As String, ct As Threading.CancellationToken) As Task
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromMinutes(10)
                Using response = Await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct)
                    response.EnsureSuccessStatusCode()
                    Using input = Await response.Content.ReadAsStreamAsync(ct)
                        Using output = New FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 81920, True)
                            Await input.CopyToAsync(output, ct)
                        End Using
                    End Using
                End Using
            End Using
        End Function

        Private Async Function RunProcessAsync(psi As ProcessStartInfo,
                                               ct As Threading.CancellationToken,
                                               Optional throwOnError As Boolean = True) As Task(Of Integer)
            Using p As New Process With {.StartInfo = psi, .EnableRaisingEvents = True}
                AddHandler p.OutputDataReceived, Sub(s, e)
                                                     If e.Data IsNot Nothing Then RaiseEvent Progress(e.Data)
                                                 End Sub
                AddHandler p.ErrorDataReceived, Sub(s, e)
                                                    If e.Data IsNot Nothing Then RaiseEvent Progress(e.Data)
                                                End Sub
                If Not p.Start() Then Throw New InvalidOperationException("Impossibile avviare il processo di installazione.")
                p.BeginOutputReadLine()
                p.BeginErrorReadLine()
                Await p.WaitForExitAsync(ct)
                If throwOnError AndAlso p.ExitCode <> 0 Then
                    Throw New InvalidOperationException($"Processo terminato con exit code {p.ExitCode}.")
                End If
                Return p.ExitCode
            End Using
        End Function
    End Class
End Namespace
