using System.Collections;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("rake")]
	public class RakeTask : ITask
	{
		public const int DefaultBuildTimeout = 600;
		public const string DefaultExecutable = @"C:\ruby\bin\rake.bat";
		private readonly ProcessExecutor executor;

		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs = "";

		[ReflectorProperty("baseDirectory", Required = false)]
		public string BaseDirectory = "";

		[ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds = DefaultBuildTimeout;

		[ReflectorProperty("quiet", Required = false)]
		public bool Quiet;

		[ReflectorProperty("executable", Required = false)]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("rakefile", Required = false)]
		public string Rakefile = "";

		[ReflectorProperty("silent", Required = false)]
		public bool Silent;

		[ReflectorArray("targetList", Required = false)]
		public string[] Targets = new string[0];

		[ReflectorProperty("trace", Required = false)]
		public bool Trace;

		public RakeTask()
			: this(new ProcessExecutor()) {}

		public RakeTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		#region ITask Members
		public void Run(IIntegrationResult result)
		{
			ProcessInfo processInfo = NewProcessInfo(result);
			result.BuildProgressInformation.SignalStartRunTask(string.Format("Executing Rake: {0}", processInfo.Arguments));
			ProcessResult processResult = AttemptToExecute(processInfo, ProcessMonitor.GetProcessMonitorByProject(result.ProjectName));

			if (!StringUtil.IsWhitespace(processResult.StandardOutput + processResult.StandardError))
			{
				// The executable produced some output.  We need to transform it into an XML build report 
				// fragment so the rest of CC.Net can process it.
				ProcessResult newResult = new ProcessResult(
					StringUtil.MakeBuildResult(processResult.StandardOutput, ""),
					StringUtil.MakeBuildResult(processResult.StandardError, "Error"),
					processResult.ExitCode,
					processResult.TimedOut,
					processResult.Failed);

				processResult = newResult;
			}
			result.AddTaskResult(new ProcessTaskResult(processResult));

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "Command Line Build timed out (after " + BuildTimeoutSeconds + " seconds)");
			}
		}
		#endregion

		private ProcessResult AttemptToExecute(ProcessInfo info, ProcessMonitor processMonitor)
		{
			try
			{
				return executor.Execute(info, processMonitor);
			}
			catch (IOException e)
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", info, e), e);
			}
		}

		private ProcessInfo NewProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable,
				CreateProcessArgs(),
				result.BaseFromWorkingDirectory(BaseDirectory));
			info.TimeOut = BuildTimeoutSeconds*1000;

			IDictionary properties = result.IntegrationProperties;
			foreach (string key in properties.Keys)
			{
				info.EnvironmentVariables[key] = StringUtil.IntegrationPropertyToString(properties[key]);
			}
			return info;
		}

		private string CreateProcessArgs()
		{
			ProcessArgumentBuilder args = new ProcessArgumentBuilder();
			args.AddArgument("--rakefile", Rakefile);

			if (Silent)
				args.AddArgument("--silent");
			else if (Quiet)
				args.AddArgument("--quiet");

			if (Trace)
				args.AddArgument("--trace");

			args.AddArgument(BuildArgs);
			AppendTargets(args);

			return args.ToString();
		}

		private void AppendTargets(ProcessArgumentBuilder buffer)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				buffer.AppendArgument(Targets[i]);
			}
		}

		public string TargetsForPresentation
		{
			get
			{
				return StringUtil.ArrayToNewLineSeparatedString(Targets);
			}
			set
			{
				Targets = StringUtil.NewLineSeparatedStringToArray(value);
			}
		}
	}
}
