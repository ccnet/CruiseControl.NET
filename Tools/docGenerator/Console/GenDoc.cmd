@echo off
Console.exe -c=generate -s=..\..\..\..\..\Build\Remote\ThoughtWorks.CruiseControl.Remote.dll -o=documentation -xsd
Console.exe -c=generate -s=..\..\..\..\..\Build\Core\ThoughtWorks.CruiseControl.Core.dll -o=documentation -xsd
Console.exe -c=generate -s=..\..\..\..\..\Build\WebDashboard\ThoughtWorks.CruiseControl.WebDashboard.dll -o=documentation -xsd
pause