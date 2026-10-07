Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports ESPFlashStudio.Services

Namespace ESPFlashStudio
    Friend Module Program
        <STAThread>
        Public Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            AddHandler Application.ThreadException, AddressOf OnThreadException
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf OnUnhandledException
            Application.Run(New MainForm())
        End Sub

        Private Sub OnThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
            WriteCrash(e.Exception)
            MessageBox.Show("Si è verificato un errore non gestito. Il dettaglio è stato salvato in Data\Logs\Crash.", "ESP Flash Studio", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Private Sub OnUnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
            Dim ex = TryCast(e.ExceptionObject, Exception)
            If ex IsNot Nothing Then WriteCrash(ex)
        End Sub

        Private Sub WriteCrash(ex As Exception)
            Try
                Dim folder = Path.Combine(AppPaths.DataRoot, "Logs", "Crash")
                Directory.CreateDirectory(folder)
                Dim crashFilePath = Path.Combine(folder, $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                Dim sb As New StringBuilder()
                sb.AppendLine("ESP Flash Studio 0.9.0-beta")
                sb.AppendLine(DateTimeOffset.Now.ToString("o"))
                sb.AppendLine(ex.ToString())
                System.IO.File.WriteAllText(crashFilePath, sb.ToString(), Encoding.UTF8)
            Catch
            End Try
        End Sub
    End Module
End Namespace
