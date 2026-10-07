Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports ESPFlashStudio.Models
Imports ESPFlashStudio.Services

Namespace ESPFlashStudio
    Public Partial Class MainForm
        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            _cts?.Cancel()
        End Sub

        Private Sub UpdateDashboard(state As String, artifactFolder As String)
            If lblDashProject Is Nothing Then Return
            Dim projectState = If(Directory.Exists(txtProject.Text.Trim()), "OK", "NON SELEZIONATO")
            Dim pioState = If(String.IsNullOrWhiteSpace(_toolLocator.FindPlatformIo(txtPio.Text.Trim())), "NON DISPONIBILE", "OK")
            Dim portState = If(String.IsNullOrWhiteSpace(cmbPort.Text), "Nessuna COM", cmbPort.Text)
            lblDashProject.Text = "Progetto: " & projectState & "   |   Engine: " & EffectiveBuildEngine()
            lblDashToolchain.Text = "Toolchain: PlatformIO " & pioState & "   |   Porta: " & portState
            lblDashArtifact.Text = "Artifacts: " & If(String.IsNullOrWhiteSpace(artifactFolder), GetEffectiveArtifactFolder(), artifactFolder)
            lblDashState.Text = state
        End Sub

        Private Sub BeginOperation(message As String)
            _cts?.Dispose()
            _cts = New Threading.CancellationTokenSource()
            lblStatus.Text = message
            progress.Visible = True
            ToggleButtons(False)
            LogLine("=== " & message & " ===")
        End Sub

        Private Sub EndOperation(message As String)
            lblStatus.Text = message
            progress.Visible = False
            ToggleButtons(True)
            LogLine("=== " & message & " ===")
            SaveSettings()
        End Sub

        Private Sub ToggleButtons(enabled As Boolean)
            btnBuild.Enabled = enabled
            btnFlash.Enabled = enabled
            btnErase.Enabled = enabled
            btnChipInfo.Enabled = enabled
            btnOpenPackage.Enabled = enabled
        End Sub

        Private Sub Runner_OutputReceived(line As String)
            LogLine(line)
        End Sub

        Private Sub LogLine(line As String)
            If txtLog.InvokeRequired Then
                txtLog.BeginInvoke(New Action(Of String)(AddressOf LogLine), line)
                Return
            End If

            Dim timestamp = $"[{DateTime.Now:HH:mm:ss}] "
            txtLog.SelectionStart = txtLog.TextLength
            txtLog.SelectionLength = 0
            txtLog.SelectionColor = Color.FromArgb(132, 132, 126)
            txtLog.AppendText(timestamp)

            txtLog.SelectionColor = LogColor(line)
            txtLog.AppendText(line & Environment.NewLine)
            txtLog.SelectionColor = UiTheme.ConsoleText
            txtLog.SelectionStart = txtLog.TextLength
            txtLog.ScrollToCaret()
        End Sub

        Private Shared Function LogColor(line As String) As Color
            Dim value = If(line, String.Empty).Trim()
            If value.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase) OrElse value.Contains(" fallita", StringComparison.OrdinalIgnoreCase) OrElse value.Contains(" failed", StringComparison.OrdinalIgnoreCase) Then
                Return Color.FromArgb(255, 118, 105)
            End If
            If value.StartsWith("WARNING", StringComparison.OrdinalIgnoreCase) OrElse value.Contains("warning", StringComparison.OrdinalIgnoreCase) Then
                Return Color.FromArgb(255, 198, 92)
            End If
            If value.Contains("completat", StringComparison.OrdinalIgnoreCase) OrElse value.Contains("success", StringComparison.OrdinalIgnoreCase) OrElse value.Contains("pront", StringComparison.OrdinalIgnoreCase) Then
                Return Color.FromArgb(109, 220, 145)
            End If
            If value.StartsWith("===") Then Return Color.FromArgb(132, 190, 153)
            If value.StartsWith("[TOOLS]", StringComparison.OrdinalIgnoreCase) Then Return Color.FromArgb(213, 184, 119)
            If value.StartsWith("ARTIFACTS:", StringComparison.OrdinalIgnoreCase) Then Return Color.FromArgb(132, 190, 153)
            Return UiTheme.ConsoleText
        End Function

        Private Shared Function Q(value As String) As String
            Dim dq As String = ChrW(34).ToString()
            Return dq & value.Replace(dq, "" & dq) & dq
        End Function
    End Class
End Namespace
