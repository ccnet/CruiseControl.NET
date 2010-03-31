Imports System
Imports System.IO
Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core

<ReflectorType("indexFileSource")> Public Class IndexFileSourceControl
    Implements ISourceControl

    Private myFileName As String
    <ReflectorProperty("file")> _
    Public Property FileName() As String
        Get
            Return myFileName
        End Get
        Set(ByVal value As String)
            myFileName = value
        End Set
    End Property

    Public Function GetModifications(ByVal from As IIntegrationResult, _
                                     ByVal [to] As IIntegrationResult) As Modification() _
                                     Implements ISourceControl.GetModifications
        Dim path = [to].BaseFromWorkingDirectory(FileName)
        Dim reader = New StreamReader(path)
        Try
            Dim parser = New IndexFileHistoryParser()
            Return parser.Parse(reader, from.StartTime, [to].StartTime)
        Finally
            reader.Dispose()
        End Try
    End Function

    Public Sub GetSource(ByVal result As IIntegrationResult) _
    Implements ISourceControl.GetSource

    End Sub

    Public Sub Initialize(ByVal project As IProject) _
    Implements ISourceControl.Initialize

    End Sub

    Public Sub LabelSourceControl(ByVal result As IIntegrationResult) _
    Implements ISourceControl.LabelSourceControl

    End Sub

    Public Sub Purge(ByVal project As IProject) _
    Implements ISourceControl.Purge

    End Sub
End Class
