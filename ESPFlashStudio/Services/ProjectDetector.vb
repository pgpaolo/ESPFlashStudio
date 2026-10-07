Imports System.IO

Namespace ESPFlashStudio.Services
    Public Class ProjectDetector
        Public Class DetectionResult
            Public Property HasPlatformIo As Boolean
            Public Property HasArduinoSketch As Boolean
            Public Property PlatformIoIni As String = ""
            Public Property ArduinoSketch As String = ""
            Public Property SuggestedEngine As String = "Raw BIN"
        End Class

        Public Function Detect(folder As String) As DetectionResult
            Dim result As New DetectionResult()
            If Not Directory.Exists(folder) Then Return result

            Dim ini = Path.Combine(folder, "platformio.ini")
            If File.Exists(ini) Then
                result.HasPlatformIo = True
                result.PlatformIoIni = ini
                result.SuggestedEngine = "PlatformIO"
            End If

            Dim ino = Directory.GetFiles(folder, "*.ino", SearchOption.TopDirectoryOnly).FirstOrDefault()
            If ino IsNot Nothing Then
                result.HasArduinoSketch = True
                result.ArduinoSketch = ino
                If Not result.HasPlatformIo Then result.SuggestedEngine = "Arduino CLI"
            End If

            Return result
        End Function

        Public Function ReadPlatformIoEnvironments(iniPath As String) As List(Of String)
            Dim result As New List(Of String)()
            If Not File.Exists(iniPath) Then Return result
            For Each rawLine In File.ReadAllLines(iniPath)
                Dim line = rawLine.Trim()
                If line.StartsWith("[env:", StringComparison.OrdinalIgnoreCase) AndAlso line.EndsWith("]") Then
                    result.Add(line.Substring(5, line.Length - 6))
                End If
            Next
            Return result
        End Function
    End Class
End Namespace
