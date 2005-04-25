using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	/// <summary>
	/// This is a builder that can run any command line process. We capture standard out and standard error
	/// and include them in the Integration Result. We use the process exit code to set whether the build has failed.
	/// TODO: Passing through build labe
	/// TODO: This is very similar to the NAntBuilder, so refactoring required (can we have subclasses with reflector properties?)
	/// </summary>
	[ReflectorType("exec")]
	public class ExecutableTask : ITask
	{
		public const int DEFAULT_BUILD_TIMEOUT = 600;

		private ProcessExecutor _executor;

		public ExecutableTask() : this(new ProcessExecutor()) { }

		public ExecutableTask(ProcessExecutor executor)
		{
			_executor = executor;
		}

		private string executable = "";
		private string configuredBaseDirectory = "";
		private string buildArgs = "";
		private int buildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;

		[ReflectorProperty("executable", Required = true)] 
		public string Executable
		{
			get { return executable; }
			set { executable = value; }
		}

		[ReflectorProperty("baseDirectory", Required = false)] 
		public string ConfiguredBaseDirectory
		{
			get { return configuredBaseDirectory; }
			set { configuredBaseDirectory = value; }
		}

		[ReflectorProperty("buildArgs", Required = false)] 
		public string BuildArgs
		{
			get { return buildArgs; }
			set { buildArgs = value; }
		}

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds
		{
			get { return buildTimeoutSeconds; }
			set { buildTimeoutSeconds = value; }
		}

		public void Run(IIntegrationResult result)
		{
			ProcessResult processResult = AttemptExecute(CreateProcessInfo(result));
			string output = processResult.StandardOutput + "\n" + processResult.StandardError;
			result.AddTaskResult(output);

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "Command Line Build timed out (after " + buildTimeoutSeconds + " seconds)");
			}

			if (processResult.ExitCode == 0)
			{
				result.Status = IntegrationStatus.Success;
			}
			else
			{
				result.Status = IntegrationStatus.Failure;
				Log.Info("Build failed: " + processResult.StandardError);
			}
		}

		private ProcessInfo CreateProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(executable, buildArgs, BaseDirectory(result));
			info.TimeOut = buildTimeoutSeconds*1000;
			return info;
		}

		private string BaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(configuredBaseDirectory);
		}


		protected ProcessResult AttemptExecute(ProcessInfo info)
		{
			try
			{
				return _executor.Execute(info);
			}			
			catch (Exception e)
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", BuildCommand, e), e);
			}
		}

		private string BuildCommand
		{
			get { return string.Format("{0} {1}", executable, buildArgs); }
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Executable: {1}", configuredBaseDirectory, executable);
		}
	}
}
