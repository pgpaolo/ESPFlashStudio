Imports System.Diagnostics
Imports System.Text

Namespace ESPFlashStudio.Services
    Public Class ProcessResult
        Public Property ExitCode As Integer
        Public Property Output As String = ""
        Public Property ErrorOutput As String = ""
        Public ReadOnly Property Success As Boolean
            Get
                Return ExitCode = 0
            End Get
        End Property
    End Class

    Public Class ProcessRunner
        Public Event OutputReceived(line As String)

        Public Async Function RunAsync(fileName As String,
                                       arguments As String,
                                       workingDirectory As String,
                                       cancellationToken As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim psi As New ProcessStartInfo With {
                .FileName = fileName,
                .Arguments = arguments,
                .WorkingDirectory = If(String.IsNullOrWhiteSpace(workingDirectory), Environment.CurrentDirectory, workingDirectory),
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .StandardOutputEncoding = Encoding.UTF8,
                .StandardErrorEncoding = Encoding.UTF8
            }

            ' Ambiente completamente locale all'applicazione.
            AppPaths.EnsureDirectories()
            psi.Environment("PLATFORMIO_CORE_DIR") = AppPaths.PlatformIoRoot
            psi.Environment("PYTHONNOUSERSITE") = "1"
            psi.Environment("PIP_DISABLE_PIP_VERSION_CHECK") = "1"
            ' PlatformIO usa requests per scaricare framework/toolchain. In reti con
            ' proxy HTTPS aziendale può presentarsi una CA interna/self-signed.
            ' Questa impostazione riguarda soltanto PlatformIO eseguito dall'app.
            psi.Environment("PLATFORMIO_SETTING_ENABLE_PROXY_STRICT_SSL") = "false"

            Dim localPathParts As New List(Of String)()
            Dim pioScripts = IO.Path.Combine(AppPaths.PlatformIoRoot, "penv", "Scripts")
            Dim gitCmd = IO.Path.Combine(AppPaths.ToolsRoot, "Git", "cmd")
            Dim gitBin = IO.Path.Combine(AppPaths.ToolsRoot, "Git", "bin")
            Dim pythonDir = IO.Path.Combine(AppPaths.ToolsRoot, "Python")
            If IO.Directory.Exists(pioScripts) Then localPathParts.Add(pioScripts)
            If IO.Directory.Exists(gitCmd) Then localPathParts.Add(gitCmd)
            If IO.Directory.Exists(gitBin) Then localPathParts.Add(gitBin)
            If IO.Directory.Exists(pythonDir) Then localPathParts.Add(pythonDir)
            Dim systemPath = Environment.GetEnvironmentVariable("PATH")
            psi.Environment("PATH") = String.Join(IO.Path.PathSeparator, localPathParts) & If(String.IsNullOrWhiteSpace(systemPath), "", IO.Path.PathSeparator & systemPath)

            Dim stdout As New StringBuilder()
            Dim stderr As New StringBuilder()

            Using p As New Process With {.StartInfo = psi, .EnableRaisingEvents = True}
                AddHandler p.OutputDataReceived, Sub(sender, e)
                                                     If e.Data IsNot Nothing Then
                                                         stdout.AppendLine(e.Data)
                                                         RaiseEvent OutputReceived(e.Data)
                                                     End If
                                                 End Sub
                AddHandler p.ErrorDataReceived, Sub(sender, e)
                                                    If e.Data IsNot Nothing Then
                                                        stderr.AppendLine(e.Data)
                                                        RaiseEvent OutputReceived(e.Data)
                                                    End If
                                                End Sub

                If Not p.Start() Then Throw New InvalidOperationException("Impossibile avviare il processo: " & fileName)
                p.BeginOutputReadLine()
                p.BeginErrorReadLine()

                Using registration = cancellationToken.Register(Sub()
                                                                     Try
                                                                         If Not p.HasExited Then p.Kill(True)
                                                                     Catch
                                                                     End Try
                                                                 End Sub)
                    Await p.WaitForExitAsync(cancellationToken)
                End Using

                Return New ProcessResult With {
                    .ExitCode = p.ExitCode,
                    .Output = stdout.ToString(),
                    .ErrorOutput = stderr.ToString()
                }
            End Using
        End Function
    End Class
End Namespace
