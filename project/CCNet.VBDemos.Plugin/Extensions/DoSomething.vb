Imports System
Imports System.Linq
Imports ThoughtWorks.CruiseControl.Remote
Imports ThoughtWorks.CruiseControl.Remote.Events

Public Class DoSomething
    Implements ICruiseServerExtension

    Private myServer As ICruiseServer
    Private myCount As Integer = 0
    Private myMaxCount As Integer = 4

    Public Sub Abort() _
    Implements ICruiseServerExtension.Abort
        Console.WriteLine("Abort")
    End Sub

    Public Sub Initialise(ByVal server As ICruiseServer, _
                          ByVal extensionConfig As ExtensionConfiguration) _
                          Implements ICruiseServerExtension.Initialise
        Dim projectsElement = extensionConfig.Items _
            .SingleOrDefault(Function(n) n.Name = "allowedProjects")
        If projectsElement IsNot Nothing Then
            myMaxCount = Integer.Parse(projectsElement.InnerText) - 1
        End If

        myServer = server
        AddHandler myServer.ProjectStarting, AddressOf Project_Starting
        AddHandler myServer.ProjectStopped, AddressOf Project_Stopped
        Console.WriteLine("Initialise")
    End Sub

    Private Sub Project_Starting(ByVal sender As Object, ByVal e As CancelProjectEventArgs)
        If myCount >= myMaxCount Then
            e.Cancel = True
        Else
            myCount += 1
        End If
    End Sub

    Private Sub Project_Stopped(ByVal sender As Object, ByVal e As ProjectEventArgs)
        myCount -= 1
    End Sub

    Public Sub Start() _
    Implements ICruiseServerExtension.Start
        Console.WriteLine("Start")
    End Sub

    Public Sub [Stop]() _
    Implements ICruiseServerExtension.Stop
        Console.WriteLine("Stop")
    End Sub
End Class
