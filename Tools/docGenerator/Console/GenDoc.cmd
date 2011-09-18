﻿@echo off
set outputfolder=documentation
rmdir %outputfolder% /q /s

rem original 
rem Console.exe -c=generate -s=..\..\..\..\..\Build\WebDashboard\ThoughtWorks.CruiseControl.WebDashboard.dll -o=%outputfolder% -xsd

Console.exe -c=generate -s=..\..\..\..\..\project\Remote\bin\debug\ThoughtWorks.CruiseControl.Remote.dll -o=%outputfolder% 
Console.exe -c=generate -s=..\..\..\..\..\project\Core\bin\debug\ThoughtWorks.CruiseControl.Core.dll -o=%outputfolder% 
Console.exe -c=generate -s=..\..\..\..\..\project\WebDashboard\bin\ThoughtWorks.CruiseControl.WebDashboard.dll -o=%outputfolder% 
pause