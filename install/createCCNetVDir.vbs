Option Explicit

' Error codes:
' 1: no installation directory provided
' 2: ccnet virtual directory already exists
' 3: created, but not confirmed

createVDir()

Function CreateVDir()
        Dim vdirName: vdirName = "ccnet"
        
	If WScript.Arguments.Count < 1 Then
		WScript.Echo "Installation directory for the CruiseControl.NET Web Dashboard not provided; cannot continue."
		WScript.Quit 2
	End If

	Dim webdashboardInstallDir: webdashboardInstallDir = WScript.Arguments(0)

        WScript.Echo "Checking whether there is an existing virtual directory with the name " & vdirName
	If DoesCCNetVDirExist(vdirName) Then
		WScript.Echo vdirName & " virtual directory already exists; cannot continue."
		WScript.Quit 3
	End If

	Dim ccnetVDir: 	Set ccnetVDir = CreateCCNetVDir(webdashboardInstallDir)
	UpdateCCNetVDirScriptMaps(ccnetVDir)
	
        ' Confirm the creation
        WScript.Echo "Confirming that the virtual directory " & vdirName & " was created..."
        If DoesCCNetVDirExist(vdirName) = False Then
                WScript.Echo vdirName  & " virtual directory creation failed."
                WScript.Quit 4
        End If
        
        WScript.Echo "Virtual directory for CruiseControl.NET Web Dashboard created successfully."
	
End Function

Function DoesCCNetVDirExist(vdirName) 

	Dim iisRoot
	Set iisRoot = GetObject("IIS://localhost/W3SVC/1/ROOT")
	Dim vdir
	For Each vdir in iisRoot
                WScript.Echo vdir.Name
		If vdir.Name = vdirName Then
			DoesCCNetVDirExist = True
			Exit Function
		End If
	Next
	DoesCCNetVDirExist = False

End Function

Function CreateCCNetVDir(webdashboardInstallDir)

        WScript.Echo "Creating virtual directory for CruiseControl.NET Web Dashboard at " & webdashboardInstallDir
	Dim iisRoot
	Set iisRoot = GetObject("IIS://localhost/W3SVC/1/ROOT")
	Dim ccnetVDir: set ccnetVDir = iisRoot.Create("IisWebVirtualDir", "ccnet")

	ccnetVDir.AppCreate2 1
	ccnetVDir.AccessRead = True
	ccnetVDir.AccessExecute = True
	ccnetVDir.AppFriendlyName = "ccnet"
	ccnetVDir.KeyType = "IisWebVirtualDir"
        ccnetVDir.DefaultDoc = "default.aspx"
        ccnetVDir.DirBrowseFlags = &H40000000
	ccnetVDir.Path = webdashboardInstallDir

	ccnetVDir.SetInfo()
        Set CreateCCNetVDir = ccnetVDir

End Function

Function UpdateCCNetVDirScriptMaps(ccnetVDir)

        WScript.Echo "Updating the script maps for CruiseControl.NET virtual directory"
	Dim scriptMapCount
	Dim scriptMap
	Dim xmlScriptMap

	For Each scriptMap In ccnetVDir.ScriptMaps
		scriptMapCount = scriptMapCount + 1
		If InStr( scriptMap, ".aspx" ) Then
			xmlScriptMap = Replace( scriptMap, ".aspx", ".xml" )
		End If
	Next

        Dim iisRoot: Set iisRoot = GetObject("IIS://localhost/W3SVC/1/ROOT" )
	Dim scriptMaps: ScriptMaps = iisRoot.Get("ScriptMaps")
	ReDim Preserve scriptMaps(scriptMapCount)
	scriptMaps(scriptMapCount) = xmlScriptMap

	ccnetVDir.PutEx 2, "ScriptMaps", scriptMaps
	ccnetVDir.SetInfo

End Function