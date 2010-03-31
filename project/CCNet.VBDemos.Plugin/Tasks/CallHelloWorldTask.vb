Imports System.Diagnostics
Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Tasks

Public Class CallHelloWorldTask
    Inherits BaseExecutableTask

    Private myPersonsName As String
    <ReflectorProperty("name")> _
    Public Property PersonsName() As String
        Get
            Return myPersonsName
        End Get
        Set(ByVal value As String)
            myPersonsName = value
        End Set
    End Property

    Private myExecutable As String
    <ReflectorProperty("executable")> _
    Public Property Executable() As String
        Get
            Return myExecutable
        End Get
        Set(ByVal value As String)
            myExecutable = value
        End Set
    End Property

    Protected Overrides Function Execute(ByVal result As IIntegrationResult) As Boolean
        Dim processResult = TryToRun(CreateProcessInfo(result), result)
        result.AddTaskResult(New ProcessTaskResult(processResult))
        Return Not processResult.Failed
    End Function

    Protected Overrides Function GetProcessArguments(ByVal result As IIntegrationResult) As String
        Return """" & myPersonsName & """"
    End Function

    Protected Overrides Function GetProcessBaseDirectory(ByVal result As IIntegrationResult) As String
        Return result.WorkingDirectory
    End Function

    Protected Overrides Function GetProcessFilename() As String
        If String.IsNullOrEmpty(myExecutable) Then
            Return "HelloWorldApp"
        Else
            Return myExecutable
        End If
    End Function

    Protected Overrides Function GetProcessPriorityClass() As ProcessPriorityClass
        Return ProcessPriorityClass.Normal
    End Function

    Protected Overrides Function GetProcessTimeout() As Integer
        Return 300
    End Function

End Class
