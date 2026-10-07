Imports System.IO
Imports System.Text.Json
Imports ESPFlashStudio.Models

Namespace ESPFlashStudio.Services
    Public Class SettingsService
        Private ReadOnly _settingsPath As String = AppPaths.SettingsFile

        Public Function Load() As AppSettings
            Try
                If File.Exists(_settingsPath) Then
                    Dim json = File.ReadAllText(_settingsPath)
                    Dim settings = JsonSerializer.Deserialize(Of AppSettings)(json)
                    If settings IsNot Nothing Then Return settings
                End If
            Catch
            End Try
            Return New AppSettings()
        End Function

        Public Sub Save(settings As AppSettings)
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath))
            Dim options As New JsonSerializerOptions With {.WriteIndented = True}
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings, options))
        End Sub
    End Class
End Namespace
