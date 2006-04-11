using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("devenv")]
	public class DevenvTask : ITask
	{
		public const string VS2003_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\7.1";
		public const string VS2002_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\7.0";
		public const string VS_REGISTRY_KEY = @"InstallDir";
		public const string DEVENV_EXE = "devenv.com";
		public const int DEFAULT_BUILD_TIMEOUT = 600;
		public const string DEFAULT_BUILDTYPE = "rebuild";
		public const string DEFAULT_PROJECT = "";

		private IRegistry registry;
		private ProcessExecutor executor;
		private string executable;

		public DevenvTask() : this(new Registry(), new ProcessExecutor()) { }

		public DevenvTask(IRegistry registry, ProcessExecutor executor)
		{
			this.registry = registry;
			this.executor = executor;
		}

		[ReflectorProperty("executable", Required=false)]
		public string Executable
		{
			get 
			{ 
				if (executable == null)
				{
					executable = ReadDevenvExecutableFromRegistry();
				}
				return executable; 
			}	
			set { executable = value; }
		}

		private string ReadDevenvExecutableFromRegistry()
		{
			string registryValue = registry.GetLocalMachineSubKeyValue(VS2003_REGISTRY_PATH, VS_REGISTRY_KEY);
			if (registryValue == null)
			{
				registryValue = registry.GetExpectedLocalMachineSubKeyValue(VS2002_REGISTRY_PATH, VS_REGISTRY_KEY);
			}
			return Path.Combine(registryValue, DEVENV_EXE);
		}

		[ReflectorProperty("solutionfile")]
		public string SolutionFile;
	
		[ReflectorProperty("configuration")]
		public string Configuration;

		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;

		[ReflectorProperty("buildtype", Required = false)]
		public string BuildType = DEFAULT_BUILDTYPE;

		[ReflectorProperty("project", Required = false)]
		public string Project  = DEFAULT_PROJECT;

		public virtual void Run(IIntegrationResult result)
		{
			ProcessResult processResult = AttemptToExecute(result.WorkingDirectory);
			result.AddTaskResult(new DevenvTaskResult(processResult));
			Log.Info("Devenv build complete.  Status: " + result.Status);
			
			if (processResult.TimedOut)
			{
				throw new BuilderException(this, string.Format("Devenv process timed out after {0} seconds.", BuildTimeoutSeconds));
			}
		}

		private ProcessResult AttemptToExecute(string workingDirectory)
		{
			ProcessInfo processInfo = new ProcessInfo(Executable, Arguments, workingDirectory);
			processInfo.TimeOut = BuildTimeoutSeconds * 1000;

			Log.Info(string.Format("Starting build: {0} {1}", processInfo.FileName, processInfo.Arguments));
			try
			{
				return executor.Execute(processInfo);
			}
			catch (IOException ex)
			{
				string message = string.Format("Unable to launch the devenv process.  Please verify that you can invoke this command from the command line: {0} {1}", processInfo.FileName, processInfo.Arguments);
				throw new BuilderException(this, message, ex);
			}
		}

		private string Arguments
		{
			get 
			{ 
				if (! StringUtil.IsBlank(Project))
				{
					return string.Format("{0} /{1} {2} /project {3}", SolutionFile, BuildType, Configuration, Project); 
				}
				else
				{
					return string.Format("{0} /{1} {2}", SolutionFile, BuildType, Configuration); 
				}
			}
		}
	}
}
