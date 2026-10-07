Imports System.IO

Namespace ESPFlashStudio.Services
    Public Class ToolLocator
        Public Function FindPlatformIo(configuredPath As String) As String
            Return If(File.Exists(AppPaths.PlatformIoExe), AppPaths.PlatformIoExe, "")
        End Function

        Public Function FindArduinoCli(configuredPath As String) As String
            Return If(File.Exists(AppPaths.ArduinoCliExe), AppPaths.ArduinoCliExe, "")
        End Function

        Public Function FindPython(configuredPath As String) As String
            If File.Exists(AppPaths.PlatformIoPython) Then Return AppPaths.PlatformIoPython
            If File.Exists(AppPaths.PortablePython) Then Return AppPaths.PortablePython
            Return ""
        End Function

        Public Function FindEsptool() As String
            Return If(File.Exists(AppPaths.EsptoolExe), AppPaths.EsptoolExe, "")
        End Function

        Public Function FindGit() As String
            Return If(File.Exists(AppPaths.GitExe), AppPaths.GitExe, "")
        End Function
    End Class
End Namespace
