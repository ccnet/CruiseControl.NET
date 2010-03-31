Imports System
Imports Exortech.NetReflector
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Config

<ReflectorType("randomLabeller")> Public Class RandomLabeller
    Implements ILabeller, IConfigurationValidation

    Public Sub New()
        myMaximumValue = Integer.MaxValue
    End Sub

    Private myMaximumValue As Integer
    <ReflectorProperty("max", Required:=False)> _
    Public Property MaximumValue() As Integer
        Get
            Return myMaximumValue
        End Get
        Set(ByVal value As Integer)
            myMaximumValue = value
        End Set
    End Property

    Public Function Generate(ByVal integrationResult As IIntegrationResult) As String _
    Implements ILabeller.Generate
        Dim rand = New Random()
        Dim label = rand.Next(myMaximumValue).ToString()
        Return label
    End Function

    Public Sub Run(ByVal result As IIntegrationResult) _
    Implements ThoughtWorks.CruiseControl.Core.ITask.Run
        result.Label = Generate(result)
    End Sub

    Public Sub Validate(ByVal configuration As IConfiguration, _
                        ByVal parent As ConfigurationTrace, _
                        ByVal errorProcesser As IConfigurationErrorProcesser) _
                        Implements IConfigurationValidation.Validate
        If myMaximumValue <= 0 Then
            errorProcesser.ProcessError( _
                    "The maximum value must be greater than zero")
        End If
    End Sub
End Class
