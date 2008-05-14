using System;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("msbuild")]
	public class MsBuildTask : ITask
	{
		public const string defaultExecutable = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe";
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
		public string Executable = defaultExecutable;

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
            result.BuildProgressInformation.SignalStartRunTask(            
                    string.Format("Executing MSBuild :BuildFile: {0}", ProjectFile));    

			ProcessResult processResult = executor.Execute(NewProcessInfo(result), ProcessMonitor.GetProcessMonitorByProject(result.ProjectName));
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
			builder.AppendArgument(GetPropertyArgs(result));
			builder.AppendArgument(BuildArgs);
			builder.AddArgument(ProjectFile);
			builder.AppendArgument(GetLoggerArgs(result));

			return builder.ToString();
		}

		private string GetPropertyArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.Append("/p:");

			int count = 0;
			// We have to sort this alphabetically, else the unit tests
			// that expect args in a certain order are unpredictable
			IDictionary properties = result.IntegrationProperties;
			foreach (string key in properties.Keys)
			{
				if (count > 0) builder.Append(";");
				builder.Append(string.Format("{0}={1}", key, StringUtil.AutoDoubleQuoteString(StringUtil.IntegrationPropertyToString(result.IntegrationProperties[key]))));
				count++;
			}

			return builder.ToString();
		}

		private string GetLoggerArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.Append("/l:");
			builder.Append(StringUtil.AutoDoubleQuoteString(Logger));
			builder.Append(";");
			builder.Append(StringUtil.AutoDoubleQuoteString(MsBuildOutputFile(result)));
			return builder.ToString();
		}

		private string MsBuildOutputFile(IIntegrationResult result)
		{
			return Path.Combine(result.ArtifactDirectory, LogFilename);
		}
	}
}
