@echo off
cls
tools\nant\NAnt.exe -t:net-1.1 -buildfile:ccnet.build -nologo %*
