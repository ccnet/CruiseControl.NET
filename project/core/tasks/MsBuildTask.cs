using System.Collections;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Reflection;
using System;


namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("msbuild")]
    public class MsBuildTask
        : BaseExecutableTask
	{
		public const string LogFilename = "msbuild-results.xml";
		public const int DefaultTimeout = 600;
        private IShadowCopier shadowCopier;
		private readonly IExecutionEnvironment executionEnvironment;

		public MsBuildTask() : this(new ProcessExecutor(), new ExecutionEnvironment(), new DefaultShadowCopier())
		{}

		public MsBuildTask(ProcessExecutor executor, IExecutionEnvironment executionEnvironment, IShadowCopier shadowCopier)
		{
			this.executor = executor;
			this.executionEnvironment = executionEnvironment;
			this.shadowCopier = shadowCopier;

			Executable = GetDefaultExecutable();
		}

		[ReflectorProperty("executable", Required=false)]
		public string Executable;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory;

		[ReflectorProperty("projectFile", Required=false)]
		public string ProjectFile;

		[ReflectorProperty("buildArgs", Required=false)]
		public string BuildArgs;

		[ReflectorProperty("targets", Required=false)]
		public string Targets;

		[ReflectorProperty("logger", Required=false)]
		public string Logger;

		[ReflectorProperty("timeout", Required=false)]
		public int Timeout = DefaultTimeout;

		protected override string GetProcessFilename()
		{
			return string.IsNullOrEmpty(Executable) ? GetDefaultExecutable() : Executable;
		}

		protected override string GetProcessArguments(IIntegrationResult result)
		{
			ProcessArgumentBuilder b = new ProcessArgumentBuilder();

			b.AddArgument("/nologo");
            if (!string.IsNullOrEmpty(Targets))
			{
				b.AddArgument("/t:");
				string targets = string.Empty;
				foreach (string target in Targets.Split(';'))
				{
					if (targets != string.Empty) 
						targets = string.Format("{0};{1}", targets, StringUtil.AutoDoubleQuoteString(target));
					else 
						targets = StringUtil.AutoDoubleQuoteString(target);
				}
				b.Append(targets);
			}
			b.AppendArgument(GetPropertyArgs(result));
			b.AppendArgument(BuildArgs);
			b.AddArgument(ProjectFile);
			b.AppendArgument(GetLoggerArgs(result));

			return b.ToString();
		}

		protected override string GetProcessBaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(WorkingDirectory);
		}

		protected override int GetProcessTimeout()
		{
			return Timeout * 1000;
		}

        protected override bool Execute(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description :
				string.Format("Executing MSBuild :BuildFile: {0}", ProjectFile));

        	ProcessResult processResult = TryToRun(CreateProcessInfo(result), result);

			string buildOutputFile = MsBuildOutputFile(result);
			if (File.Exists(buildOutputFile))
				result.AddTaskResult(new FileTaskResult(buildOutputFile));

            result.AddTaskResult(new ProcessTaskResult(processResult, true));

			if (processResult.TimedOut)
				throw new BuilderException(this, "MSBuild process timed out (after " + Timeout + " seconds)");

            return !processResult.Failed;
		}

		private static string GetPropertyArgs(IIntegrationResult result)
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
			if (string.IsNullOrEmpty(Logger))
			{
                // Since hot-swapping shadow copies the files, we also need to move the logger over
                var loggerPath = shadowCopier.RetrieveFilePath("ThoughtWorks.CruiseControl.MsBuild.dll");
                if (!string.IsNullOrEmpty(loggerPath)) builder.Append(StringUtil.AutoDoubleQuoteString(loggerPath) + ";");
			}
			else
			{
				builder.Append(CheckAndQuoteLoggerSetting(Logger) + ";");
			}

			builder.Append(StringUtil.AutoDoubleQuoteString(MsBuildOutputFile(result)));
			return builder.ToString();
		}

		private static string MsBuildOutputFile(IIntegrationResult result)
		{
			return Path.Combine(result.ArtifactDirectory, LogFilename);
		}

		private static string CheckAndQuoteLoggerSetting(string logger)
		{
			if (logger.IndexOf(';') > -1)
			{
				Log.Error("The <logger> setting contains semicolons. Only commas are allowed.");
				throw new CruiseControlException("The <logger> setting contains semicolons. Only commas are allowed.");
			}

			bool spaceFound = false;
			StringBuilder b = new StringBuilder();			
			foreach (string part in logger.Split(','))
			{
				if (part.IndexOf(' ') > -1)
				{
					if (spaceFound)
					{
						Log.Error("The <logger> setting contains multiple spaces. Only the assembly name is allowed to contain spaces.");
						throw new CruiseControlException("The <logger> setting contains multiple spaces. Only the assembly name is allowed to contain spaces.");
					}
					
					b.Append(StringUtil.AutoDoubleQuoteString(part));
					spaceFound = true;
				}
				else
				{
					b.Append(part);
				}
				b.Append(",");
			}
			return b.ToString().TrimEnd(',');
		}

		/// <summary>
		/// Gets the default msbuild/xbuild executable.
		/// 
		/// On Windows/.NET:
		///		Return the path of the msbuild.exe of the current .NET framework CCNet is running on.
		/// 
		/// On Unix/Mono:
		///		Return xbuild.
		/// </summary>
		/// <returns></returns>
		private string GetDefaultExecutable()
		{
			if(executionEnvironment.IsRunningOnWindows)
			{
				return Path.Combine(executionEnvironment.RuntimeDirectory, "MSBuild.exe");
			}
			else
			{
				return "xbuild";
			}
		}
	}
}
