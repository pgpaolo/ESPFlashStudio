Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports ESPFlashStudio.Models
Imports ESPFlashStudio.Services

Namespace ESPFlashStudio
    Public Partial Class MainForm
        Private Async Sub btnBuild_Click(sender As Object, e As EventArgs) Handles btnBuild.Click
            Dim projectFolder = txtProject.Text.Trim()
            If Not Directory.Exists(projectFolder) Then
                MessageBox.Show(Me, "Seleziona una cartella progetto valida.", "Build", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim engine = EffectiveBuildEngine()
            If engine = "Raw BIN" Then
                MessageBox.Show(Me, "La modalità Raw BIN non compila sorgenti. Seleziona PlatformIO o Arduino CLI.", "Build", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Try
                BeginOperation("Pre-flight build...")
                LogLine("PREFLIGHT: progetto=" & projectFolder & "; engine=" & engine & "; env=" & cmbEnvironment.Text)
                lblStatus.Text = "Compilazione in corso..."
                Dim result As ProcessResult
                If engine = "PlatformIO" Then
                    Dim pioPath = _toolLocator.FindPlatformIo(txtPio.Text.Trim())
                    If String.IsNullOrWhiteSpace(pioPath) Then
                        Throw New FileNotFoundException("PlatformIO locale non trovato nella cartella ToolsPlatformIO. Premi 'Installa strumenti integrati'.")
                    End If
                    txtPio.Text = pioPath
                    result = Await _buildService.BuildPlatformIoAsync(pioPath, projectFolder, cmbEnvironment.Text, _cts.Token)
                    If result.Success Then
                        Dim outRoot = txtOutput.Text.Trim()
                        If String.IsNullOrWhiteSpace(outRoot) Then outRoot = Path.Combine(projectFolder, "artifacts")
                        txtOutput.Text = outRoot
                        Dim artifactResult = _buildService.ExportPlatformIoArtifacts(projectFolder, cmbEnvironment.Text, outRoot, AppVersion)
                        If artifactResult.Items.Count = 0 Then Throw New InvalidOperationException("Build completata ma non sono stati individuati BIN/offset da pubblicare. Verifica flash_args o l'output PlatformIO.")
                        PopulatePioFlashGrid(artifactResult.Items)
                        _lastArtifactZip = artifactResult.ZipPath
                        LogLine("ARTIFACTS: latest = " & artifactResult.LatestFolder)
                        LogLine("ARTIFACTS: storico = " & artifactResult.VersionFolder)
                        LogLine("ARTIFACTS: package = " & artifactResult.ZipPath)
                        UpdateDashboard("SUCCESS", artifactResult.LatestFolder)
                    End If
                Else
                    Dim detected = _detector.Detect(projectFolder)
                    If Not detected.HasArduinoSketch Then
                        If detected.HasPlatformIo Then
                            Throw New InvalidOperationException("Il progetto contiene platformio.ini ed è un progetto PlatformIO. Seleziona PlatformIO come motore di build.")
                        End If
                        Throw New FileNotFoundException("Nella cartella selezionata non è presente alcuno sketch Arduino .ino.")
                    End If
                    Dim board = TryCast(cmbBoard.SelectedItem, BoardProfile)
                    If board Is Nothing Then Throw New InvalidOperationException("Seleziona una board.")
                    Dim outDir = txtOutput.Text.Trim()
                    If String.IsNullOrWhiteSpace(outDir) Then outDir = Path.Combine(projectFolder, "artifacts")
                    txtOutput.Text = outDir
                    Dim arduinoPath = _toolLocator.FindArduinoCli(txtArduinoCli.Text.Trim())
                    If String.IsNullOrWhiteSpace(arduinoPath) Then Throw New FileNotFoundException("Arduino CLI locale non trovato.")
                    txtArduinoCli.Text = arduinoPath
                    result = Await _buildService.BuildArduinoAsync(arduinoPath, projectFolder, board.ArduinoFqbn, outDir, _cts.Token)
                End If
                EndOperation(If(result.Success, "Build completata.", $"Build fallita (exit code {result.ExitCode})."))
                If result.Success Then MessageBox.Show(Me, "Compilazione completata." & Environment.NewLine & Environment.NewLine & "BIN pubblicati in:" & Environment.NewLine & GetEffectiveArtifactFolder(), "Build", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OperationCanceledException
                EndOperation("Operazione annullata.")
            Catch ex As Exception
                EndOperation("Errore build.")
                LogLine("ERROR: " & ex.Message)
                MessageBox.Show(Me, ex.Message, "Errore build", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub PopulatePioFlashGrid(items As IEnumerable(Of FlashItem))
            gridFlash.Rows.Clear()
            For Each item In items
                gridFlash.Rows.Add(item.Offset, item.FilePath)
            Next
        End Sub

        Private Function GetEffectiveArtifactFolder() As String
            Dim root = txtOutput.Text.Trim()
            If String.IsNullOrWhiteSpace(root) Then Return root
            If EffectiveBuildEngine() = "PlatformIO" AndAlso Not String.IsNullOrWhiteSpace(cmbEnvironment.Text) Then
                Dim latest = Path.Combine(root, cmbEnvironment.Text.Trim(), "latest")
                If Directory.Exists(latest) Then Return latest
                Return Path.Combine(root, cmbEnvironment.Text.Trim())
            End If
            Return root
        End Function

        Private Sub btnOpenOutput_Click(sender As Object, e As EventArgs) Handles btnOpenOutput.Click
            Dim path = GetEffectiveArtifactFolder()
            If String.IsNullOrWhiteSpace(path) OrElse Not Directory.Exists(path) Then
                MessageBox.Show(Me, "Cartella output non disponibile.", "Output", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Process.Start(New ProcessStartInfo("explorer.exe", Q(path)) With {.UseShellExecute = True})
        End Sub

        Private Sub btnOpenPackage_Click(sender As Object, e As EventArgs) Handles btnOpenPackage.Click
            If String.IsNullOrWhiteSpace(_lastArtifactZip) OrElse Not File.Exists(_lastArtifactZip) Then
                Dim root = txtOutput.Text.Trim()
                If Not String.IsNullOrWhiteSpace(root) AndAlso Not String.IsNullOrWhiteSpace(cmbEnvironment.Text) Then
                    Dim envRoot = Path.Combine(root, cmbEnvironment.Text.Trim())
                    If Directory.Exists(envRoot) Then
                        _lastArtifactZip = Directory.GetFiles(envRoot, "ESPFlashStudio_*.zip").OrderByDescending(Function(f) File.GetLastWriteTimeUtc(f)).FirstOrDefault()
                    End If
                End If
            End If
            If String.IsNullOrWhiteSpace(_lastArtifactZip) OrElse Not File.Exists(_lastArtifactZip) Then
                MessageBox.Show(Me, "Nessun pacchetto firmware disponibile. Esegui prima una build.", "Firmware package", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Process.Start(New ProcessStartInfo("explorer.exe", "/select," & Q(_lastArtifactZip)) With {.UseShellExecute = True})
        End Sub

        Private Sub btnRefreshPorts_Click(sender As Object, e As EventArgs) Handles btnRefreshPorts.Click
            RefreshPorts()
        End Sub

        Private Sub RefreshPorts()
            Dim previous = cmbPort.Text
            cmbPort.Items.Clear()
            For Each port In SerialPort.GetPortNames().OrderBy(Function(s) s)
                cmbPort.Items.Add(port)
            Next
            If cmbPort.Items.Contains(previous) Then
                cmbPort.SelectedItem = previous
            ElseIf cmbPort.Items.Count > 0 Then
                cmbPort.SelectedIndex = 0
            End If
        End Sub

        Private Sub cmbBoard_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBoard.SelectedIndexChanged
            Dim board = TryCast(cmbBoard.SelectedItem, BoardProfile)
            If board IsNot Nothing Then nudBaud.Value = Math.Min(nudBaud.Maximum, Math.Max(nudBaud.Minimum, board.DefaultBaud))
        End Sub

        Private Sub btnAddBin_Click(sender As Object, e As EventArgs) Handles btnAddBin.Click
            Using dlg As New OpenFileDialog With {.Filter = "BIN firmware (*.bin)|*.bin|Tutti i file (*.*)|*.*", .Multiselect = True}
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    For Each file In dlg.FileNames
                        gridFlash.Rows.Add(DefaultOffsetForFile(file), file)
                    Next
                End If
            End Using
        End Sub

        Private Function DefaultOffsetForFile(file As String) As String
            Dim name = Path.GetFileName(file).ToLowerInvariant()
            Dim board = TryCast(cmbBoard.SelectedItem, BoardProfile)
            If board IsNot Nothing AndAlso board.Family = "ESP8266" Then Return "0x0"
            If name.Contains("bootloader") Then Return "0x1000"
            If name.Contains("partition") Then Return "0x8000"
            If name.Contains("boot_app0") Then Return "0xE000"
            Return "0x10000"
        End Function

        Private Sub btnRemoveBin_Click(sender As Object, e As EventArgs) Handles btnRemoveBin.Click
            If gridFlash.SelectedRows.Count > 0 Then gridFlash.Rows.Remove(gridFlash.SelectedRows(0))
        End Sub

        Private Function GetFlashItems() As List(Of FlashItem)
            Dim items As New List(Of FlashItem)()
            For Each row As DataGridViewRow In gridFlash.Rows
                If row.IsNewRow Then Continue For
                Dim offset = Convert.ToString(row.Cells(0).Value)?.Trim()
                Dim file = Convert.ToString(row.Cells(1).Value)?.Trim()
                If Not String.IsNullOrWhiteSpace(file) Then items.Add(New FlashItem With {.Offset = If(String.IsNullOrWhiteSpace(offset), "0x0", offset), .FilePath = file})
            Next
            Return items
        End Function
    End Class
End Namespace
