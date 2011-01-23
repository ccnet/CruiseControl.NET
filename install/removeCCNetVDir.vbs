Option Explicit

RemoveVDir()

Function RemoveVDir()
	Dim vdirName: vdirName = "ccnet"

	WScript.Echo "Attempting to remove the CruiseControl.NET Web Dashboard virtual directory..."

	Dim iisRoot: Set iisRoot = GetObject("IIS://localhost/W3SVC/1/ROOT")

	iisRoot.Delete "IisWebVirtualDir", vdirName 

	' Confirm the deletion
	WScript.Echo "Confirming that the Web Dashboard virtual directory has been removed."
	If DoesCCNetVDirExist(vdirName) = True Then
		WScript.Echo vdirName & " virtual directory removal failed."
		WScript.Quit 1
	End If

	WScript.Echo "Virtual directory for CruiseControl.NET Web Dashboard removed successfully."

End Function

Function DoesCCNetVDirExist(vdirName) 

	Dim iisRoot
	Set iisRoot = GetObject("IIS://localhost/W3SVC/1/ROOT")
	Dim vdir
	For Each vdir in iisRoot
		If vdir.Name = vdirName Then
			DoesCCNetVDirExist = True
			Exit Function
		End If
	Next
	DoesCCNetVDirExist = False

End Function