//////////////////////////////////////////////////////////////////////
// IMPORTS
//////////////////////////////////////////////////////////////////////
#addin "nuget:?package=Cake.Incubator&version=5.0.1"

#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////
// Project details
var product = "CruiseControl.NET";

var target = Argument("target", "default");
var configuration = Argument("configuration", "Debug"); 
var verbosity = Argument("verbosity", "Normal");

var nantExe = @".\Tools\NAnt\NAnt.exe";
GitVersion gitVersionResults;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

//NOTE: Executed BEFORE the first task.
Setup(context =>
{
    Information("Determine build environment");
    
    Information("Determine build version");
    gitVersionResults = GitVersion(new GitVersionSettings());

    Information("Building version {0} of {1}.", gitVersionResults.InformationalVersion, product);
    Information("Target: {0}.", target);
});

// NOTE: Executed AFTER the last task.
Teardown(context =>
{
    Information("Finished building version {0} of {1}.", gitVersionResults.InformationalVersion, product);
});


Task("default")
  .Does(() =>
{
  Information("Available targets");
  Information("  build          : Builds the project by running the clean and build targets from ccnet.build script");
  Information("  build-all      : Builds the project, runs tests and packages artifacts by running the all target from ccnet.build script");
  Information("  run-tests      : Run projects tests by executing the runTests target from ccnet.build script");
  Information("  package        : Packages project artifacts by running package target from ccnet.build script");
  Information("  web-packages   : Packages the project webdashboards by running build.packages from ccnet.build script");
});

Task("build")
  .Does(()=> {
    //Tools\NAnt\NAnt.exe clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -D:version=" + gitVersionResults.AssemblySemVer + " -D:fversion=" + gitVersionResults.AssemblySemFileVer + " -D:iversion=\"" + gitVersionResults.InformationalVersion + "\" -nologo -logfile:nant-build.log.txt %*" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                                Silent = false
                                              }))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
  });

Task("build-all")
  .Does(()=> {
    //Tools\NAnt\NAnt.exe clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " all -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -D:version=" + gitVersionResults.AssemblySemVer + " -D:fversion=" + gitVersionResults.AssemblySemFileVer + " -D:iversion=\"" + gitVersionResults.InformationalVersion + "\" -nologo -logfile:nant-build.log.txt %*" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                                Silent = false
                                              }))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
  });

Task("run-tests")
  .Does(()=>{
    //Tools\NAnt\NAnt.exe runTests -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build-tests.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " runTests -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -D:version=" + gitVersionResults.AssemblySemVer + " -D:fversion=" + gitVersionResults.AssemblySemFileVer + " -D:iversion=\"" + gitVersionResults.InformationalVersion + "\" -nologo -logfile:nant-build-tests.log.txt %*" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                              }))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
  });

Task("package")
  .Does(()=>{
    //Tools\NAnt\NAnt.exe package -buildfile:ccnet.build -D:CCNetLabel=1.5.0.0 -nologo -logfile:nant-build-package.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " package -buildfile:ccnet.build -D:version=" + gitVersionResults.AssemblySemVer + " -D:fversion=" + gitVersionResults.AssemblySemFileVer + " -D:iversion=\"" + gitVersionResults.InformationalVersion + "\" -nologo -logfile:nant-build-package.log.txt %*" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                              }))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
  });

Task("web-packages")
  .Does(()=>{
    //Tools\NAnt\NAnt.exe build.packages -buildfile:ccnet.build -nologo -logfile:nant-build-web-packages.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " build.packages -buildfile:ccnet.build -D:version=" + gitVersionResults.AssemblySemVer + " -D:fversion=" + gitVersionResults.AssemblySemFileVer + " -D:iversion=\"" + gitVersionResults.InformationalVersion + "\" -nologo -logfile:nant-build-web-packages.log.txt %*" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                              }))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
  });

RunTarget(target);