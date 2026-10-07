Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports ESPFlashStudio.Models
Imports ESPFlashStudio.Services

Namespace ESPFlashStudio
    Public Partial Class MainForm
        Private ReadOnly _settingsService As New SettingsService()
        Private ReadOnly _detector As New ProjectDetector()
        Private ReadOnly _toolLocator As New ToolLocator()
        Private ReadOnly _runner As New ProcessRunner()
        Private ReadOnly _bootstrap As New ToolBootstrapService()
        Private ReadOnly _buildService As BuildService
        Private ReadOnly _flashService As FlashService
        Private _settings As AppSettings
        Private _cts As Threading.CancellationTokenSource
        Private Const AppVersion As String = "0.9.0-beta"
        Private _lastArtifactZip As String = ""

        Public Sub New()
            InitializeComponent()
            UiTheme.Apply(Me)
            _buildService = New BuildService(_runner)
            _flashService = New FlashService(_runner)
            AddHandler _runner.OutputReceived, AddressOf Runner_OutputReceived
            AddHandler _bootstrap.Progress, AddressOf Bootstrap_Progress
            _settings = _settingsService.Load()
        End Sub

        Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            For Each p In BoardProfile.Defaults()
                cmbBoard.Items.Add(p)
            Next
            If cmbBoard.Items.Count > 0 Then cmbBoard.SelectedIndex = 0
            cmbEngine.SelectedItem = If(String.IsNullOrWhiteSpace(_settings.PreferredEngine), "Auto", _settings.PreferredEngine)
            If cmbEngine.SelectedIndex < 0 Then cmbEngine.SelectedIndex = 0
            cmbFlashEngine.SelectedIndex = 0
            chkVerifyAfterFlash.Checked = _settings.VerifyAfterFlash
            chkBackupBeforeFlash.Checked = _settings.BackupBeforeFlash
            lblVersion.Text = AppVersion

            txtPio.Text = _settings.PlatformIoPath
            txtArduinoCli.Text = _settings.ArduinoCliPath
            txtPython.Text = _settings.PythonPath
            txtEsptoolArgs.Text = _settings.EsptoolCommand

            Dim detectedPio = _toolLocator.FindPlatformIo(txtPio.Text)
            If Not String.IsNullOrWhiteSpace(detectedPio) Then txtPio.Text = detectedPio
            Dim detectedArduino = _toolLocator.FindArduinoCli(txtArduinoCli.Text)
            If Not String.IsNullOrWhiteSpace(detectedArduino) Then txtArduinoCli.Text = detectedArduino
            Dim detectedPython = _toolLocator.FindPython(txtPython.Text)
            If Not String.IsNullOrWhiteSpace(detectedPython) Then txtPython.Text = detectedPython
            txtProject.Text = _settings.LastProjectFolder
            txtOutput.Text = _settings.LastOutputFolder
            If String.IsNullOrWhiteSpace(txtOutput.Text) AndAlso Not String.IsNullOrWhiteSpace(txtProject.Text) Then
                txtOutput.Text = Path.Combine(txtProject.Text, "artifacts")
            End If

            RefreshPorts()
            If Directory.Exists(txtProject.Text) Then DetectProject()
        End Sub

        Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            SaveSettings()
        End Sub

        Private Sub SaveSettings()
            _settings.PlatformIoPath = txtPio.Text.Trim()
            _settings.ArduinoCliPath = txtArduinoCli.Text.Trim()
            _settings.PythonPath = txtPython.Text.Trim()
            _settings.EsptoolCommand = txtEsptoolArgs.Text.Trim()
            _settings.LastProjectFolder = txtProject.Text.Trim()
            _settings.LastOutputFolder = txtOutput.Text.Trim()
            _settings.PreferredEngine = If(TryCast(cmbEngine.SelectedItem, String), "Auto")
            _settings.VerifyAfterFlash = chkVerifyAfterFlash.Checked
            _settings.BackupBeforeFlash = chkBackupBeforeFlash.Checked
            _settingsService.Save(_settings)
        End Sub

        Private Sub btnBrowseProject_Click(sender As Object, e As EventArgs) Handles btnBrowseProject.Click
            Using dlg As New FolderBrowserDialog With {.Description = "Seleziona il progetto PlatformIO o Arduino"}
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    txtProject.Text = dlg.SelectedPath
                    If String.IsNullOrWhiteSpace(txtOutput.Text) Then txtOutput.Text = Path.Combine(dlg.SelectedPath, "artifacts")
                    DetectProject()
                End If
            End Using
        End Sub

        Private Sub btnBrowseOutput_Click(sender As Object, e As EventArgs) Handles btnBrowseOutput.Click
            Using dlg As New FolderBrowserDialog With {.Description = "Seleziona la cartella di output dei binari"}
                If dlg.ShowDialog(Me) = DialogResult.OK Then txtOutput.Text = dlg.SelectedPath
            End Using
        End Sub

        Private Sub OutputSelectionChanged(sender As Object, e As EventArgs) Handles txtOutput.TextChanged, cmbEnvironment.SelectedIndexChanged, cmbEngine.SelectedIndexChanged
            UpdateOutputHint()
        End Sub

        Private Sub UpdateOutputHint()
            If lblOutputHint Is Nothing Then Return
            Dim root = txtOutput.Text.Trim()
            If String.IsNullOrWhiteSpace(root) Then
                lblOutputHint.Text = "Se non specificata, verrà usata <progetto>artifacts."
                Return
            End If
            Dim target = root
            If (cmbEngine.SelectedItem?.ToString() = "PlatformIO" OrElse cmbEngine.SelectedItem?.ToString() = "Auto") AndAlso Not String.IsNullOrWhiteSpace(cmbEnvironment.Text) Then
                target = Path.Combine(root, cmbEnvironment.Text.Trim(), "latest")
            End If
            lblOutputHint.Text = "Destinazione effettiva BIN: " & target
        End Sub

        Private Sub txtProject_Leave(sender As Object, e As EventArgs) Handles txtProject.Leave
            DetectProject()
        End Sub

        Private Sub DetectProject()
            Dim folder = txtProject.Text.Trim()
            cmbEnvironment.Items.Clear()
            If Not Directory.Exists(folder) Then
                lblDetected.Text = "Rilevamento progetto: cartella non valida"
                Return
            End If

            Dim info = _detector.Detect(folder)
            Dim tags As New List(Of String)()
            If info.HasPlatformIo Then
                tags.Add("PlatformIO")
                For Each env In _detector.ReadPlatformIoEnvironments(info.PlatformIoIni)
                    cmbEnvironment.Items.Add(env)
                Next
                If cmbEnvironment.Items.Count > 0 Then cmbEnvironment.SelectedIndex = 0
            End If
            If info.HasArduinoSketch Then tags.Add("Arduino sketch")
            If tags.Count = 0 Then tags.Add("nessun progetto noto / modalità BIN")
            lblDetected.Text = "Rilevamento progetto: " & String.Join(" + ", tags)

            If info.HasPlatformIo Then
                cmbEngine.SelectedItem = "PlatformIO"
                cmbFlashEngine.SelectedItem = "PlatformIO"
            ElseIf info.HasArduinoSketch Then
                If cmbEngine.SelectedItem?.ToString() <> "Raw BIN" Then cmbEngine.SelectedItem = "Arduino CLI"
                If cmbFlashEngine.SelectedItem?.ToString() <> "esptool / Raw BIN" Then cmbFlashEngine.SelectedItem = "Arduino CLI"
            End If
            UpdateOutputHint()
            UpdateDashboard("READY", "")
        End Sub

        Private Function EffectiveBuildEngine() As String
            Dim selected = cmbEngine.SelectedItem?.ToString()
            If String.IsNullOrWhiteSpace(selected) OrElse selected = "Auto" Then
                Dim info = _detector.Detect(txtProject.Text.Trim())
                If info.HasPlatformIo Then Return "PlatformIO"
                If info.HasArduinoSketch Then Return "Arduino CLI"
                Return "Raw BIN"
            End If
            Return selected
        End Function

        Private Function EffectiveFlashEngine() As String
            Dim selected = cmbFlashEngine.SelectedItem?.ToString()
            If String.IsNullOrWhiteSpace(selected) OrElse selected = "Auto" Then
                Dim info = _detector.Detect(txtProject.Text.Trim())
                If info.HasPlatformIo Then Return "PlatformIO"
                If info.HasArduinoSketch Then Return "Arduino CLI"
                Return "esptool / Raw BIN"
            End If
            Return selected
        End Function
    End Class
End Namespace
