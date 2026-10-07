Namespace ESPFlashStudio.Models
    Public Class AppSettings
        Public Property PlatformIoPath As String = "pio"
        Public Property ArduinoCliPath As String = "arduino-cli"
        Public Property PythonPath As String = "python"
        Public Property EsptoolCommand As String = "-m esptool"
        Public Property LastProjectFolder As String = ""
        Public Property LastOutputFolder As String = ""
        Public Property PreferredEngine As String = "Auto"
        Public Property VerifyAfterFlash As Boolean = True
        Public Property BackupBeforeFlash As Boolean = False
    End Class
End Namespace
