Imports System.Text
Imports ESPFlashStudio.Models

Namespace ESPFlashStudio.Services
    Public Class FlashService
        Private ReadOnly _runner As ProcessRunner

        Public Sub New(runner As ProcessRunner)
            _runner = runner
        End Sub

        Public Async Function UploadPlatformIoAsync(pioPath As String,
                                                    projectFolder As String,
                                                    environmentName As String,
                                                    port As String,
                                                    ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim args = $"run -d {Q(projectFolder)}"
            If Not String.IsNullOrWhiteSpace(environmentName) Then args &= $" -e {Q(environmentName)}"
            args &= " -t upload"
            If Not String.IsNullOrWhiteSpace(port) Then args &= $" --upload-port {Q(port)}"
            Return Await _runner.RunAsync(pioPath, args, projectFolder, ct)
        End Function

        Public Async Function UploadArduinoAsync(cliPath As String,
                                                 projectFolder As String,
                                                 fqbn As String,
                                                 port As String,
                                                 inputDir As String,
                                                 ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim args = $"upload -p {Q(port)} --fqbn {Q(fqbn)}"
            If Not String.IsNullOrWhiteSpace(inputDir) AndAlso IO.Directory.Exists(inputDir) Then args &= $" --input-dir {Q(inputDir)}"
            args &= $" {Q(projectFolder)}"
            Return Await _runner.RunAsync(cliPath, args, projectFolder, ct)
        End Function

        Public Async Function FlashRawAsync(pythonPath As String,
                                            esptoolCommand As String,
                                            family As String,
                                            port As String,
                                            baud As Integer,
                                            items As IEnumerable(Of FlashItem),
                                            ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim sb As New StringBuilder()
            sb.Append(esptoolCommand).Append(" --chip ").Append(ChipName(family))
            sb.Append(" --port ").Append(Q(port)).Append(" --baud ").Append(baud)
            sb.Append(" --before default-reset --after hard-reset write-flash")
            AppendItems(sb, items)
            Return Await _runner.RunAsync(pythonPath, sb.ToString(), Environment.CurrentDirectory, ct)
        End Function

        Public Async Function VerifyFlashAsync(pythonPath As String,
                                               esptoolCommand As String,
                                               family As String,
                                               port As String,
                                               baud As Integer,
                                               items As IEnumerable(Of FlashItem),
                                               ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim sb As New StringBuilder()
            sb.Append(esptoolCommand).Append(" --chip ").Append(ChipName(family))
            sb.Append(" --port ").Append(Q(port)).Append(" --baud ").Append(baud)
            sb.Append(" verify-flash")
            AppendItems(sb, items)
            Return Await _runner.RunAsync(pythonPath, sb.ToString(), Environment.CurrentDirectory, ct)
        End Function

        Public Async Function BackupFlashAsync(pythonPath As String,
                                               esptoolCommand As String,
                                               family As String,
                                               port As String,
                                               outputFile As String,
                                               ct As Threading.CancellationToken) As Task(Of ProcessResult)
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(outputFile))
            Dim args = $"{esptoolCommand} --chip {ChipName(family)} --port {Q(port)} read-flash 0 ALL {Q(outputFile)}"
            Return Await _runner.RunAsync(pythonPath, args, Environment.CurrentDirectory, ct)
        End Function

        Public Async Function EraseAsync(pythonPath As String,
                                         esptoolCommand As String,
                                         family As String,
                                         port As String,
                                         ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim args = $"{esptoolCommand} --chip {ChipName(family)} --port {Q(port)} erase-flash"
            Return Await _runner.RunAsync(pythonPath, args, Environment.CurrentDirectory, ct)
        End Function

        Public Async Function ChipIdAsync(pythonPath As String,
                                          esptoolCommand As String,
                                          port As String,
                                          ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim args = $"{esptoolCommand} --port {Q(port)} chip-id"
            Return Await _runner.RunAsync(pythonPath, args, Environment.CurrentDirectory, ct)
        End Function

        Private Shared Sub AppendItems(sb As StringBuilder, items As IEnumerable(Of FlashItem))
            For Each item In items
                If Not String.IsNullOrWhiteSpace(item.FilePath) Then sb.Append(" ").Append(item.Offset).Append(" ").Append(Q(item.FilePath))
            Next
        End Sub

        Private Shared Function ChipName(family As String) As String
            If family.Equals("ESP8266", StringComparison.OrdinalIgnoreCase) Then Return "esp8266"
            Return "esp32"
        End Function

        Private Shared Function Q(value As String) As String
            Dim dq As String = ChrW(34).ToString()
            Return dq & value.Replace(dq, "" & dq) & dq
        End Function
    End Class
End Namespace
