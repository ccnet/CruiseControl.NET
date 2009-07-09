We shipp following build scripts with CruiseControl.NET

1.) build.bat/.sh

Use this if you want to specify a special target to build.
Example: $ build.sh myNAntTargetToRun

2.) build-all.bat/sh

This is the same target that runs on our build server.
Cleanup -> Init -> Build -> Unit Tests -> code Analysis -> Packaging

3.) build-tests.bat/sh

This will call only the runUnitTests target in our NAnt script.
Cleanup -> Init -> Build -> Unit Tests

4.) build-package.bat/sh

This only build and package the CruiseControl.NET distribution.
Cleanup -> Init -> Build -> Packaging

The packaged distribution can be found in the "Publish" folder.