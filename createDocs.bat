@ECHO OFF

ECHO Compiling docGenerator
Tools\NAnt\NAnt.exe -buildfile:docPrep.build all -nologo -logfile:nant-docs.log %*

ECHO Create .\docgen folder
SET "outputfolder=.\docgen"

IF EXIST %outputfolder% (
    RMDIR %outputfolder% /s /q
) ELSE (
    MKDIR %outputfolder%
)

ECHO Run docGenerator on .dlls
Tools\docGenerator\Console\bin\Debug\Console.exe -c=generate -s=Build\Server\ThoughtWorks.CruiseControl.Remote.dll -o=%outputfolder%
Tools\docGenerator\Console\bin\Debug\Console.exe -c=generate -s=Build\Server\ThoughtWorks.CruiseControl.Core.dll -o=%outputfolder% 
Tools\docGenerator\Console\bin\Debug\Console.exe -c=generate -s=Build\WebDashboard\ThoughtWorks.CruiseControl.WebDashboard.dll -o=%outputfolder%

IF "%2"=="" GOTO Continue

ECHO Publish documentation
Tools\docGenerator\Console\bin\Debug\Console.exe -c=publish -u=%1 -p=%2 -o=%outputfolder%

:Continue

pause