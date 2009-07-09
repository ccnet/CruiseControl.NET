@echo off
cls
Tools\NAnt\NAnt.exe clean init build runTests -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*
echo %time% %date%
pause