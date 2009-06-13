using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("rake")]
	public class RakeTask
        : BaseExecutableTask, ITask
	{
		public const int DefaultBuildTimeout = 600;
		public const string DefaultExecutable = @"rake";

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

		protected override bool Execute(IIntegrationResult result)
		{
			ProcessInfo processInfo = CreateProcessInfo(result);
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : string.Format("Executing Rake: {0}", processInfo.Arguments));
			ProcessResult processResult = TryToRun(processInfo);

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
				throw new BuilderException(this, "Command Line Build timed out (after " + BuildTimeoutSeconds + " seconds)");

            return (!processResult.Failed);
		}

		protected override string GetProcessArguments(IIntegrationResult result)
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

			foreach (string t in Targets)
				args.AppendArgument(t);

			return args.ToString();
		}

		protected override string GetProcessBaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(BaseDirectory);
		}

		protected override int GetProcessTimeout()
		{
			return BuildTimeoutSeconds*1000;
		}

		protected override string GetProcessFilename()
		{
			return Executable;
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
