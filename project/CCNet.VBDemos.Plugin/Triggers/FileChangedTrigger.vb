Imports System
Imports System.IO
Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Config
Imports ThoughtWorks.CruiseControl.Core.Triggers
Imports ThoughtWorks.CruiseControl.Remote

<ReflectorType("fileChangedTrigger")> Public Class FileChangedTrigger
    Implements ITrigger, IConfigurationValidation

    Private myLastChanged As DateTime?

    Public Sub New()
        BuildCondition = BuildCondition.IfModificationExists
        Dim defaultTrigger = New IntervalTrigger()
        defaultTrigger.IntervalSeconds = 5
        InnerTrigger = defaultTrigger
    End Sub

    Private myMonitorFile As String
    <ReflectorProperty("file")> _
    Public Property MonitorFile() As String
        Get
            Return myMonitorFile
        End Get
        Set(ByVal value As String)
            myMonitorFile = value
        End Set
    End Property

    Private myBuildCondition As BuildCondition
    <ReflectorProperty("buildCondition", Required:=False)> _
    Public Property BuildCondition() As BuildCondition
        Get
            Return myBuildCondition
        End Get
        Set(ByVal value As BuildCondition)
            myBuildCondition = value
        End Set
    End Property

    Private myInnerTrigger As ITrigger
    <ReflectorProperty("trigger", InstanceTypeKey:="type", Required:=False)> _
    Public Property InnerTrigger() As ITrigger
        Get
            Return myInnerTrigger
        End Get
        Set(ByVal value As ITrigger)
            myInnerTrigger = value
        End Set
    End Property

    Public Function Fire() As IntegrationRequest Implements ITrigger.Fire
        Dim request As IntegrationRequest = Nothing
        If myLastChanged.HasValue Then
            If myInnerTrigger.Fire() IsNot Nothing Then
                Dim changeTime = File.GetLastWriteTime(MonitorFile)
                If (changeTime > myLastChanged.Value) Then
                    request = New IntegrationRequest(BuildCondition, _
                                                     Me.GetType().Name, _
                                                     Nothing)
                    myLastChanged = changeTime
                End If
            End If
        Else
            myLastChanged = File.GetLastWriteTime(MonitorFile)
        End If

        Return request
    End Function

    Public Sub IntegrationCompleted() Implements ITrigger.IntegrationCompleted
        InnerTrigger.IntegrationCompleted()
        myLastChanged = File.GetLastWriteTime(MonitorFile)
    End Sub

    Public ReadOnly Property NextBuild() As Date Implements ITrigger.NextBuild
        Get
            Return DateTime.MaxValue
        End Get
    End Property

    Public Sub Validate(ByVal configuration As IConfiguration, _
                        ByVal parent As ConfigurationTrace, _
                        ByVal errorProcesser As IConfigurationErrorProcesser) _
                        Implements IConfigurationValidation.Validate
        If (String.IsNullOrEmpty(myMonitorFile)) Then
            errorProcesser.ProcessError("File cannot be empty")
        ElseIf (Not File.Exists(MonitorFile)) Then
            errorProcesser.ProcessWarning("File '" & MonitorFile & "' does not exist")
        End If
    End Sub
End Class
