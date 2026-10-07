Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports ESPFlashStudio.Models
Imports ESPFlashStudio.Services

Namespace ESPFlashStudio
    Public Partial Class MainForm
        Private Async Sub btnFlash_Click(sender As Object, e As EventArgs) Handles btnFlash.Click
            If String.IsNullOrWhiteSpace(cmbPort.Text) Then
                MessageBox.Show(Me, "Seleziona una porta COM.", "Flash", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Try
                BeginOperation("Pre-flight flash...")
                Dim engine = EffectiveFlashEngine()
                Dim board = TryCast(cmbBoard.SelectedItem, BoardProfile)
                Dim family = If(board?.Family, "ESP32")
                Dim py = If(File.Exists(AppPaths.PlatformIoPython), AppPaths.PlatformIoPython, _toolLocator.FindPython(txtPython.Text.Trim()))
                Dim esptoolArgs = If(File.Exists(AppPaths.PlatformIoPython), "-m esptool", txtEsptoolArgs.Text.Trim())
                Dim items = GetFlashItems()

                LogLine("PREFLIGHT: engine=" & engine & "; port=" & cmbPort.Text & "; baud=" & nudBaud.Value.ToString())
                If chkBackupBeforeFlash.Checked Then
                    If String.IsNullOrWhiteSpace(py) Then Throw New FileNotFoundException("Python/esptool locale non disponibile per il backup.")
                    Dim backupDir = Path.Combine(If(String.IsNullOrWhiteSpace(txtOutput.Text), Path.Combine(txtProject.Text, "artifacts"), txtOutput.Text), "backups")
                    Dim backupFile = Path.Combine(backupDir, $"flash_backup_{DateTime.Now:yyyyMMdd_HHmmss}.bin")
                    LogLine("BACKUP: lettura flash in " & backupFile)
                    Dim backupResult = Await _flashService.BackupFlashAsync(py, esptoolArgs, family, cmbPort.Text, backupFile, _cts.Token)
                    If Not backupResult.Success Then Throw New InvalidOperationException("Backup flash fallito. Operazione di flash interrotta.")
                    LogLine("BACKUP: completato.")
                End If

                lblStatus.Text = "Flash in corso..."
                Dim result As ProcessResult
                If engine = "PlatformIO" Then
                    Dim pioPath = _toolLocator.FindPlatformIo(txtPio.Text.Trim())
                    If String.IsNullOrWhiteSpace(pioPath) Then Throw New FileNotFoundException("PlatformIO locale non trovato nella cartella ToolsPlatformIO.")
                    txtPio.Text = pioPath
                    result = Await _flashService.UploadPlatformIoAsync(pioPath, txtProject.Text.Trim(), cmbEnvironment.Text, cmbPort.Text, _cts.Token)
                    If items.Count = 0 Then items = _buildService.FindPlatformIoBinaries(txtProject.Text.Trim(), cmbEnvironment.Text)
                ElseIf engine = "Arduino CLI" Then
                    If board Is Nothing Then Throw New InvalidOperationException("Seleziona una board.")
                    result = Await _flashService.UploadArduinoAsync(txtArduinoCli.Text.Trim(), txtProject.Text.Trim(), board.ArduinoFqbn, cmbPort.Text, txtOutput.Text.Trim(), _cts.Token)
                Else
                    If items.Count = 0 Then Throw New InvalidOperationException("Aggiungi almeno un file BIN con il relativo offset.")
                    For Each item In items
                        If Not File.Exists(item.FilePath) Then Throw New FileNotFoundException("File BIN non trovato", item.FilePath)
                    Next
                    result = Await _flashService.FlashRawAsync(py, esptoolArgs, family, cmbPort.Text, CInt(nudBaud.Value), items, _cts.Token)
                End If

                If result.Success AndAlso chkVerifyAfterFlash.Checked AndAlso items.Count > 0 Then
                    If String.IsNullOrWhiteSpace(py) Then Throw New FileNotFoundException("Python/esptool locale non disponibile per la verifica.")
                    lblStatus.Text = "Verifica flash..."
                    LogLine("VERIFY: verifica contenuto flash...")
                    Dim verify = Await _flashService.VerifyFlashAsync(py, esptoolArgs, family, cmbPort.Text, CInt(nudBaud.Value), items, _cts.Token)
                    If Not verify.Success Then Throw New InvalidOperationException("Flash scritto, ma verifica successiva fallita.")
                    LogLine("VERIFY: completata con successo.")
                End If

                EndOperation(If(result.Success, "Flash completato.", $"Flash fallito (exit code {result.ExitCode})."))
                If result.Success Then MessageBox.Show(Me, "Flash completato con successo.", "Flash", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OperationCanceledException
                EndOperation("Operazione annullata.")
            Catch ex As Exception
                EndOperation("Errore flash.")
                LogLine("ERROR: " & ex.Message)
                MessageBox.Show(Me, ex.Message, "Errore flash", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Async Sub btnChipInfo_Click(sender As Object, e As EventArgs) Handles btnChipInfo.Click
            If String.IsNullOrWhiteSpace(cmbPort.Text) Then Return
            Try
                BeginOperation("Rilevamento chip...")
                Dim py = _toolLocator.FindPython(txtPython.Text.Trim())
                If String.IsNullOrWhiteSpace(py) Then Throw New FileNotFoundException("Python locale non trovato.")
                If Not File.Exists(AppPaths.PlatformIoPython) Then
                    Throw New InvalidOperationException("PlatformIO/esptool non è ancora installato. Completa prima 'Installa strumenti integrati'.")
                End If
                txtPython.Text = AppPaths.PlatformIoPython
                Dim result = Await _flashService.ChipIdAsync(AppPaths.PlatformIoPython, "-m esptool", cmbPort.Text, _cts.Token)
                EndOperation(If(result.Success, "Chip rilevato.", "Rilevamento chip fallito."))
            Catch ex As Exception
                EndOperation("Rilevamento chip fallito.")
                LogLine("ERROR: " & ex.Message)
            End Try
        End Sub

        Private Async Sub btnErase_Click(sender As Object, e As EventArgs) Handles btnErase.Click
            If String.IsNullOrWhiteSpace(cmbPort.Text) Then Return
            If MessageBox.Show(Me, "Cancellare completamente la flash del dispositivo?", "Conferma erase", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return
            Try
                BeginOperation("Cancellazione flash...")
                Dim board = TryCast(cmbBoard.SelectedItem, BoardProfile)
                Dim result = Await _flashService.EraseAsync(txtPython.Text.Trim(), txtEsptoolArgs.Text.Trim(), If(board?.Family, "ESP32"), cmbPort.Text, _cts.Token)
                EndOperation(If(result.Success, "Flash cancellata.", "Erase fallito."))
            Catch ex As Exception
                EndOperation("Erase fallito.")
                LogLine("ERROR: " & ex.Message)
            End Try
        End Sub

        Private Async Sub btnInstallTools_Click(sender As Object, e As EventArgs) Handles btnInstallTools.Click
            If MessageBox.Show(Me,
                               "Scaricare e installare nella cartella del programma Python portabile, PlatformIO Core, esptool e Arduino CLI?",
                               "Strumenti integrati", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
            Try
                BeginOperation("Preparazione strumenti integrati...")
                Await _bootstrap.InstallAllAsync(_cts.Token)
                txtPio.Text = _toolLocator.FindPlatformIo("")
                txtArduinoCli.Text = _toolLocator.FindArduinoCli("")
                txtPython.Text = _toolLocator.FindPython("")
                txtEsptoolArgs.Text = If(File.Exists(AppPaths.EsptoolExe), "", "-m esptool")
                EndOperation("Strumenti integrati pronti.")
                MessageBox.Show(Me, "Installazione completata. Gli strumenti sono nella sottocartella Tools del programma.", "Strumenti integrati", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OperationCanceledException
                EndOperation("Installazione annullata.")
            Catch ex As Exception
                EndOperation("Errore installazione strumenti.")
                LogLine("ERROR: " & ex.Message)
                MessageBox.Show(Me, ex.Message, "Errore strumenti", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub Bootstrap_Progress(message As String)
            If Me.InvokeRequired Then
                Me.BeginInvoke(New Action(Of String)(AddressOf Bootstrap_Progress), message)
                Return
            End If
            LogLine("[TOOLS] " & message)
            lblStatus.Text = message
        End Sub

        Private Sub btnDetectTools_Click(sender As Object, e As EventArgs) Handles btnDetectTools.Click
            Dim pio = _toolLocator.FindPlatformIo(txtPio.Text.Trim())
            Dim arduino = _toolLocator.FindArduinoCli(txtArduinoCli.Text.Trim())
            Dim python = _toolLocator.FindPython(txtPython.Text.Trim())
            If Not String.IsNullOrWhiteSpace(pio) Then txtPio.Text = pio
            If Not String.IsNullOrWhiteSpace(arduino) Then txtArduinoCli.Text = arduino
            If Not String.IsNullOrWhiteSpace(python) Then txtPython.Text = python
            MessageBox.Show(Me,
                            $"Modalità PORTABLE - ricerca esclusivamente nella cartella dell'app.{Environment.NewLine}{Environment.NewLine}" &
                            $"Root app: {AppPaths.AppRoot}{Environment.NewLine}" &
                            $"PlatformIO: {If(String.IsNullOrWhiteSpace(pio), "non trovato", pio)}{Environment.NewLine}" &
                            $"Arduino CLI: {If(String.IsNullOrWhiteSpace(arduino), "non trovato", arduino)}{Environment.NewLine}" &
                            $"Python: {If(String.IsNullOrWhiteSpace(python), "non trovato", python)}{Environment.NewLine}" &
                            $"PlatformIO CORE_DIR: {AppPaths.PlatformIoRoot}",
                            "Rilevamento strumenti locali", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Private Sub btnSaveLog_Click(sender As Object, e As EventArgs) Handles btnSaveLog.Click
            Using dlg As New SaveFileDialog With {
                .Filter = "File di log (*.log)|*.log|File di testo (*.txt)|*.txt|Tutti i file (*.*)|*.*",
                .FileName = $"ESPFlashStudio_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            }
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    File.WriteAllText(dlg.FileName, txtLog.Text, System.Text.Encoding.UTF8)
                    lblStatus.Text = "Log esportato: " & Path.GetFileName(dlg.FileName)
                End If
            End Using
        End Sub

        Private Sub btnClearLog_Click(sender As Object, e As EventArgs) Handles btnClearLog.Click
            txtLog.Clear()
        End Sub
    End Class
End Namespace
