Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports ESPFlashStudio.Models

Namespace ESPFlashStudio.Services
    Public Class ArtifactExportResult
        Public Property Items As New List(Of FlashItem)()
        Public Property VersionFolder As String = ""
        Public Property LatestFolder As String = ""
        Public Property ManifestPath As String = ""
        Public Property ZipPath As String = ""
    End Class

    Public Class BuildService
        Private ReadOnly _runner As ProcessRunner

        Public Sub New(runner As ProcessRunner)
            _runner = runner
        End Sub

        Public Async Function BuildPlatformIoAsync(pioPath As String,
                                                   projectFolder As String,
                                                   environmentName As String,
                                                   ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Dim args = $"run -d {Q(projectFolder)}"
            If Not String.IsNullOrWhiteSpace(environmentName) Then args &= $" -e {Q(environmentName)}"
            Return Await _runner.RunAsync(pioPath, args, projectFolder, ct)
        End Function

        Public Async Function BuildArduinoAsync(cliPath As String,
                                                projectFolder As String,
                                                fqbn As String,
                                                outputFolder As String,
                                                ct As Threading.CancellationToken) As Task(Of ProcessResult)
            Directory.CreateDirectory(outputFolder)
            Dim args = $"compile --fqbn {Q(fqbn)} --output-dir {Q(outputFolder)} {Q(projectFolder)}"
            Return Await _runner.RunAsync(cliPath, args, projectFolder, ct)
        End Function

        Public Function FindPlatformIoBinaries(projectFolder As String, environmentName As String) As List(Of FlashItem)
            Dim buildDir = Path.Combine(projectFolder, ".pio", "build", environmentName)
            If Not Directory.Exists(buildDir) Then Return New List(Of FlashItem)()

            Dim parsed = ParseFlashArgs(buildDir)
            If parsed.Count > 0 Then Return parsed

            ' Fallback solo se il framework non ha prodotto flash_args.
            Dim items As New List(Of FlashItem)()
            Dim firmware = Path.Combine(buildDir, "firmware.bin")
            Dim bootloader = Path.Combine(buildDir, "bootloader.bin")
            Dim partitions = Path.Combine(buildDir, "partitions.bin")
            Dim bootApp0 = Path.Combine(buildDir, "boot_app0.bin")

            If File.Exists(bootloader) Then items.Add(New FlashItem With {.Offset = "0x1000", .FilePath = bootloader})
            If File.Exists(partitions) Then items.Add(New FlashItem With {.Offset = "0x8000", .FilePath = partitions})
            If File.Exists(bootApp0) Then items.Add(New FlashItem With {.Offset = "0xE000", .FilePath = bootApp0})
            If File.Exists(firmware) Then items.Add(New FlashItem With {.Offset = "0x10000", .FilePath = firmware})
            Return items
        End Function

        Private Function ParseFlashArgs(buildDir As String) As List(Of FlashItem)
            Dim result As New List(Of FlashItem)()
            Dim candidates = New String() {
                Path.Combine(buildDir, "flash_args"),
                Path.Combine(buildDir, "flash_args.txt"),
                Path.Combine(buildDir, "flash_project_args")
            }
            Dim argsFile = candidates.FirstOrDefault(Function(p) File.Exists(p))
            If String.IsNullOrWhiteSpace(argsFile) Then Return result

            For Each raw In File.ReadAllLines(argsFile)
                Dim line = raw.Trim()
                If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("--") Then Continue For
                Dim m = Regex.Match(line, "^(0x[0-9A-Fa-f]+|\d+)\s+(.+)$")
                If Not m.Success Then Continue For
                Dim offset = m.Groups(1).Value
                Dim token = m.Groups(2).Value.Trim().Trim(""""c)
                Dim resolved = ResolveBinary(buildDir, token)
                If Not String.IsNullOrWhiteSpace(resolved) AndAlso File.Exists(resolved) Then
                    result.Add(New FlashItem With {.Offset = offset, .FilePath = resolved})
                End If
            Next
            Return result
        End Function

        Private Function ResolveBinary(buildDir As String, token As String) As String
            If Path.IsPathRooted(token) AndAlso File.Exists(token) Then Return token
            Dim local = Path.Combine(buildDir, token)
            If File.Exists(local) Then Return local

            ' boot_app0.bin e altri componenti possono risiedere nei package PlatformIO.
            Try
                Dim name = Path.GetFileName(token)
                Dim matches = Directory.GetFiles(AppPaths.PlatformIoRoot, name, SearchOption.AllDirectories)
                If matches.Length > 0 Then Return matches(0)
            Catch
            End Try
            Return ""
        End Function

        Public Function ExportPlatformIoArtifacts(projectFolder As String,
                                                   environmentName As String,
                                                   outputRoot As String,
                                                   Optional appVersion As String = "0.9.0-beta") As ArtifactExportResult
            Dim sourceItems = FindPlatformIoBinaries(projectFolder, environmentName)
            Dim result As New ArtifactExportResult()
            If sourceItems.Count = 0 Then Return result

            If String.IsNullOrWhiteSpace(outputRoot) Then outputRoot = Path.Combine(projectFolder, "artifacts")
            Dim safeEnv = If(String.IsNullOrWhiteSpace(environmentName), "default", environmentName.Trim())
            Dim envRoot = Path.Combine(outputRoot, safeEnv)
            Dim stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss")
            Dim historyDir = Path.Combine(envRoot, "history", stamp)
            Dim latestDir = Path.Combine(envRoot, "latest")

            Directory.CreateDirectory(historyDir)
            If Directory.Exists(latestDir) Then Directory.Delete(latestDir, True)
            Directory.CreateDirectory(latestDir)

            For Each item In sourceItems
                Dim fileName = Path.GetFileName(item.FilePath)
                Dim versionTarget = Path.Combine(historyDir, fileName)
                Dim latestTarget = Path.Combine(latestDir, fileName)
                File.Copy(item.FilePath, versionTarget, True)
                File.Copy(item.FilePath, latestTarget, True)
                result.Items.Add(New FlashItem With {.Offset = item.Offset, .FilePath = latestTarget})
            Next

            result.VersionFolder = historyDir
            result.LatestFolder = latestDir
            result.ManifestPath = Path.Combine(latestDir, "firmware-manifest.json")
            WriteManifest(result.ManifestPath, appVersion, projectFolder, safeEnv, result.Items, historyDir)
            File.Copy(result.ManifestPath, Path.Combine(historyDir, Path.GetFileName(result.ManifestPath)), True)

            Dim txtManifest = Path.Combine(latestDir, "firmware-manifest.txt")
            WriteTextManifest(txtManifest, appVersion, projectFolder, safeEnv, result.Items, historyDir)
            File.Copy(txtManifest, Path.Combine(historyDir, Path.GetFileName(txtManifest)), True)

            result.ZipPath = Path.Combine(envRoot, $"ESPFlashStudio_{safeEnv}_{stamp}.zip")
            If File.Exists(result.ZipPath) Then File.Delete(result.ZipPath)
            ZipFile.CreateFromDirectory(historyDir, result.ZipPath, CompressionLevel.Optimal, False)
            Return result
        End Function

        ' Compatibilità con le chiamate della 0.3.x.
        Public Function ExportPlatformIoBinaries(projectFolder As String, environmentName As String, outputRoot As String) As List(Of FlashItem)
            Return ExportPlatformIoArtifacts(projectFolder, environmentName, outputRoot).Items
        End Function

        Private Shared Sub WriteManifest(manifestPath As String,
                                         appVersion As String,
                                         projectFolder As String,
                                         environmentName As String,
                                         items As IEnumerable(Of FlashItem),
                                         versionFolder As String)
            Dim files = items.Select(Function(item) New With {
                .offset = item.Offset,
                .file = System.IO.Path.GetFileName(item.FilePath),
                .size = New FileInfo(item.FilePath).Length,
                .sha256 = ComputeSha256(item.FilePath)
            }).ToArray()
            Dim payload = New With {
                .schema = "espflashstudio-artifact-v1",
                .app_version = appVersion,
                .generated = DateTimeOffset.Now.ToString("o"),
                .project = projectFolder,
                .environment = environmentName,
                .history_folder = versionFolder,
                .files = files
            }
            System.IO.File.WriteAllText(manifestPath, JsonSerializer.Serialize(payload, New JsonSerializerOptions With {.WriteIndented = True}), Encoding.UTF8)
        End Sub

        Private Shared Sub WriteTextManifest(manifestPath As String,
                                             appVersion As String,
                                             projectFolder As String,
                                             environmentName As String,
                                             items As IEnumerable(Of FlashItem),
                                             versionFolder As String)
            Using sw As New StreamWriter(manifestPath, False, Encoding.UTF8)
                sw.WriteLine("ESP Flash Studio - Firmware Artifact Manifest")
                sw.WriteLine("Version: " & appVersion)
                sw.WriteLine("Generated: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                sw.WriteLine("Project: " & projectFolder)
                sw.WriteLine("Environment: " & environmentName)
                sw.WriteLine("History: " & versionFolder)
                sw.WriteLine()
                For Each item In items
                    Dim fi As New FileInfo(item.FilePath)
                    sw.WriteLine($"{item.Offset}    {System.IO.Path.GetFileName(item.FilePath)}    {fi.Length} bytes    SHA256={ComputeSha256(item.FilePath)}")
                Next
            End Using
        End Sub

        Private Shared Function ComputeSha256(filePath As String) As String
            Using sha As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create(), stream As FileStream = System.IO.File.OpenRead(filePath)
                Return Convert.ToHexString(sha.ComputeHash(stream)).ToLowerInvariant()
            End Using
        End Function

        Private Shared Function Q(value As String) As String
            Dim dq As String = ChrW(34).ToString()
            Return dq & value.Replace(dq, "\" & dq) & dq
        End Function
    End Class
End Namespace
