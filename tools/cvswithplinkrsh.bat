@echo off
REM Set the following to 'c:\putty\plink.exe' if you are using key-based authentication
set CVS_RSH=plinkwithpw.bat
"c:\Program Files\CruiseControl.NET\tools\cvs.exe" %*