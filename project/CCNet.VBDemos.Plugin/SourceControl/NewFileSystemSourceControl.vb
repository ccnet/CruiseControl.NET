Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core

<ReflectorType("fileSystemSource")> Public Class NewFileSystemSourceControl
    Implements ISourceControl

    Private myFiles As Dictionary(Of String, Date) = New Dictionary(Of String, Date)()

    Private myDirectoryName As String
    <ReflectorProperty("directory")> _
    Public Property DirectoryName() As String
        Get
            Return myDirectoryName
        End Get
        Set(ByVal value As String)
            myDirectoryName = value
        End Set
    End Property

    Public Function GetModifications(ByVal from As IIntegrationResult, _
                                     ByVal [to] As IIntegrationResult) As Modification() _
                                     Implements ISourceControl.GetModifications
        Dim newList = New Dictionary(Of String, DateTime)()
        Dim modifications = New List(Of Modification)()
        Dim directory = New DirectoryInfo(DirectoryName)
        Dim newFiles = directory.GetFiles()
        For Each file In newFiles
            Dim inList = myFiles.ContainsKey(file.FullName)
            If (Not inList Or (myFiles(file.FullName) < file.LastWriteTime)) Then
                newList.Add(file.FullName, file.LastWriteTime)
                Dim newModification = New Modification()
                newModification.FileName = file.Name
                newModification.ModifiedTime = file.LastWriteTime
                newModification.FolderName = file.DirectoryName
                If inList Then
                    newModification.Type = "Added"
                Else
                    newModification.Type = "Modified"
                End If

                modifications.Add(newModification)
            End If

                If (inList) Then
                myFiles.Remove(file.Name)
            End If
        Next

        For Each file In myFiles.Keys
            Dim oldModification = New Modification()
            oldModification.FileName = Path.GetFileName(file)
            oldModification.ModifiedTime = Date.Now
            oldModification.FolderName = Path.GetDirectoryName(file)
            oldModification.Type = "Deleted"
            modifications.Add(oldModification)
        Next

        myFiles = newList
        Return modifications.ToArray()
    End Function

    Public Sub GetSource(ByVal result As IIntegrationResult) _
    Implements ISourceControl.GetSource
        For Each Modification In result.Modifications
            Dim source = Path.Combine(Modification.FolderName, _
                Modification.FileName)
            Dim destination = result.BaseFromWorkingDirectory(Modification.FileName)
            If (File.Exists(source)) Then
                File.Copy(source, destination, True)
            Else
                File.Delete(destination)
            End If
        Next
    End Sub

    Public Sub Initialize(ByVal project As IProject) _
    Implements ISourceControl.Initialize

    End Sub

    Public Sub LabelSourceControl(ByVal result As IIntegrationResult) _
    Implements ISourceControl.LabelSourceControl
        Dim fileName = Path.Combine(myDirectoryName, _
                                    Date.Now.ToString("yyyyMMddHHmmss") & ".label")
        File.WriteAllText(fileName, result.Label)
        myFiles.Add(fileName, DateTime.Now)
    End Sub

    Public Sub Purge(ByVal project As IProject) _
    Implements ISourceControl.Purge

    End Sub
End Class
