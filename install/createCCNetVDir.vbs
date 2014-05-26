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

' We're going to create the virtual directory on web site : Default Web Site
Dim webSiteName: webSiteName = "Default Web Site"

'dedicated application pool for CruiseControl.Net
Dim AppPoolName: AppPoolName="CruiseControl.Net"


' Are we debugging this script?
Dim DEBUG: DEBUG = false

createVDir()

'good site with documentation about appcmd
'http://blogs.msdn.com/b/mikezh/archive/2012/04/23/iis-appcmd-quick-reference.aspx


' Create a virtual directory named vdirName on web site webSiteName using .NET version DotNetFrameworkVersion
Function CreateVDir()
        
	If WScript.Arguments.Count < 1 Then
		WScript.Echo "Installation directory for the CruiseControl.NET Web Dashboard not provided; cannot continue."
		WScript.Quit 2
	End If

	Dim webdashboardInstallDir: webdashboardInstallDir = WScript.Arguments(0)
	
    WScript.Echo "Checking whether there is an existing virtual directory with the name " & vdirName
	If not DoesCCNetVDirExist(vdirName) Then
		CreateCCNetVDir webdashboardInstallDir, webSiteName, vdirName
	End If
	
	SetASPNETVersion AppPoolName, DotNetFrameworkVersion, webSiteName, vdirName
	
    ' Confirm the creation
    WScript.Echo "Confirming that the virtual directory " & vdirName & " was created..."
    If not DoesCCNetVDirExist(vdirName) Then
        WScript.Echo vdirName  & " virtual directory creation failed."
        WScript.Quit 4
    End If
        
    WScript.Echo "Virtual directory for CruiseControl.NET Web Dashboard created successfully."
	
End Function

' Check if the specified virtual directory exists on the specified web site
' Returns true if so, false if not.
Function DoesCCNetVDirExist(vdirName) 
	DoesCCNetVDirExist = False

	dim iisDirs 
	iisDirs = getCommandOutput("cmd /c %systemroot%\system32\inetsrv\APPCMD list vdir")
	iisDirs = lcase(iisDirs)

	If DEBUG Then : WScript.Echo "[DEBUG] IIs dirs  found : " & vbcrlf & iisDirs : End If
	
	if instr(iisDirs,vdirName) > 0 then 'vbs is NOT zero based !
		DoesCCNetVDirExist = true
    end if	
	
End Function

Sub CreateCCNetVDir(webdashboardInstallDir, webSiteName, vdirName)
    
    dim result
	dim command
	WScript.Echo "Creating virtual directory for CruiseControl.NET Web Dashboard at " & webdashboardInstallDir
	 
	command = "%systemroot%\system32\inetsrv\APPCMD add app /site.name:" & chr(34) &  webSiteName & chr(34) & " /path:/" & vdirName & " /physicalpath:" & chr(34) & webdashboardInstallDir & chr(34)
	 
	result = getCommandOutput("cmd /c " & command)
	
	If DEBUG Then : WScript.Echo "[DEBUG] IIs app creation : " & vbcrlf & result : End If

End Sub


Sub SetASPNETVersion(AppPoolName, DotNetFrameworkVersion, webSiteName, vdirName)

	dim appPools 
    dim result
	dim command

	WScript.Echo "Setting application pool for CruiseControl.NET Web Dashboard " & AppPoolName & " to frameworkversion : " & DotNetFrameworkVersion

	appPools = getCommandOutput("cmd /c %systemroot%\system32\inetsrv\APPCMD list apppool")
	appPools = lcase(appPools)

	If DEBUG Then : WScript.Echo "[DEBUG] IIs app pools  found : " & vbcrlf & appPools : End If
	
	if instr(appPools,lcase(AppPoolName)) = 0 then 'vbs is NOT zero based !
		WScript.Echo "  * Creating application pool for CruiseControl.NET Web Dashboard " & AppPoolName & " frameworkversion : " & DotNetFrameworkVersion
				
		command = "%systemroot%\system32\inetsrv\APPCMD ADD apppool /name:" & chr(34) &  AppPoolName & chr(34) & " /managedRuntimeVersion:" & DotNetFrameworkVersion	 
		result = getCommandOutput("cmd /c " & command)	
		If DEBUG Then : WScript.Echo "[DEBUG] IIs app creation : " & vbcrlf & result : End If			
    end if	
		
	
	command = "%systemroot%\system32\inetsrv\APPCMD set apppool " & chr(34) &  AppPoolName & chr(34) & " /managedRuntimeVersion:" & DotNetFrameworkVersion
	result = getCommandOutput("cmd /c " & command)
	If DEBUG Then : WScript.Echo "[DEBUG] update app pool framework version : " & vbcrlf & result : End If			
	
		
	command = "%systemroot%\system32\inetsrv\APPCMD set app " & chr(34) &  webSiteName & "/" & vdirName & chr(34) & " /applicationpool:" & AppPoolName 
	result = getCommandOutput("cmd /c " & command)
	If DEBUG Then : WScript.Echo "[DEBUG] IIs app set apppool : " & vbcrlf & result : End If			
	
	
	
End Sub


'
' Capture the results of a command line execution and
' return them to the caller.
'
Function getCommandOutput(theCommand)

    Dim objShell, objCmdExec
    Set objShell = CreateObject("WScript.Shell")
    Set objCmdExec = objshell.exec(thecommand)
    getCommandOutput = objCmdExec.StdOut.ReadAll

end Function