#!/bin/sh
export MONO_IOMAP=all
mono Tools/NAnt/NAnt.exe -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build-all.log
