@echo off
Console.exe -c=generate -s=..\..\..\..\..\project\Remote\bin\Debug\ThoughtWorks.CruiseControl.Remote.dll -o=documentation -xsd
Console.exe -c=generate -s=..\..\..\..\..\project\core\bin\Debug\ThoughtWorks.CruiseControl.Core.dll -o=documentation -xsd
Console.exe -c=generate -s=..\..\..\..\..\project\WebDashboard\bin\ThoughtWorks.CruiseControl.WebDashboard.dll -o=documentation -xsd
pause