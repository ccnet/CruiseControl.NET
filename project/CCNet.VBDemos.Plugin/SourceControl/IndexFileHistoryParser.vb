Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Xml.Linq
Imports ThoughtWorks.CruiseControl.Core
Imports ThoughtWorks.CruiseControl.Core.Sourcecontrol

Public Class IndexFileHistoryParser
    Implements IHistoryParser


    Public Function Parse(ByVal history As TextReader, _
                          ByVal from As Date, ByVal [to] As Date) As Modification() _
                          Implements IHistoryParser.Parse
        Dim modifications = New List(Of Modification)()
        Dim document = XDocument.Load(history)
        Dim fromDate = from.ToString("s")
        Dim toDate = [to].ToString("s")
        For Each change As XElement In document.Descendants("change")
            Dim changeDate = DateTime.Parse(change.Attribute("date").Value)
            If (changeDate >= from) And (changeDate <= [to]) Then
                Dim newModification = New Modification()
                newModification.FileName = change.Attribute("file").Value
                newModification.ModifiedTime = changeDate
                newModification.FolderName = change.Attribute("folder").Value
                newModification.Type = change.Attribute("type").Value
                modifications.Add(newModification)
            End If
        Next
        Return modifications.ToArray()
    End Function
End Class
