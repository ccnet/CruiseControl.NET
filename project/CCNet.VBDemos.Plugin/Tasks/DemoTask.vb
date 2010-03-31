Imports ThoughtWorks.CruiseControl.Core
Imports Exortech.NetReflector
Imports System.Xml

<ReflectorType("demoTask")> Public Class DemoTask
    Implements ITask

    Private myName As String
    <ReflectorProperty("name")> _
    Public Property Name() As String
        Get
            Return myName
        End Get
        Set(ByVal value As String)
            myName = value
        End Set
    End Property

    Private myAuthor As String
    <ReflectorProperty("author", Required:=False)> _
    Public Property Author() As String
        Get
            Return myAuthor
        End Get
        Set(ByVal value As String)
            myAuthor = value
        End Set
    End Property

    Private myAge As Integer
    <ReflectorProperty("age", Required:=False)> _
    Public Property Age() As Integer
        Get
            Return myAge
        End Get
        Set(ByVal value As Integer)
            myAge = value
        End Set
    End Property

    Private myIsNonsense As Boolean
    <ReflectorProperty("isNonsense", Required:=False)> _
    Public Property IsNonsense() As Boolean
        Get
            Return myIsNonsense
        End Get
        Set(ByVal value As Boolean)
            myIsNonsense = value
        End Set
    End Property

    Private myInnerItems() As DemoTask
    <ReflectorProperty("items", Required:=False)> _
    Public Property InnerItems() As DemoTask()
        Get
            Return myInnerItems
        End Get
        Set(ByVal value As DemoTask())
            myInnerItems = value
        End Set
    End Property

    Private myChild As DemoTask
    <ReflectorProperty("child", Required:=False, InstanceType:=GetType(DemoTask))> _
        Public Property Child() As DemoTask
        Get
            Return myChild
        End Get
        Set(ByVal value As DemoTask)
            myChild = value
        End Set
    End Property

    Private myTypedChild As DemoTask
    <ReflectorProperty("typedChild", Required:=False, InstanceTypeKey:="type")> _
    Public Property TypedChild() As DemoTask
        Get
            Return myTypedChild
        End Get
        Set(ByVal value As DemoTask)
            myTypedChild = value
        End Set
    End Property

    Public Sub Run(ByVal result As IIntegrationResult) _
    Implements ITask.Run

    End Sub

    <ReflectionPreprocessor()> _
    Public Function PreprocessParameters(ByVal typeTable As NetReflectorTypeTable, _
                                      ByVal inputNode As XmlNode) As XmlNode
        Dim dobNode = (From node In inputNode.ChildNodes.OfType(Of XmlNode)() _
                      Where node.Name = "dob" _
                      Select node).SingleOrDefault()
        If dobNode IsNot Nothing Then
            Dim dob = CDate(dobNode.InnerText)
            inputNode.RemoveChild(dobNode)
            Dim ageNode = inputNode.OwnerDocument.CreateElement("age")
            ageNode.InnerText = CInt((Date.Now - dob).TotalDays / 365).ToString()
            inputNode.AppendChild(ageNode)
        End If
        Return inputNode
    End Function

End Class
