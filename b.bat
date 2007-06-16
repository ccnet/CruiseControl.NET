@echo off

cls
tools\nant\NAnt.exe -buildfile:ccnet.build -nologo -logfile:build.txt %*
echo %time%
