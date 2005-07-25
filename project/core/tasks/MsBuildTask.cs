using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("msbuild")]
	public class MsBuildTask : ITask
	{
		public const string DefaultExecutable = "MSBuild.exe";
		public const int DefaultTimeout = 600;

		private readonly ProcessExecutor executor;

		public MsBuildTask() : this(new ProcessExecutor())
		{}

		public MsBuildTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		[ReflectorProperty("executable", Required=false)]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory;

		[ReflectorProperty("projectFile", Required=false)]
		public string ProjectFile;

		[ReflectorProperty("buildArgs", Required=false)]
		public string BuildArgs;

		[ReflectorProperty("targets", Required=false)]
		public string Targets;

		[ReflectorProperty("timeout", Required=false)]
		public int Timeout = DefaultTimeout;

		public void Run(IIntegrationResult result)
		{
			ProcessResult processResult = executor.Execute(NewProcessInfo(result));
			result.AddTaskResult(new ProcessTaskResult(processResult));
		}

		private ProcessInfo NewProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable, Args(result), result.BaseFromWorkingDirectory(WorkingDirectory));
			info.TimeOut = Timeout*1000;
			return info;
		}

		private string Args(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("/nologo");
			builder.AppendIf(! StringUtil.IsBlank(Targets), "/t:{0}", Targets);

			builder.AppendArgument("/p:");

			int count = 0;
			foreach (string key in result.IntegrationProperties.Keys)
			{
				if (count > 0) builder.Append(";");
				builder.Append(string.Format("{0}={1}", key, result.IntegrationProperties[key]));
				count++;
			}

			builder.AddArgument(BuildArgs);
			builder.AddArgument(ProjectFile);
			return builder.ToString();
		}
	}
}