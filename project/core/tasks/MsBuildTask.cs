using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("msbuild")]
	public class MsBuildTask : ITask
	{
		public const string DefaultExecutable = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215\MSBuild.exe";
		public const string DefaultLogger = "ThoughtWorks.CruiseControl.MsBuild.XmlLogger,ThoughtWorks.CruiseControl.MsBuild.dll";
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

		[ReflectorProperty("logger", Required=false)]
		public string Logger = DefaultLogger;

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
			if (! StringUtil.IsBlank(Targets)) builder.AddArgument("/t:" + Targets);
			builder.AddArgument(GetPropertyArgs(result));
			builder.AddArgument(BuildArgs);
			builder.AddArgument(ProjectFile);
			builder.AddArgument(GetLoggerArgs(result));

			return builder.ToString();
		}

		private string GetPropertyArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.Append("/p:");

			int count = 0;
			foreach (string key in result.IntegrationProperties.Keys)
			{
				if (count > 0) builder.Append(";");
				builder.Append(string.Format("{0}={1}", key, result.IntegrationProperties[key]));
				count++;
			}

			return builder.ToString();
		}

		private string GetLoggerArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.Append("/l:");
			builder.Append(Logger);
			builder.Append(";");
			builder.Append(Path.Combine(result.ArtifactDirectory, "msbuild-results.xml"));
			return builder.ToString();
		}
	}
}