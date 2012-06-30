echo Compiling
Tools\NAnt\NAnt.exe clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*

set outputfolder=docgen

rmdir %outputfolder% /s /q
mkdir %outputfolder%

Tools\docGenerator\Console\bin\Debug\Console.exe -c=generate -s=Build\Server\ThoughtWorks.CruiseControl.Remote.dll -o=%outputfolder%
Tools\docGenerator\Console\bin\Debug\Console.exe -c=generate -s=Build\Server\\ThoughtWorks.CruiseControl.Core.dll -o=%outputfolder% 
Tools\docGenerator\Console\bin\Debug\Console.exe -c=generate -s=Build\WebDashboard\ThoughtWorks.CruiseControl.WebDashboard.dll -o=%outputfolder%

IF "%2"=="" GOTO Continue
Tools\docGenerator\Console\bin\Debug\Console.exe -c=publish -u=%1 -p=%2 -o=%outputfolder%
:Continue
pause