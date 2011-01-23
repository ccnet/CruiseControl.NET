Option Explicit

'Create the CCNet web dashboard's virtual directory
'Syntax: createCCNetVDir <dashboardInstallDir>

' Error codes:
' 2: no installation directory provided
' 3: ccnet virtual directory already exists
' 4: created, but not confirmed
' 5: Unable to set ASP.NET version
' 6: Unable to locate .NET framework

'Constants we use:
' Starting with CCNet 1.3, we are a .NET 2.0 application:
Dim DotNetFrameworkVersion: DotNetFrameworkVersion = "v2.0"
' We're going to create a "ccnet" virtual directory:
Dim vdirName: vdirName = "ccnet"
' We're going to create the virtual directory on web site #1:
Dim webSiteNumber: webSiteNumber = "1"

' Are we debugging this script?
Dim DEBUG: DEBUG = false

createVDir()

' Create a virtual directory named vdirName on web site webSiteNumber using .NET version DotNetFrameworkVersion
Function CreateVDir()
        
	If WScript.Arguments.Count < 1 Then
		WScript.Echo "Installation directory for the CruiseControl.NET Web Dashboard not provided; cannot continue."
		WScript.Quit 2
	End If

	Dim webdashboardInstallDir: webdashboardInstallDir = WScript.Arguments(0)

	WScript.Echo "Searching for location of .NET framework version " & DotNetFrameworkVersion
	Dim DotNetFrameworkLocation
	DotNetFrameworkLocation = GetDotNetLocation(DotNetFrameworkVersion)
	If DotNetFrameworkLocation = "" Then
		WScript.Echo "Unable to location .NET framework version " & DotNetFrameworkVersion & "; cannot continue."
		WScript.Quit 6
	End If


        WScript.Echo "Checking whether there is an existing virtual directory with the name " & vdirName
	If DoesCCNetVDirExist(vdirName, webSiteNumber) Then
		WScript.Echo vdirName & " virtual directory already exists; cannot continue."
		WScript.Quit 3
	End If

	Dim ccnetVDir: 	Set ccnetVDir = CreateCCNetVDir(webdashboardInstallDir, webSiteNumber)
	UpdateCCNetVDirScriptMaps ccnetVDir, webSiteNumber

	If SetASPNETVersion(vdirName, webSiteNumber, DotNetFrameworkLocation) = False then
		WScript.Quit 5
	End if
	
        ' Confirm the creation
        WScript.Echo "Confirming that the virtual directory " & vdirName & " was created..."
        If DoesCCNetVDirExist(vdirName, webSiteNumber) = False Then
                WScript.Echo vdirName  & " virtual directory creation failed."
                WScript.Quit 4
        End If
        
        WScript.Echo "Virtual directory for CruiseControl.NET Web Dashboard created successfully."
	
End Function

' Locate the installed copy of the specified .NET version
' MajorVersion should be "vx.y" (e.g., "v2.0" for .NET 2.0)
' Returns the full path to the framework (e.g., "C:\Windows\Microsoft.NET\Framework\v2.0.0.50727" for .NET 2.0).
'
' Based on an NSIS-script version from the NSIS wiki article "Get directory of installed .NET runtime"
' (http://nsis.sourceforge.net/Get_directory_of_installed_.NET_runtime), converted to VBScript.
Function GetDotNetLocation(majorVersion)
	' Assume failure:
	GetDotNetLocation = ""

	Dim keyName
	' Because we can't just type "HKLM":
	Const wmi_HKLM = &H80000002

	' Find the .NET framework via the registry:
	On Error Resume Next
	Dim objReg: Set objReg = GetObject("winmgmts:\\.\root\default:StdRegProv")
	If Err.Number <> 0 Then
		WScript.Echo "Error " & Err.Number & " opening the registry: " & Err.Description
		Exit Function
	End If
	keyName = "SOFTWARE\Microsoft\.NETFramework\policy\" & MajorVersion
	Dim subKeyList(), subkeyValues() : objReg.EnumValues wmi_HKLM, keyName, subKeyList, subKeyValues
	If DEBUG Then : WScript.Echo "[DEBUG] GetDotNetLocation(): found " & UBound(subKeyList)+1 & " keys under " & keyName : End If
	If Err.Number <> 0 Then
		WScript.Echo "Error " & Err.Number & " finding installed keys under HKLM\" & keyName & ": " & Err.Description
		Exit Function
	End If

	Dim minorVersion : minorVersion = subKeyList(0)
	If DEBUG Then : WScript.Echo "[DEBUG] GetDotNetLocation(): minorVersion=" & minorVersion : End If
	keyName = "SOFTWARE\Microsoft\.NETFramework"
	Dim installRoot: objReg.GetStringValue wmi_HKLM, keyName, "InstallRoot", installRoot
	If DEBUG Then : WScript.Echo "[DEBUG] GetDotNetLocation(): installRoot=" & installRoot : End If
	If Err.Number <> 0 Then
		WScript.Echo "Error " & Err.Number & " getting InstallRoot key: " & Err.Description
		Exit Function
	End If
	GetDotNetLocation = installRoot & majorVersion & "." & minorVersion
	If DEBUG Then : WScript.Echo "[DEBUG] GetDotNetLocation(): GetDotNetLocation=" & GetDotNetLocation : End If

End Function

' Check if the specified virtual directory exists on the specified web site
' Returns true if so, false if not.
Function DoesCCNetVDirExist(vdirName, webSiteNumber) 

	Dim iisRoot
	Set iisRoot = GetObject("IIS://localhost/W3SVC/" & webSiteNumber & "/ROOT")
	Dim vdir
	For Each vdir in iisRoot
		If DEBUG Then : WScript.Echo "[DEBUG] DoesCCNetVDirExist(): found " &  vdir.Name : End If
		If vdir.Name = vdirName Then
			DoesCCNetVDirExist = True
			Exit Function
		End If
	Next
	DoesCCNetVDirExist = False

End Function

Function CreateCCNetVDir(webdashboardInstallDir, webSiteNumber)

        WScript.Echo "Creating virtual directory for CruiseControl.NET Web Dashboard at " & webdashboardInstallDir
	Dim iisRoot
	Set iisRoot = GetObject("IIS://localhost/W3SVC/" & webSiteNumber & "/ROOT")
	Dim ccnetVDir: set ccnetVDir = iisRoot.Create("IisWebVirtualDir", vdirName)

	ccnetVDir.AppCreate2 1
	ccnetVDir.AccessRead = True
	ccnetVDir.AccessScript = True
	' ccnetVDir.AccessNoRemoteExecute = True
	ccnetVDir.AppFriendlyName = "ccnet"
	ccnetVDir.KeyType = "IisWebVirtualDir"
        ccnetVDir.DefaultDoc = "default.aspx"
        ccnetVDir.DirBrowseFlags = &H40000000
	ccnetVDir.Path = webdashboardInstallDir

	ccnetVDir.SetInfo()
        Set CreateCCNetVDir = ccnetVDir

End Function

Function UpdateCCNetVDirScriptMaps(ccnetVDir, webSiteNumber)

        WScript.Echo "Updating the script maps for CruiseControl.NET virtual directory"
	Dim scriptMapCount
	Dim scriptMap
	Dim xmlScriptMap

	For Each scriptMap In ccnetVDir.ScriptMaps
		scriptMapCount = scriptMapCount + 1
		' Create a ".xml" script map based on the ".aspx" map:
		If InStr( scriptMap, ".aspx" ) Then
			xmlScriptMap = Replace( scriptMap, ".aspx", ".xml" )
		End If
	Next

        Dim iisRoot: Set iisRoot = GetObject("IIS://localhost/W3SVC/" & webSiteNumber & "/ROOT" )
	Dim scriptMaps: ScriptMaps = iisRoot.Get("ScriptMaps")
	ReDim Preserve scriptMaps(scriptMapCount)
	scriptMaps(scriptMapCount) = xmlScriptMap

	ccnetVDir.PutEx 2, "ScriptMaps", scriptMaps
	ccnetVDir.SetInfo

End Function

Function SetASPNETVersion(vdirName, webSiteNumber, DotNetFrameworkLocation)

	WScript.Echo "Setting .NET framework to " & DotNetFrameworkLocation & " for CruiseControl.NET virtual directory"
	Dim cmd: cmd = """" & DotNetFrameworkLocation & "\aspnet_regiis.exe"" -s ""W3SVC/" & webSiteNumber & "/ROOT/" & vdirName & """"
	If DEBUG Then : WScript.Echo "[DEBUG] SetASPNETVersion(): cmd=" & cmd : End If
	Dim wsh: Set wsh = WScript.CreateObject("WScript.Shell")
	Dim response: response = wsh.Run(cmd , 1, true )
	If ( response <> 0 ) Then
		WScript.Echo cmd & " failed; rc=" & response
		SetASPNETVersion = false
		Exit function
	End If
	SetASPNETVersion = true

End Function

