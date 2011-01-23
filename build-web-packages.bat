@echo off
cls
Tools\NAnt\NAnt.exe build.packages -buildfile:ccnet.build -nologo -logfile:nant-build-web-packages.log.txt %*
echo %time% %date%
pause