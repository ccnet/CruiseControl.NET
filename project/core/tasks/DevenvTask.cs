namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Exortech.NetReflector;
	using ThoughtWorks.CruiseControl.Core.Util;

	/// <summary>
	/// <para>
	/// Most complex build processes use <link>NAnt Task</link> or <link>MSBuild Task</link> to script the build. However, for simple
	/// projects that just need to build a Visual Studio.NET solution, the Visual Studio task &lt;devenv&gt; provides an easier method.
	/// </para>
	/// </summary>
	/// <title>Visual Studio Task</title>
	/// <version>1.0</version>
	/// <remarks>
	/// <para>
	/// If executable and version are not specified, CC.NET will search the registry for VS.NET 2010, 2008, 2005, 2003, and 2002 in that order.
	/// If you need to use a specific version when a newer version is installed, you should specify the version property to identify it,
	/// or specify the executable property to point to the location of correct version of devenv.com.
	/// </para>
	/// <para type="warning">
	/// This task requires you to have Visual Studio .NET installed on your integration server.
	/// </para>
	/// <para>
	/// Often programmers like to use a centralised project to build an entire software system. They define specific dependencies and the
	/// build order on that specific project to reproduce the behaviours of an nmake build.
	/// </para>
	/// <includePage>Integration Properties</includePage>
	/// </remarks>
	/// <example>
	/// <code title="Minimalist example">
	/// &lt;devenv&gt;
	/// &lt;solutionfile&gt;src\MyProject.sln&lt;/solutionfile&gt;
	/// &lt;configuration&gt;Debug&lt;/configuration&gt;
	/// &lt;/devenv&gt;
	/// </code>
	/// <code title="Full example">
	/// &lt;devenv&gt;
	/// &lt;solutionfile&gt;src\MyProject.sln&lt;/solutionfile&gt;
	/// &lt;configuration&gt;Debug&lt;/configuration&gt;
	/// &lt;buildtype&gt;Build&lt;/buildtype&gt;
	/// &lt;project&gt;MyProject&lt;/project&gt;
	/// &lt;executable&gt;c:\program files\Microsoft Visual Studio .NET\Common7\IDE\devenv.com&lt;/executable&gt;
	/// &lt;buildTimeoutSeconds&gt;600&lt;/buildTimeoutSeconds&gt;
	/// &lt;version&gt;VS2002&lt;/version&gt;
	/// &lt;/devenv&gt;
	/// </code>
	/// </example>    
	[ReflectorType("devenv")]
	public class DevenvTask
				: TaskBase
	{
		public const string LogFilename = "devenv-results-{0}.xml";
		public readonly Guid LogFileId = Guid.NewGuid();
		public const string VS2010_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\10.0";
		public const string VS2008_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\9.0";
		public const string VS2005_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\8.0";
		public const string VS2003_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\7.1";
		public const string VS2002_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\7.0";
		public const string VS_REGISTRY_KEY = @"InstallDir";
		public const string DEVENV_EXE = "devenv.com";
		public const int DEFAULT_BUILD_TIMEOUT = 600;
		public const string DEFAULT_BUILDTYPE = "rebuild";
		public const string DEFAULT_PROJECT = "";
		public const ProcessPriorityClass DEFAULT_PRIORITY = ProcessPriorityClass.Normal;

		private readonly IRegistry registry;
		private readonly ProcessExecutor executor;
		private string executable;
		private string version;

		public DevenvTask() :
			this(new Registry(), new ProcessExecutor()) { }

		public DevenvTask(IRegistry registry, ProcessExecutor executor)
		{
			this.registry = registry;
			this.executor = executor;
			this.BuildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;
			this.BuildType = DEFAULT_BUILDTYPE;
			this.Project = DEFAULT_PROJECT;
			this.Priority = DEFAULT_PRIORITY;
		}

		private readonly string[] ExpectedVisualStudioVersions =
			new string[]
				{
					"10.0", "9.0", "8.0", "7.1", "7.0",
					"VS2010", "VS2008", "VS2005", "VS2003", "VS2002"
				};

		private readonly string[] RegistryScanOrder =
			new string[]
				{
					VS2010_REGISTRY_PATH, VS2008_REGISTRY_PATH, VS2005_REGISTRY_PATH, VS2003_REGISTRY_PATH, VS2002_REGISTRY_PATH
				};

		/// <summary>
		/// The version of Visual Studio.
		/// </summary>
		/// <version>1.0</version>
		/// <default>See below</default>
		/// <values>
		/// <value>VS2002</value>
		/// <value>VS2003</value>
		/// <value>VS2005</value>
		/// <value>VS2008</value>
		/// <value>VS2010</value>
		/// <value>7.0</value>
		/// <value>7.1</value>
		/// <value>8.0</value>
		/// <value>9.0</value>
		/// <value>10.0</value>
		/// </values>
		[ReflectorProperty("version", Required = false)]
		public string Version
		{
			get { return version; }

			set
			{
				if (Array.IndexOf(ExpectedVisualStudioVersions, value) == -1)
					throw new CruiseControlException("Invalid value for Version, expected one of: " +
						StringUtil.Join(", ", ExpectedVisualStudioVersions));

				version = value;
			}
		}

		/// <summary>
		/// The path to devenv.com.
		/// </summary>
		/// <version>1.0</version>
		/// <default>See below</default>
		[ReflectorProperty("executable", Required = false)]
		public string Executable
		{
			get
			{
				if (executable == null)
					executable = ReadDevenvExecutableFromRegistry();

				return executable;
			}
			set { executable = value; }
		}

		/// <summary>
		/// Get the name of the Visual Studio executable for the highest version installed on this machine.
		/// </summary>
		/// <returns>The fully-qualified pathname of the executable.</returns>
		private string ReadDevenvExecutableFromRegistry()
		{
			// If null, scan for any version.
			if (Version == null)
				return Path.Combine(ScanForRegistryForVersion(), DEVENV_EXE);

			string path;

			switch (Version)
			{
				case "VS2010":
				case "10.0":
					path = registry.GetExpectedLocalMachineSubKeyValue(VS2010_REGISTRY_PATH, VS_REGISTRY_KEY);
					break;
				case "VS2008":
				case "9.0":
					path = registry.GetExpectedLocalMachineSubKeyValue(VS2008_REGISTRY_PATH, VS_REGISTRY_KEY);
					break;
				case "VS2005":
				case "8.0":
					path = registry.GetExpectedLocalMachineSubKeyValue(VS2005_REGISTRY_PATH, VS_REGISTRY_KEY);
					break;
				case "VS2003":
				case "7.1":
					path = registry.GetExpectedLocalMachineSubKeyValue(VS2003_REGISTRY_PATH, VS_REGISTRY_KEY);
					break;
				case "VS2002":
				case "7.0":
					path = registry.GetExpectedLocalMachineSubKeyValue(VS2002_REGISTRY_PATH, VS_REGISTRY_KEY);
					break;
				default:
					throw new Exception("Unknown version of Visual Studio.");
			}

			return Path.Combine(path, DEVENV_EXE);
		}

		private string ScanForRegistryForVersion()
		{
			foreach (string x in RegistryScanOrder)
			{
				string path = registry.GetLocalMachineSubKeyValue(x, VS_REGISTRY_KEY);
				if (path != null)
					return path;
			}

			throw new Exception("Unknown version of Visual Studio, or no version found.");
		}

		/// <summary>
		/// The path of the solution file to build. If relative, it is relative to the Project Working Directory. 
		/// </summary>
		/// <default>n/a</default>
		/// <version>1.0</version>
		[ReflectorProperty("solutionfile")]
		public string SolutionFile { get; set; }

		/// <summary>
		/// The solution configuration to use (not case sensitive). 
		/// </summary>
		/// <default>n/a</default>
		/// <version>1.0</version>
		[ReflectorProperty("configuration")]
		public string Configuration { get; set; }

		/// <summary>
		/// Number of seconds to wait before assuming that the process has hung and should be killed. 
		/// </summary>
		/// <default>600 (10 minutes)</default>
		/// <version>1.0</version>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds { get; set; }

		/// <summary>
		/// The type of build.
		/// </summary>
		/// <version>1.0</version>
		/// <default>rebuild</default>
		/// <values>
		/// <value>Rebuild</value>
		/// <value>Build</value>
		/// <value>Clean</value>
		/// </values>
		[ReflectorProperty("buildtype", Required = false)]
		public string BuildType { get; set; }

		/// <summary>
		/// A specific project in the solution, if you only want to build one project (not case sensitive). 
		/// </summary>
		/// <version>1.0</version>
		/// <default>All projects</default>
		[ReflectorProperty("project", Required = false)]
		public string Project { get; set; }

		/// <summary>
		/// The priority class of the spawned process.
		/// </summary>
		/// <version>1.5</version>
		/// <default>Normal</default>
		[ReflectorProperty("priority", Required = false)]
		public ProcessPriorityClass Priority { get; set; }

		protected override bool Execute(IIntegrationResult result)
		{
			result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : string.Format(System.Globalization.CultureInfo.CurrentCulture,"Executing Devenv :{0}", GetArguments(result)));
			ProcessResult processResult = TryToRun(result);

			// rei added 30.5.2010, merge devenv output to task result 
			string buildOutputFile = DevEnvOutputFile(result);
			if (File.Exists(buildOutputFile))
				result.AddTaskResult(new FileTaskResult(buildOutputFile) { WrapInCData=true} );

			result.AddTaskResult(new DevenvTaskResult(processResult));
			Log.Info("Devenv build complete.  Status: " + result.Status);

			if (processResult.TimedOut)
				throw new BuilderException(this, string.Format(System.Globalization.CultureInfo.CurrentCulture,"Devenv process timed out after {0} seconds.", BuildTimeoutSeconds));

			return !processResult.Failed;
		}

		private ProcessResult TryToRun(IIntegrationResult result)
		{
			ProcessInfo processInfo = new ProcessInfo(Executable, GetArguments(result), result.WorkingDirectory, Priority);
			processInfo.TimeOut = BuildTimeoutSeconds * 1000;
			IDictionary properties = result.IntegrationProperties;

			// Pass the integration environment variables to devenv.
			foreach (string key in properties.Keys)
			{
				processInfo.EnvironmentVariables[key] = StringUtil.IntegrationPropertyToString(properties[key]);
			}

			Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Starting build: {0} {1}", processInfo.FileName, processInfo.PublicArguments));
			try
			{
				return executor.Execute(processInfo);
			}
			catch (IOException ex)
			{
				string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to launch the devenv process.  Please verify that you can invoke this command from the command line: {0} {1}", processInfo.FileName, processInfo.PublicArguments);
				throw new BuilderException(this, message, ex);
			}
		}

		private string GetArguments(IIntegrationResult result)
		{
			StringBuilder sb = new StringBuilder();

			if (SolutionFile.StartsWith("\""))
				sb.Append(SolutionFile);
			else
				sb.AppendFormat("\"{0}\"", SolutionFile);

			sb.AppendFormat(" /{0}", BuildType);

			if (Configuration.StartsWith("\""))
				sb.AppendFormat(" {0}", Configuration);
			else
				sb.AppendFormat(" \"{0}\"", Configuration);

			if (!string.IsNullOrEmpty(Project))
			{
				if (Project.StartsWith("\""))
					sb.AppendFormat(" /project {0}", Project);
				else
					sb.AppendFormat(" /project \"{0}\"", Project);
			}

			// always create an out file, will be merged into build log later
			sb.AppendFormat(" /out \"{0}\"", DevEnvOutputFile(result));

			return sb.ToString();
		}


		private string DevEnvOutputFile(IIntegrationResult result)
		{
			return Path.Combine(result.ArtifactDirectory, string.Format(LogFilename, LogFileId));
		}

	}
}
