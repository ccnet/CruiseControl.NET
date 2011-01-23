Imports ThoughtWorks.CruiseControl.Core

Public Class HelloWorldTaskResult
    Implements ITaskResult

    Private ReadOnly myPersonName As String
    Private ReadOnly myResult As IIntegrationResult

    Public Sub New(ByVal personName As String, ByVal result As IIntegrationResult)
        myPersonName = personName
        myResult = result
    End Sub

    Public Function CheckIfSuccess() As Boolean _
    Implements ITaskResult.CheckIfSuccess
        Return True
    End Function

    Public ReadOnly Property Data() As String _
    Implements ITaskResult.Data
        Get
            Return "Hello " & myPersonName & _
                    " from " & myResult.ProjectName & _
                    "(build started " & myResult.StartTime.ToString() & ")"
        End Get
    End Property
End Class
