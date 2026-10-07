Imports System.IO

Namespace ESPFlashStudio.Services
    Public NotInheritable Class AppPaths
        Private Sub New()
        End Sub

        Public Shared ReadOnly Property AppRoot As String
            Get
                Return Path.GetFullPath(AppContext.BaseDirectory)
            End Get
        End Property

        Public Shared ReadOnly Property ToolsRoot As String
            Get
                Return Path.Combine(AppRoot, "Tools")
            End Get
        End Property

        Public Shared ReadOnly Property DataRoot As String
            Get
                Return Path.Combine(AppRoot, "Data")
            End Get
        End Property

        Public Shared ReadOnly Property PlatformIoRoot As String
            Get
                Return Path.Combine(ToolsRoot, "PlatformIO")
            End Get
        End Property

        Public Shared ReadOnly Property PlatformIoExe As String
            Get
                Return Path.Combine(PlatformIoRoot, "penv", "Scripts", "pio.exe")
            End Get
        End Property

        Public Shared ReadOnly Property PlatformIoPython As String
            Get
                Return Path.Combine(PlatformIoRoot, "penv", "Scripts", "python.exe")
            End Get
        End Property

        Public Shared ReadOnly Property PortablePython As String
            Get
                Return Path.Combine(ToolsRoot, "Python", "python.exe")
            End Get
        End Property

        Public Shared ReadOnly Property ArduinoCliExe As String
            Get
                Return Path.Combine(ToolsRoot, "ArduinoCli", "arduino-cli.exe")
            End Get
        End Property

        Public Shared ReadOnly Property EsptoolExe As String
            Get
                Return Path.Combine(ToolsRoot, "Esptool", "esptool.exe")
            End Get
        End Property

        Public Shared ReadOnly Property GitExe As String
            Get
                Return Path.Combine(ToolsRoot, "Git", "cmd", "git.exe")
            End Get
        End Property

        Public Shared ReadOnly Property SettingsFile As String
            Get
                Return Path.Combine(DataRoot, "settings.json")
            End Get
        End Property

        Public Shared Sub EnsureDirectories()
            Directory.CreateDirectory(ToolsRoot)
            Directory.CreateDirectory(DataRoot)
            Directory.CreateDirectory(PlatformIoRoot)
        End Sub
    End Class
End Namespace
