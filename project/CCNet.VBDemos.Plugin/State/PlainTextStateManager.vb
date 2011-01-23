Imports System
Imports System.IO
Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.State
Imports ThoughtWorks.CruiseControl.Core.Util
Imports ThoughtWorks.CruiseControl.Remote

<ReflectorType("plainTextState")> Public Class PlainTextStateManager
    Implements IStateManager

    Private ReadOnly myFileSystem As IFileSystem

    Public Sub New()
        Me.New(New SystemIoFileSystem())
    End Sub

    Public Sub New(ByVal fileSystem As IFileSystem)
        myFileSystem = fileSystem
    End Sub

    Public Function HasPreviousState(ByVal project As String) As Boolean _
    Implements IStateManager.HasPreviousState
        Dim filePath = GeneratePath(project)
        Return myFileSystem.FileExists(filePath)
    End Function

    Public Function LoadState(ByVal project As String) As IIntegrationResult _
    Implements IStateManager.LoadState
        Dim filePath = GeneratePath(project)
        Dim reader = New StreamReader(myFileSystem.OpenInputStream(filePath))
        Try
            Dim statusType = GetType(IntegrationStatus)
            Dim status = CType([Enum].Parse(statusType, reader.ReadLine()), IntegrationStatus)
            Dim lastSummary = New IntegrationSummary(status, _
                reader.ReadLine(), _
                reader.ReadLine(), _
                DateTime.Parse(reader.ReadLine()))
            Dim result = New IntegrationResult(project, _
                reader.ReadLine(), _
                reader.ReadLine(), _
                Nothing, _
                lastSummary)
            Return result
        Finally
            reader.Dispose()
        End Try
    End Function

    Public Sub SaveState(ByVal result As IIntegrationResult) _
    Implements IStateManager.SaveState
        Dim filePath = GeneratePath(result.ProjectName)
        Dim writer = New StreamWriter(myFileSystem.OpenOutputStream(filePath))
        Try
            writer.WriteLine(result.Status)
            writer.WriteLine(result.Label)
            writer.WriteLine(result.LastSuccessfulIntegrationLabel)
            writer.WriteLine(result.StartTime.ToString("o"))
            writer.WriteLine(result.WorkingDirectory)
            writer.WriteLine(result.ArtifactDirectory)
        Finally
            writer.Dispose()
        End Try
    End Sub

    Private Function GeneratePath(ByVal project As String) As String
        Dim filePath = Path.Combine(Environment.CurrentDirectory, project + ".txt")
        Return filePath
    End Function
End Class
