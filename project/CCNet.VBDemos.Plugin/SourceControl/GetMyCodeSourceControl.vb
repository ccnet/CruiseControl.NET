Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Sourcecontrol
Imports ThoughtWorks.CruiseControl.Core.Tasks
Imports ThoughtWorks.CruiseControl.Core.Util

<ReflectorType("getMyCode")> Public Class GetMyCodeSourceControl
    Inherits ProcessSourceControl

    Public Sub New()
        Me.new(New ProcessExecutor(), New IndexFileHistoryParser())
    End Sub

    Public Sub New(ByVal executor As ProcessExecutor, _
                   ByVal parser As IHistoryParser)
        MyBase.New(parser, executor)
    End Sub

    Private myExecutable As String
    <ReflectorProperty("executable", Required:=False)> _
    Public Property Executable() As String
        Get
            Return myExecutable
        End Get
        Set(ByVal value As String)
            myExecutable = value
        End Set
    End Property

    Private mySource As String
    <ReflectorProperty("source")> _
    Public Property Source() As String
        Get
            Return mySource
        End Get
        Set(ByVal value As String)
            mySource = value
        End Set
    End Property

    Public Overloads Overrides Function GetModifications(ByVal from As IIntegrationResult, _
                                                         ByVal [to] As IIntegrationResult) As Modification()
        Dim processResult = ExecuteCommand([to], "list")
        Dim modifications = ParseModifications(processResult, _
            from.StartTime, [to].StartTime)
        Return modifications
    End Function

    Public Overrides Sub LabelSourceControl(ByVal result As IIntegrationResult)
        If result.Succeeded Then
            Dim processResult = ExecuteCommand(result, "label", result.Label)
            result.AddTaskResult(New ProcessTaskResult(processResult))
        End If
    End Sub

    Public Overrides Sub GetSource(ByVal result As IIntegrationResult)
        Dim processResult = ExecuteCommand(result, "get")
        result.AddTaskResult(New ProcessTaskResult(processResult))
    End Sub

    Private Function ExecuteCommand(ByVal result As IIntegrationResult, _
            ByVal command As String, Optional ByVal arg As String = Nothing)
        Dim buffer = New PrivateArguments(command)
        buffer.Add(Source)
        If arg IsNot Nothing Then
            buffer.Add(String.Empty, arg, True)
        End If
        Dim executable As String
        If String.IsNullOrEmpty(myExecutable) Then
            executable = "GetMyCode"
        Else
            executable = myExecutable
        End If
        Dim processInfo = New ProcessInfo( _
            result.BaseFromWorkingDirectory(executable), _
            buffer, _
            result.WorkingDirectory)
        Dim processResult = Execute(processInfo)
        Return processResult
    End Function
End Class
