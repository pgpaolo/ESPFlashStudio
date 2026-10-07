Imports System.Drawing
Imports System.Windows.Forms

Namespace ESPFlashStudio.Services
    Friend NotInheritable Class UiTheme
        Private Sub New()
        End Sub

        Public Shared ReadOnly Graphite As Color = Color.FromArgb(43, 47, 51)
        Public Shared ReadOnly Accent As Color = Color.FromArgb(55, 103, 83)
        Public Shared ReadOnly AccentDark As Color = Color.FromArgb(42, 82, 65)
        Public Shared ReadOnly Surface As Color = Color.White
        Public Shared ReadOnly Canvas As Color = Color.FromArgb(246, 245, 242)
        Public Shared ReadOnly Border As Color = Color.FromArgb(213, 211, 205)
        Public Shared ReadOnly TextPrimary As Color = Color.FromArgb(48, 49, 46)
        Public Shared ReadOnly TextSecondary As Color = Color.FromArgb(104, 103, 98)
        Public Shared ReadOnly Success As Color = Color.FromArgb(16, 124, 65)
        Public Shared ReadOnly Warning As Color = Color.FromArgb(184, 110, 0)
        Public Shared ReadOnly [Error] As Color = Color.FromArgb(196, 43, 28)
        Public Shared ReadOnly ConsoleBack As Color = Color.FromArgb(30, 31, 29)
        Public Shared ReadOnly ConsoleText As Color = Color.FromArgb(224, 224, 218)

        Public Shared Sub Apply(form As MainForm)
            form.BackColor = Canvas
            form.Font = New Font("Segoe UI", 9.0F)
            form.pnlHeader.BackColor = Graphite
            form.lblAppTitle.ForeColor = Color.White
            form.lblAppSubtitle.ForeColor = Color.FromArgb(198, 198, 190)
            form.lblModeBadge.BackColor = Color.FromArgb(66, 70, 66)
            form.lblModeBadge.ForeColor = Color.FromArgb(204, 226, 212)
            form.tabMain.Font = New Font("Segoe UI Semibold", 9.0F)
            For Each page As TabPage In form.tabMain.TabPages
                page.BackColor = Canvas
            Next
            StyleTree(form)
            StyleGrid(form.gridFlash)
            form.txtLog.BackColor = ConsoleBack
            form.txtLog.ForeColor = ConsoleText
            form.txtLog.BorderStyle = BorderStyle.FixedSingle
            form.txtLog.Font = New Font("Cascadia Mono", 9.0F)
            form.txtLog.DetectUrls = False
            form.statusStrip.BackColor = Color.FromArgb(235, 233, 228)
            form.statusStrip.ForeColor = TextSecondary
            form.statusStrip.SizingGrip = False
        End Sub

        Private Shared Sub StyleTree(parent As Control)
            For Each c As Control In parent.Controls
                If TypeOf c Is Button Then
                    StyleButton(DirectCast(c, Button))
                ElseIf TypeOf c Is TextBox Then
                    Dim t = DirectCast(c, TextBox)
                    t.BorderStyle = BorderStyle.FixedSingle
                    t.BackColor = Surface
                    t.ForeColor = TextPrimary
                ElseIf TypeOf c Is ComboBox Then
                    Dim cb = DirectCast(c, ComboBox)
                    cb.FlatStyle = FlatStyle.Flat
                    cb.BackColor = Surface
                    cb.ForeColor = TextPrimary
                ElseIf TypeOf c Is NumericUpDown Then
                    c.BackColor = Surface
                    c.ForeColor = TextPrimary
                ElseIf TypeOf c Is GroupBox Then
                    c.ForeColor = TextPrimary
                    c.BackColor = Canvas
                ElseIf TypeOf c Is Label Then
                    If c.Name = "lblOutputHint" Then
                        c.ForeColor = TextSecondary
                    ElseIf c.Name <> "lblAppTitle" AndAlso c.Name <> "lblAppSubtitle" AndAlso c.Name <> "lblModeBadge" Then
                        c.ForeColor = TextPrimary
                    End If
                End If
                If c.HasChildren Then StyleTree(c)
            Next
        End Sub

        Private Shared Sub StyleButton(button As Button)
            button.FlatStyle = FlatStyle.Flat
            button.FlatAppearance.BorderSize = 1
            button.FlatAppearance.BorderColor = Border
            button.BackColor = Surface
            button.ForeColor = TextPrimary
            button.Cursor = Cursors.Hand
            button.Padding = New Padding(4, 0, 4, 0)
            Select Case button.Name
                Case "btnBuild", "btnFlash", "btnInstallTools", "btnOpenPackage"
                    button.BackColor = Accent
                    button.ForeColor = Color.White
                    button.FlatAppearance.BorderColor = Accent
                Case "btnErase"
                    button.BackColor = Color.FromArgb(255, 247, 235)
                    button.ForeColor = Warning
                    button.FlatAppearance.BorderColor = Color.FromArgb(239, 196, 130)
                Case "btnCancel"
                    button.BackColor = Color.FromArgb(255, 239, 237)
                    button.ForeColor = [Error]
                    button.FlatAppearance.BorderColor = Color.FromArgb(237, 176, 168)
            End Select
        End Sub

        Private Shared Sub StyleGrid(grid As DataGridView)
            grid.BackgroundColor = Surface
            grid.BorderStyle = BorderStyle.FixedSingle
            grid.GridColor = Border
            grid.EnableHeadersVisualStyles = False
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 230, 224)
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary
            grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
            grid.ColumnHeadersHeight = 34
            grid.DefaultCellStyle.BackColor = Surface
            grid.DefaultCellStyle.ForeColor = TextPrimary
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 232, 224)
            grid.DefaultCellStyle.SelectionForeColor = TextPrimary
            grid.DefaultCellStyle.Padding = New Padding(4)
            grid.RowTemplate.Height = 30
        End Sub
    End Class
End Namespace
