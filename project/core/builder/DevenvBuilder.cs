using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder
{
	[ReflectorType("devenv")]
	public class DevenvBuilder : IBuilder
	{
		public const string VS2003_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\7.1";
		public const string VS2002_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\7.0";
		public const string VS_REGISTRY_KEY = @"InstallDir";
		public const string DEVENV_EXE = "devenv.com";
		public const int DEFAULT_BUILD_TIMEOUT = 600;

		private IRegistry registry;
		private ProcessExecutor executor;
		private string executable;

		public DevenvBuilder() : this(new Registry(), new ProcessExecutor()) { }

		public DevenvBuilder(IRegistry registry, ProcessExecutor executor)
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

		public virtual void Run(IntegrationResult result, IProject project)
		{
			ProcessResult processResult = AttemptToExecute();
			result.Status = (processResult.Failed) ? IntegrationStatus.Failure : IntegrationStatus.Success;
			result.Output = processResult.StandardOutput;
			result.TaskResults.Add(new DevenvTaskResult(result.Output));
			Log.Info("Devenv build complete.  Status: " + result.Status);
			
			if (processResult.TimedOut)
			{
				throw new BuilderException(this, string.Format("Devenv process timed out after {0} seconds.", BuildTimeoutSeconds));
			}
		}

		private ProcessResult AttemptToExecute()
		{
			ProcessInfo processInfo = new ProcessInfo(Executable, Arguments);
			processInfo.TimeOut = BuildTimeoutSeconds * 1000;

			Log.Info(string.Format("Starting build: {0} {1}", processInfo.FileName, processInfo.Arguments));
			try
			{
				return executor.Execute(processInfo);
			}
			catch (Exception ex)
			{
				string message = string.Format("Unable to launch the devenv process.  Please verify that you can invoke this command from the command line: {0} {1}", processInfo.FileName, processInfo.Arguments);
				throw new BuilderException(this, message, ex);
			}
		}

		private string Arguments
		{
			get { return string.Format("{0} /rebuild {1}", SolutionFile, Configuration); }
		}

		public bool ShouldRun(IntegrationResult result, IProject project)
		{
			return result.Working && result.HasModifications();
		}
	}
}
