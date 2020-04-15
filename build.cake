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

string assemblySemVer = "";
string assemblySemFileVer = "";
string informationalVersion = "";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

//NOTE: Executed BEFORE the first task.
Setup(context =>
{
    Information("Determine build environment...");

    if(BuildSystem.IsRunningOnAppVeyor)
    {
      assemblySemVer = EnvironmentVariable("GitVersion_AssemblySemVer");
      assemblySemFileVer = EnvironmentVariable("GitVersion_AssemblySemFileVer");
      informationalVersion = EnvironmentVariable("GitVersion_InformationalVersion");
    }
    else if(BuildSystem.IsRunningOnTravisCI)
    {
        //TODO: When linux build is fixed
    }
    else
    {
      var gitVersionResults = GitVersion(new GitVersionSettings());
      assemblySemVer = gitVersionResults.AssemblySemVer;
      assemblySemFileVer = gitVersionResults.AssemblySemFileVer;
      informationalVersion = gitVersionResults.InformationalVersion;
    }
    
    Information("Building version {0} of {1}.", informationalVersion, product);
    Information("Target: {0}.", target);
});

// NOTE: Executed AFTER the last task.
Teardown(context =>
{
    Information("Finished building version {0} of {1}.", informationalVersion, product);
});


Task("default")
  .Does(() =>
{
  Information("Available targets");
  Information("  clean          : Removes artifacts created by a previous build");
  Information("  build          : Builds the project by running the clean and build targets from ccnet.build script");
  Information("  build-all      : Builds the project, runs tests and packages artifacts by running the all target from ccnet.build script");
  Information("  run-tests      : Run projects tests by executing the runTests target from ccnet.build script");
  Information("  package        : Packages project artifacts by running package target from ccnet.build script");
  Information("  web-packages   : Packages the project webdashboards by running build.packages from ccnet.build script");
});

Task("clean")
  .Does(()=> {
    //Tools\NAnt\NAnt.exe clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " clean -buildfile:ccnet.build -nologo -logfile:nant-clean.log" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                                Silent = false
                                              }))
    {
        process.WaitForExit();
        Information("Nant: build target exit code: {0}", process.GetExitCode());

        if(process.GetExitCode() > 0)
        {
          throw new Exception("Cake: build target failed");
        }
    }
  });

Task("build")
  .Does(()=> {
    //Tools\NAnt\NAnt.exe clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -D:version=" + assemblySemVer + " -D:fversion=" + assemblySemFileVer + " -D:iversion=\"" + informationalVersion + "\" -nologo -logfile:nant-build.log" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                                Silent = false
                                              }))
    {
        process.WaitForExit();
        Information("Nant: build target exit code: {0}", process.GetExitCode());

        if(process.GetExitCode() > 0)
        {
          throw new Exception("Cake: build target failed");
        }
    }
  });

Task("build-all")
  .Does(()=> {
    //Tools\NAnt\NAnt.exe clean build -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " all -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -D:version=" + assemblySemVer + " -D:fversion=" + assemblySemFileVer + " -D:iversion=\"" + informationalVersion + "\" -nologo -logfile:nant-build-all.log" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                                Silent = false
                                              }))
    {
        process.WaitForExit();
        Information("Nant: all target exit code: {0}", process.GetExitCode());

        if(process.GetExitCode() > 0)
        {
          throw new Exception("Cake: build-all target failed");
        }
    }
  });

Task("run-tests")
  .Does(()=>{
    //Tools\NAnt\NAnt.exe runTests -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -nologo -logfile:nant-build-tests.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " runTests -buildfile:ccnet.build -D:codemetrics.output.type=HtmlFile -D:version=" + assemblySemVer + " -D:fversion=" + assemblySemFileVer + " -D:iversion=\"" + informationalVersion + "\" -nologo -logfile:nant-build-tests.log" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                              }))
    {
        process.WaitForExit();
        Information("Nant: runTests target exit code: {0}", process.GetExitCode());

        if(process.GetExitCode() > 0)
        {
          throw new Exception("Cake: run-tests target failed");
        }
    }
  });

Task("package")
  .Does(()=>{
    if (IsRunningOnUnix())
    {
        Information("CruiseControl.NET packages cannot be created on Linux");
        return;
    }

    //Tools\NAnt\NAnt.exe package -buildfile:ccnet.build -D:CCNetLabel=1.5.0.0 -nologo -logfile:nant-build-package.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " package -buildfile:ccnet.build -D:version=" + assemblySemVer + " -D:fversion=" + assemblySemFileVer + " -D:iversion=\"" + informationalVersion + "\" -nologo -logfile:nant-build-package.log" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                              }))
    {
        process.WaitForExit();
        Information("Nant: package target exit code: {0}", process.GetExitCode());

        if(process.GetExitCode() > 0)
        {
          throw new Exception("Cake: package target failed");
        }
    }
  });

Task("web-packages")
  .Does(()=>{
    if (IsRunningOnUnix())
    {
        Information("CruiseControl.NET web packages cannot be created on Linux");
        return;
    }

    //Tools\NAnt\NAnt.exe build.packages -buildfile:ccnet.build -nologo -logfile:nant-build-web-packages.log.txt %*
    using(var process = StartAndReturnProcess(nantExe, 
                                              new ProcessSettings{ 
                                                Arguments = " build.packages -buildfile:ccnet.build -D:version=" + assemblySemVer + " -D:fversion=" + assemblySemFileVer + " -D:iversion=\"" + informationalVersion + "\" -nologo -logfile:nant-build-web-packages.log" ,
                                                RedirectStandardError = false,
                                                RedirectStandardOutput = false,
                                              }))
    {
        process.WaitForExit();
        Information("Nant: build.packages target exit code: {0}", process.GetExitCode());

        if(process.GetExitCode() > 0)
        {
          throw new Exception("Cake: web-packages target failed");
        }
    }
  });

RunTarget(target);