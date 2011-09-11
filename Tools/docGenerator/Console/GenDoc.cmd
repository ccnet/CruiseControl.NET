@echo off
set outputfolder=documentation
rmdir %outputfolder% /q /s

rem Console.exe -c=generate -s=..\..\..\..\..\Build\Remote\ThoughtWorks.CruiseControl.Remote.dll -o=%outputfolder% -xsd
Console.exe -c=generate -s=..\..\..\..\..\project\Core\bin\debug\ThoughtWorks.CruiseControl.Core.dll -o=%outputfolder% 
rem Console.exe -c=generate -s=..\..\..\..\..\Build\WebDashboard\ThoughtWorks.CruiseControl.WebDashboard.dll -o=%outputfolder% -xsd
pause