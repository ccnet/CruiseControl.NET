Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Config
Imports ThoughtWorks.CruiseControl.Core.Tasks
Imports ThoughtWorks.CruiseControl.Remote

<ReflectorType("helloWorld2")> Public Class HelloWorldTask2
    Inherits TaskBase
    Implements IConfigurationValidation

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

    Private myRepeatCount As Integer = 1
    <ReflectorProperty("count", Required:=False)> _
    Public Property RepeatCount() As Integer
        Get
            Return myRepeatCount
        End Get
        Set(ByVal value As Integer)
            myRepeatCount = value
        End Set
    End Property

    Protected Overrides Function Execute(ByVal result As IIntegrationResult) As Boolean
        result.BuildProgressInformation() _
                .SignalStartRunTask("Sending a hello world greeting")

        For i = 0 To myRepeatCount
            Dim taskresult = New HelloWorldTaskResult(myPersonsName, result)
            result.AddTaskResult(taskresult)
        Next

        Return True
    End Function

    Public Sub Validate(ByVal configuration As IConfiguration, _
                        ByVal parent As ConfigurationTrace, _
                        ByVal errorProcesser As IConfigurationErrorProcesser) _
                        Implements IConfigurationValidation.Validate
        If myRepeatCount <= 0 Then
            errorProcesser.ProcessWarning("count is less than 1!")
        End If
    End Sub

End Class
