@echo off
cls
Tools\NAnt\NAnt.exe runTests -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build-tests.log.txt %*
echo %time% %date%
pause