Imports System.Windows.Forms
Imports System.Drawing

Namespace ESPFlashStudio
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class MainForm
        Inherits Form

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then components.Dispose()
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            components = New System.ComponentModel.Container()

            pnlHeader = New Panel()
            lblAppTitle = New Label()
            lblAppSubtitle = New Label()
            lblModeBadge = New Label()
            lblVersion = New Label()

            tabMain = New TabControl()
            tabProject = New TabPage()
            tabFlash = New TabPage()
            tabTools = New TabPage()
            tabLog = New TabPage()

            txtProject = New TextBox()
            btnBrowseProject = New Button()
            lblProject = New Label()
            txtOutput = New TextBox()
            btnBrowseOutput = New Button()
            lblOutput = New Label()
            lblOutputHint = New Label()
            cmbEngine = New ComboBox()
            lblEngine = New Label()
            cmbBoard = New ComboBox()
            lblBoard = New Label()
            cmbEnvironment = New ComboBox()
            lblEnvironment = New Label()
            btnBuild = New Button()
            btnOpenOutput = New Button()
            btnOpenPackage = New Button()
            lblDetected = New Label()
            grpDashboard = New GroupBox()
            lblDashProject = New Label()
            lblDashToolchain = New Label()
            lblDashArtifact = New Label()
            lblDashState = New Label()

            cmbFlashEngine = New ComboBox()
            lblFlashEngine = New Label()
            cmbPort = New ComboBox()
            lblPort = New Label()
            btnRefreshPorts = New Button()
            nudBaud = New NumericUpDown()
            lblBaud = New Label()
            btnFlash = New Button()
            btnChipInfo = New Button()
            btnErase = New Button()
            gridFlash = New DataGridView()
            colOffset = New DataGridViewTextBoxColumn()
            colFile = New DataGridViewTextBoxColumn()
            btnAddBin = New Button()
            btnRemoveBin = New Button()
            chkVerifyAfterFlash = New CheckBox()
            chkBackupBeforeFlash = New CheckBox()

            txtPio = New TextBox()
            lblPio = New Label()
            txtArduinoCli = New TextBox()
            lblArduinoCli = New Label()
            txtPython = New TextBox()
            lblPython = New Label()
            txtEsptoolArgs = New TextBox()
            lblEsptoolArgs = New Label()
            btnDetectTools = New Button()
            btnInstallTools = New Button()

            txtLog = New RichTextBox()
            btnClearLog = New Button()
            btnSaveLog = New Button()
            btnCancel = New Button()

            statusStrip = New StatusStrip()
            lblStatus = New ToolStripStatusLabel()
            progress = New ToolStripProgressBar()

            SuspendLayout()

            ' Header
            pnlHeader.Dock = DockStyle.Top
            pnlHeader.Height = 72
            pnlHeader.Padding = New Padding(24, 8, 24, 8)
            lblAppTitle.Text = "ESP Flash Studio"
            lblAppTitle.Font = New Font("Segoe UI Semibold", 18.0F, FontStyle.Bold)
            lblAppTitle.AutoSize = True
            lblAppTitle.Location = New Point(22, 8)
            lblAppSubtitle.Text = "Portable build, artifact management and flashing workstation for ESP32 / ESP8266"
            lblAppSubtitle.AutoSize = True
            lblAppSubtitle.Location = New Point(24, 45)
            lblModeBadge.Text = "PORTABLE TOOLCHAIN"
            lblModeBadge.AutoSize = True
            lblModeBadge.Padding = New Padding(10, 5, 10, 5)
            lblModeBadge.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblModeBadge.Location = New Point(865, 17)
            lblVersion.Text = "0.9.0-beta"
            lblVersion.AutoSize = True
            lblVersion.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblVersion.Location = New Point(930, 49)
            pnlHeader.Controls.AddRange(New Control() {lblAppTitle, lblAppSubtitle, lblModeBadge, lblVersion})

            ' Main tabs
            tabMain.Dock = DockStyle.Fill
            tabMain.Padding = New Point(18, 7)
            tabMain.Controls.AddRange(New Control() {tabProject, tabFlash, tabTools, tabLog})
            tabProject.Text = "Project / Build"
            tabFlash.Text = "Flash device"
            tabTools.Text = "Tools"
            tabLog.Text = "Log"
            For Each page As TabPage In tabMain.TabPages
                page.Padding = New Padding(18)
            Next

            ' Project tab
            lblProject.Text = "Project folder"
            lblProject.SetBounds(22, 28, 125, 24)
            txtProject.SetBounds(160, 25, 720, 26)
            txtProject.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            btnBrowseProject.Text = "Browse"
            btnBrowseProject.SetBounds(890, 24, 80, 28)
            btnBrowseProject.Anchor = AnchorStyles.Top Or AnchorStyles.Right

            lblOutput.Text = "Artifact root"
            lblOutput.SetBounds(22, 68, 125, 24)
            txtOutput.SetBounds(160, 65, 720, 26)
            txtOutput.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            btnBrowseOutput.Text = "Browse"
            btnBrowseOutput.SetBounds(890, 64, 80, 28)
            btnBrowseOutput.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblOutputHint.Text = "Destination effective BIN: -"
            lblOutputHint.AutoEllipsis = True
            lblOutputHint.SetBounds(160, 95, 810, 22)
            lblOutputHint.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

            lblEngine.Text = "Build engine"
            lblEngine.SetBounds(22, 130, 125, 24)
            cmbEngine.DropDownStyle = ComboBoxStyle.DropDownList
            cmbEngine.Items.AddRange(New Object() {"Auto", "PlatformIO", "Arduino CLI", "Raw BIN"})
            cmbEngine.SetBounds(160, 127, 200, 26)

            lblBoard.Text = "Board"
            lblBoard.SetBounds(390, 130, 60, 24)
            cmbBoard.DropDownStyle = ComboBoxStyle.DropDownList
            cmbBoard.SetBounds(450, 127, 245, 26)

            lblEnvironment.Text = "PIO environment"
            lblEnvironment.SetBounds(710, 130, 110, 24)
            cmbEnvironment.DropDownStyle = ComboBoxStyle.DropDownList
            cmbEnvironment.SetBounds(825, 127, 145, 26)
            cmbEnvironment.Anchor = AnchorStyles.Top Or AnchorStyles.Right

            lblDetected.Text = "Project detection: -"
            lblDetected.SetBounds(22, 170, 600, 24)

            btnBuild.Text = "BUILD / CREATE BIN"
            btnBuild.SetBounds(650, 202, 155, 38)
            btnOpenOutput.Text = "Open artifacts"
            btnOpenOutput.SetBounds(815, 202, 155, 38)
            btnOpenPackage.Text = "Open package ZIP"
            btnOpenPackage.SetBounds(485, 202, 155, 38)

            grpDashboard.Text = "Readiness / operational state"
            grpDashboard.SetBounds(22, 270, 948, 150)
            grpDashboard.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblDashProject.SetBounds(20, 30, 700, 22)
            lblDashProject.Text = "Project: not evaluated"
            lblDashToolchain.SetBounds(20, 56, 700, 22)
            lblDashToolchain.Text = "Toolchain: not evaluated"
            lblDashArtifact.SetBounds(20, 82, 880, 22)
            lblDashArtifact.AutoEllipsis = True
            lblDashArtifact.Text = "Artifacts: -"
            lblDashState.SetBounds(805, 28, 110, 28)
            lblDashState.TextAlign = ContentAlignment.MiddleCenter
            lblDashState.Text = "READY"
            grpDashboard.Controls.AddRange(New Control() {lblDashProject, lblDashToolchain, lblDashArtifact, lblDashState})

            tabProject.Controls.AddRange(New Control() {
                lblProject, txtProject, btnBrowseProject, lblOutput, txtOutput, btnBrowseOutput, lblOutputHint,
                lblEngine, cmbEngine, lblBoard, cmbBoard, lblEnvironment, cmbEnvironment, lblDetected,
                btnBuild, btnOpenOutput, btnOpenPackage, grpDashboard
            })

            ' Flash tab
            lblFlashEngine.Text = "Flash engine"
            lblFlashEngine.SetBounds(22, 28, 85, 24)
            cmbFlashEngine.DropDownStyle = ComboBoxStyle.DropDownList
            cmbFlashEngine.Items.AddRange(New Object() {"Auto", "PlatformIO", "Arduino CLI", "esptool / Raw BIN"})
            cmbFlashEngine.SetBounds(115, 25, 170, 26)

            lblPort.Text = "COM"
            lblPort.SetBounds(310, 28, 40, 24)
            cmbPort.DropDownStyle = ComboBoxStyle.DropDownList
            cmbPort.SetBounds(355, 25, 120, 26)
            btnRefreshPorts.Text = "Refresh"
            btnRefreshPorts.SetBounds(485, 24, 85, 28)

            lblBaud.Text = "Baud"
            lblBaud.SetBounds(595, 28, 45, 24)
            nudBaud.Minimum = 9600D
            nudBaud.Maximum = 2000000D
            nudBaud.Increment = 115200D
            nudBaud.Value = 460800D
            nudBaud.SetBounds(645, 25, 135, 26)

            btnChipInfo.Text = "Detect chip"
            btnChipInfo.SetBounds(22, 70, 125, 34)
            btnErase.Text = "Erase flash"
            btnErase.SetBounds(157, 70, 125, 34)
            btnFlash.Text = "FLASH DEVICE"
            btnFlash.SetBounds(805, 70, 165, 36)
            btnFlash.Anchor = AnchorStyles.Top Or AnchorStyles.Right

            chkVerifyAfterFlash.Text = "Verify after flash"
            chkVerifyAfterFlash.AutoSize = True
            chkVerifyAfterFlash.SetBounds(310, 78, 125, 24)
            chkBackupBeforeFlash.Text = "Backup before flash"
            chkBackupBeforeFlash.AutoSize = True
            chkBackupBeforeFlash.SetBounds(450, 78, 140, 24)

            gridFlash.SetBounds(22, 125, 948, 365)
            gridFlash.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            gridFlash.AllowUserToAddRows = False
            gridFlash.AllowUserToDeleteRows = False
            gridFlash.RowHeadersVisible = False
            gridFlash.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            gridFlash.MultiSelect = False
            gridFlash.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            colOffset.Name = "colOffset"
            colOffset.HeaderText = "Offset"
            colOffset.FillWeight = 20
            colFile.Name = "colFile"
            colFile.HeaderText = "BIN file"
            colFile.FillWeight = 80
            gridFlash.Columns.AddRange(New DataGridViewColumn() {colOffset, colFile})

            btnAddBin.Text = "Add BIN"
            btnAddBin.SetBounds(720, 505, 120, 30)
            btnAddBin.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnRemoveBin.Text = "Remove row"
            btnRemoveBin.SetBounds(850, 505, 120, 30)
            btnRemoveBin.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right

            tabFlash.Controls.AddRange(New Control() {
                lblFlashEngine, cmbFlashEngine, lblPort, cmbPort, btnRefreshPorts, lblBaud, nudBaud,
                btnChipInfo, btnErase, btnFlash, chkVerifyAfterFlash, chkBackupBeforeFlash,
                gridFlash, btnAddBin, btnRemoveBin
            })

            ' Tools tab
            AddToolRow(tabTools, lblPio, txtPio, "PlatformIO CLI", 28)
            AddToolRow(tabTools, lblArduinoCli, txtArduinoCli, "Arduino CLI", 70)
            AddToolRow(tabTools, lblPython, txtPython, "Python", 112)
            AddToolRow(tabTools, lblEsptoolArgs, txtEsptoolArgs, "esptool args", 154)
            btnInstallTools.Text = "Install integrated tools"
            btnInstallTools.SetBounds(605, 205, 200, 38)
            btnDetectTools.Text = "Detect tools"
            btnDetectTools.SetBounds(815, 205, 155, 38)
            tabTools.Controls.AddRange(New Control() {btnInstallTools, btnDetectTools})

            ' Log tab
            txtLog.SetBounds(18, 18, 952, 500)
            txtLog.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            txtLog.ReadOnly = True
            txtLog.WordWrap = False
            txtLog.ScrollBars = RichTextBoxScrollBars.Both
            btnSaveLog.Text = "Export log"
            btnSaveLog.SetBounds(585, 535, 120, 32)
            btnSaveLog.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnClearLog.Text = "Clear log"
            btnClearLog.SetBounds(715, 535, 120, 32)
            btnClearLog.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnCancel.Text = "Cancel"
            btnCancel.SetBounds(845, 535, 125, 32)
            btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            tabLog.Controls.AddRange(New Control() {txtLog, btnSaveLog, btnClearLog, btnCancel})

            ' Status
            statusStrip.Items.AddRange(New ToolStripItem() {lblStatus, progress})
            statusStrip.Dock = DockStyle.Bottom
            lblStatus.Spring = True
            lblStatus.TextAlign = ContentAlignment.MiddleLeft
            lblStatus.Text = "Ready"
            progress.Style = ProgressBarStyle.Marquee
            progress.Visible = False

            ' Form
            AutoScaleMode = AutoScaleMode.Font
            ClientSize = New Size(1100, 760)
            MinimumSize = New Size(1000, 700)
            StartPosition = FormStartPosition.CenterScreen
            Text = "ESP Flash Studio - ESP32 / ESP8266"
            Controls.Add(tabMain)
            Controls.Add(pnlHeader)
            Controls.Add(statusStrip)

            ResumeLayout(False)
            PerformLayout()
        End Sub

        Private Shared Sub AddToolRow(page As TabPage, label As Label, box As TextBox, caption As String, y As Integer)
            label.Text = caption
            label.SetBounds(22, y + 3, 135, 24)
            box.SetBounds(170, y, 800, 26)
            box.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            page.Controls.Add(label)
            page.Controls.Add(box)
        End Sub

        Friend WithEvents pnlHeader As Panel
        Friend WithEvents lblAppTitle As Label
        Friend WithEvents lblAppSubtitle As Label
        Friend WithEvents lblModeBadge As Label
        Friend WithEvents lblVersion As Label
        Friend WithEvents tabMain As TabControl
        Friend WithEvents tabProject As TabPage
        Friend WithEvents lblDetected As Label
        Friend WithEvents grpDashboard As GroupBox
        Friend WithEvents lblDashProject As Label
        Friend WithEvents lblDashToolchain As Label
        Friend WithEvents lblDashArtifact As Label
        Friend WithEvents lblDashState As Label
        Friend WithEvents btnOpenPackage As Button
        Friend WithEvents btnOpenOutput As Button
        Friend WithEvents btnBuild As Button
        Friend WithEvents cmbEnvironment As ComboBox
        Friend WithEvents lblEnvironment As Label
        Friend WithEvents cmbBoard As ComboBox
        Friend WithEvents lblBoard As Label
        Friend WithEvents cmbEngine As ComboBox
        Friend WithEvents lblEngine As Label
        Friend WithEvents btnBrowseOutput As Button
        Friend WithEvents txtOutput As TextBox
        Friend WithEvents lblOutput As Label
        Friend WithEvents lblOutputHint As Label
        Friend WithEvents btnBrowseProject As Button
        Friend WithEvents txtProject As TextBox
        Friend WithEvents lblProject As Label
        Friend WithEvents tabFlash As TabPage
        Friend WithEvents btnRemoveBin As Button
        Friend WithEvents chkVerifyAfterFlash As CheckBox
        Friend WithEvents chkBackupBeforeFlash As CheckBox
        Friend WithEvents btnAddBin As Button
        Friend WithEvents gridFlash As DataGridView
        Friend WithEvents colOffset As DataGridViewTextBoxColumn
        Friend WithEvents colFile As DataGridViewTextBoxColumn
        Friend WithEvents btnErase As Button
        Friend WithEvents btnChipInfo As Button
        Friend WithEvents btnFlash As Button
        Friend WithEvents nudBaud As NumericUpDown
        Friend WithEvents lblBaud As Label
        Friend WithEvents btnRefreshPorts As Button
        Friend WithEvents cmbPort As ComboBox
        Friend WithEvents lblPort As Label
        Friend WithEvents cmbFlashEngine As ComboBox
        Friend WithEvents lblFlashEngine As Label
        Friend WithEvents tabTools As TabPage
        Friend WithEvents btnInstallTools As Button
        Friend WithEvents btnDetectTools As Button
        Friend WithEvents txtEsptoolArgs As TextBox
        Friend WithEvents lblEsptoolArgs As Label
        Friend WithEvents txtPython As TextBox
        Friend WithEvents lblPython As Label
        Friend WithEvents txtArduinoCli As TextBox
        Friend WithEvents lblArduinoCli As Label
        Friend WithEvents txtPio As TextBox
        Friend WithEvents lblPio As Label
        Friend WithEvents tabLog As TabPage
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnSaveLog As Button
        Friend WithEvents btnClearLog As Button
        Friend WithEvents txtLog As RichTextBox
        Friend WithEvents statusStrip As StatusStrip
        Friend WithEvents lblStatus As ToolStripStatusLabel
        Friend WithEvents progress As ToolStripProgressBar
    End Class
End Namespace
