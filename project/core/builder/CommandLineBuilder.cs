using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder
{
	/// <summary>
	/// This is a builder that can run any command line process. We capture standard out and standard error
	/// and include them in the Integration Result. We use the process exit code to set whether the build has failed.
	/// TODO: Passing through build labe
	/// TODO: This is very similar to the NAntBuilder, so refactoring required (can we have subclasses with reflector properties?)
	/// </summary>
	[ReflectorType("commandLineBuilder")]
	public class CommandLineBuilder : IBuilder
	{
		public const int DEFAULT_BUILD_TIMEOUT = 600;
		public const string DEFAULT_BUILDARGS = "";

		private ProcessExecutor _executor;

		public CommandLineBuilder() : this(new ProcessExecutor()) { }

		public CommandLineBuilder(ProcessExecutor executor)
		{
			_executor = executor;
		}

		[ReflectorProperty("executable", Required = true)] 
		public string Executable;

		[ReflectorProperty("baseDirectory", Required = false)] 
		public string ConfiguredBaseDirectory;

		[ReflectorProperty("buildArgs", Required = false)] 
		public string BuildArgs = DEFAULT_BUILDARGS;

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;

		public void Run(IntegrationResult result, IProject project)
		{
			ProcessResult processResult = AttemptExecute(CreateProcessInfo(result, project));
			result.Output = processResult.StandardOutput + "\n" + processResult.StandardError;

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "Command Line Build timed out (after " + BuildTimeoutSeconds + " seconds)");
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

		private ProcessInfo CreateProcessInfo(IntegrationResult result, IProject project)
		{
			ProcessInfo info = new ProcessInfo(Executable, BuildArgs, BaseDirectory(project));
			info.TimeOut = BuildTimeoutSeconds*1000;
			return info;
		}

		private string BaseDirectory(IProject project)
		{
			if (ConfiguredBaseDirectory == null || ConfiguredBaseDirectory == "")
			{
				return project.WorkingDirectory;
			}
			else if (Path.IsPathRooted(ConfiguredBaseDirectory))
			{
				return ConfiguredBaseDirectory;
			}
			else
			{
				return Path.Combine(project.WorkingDirectory, ConfiguredBaseDirectory);
			}
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
			get { return string.Format("{0} {1}", Executable, BuildArgs); }
		}

		public bool ShouldRun(IntegrationResult result, IProject project)
		{
			return result.Working && result.HasModifications();
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Executable: {1}", ConfiguredBaseDirectory, Executable);
		}
	}
}
