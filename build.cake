//////////////////////////////////////////////////////////////////////
// IMPORTS
//////////////////////////////////////////////////////////////////////
#addin "nuget:?package=Cake.Incubator&version=5.0.1"

#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"

var target = Argument("target", "Default");

Task("Default")
  .Does(() =>
{
  Information("Hello World!");
});

RunTarget(target);