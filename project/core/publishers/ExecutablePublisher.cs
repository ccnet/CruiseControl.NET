using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// Publisher that provides for executing an arbitrary command.
	/// </summary>
	/// 
	// TODO - Remove this since ExecutableTask should be sufficient - merge features as necessary
	[ReflectorType("executable")]
	public class ExecutablePublisher : PublisherBase
	{
		private static readonly string _LABEL_ENVIRONMENT_VARIABLE = "ccnet.label";

		private readonly ProcessExecutor executor;

		/// <summary>
		/// The arguments that are passed to the executable.  Defaults to empty.
		/// </summary>
		[ReflectorProperty("arguments", Required = false)]
		public string Arguments = string.Empty;

		/// <summary>
		/// The filename of the executable to be run.
		/// </summary>
		[ReflectorProperty("executable", Required = true)]
		public string Executable = null;

		/// <summary>
		/// The maximum amount of time that the publisher should wait for the executable to exit.  Measured
		/// in milliseconds; defaults to 60 seconds.
		/// </summary>
		[ReflectorProperty("timeout", Required = false)]
		public int Timeout = 60*1000; // 60 seconds

		/// <summary>
		/// The working directory of the executable; defaults to the current directory.
		/// </summary>
		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory = Environment.CurrentDirectory;

		/// <summary>
		/// Whether a nonzero exit status should be considered fatal (throw a CruiseControlException).
		/// </summary>
		[ReflectorProperty("nonzeroExitFatal", Required = false)]
		public bool NonzeroExitFatal = false;

		/// <summary>
		/// The standard output from the process, if any.
		/// </summary>
		public string StandardOutput;

		/// <summary>
		/// The standard error from the process, if any.
		/// </summary>
		public string StandardError;

		public ExecutablePublisher() : this(new ProcessExecutor())
		{}

		public ExecutablePublisher(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		public override void PublishIntegrationResults(IIntegrationResult result)
		{
			ProcessResult output = executor.Execute(CreateProcessInfo(result));
			StandardOutput = output.StandardOutput;
			StandardError = output.StandardError;
			HandleProcessExit(output);
		}

		private ProcessInfo CreateProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable, Arguments, WorkingDirectory);
			info.EnvironmentVariables.Add(_LABEL_ENVIRONMENT_VARIABLE, result.Label);
			return info;
		}

		private void HandleProcessExit(ProcessResult result)
		{
			if (NonzeroExitFatal && result.ExitCode != 0)
			{
				throw new CruiseControlException(string.Format("{0} {1} returned nonzero exit code: {2}", Executable, Arguments, result.ExitCode));
			}
			else if (result.TimedOut)
			{
				throw new CruiseControlException(string.Format("\"{0} {1}\" didn't exit before {2} millisecond timeout", Executable, Arguments, Timeout));
			}
		}
	}
}