using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("msbuild")]
	public class MsBuildTask : ITask
	{
		public const string DefaultExecutable = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe";
		public const string DefaultLogger = "ThoughtWorks.CruiseControl.MsBuild.XmlLogger,ThoughtWorks.CruiseControl.MsBuild.dll";
		public const string LogFilename = "msbuild-results.xml";
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
			string buildOutputFile = MsBuildOutputFile(result);
			if (File.Exists(buildOutputFile))
			{
				result.AddTaskResult(new FileTaskResult(buildOutputFile));
			}
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
			builder.AppendArgument(BuildArgs);
			builder.AddArgument(ProjectFile);
			builder.AddArgument(GetLoggerArgs(result));

			return builder.ToString();
		}

		private string GetPropertyArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.Append("/p:");

			int count = 0;
			// We have to sort this alphabetically, else the unit tests
			// that expect args in a certain order are unpredictable
			SortedList sortedList = new SortedList(result.IntegrationProperties);
			foreach (string key in sortedList.Keys)
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
			builder.Append(MsBuildOutputFile(result));
			return builder.ToString();
		}

		private string MsBuildOutputFile(IIntegrationResult result)
		{
			return Path.Combine(result.ArtifactDirectory, LogFilename);
		}
	}
}