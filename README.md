# CruiseControl.NET

**CruiseControl.NET** is an automated continuous integration server for the .NET platform. It is a C# port of CruiseControl for Java.



## Releases
Releases up to 1.8.5 can be downloaded from [sourceforge.net](https://sourceforge.net/projects/ccnet/)


## Development

### Requirements

## Windows
1. Visual Studio 2019 or Visual Studio 2017 + Visual Studio 2019 Build Tools
2. Sandcastle Help Builder
3. NSIS 3.05
4. Visual Studio Code with NSIS extension (from idleberg)

## Linux
1. MonoDevelop or Rider from Jetbrains
2. Visual Studio Code with NSIS extension (from idleberg)

### Compile and build
We provide the following build scripts with CruiseControl.NET:

## Windows
1. ```ps build.ps1 --target=default```

It will display the existing targets in the cake build script.

2. ```ps build.ps1 --target=build```

Use this if you want to build the project.

3. ```ps build.ps1 --target=build-all```

Full build, including running tests, doing some code analysis and packaging artifacts.
Cleanup -> Init -> Build -> Unit Tests -> code Analysis -> Packaging

4. ```ps build.ps1 --target=run-tests```

This will call only the runUnitTests target in ccnet.build script.
Cleanup -> Init -> Build -> Unit Tests

5. ```ps build.ps1 --target=package```

This only build and package the CruiseControl.NET distribution.
Cleanup -> Init -> Build -> Packaging

The packaged distribution can be found in the "Publish" folder.

6. ```ps build.ps1 --target=web-packages```

This builds and packages the project WebDashboards.

7. ```ps build.ps1 --target=clean```

This cleans the Build, Dist and Publish folders of previous artifacts.

If running powershell scripts are disabled on your machine, you can run powershell with ExecutionPolicy disabled for the CruiseControl.NET build file:

```powershell -ExecutionPolicy ByPass -File ./build.ps1 -target=build```

## Linux
If you just cloned the CruiseControl.NET repository, run ```chmod u+x build.sh``` so you have execute permission on the build script.

1. ```./build.sh --target=default```

It will display the existing targets in the cake build script.

2. ```./build.sh --target=build```

Use this if you want to build the project.

3. ```./build.sh --target=build-all```

Full build, including running tests, doing some code analysis and packaging artifacts.
Cleanup -> Init -> Build -> Unit Tests -> code Analysis -> Packaging

4. ```./build.sh --target=run-tests```

This will call only the runUnitTests target in ccnet.build script.
Cleanup -> Init -> Build -> Unit Tests

7. ```./build.sh --target=clean```

This cleans the Build, Dist and Publish folders of previous artifacts.


**_Building CruiseControl.NET installers and documentation is currently unavailable on linux since it requires NSIS and Sandcastle Help Builder.
