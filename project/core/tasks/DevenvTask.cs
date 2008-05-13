using System.Collections;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("devenv")]
	public class DevenvTask : ITask
	{
        public const string VS2008_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\9.0";
        public const string VS2005_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\8.0";
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
		private string version;

		public DevenvTask() : this(new Registry(), new ProcessExecutor()) { }

		public DevenvTask(IRegistry registry, ProcessExecutor executor)
		{
			this.registry = registry;
			this.executor = executor;
		}

		[ReflectorProperty("version", Required = false)]
		public string Version
		{
			get { return version; }

			set
			{
				if (value != "9.0" && 
					value != "8.0" && 
					value != "7.1" && 
					value != "7.0" &&
					value != "VS2008" &&
					value != "VS2005" &&
					value != "VS2003" &&
					value != "VS2002")
				{
					throw new CruiseControlException("Invalid value for Version, expected one of 9.0, 8.0, 7.1, 7.0, VS2008, VS2005, VS2003 or VS2002");
				}

				version = value;
			}
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

        /// <summary>
        /// Get the name of the Visual Studio executable for the highest version installed on this machine.
        /// </summary>
        /// <returns>The fully-qualified pathname of the executable.</returns>
		private string ReadDevenvExecutableFromRegistry()
		{
			string registryValue = null;

			if (Version == null)
			{
				registryValue = registry.GetLocalMachineSubKeyValue(VS2008_REGISTRY_PATH, VS_REGISTRY_KEY);

				if (registryValue == null)
				{
					registryValue = registry.GetLocalMachineSubKeyValue(VS2005_REGISTRY_PATH, VS_REGISTRY_KEY);
				}

				if (registryValue == null)
				{
					registryValue = registry.GetLocalMachineSubKeyValue(VS2003_REGISTRY_PATH, VS_REGISTRY_KEY);
				}

				if (registryValue == null)
				{
					registryValue = registry.GetExpectedLocalMachineSubKeyValue(VS2002_REGISTRY_PATH, VS_REGISTRY_KEY);
				}
			}
			else
			{
				if (Version == "9.0" || Version == "VS2008")
				{
					registryValue = registry.GetExpectedLocalMachineSubKeyValue(VS2008_REGISTRY_PATH, VS_REGISTRY_KEY);
				}

				if (Version == "8.0" || Version == "VS2005")
				{
					registryValue = registry.GetExpectedLocalMachineSubKeyValue(VS2005_REGISTRY_PATH, VS_REGISTRY_KEY);
				}

				if (Version == "7.1" || Version == "VS2003")
				{
					registryValue = registry.GetExpectedLocalMachineSubKeyValue(VS2003_REGISTRY_PATH, VS_REGISTRY_KEY);
				}

				if (Version == "7.0" || Version == "VS2002")
				{
					registryValue = registry.GetExpectedLocalMachineSubKeyValue(VS2002_REGISTRY_PATH, VS_REGISTRY_KEY);
				}
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
            Util.ListenerFile.WriteInfo(result.ListenerFile,
                string.Format("Executing Devenv :{0}", Arguments));          
                                                                  
			ProcessResult processResult = AttemptToExecute(result, ProcessMonitor.GetProcessMonitorByProject(result.ProjectName));
			result.AddTaskResult(new DevenvTaskResult(processResult));
			Log.Info("Devenv build complete.  Status: " + result.Status);
			
			if (processResult.TimedOut)
			{
				throw new BuilderException(this, string.Format("Devenv process timed out after {0} seconds.", BuildTimeoutSeconds));
			}

            Util.ListenerFile.RemoveListenerFile(result.ListenerFile);
		}

        private ProcessResult AttemptToExecute(IIntegrationResult result, ProcessMonitor processMonitor)
		{
            string workingDirectory = result.WorkingDirectory;
			ProcessInfo processInfo = new ProcessInfo(Executable, Arguments, workingDirectory);
			processInfo.TimeOut = BuildTimeoutSeconds * 1000;
            IDictionary properties = result.IntegrationProperties;
            // Pass the integration environment variables to devenv.
            foreach (string key in properties.Keys)
            {
                if (properties[key] == null)
                    processInfo.EnvironmentVariables[key] = null;
                else
                    processInfo.EnvironmentVariables[key] = StringUtil.IntegrationPropertyToString(properties[key]);
            }

			Log.Info(string.Format("Starting build: {0} {1}", processInfo.FileName, processInfo.Arguments));
			try
			{
				return executor.Execute(processInfo, processMonitor);
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
                StringBuilder text = new StringBuilder();

                if (SolutionFile.StartsWith("\""))
                    text.Append(SolutionFile);
                else
                    text.AppendFormat("\"{0}\"", SolutionFile);

                text.AppendFormat(" /{0}", BuildType);

                if (Configuration.StartsWith("\""))
                    text.AppendFormat(" {0}", Configuration);
                else
                    text.AppendFormat(" \"{0}\"", Configuration);

                if (!StringUtil.IsBlank(Project))
                {
                    if (Project.StartsWith("\""))
                        text.AppendFormat(" /project {0}", Project);
                    else
                        text.AppendFormat(" /project \"{0}\"", Project);
                }

			    return text.ToString();
			}
		}
	}
}
