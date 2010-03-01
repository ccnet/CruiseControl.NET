namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// The &lt;msbuild&gt; task is used to execute MsBuild projects, which are the default project format for Visual Studio 2005 projects
    /// and can also be compiled by using the MSBuild application that ships with the .NET 2 Framework.
    /// </para>
    /// <para>
    /// In order to work with the results of MsBuild it is important to use a custom xml logger to format the build results. For details on
    /// this, and a tutorial on how to use the task, see <link>Using CruiseControl.NET with MSBuild</link>.
    /// </para>
    /// </summary>
    /// <title>MSBuild Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;msbuild /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;msbuild&gt;
    /// &lt;executable&gt;C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe&lt;/executable&gt;
    /// &lt;workingDirectory&gt;C:\dev\ccnet&lt;/workingDirectory&gt;
    /// &lt;projectFile&gt;CCNet.sln&lt;/projectFile&gt;
    /// &lt;buildArgs&gt;/noconsolelogger /p:Configuration=Debug /v:diag&lt;/buildArgs&gt;
    /// &lt;targets&gt;Build;Test&lt;/targets&gt;
    /// &lt;timeout&gt;900&lt;/timeout&gt;
    /// &lt;logger&gt;C:\Program Files\CruiseControl.NET\server\ThoughtWorks.CruiseControl.MsBuild.dll&lt;/logger&gt;
    /// &lt;/msbuild&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <includePage>Integration Properties</includePage>
    /// <para>
    /// Many thanks to Szymon Kobalczyk for helping out with this part of CruiseControl.NET.
    /// </para>
    /// </remarks>
    [ReflectorType("msbuild")]
    public class MsBuildTask
        : BaseExecutableTask
	{
		public const string LogFilename = "msbuild-results-{0}.xml";
        public readonly Guid LogFileId = Guid.NewGuid();
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

        #region Public fields
        #region Executable
        /// <summary>
        /// The location of the MSBuild.exe executable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>MSBuild with .NET Framework, xbuild on Unix with Mono.</default>
        [ReflectorProperty("executable", Required=false)]
		public string Executable;
        #endregion

        #region WorkingDirectory
        /// <summary>
        /// The directory to run MSBuild in - this is generally the directory containing your build project. If relative, is a
        /// subdirectory of the Project Working Directory.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory;
        #endregion

        #region ProjectFile
        /// <summary>
        /// The name of the build project to run, relative to the workingDirectory. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>Default MSBuild file.</default>
        [ReflectorProperty("projectFile", Required = false)]
		public string ProjectFile;
        #endregion

        #region BuildArgs
        /// <summary>
        /// Any extra arguments to pass through to MSBuild.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs;
        #endregion

        #region Targets
        /// <summary>
        /// A semicolon-separated list of the targets to run.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Default project target.</default>
        [ReflectorProperty("targets", Required = false)]
		public string Targets;
        #endregion

        #region Logger
        /// <summary>
        /// The full path to the assembly containing the custom logger to use. Arguments can be passed to the logger by appending them
        /// after the logger name separated by a semicolon. Only if the assembly contains more than one logger implementation you need to
        /// specify the logger class (see MSBuild reference): [LoggerClass,]LoggerAssembly[;LoggerParameters] 
        /// </summary>
        /// <version>1.0</version>
        /// <default>ThoughtWorks.CruiseControl.MsBuild.XmlLogger, ThoughtWorks.CruiseControl.MsBuild.dll</default>
        [ReflectorProperty("logger", Required = false)]
		public string Logger;
        #endregion

        #region Timeout
        /// <summary>
        /// Number of seconds to wait before assuming that the process has hung and should be killed. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
		public int Timeout = DefaultTimeout;
        #endregion

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority = ProcessPriorityClass.Normal;
        #endregion

        #endregion

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

        /// <summary>
        /// Gets the requested priority class value for this Task.
        /// </summary>
        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return this.Priority;
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

		private string MsBuildOutputFile(IIntegrationResult result)
		{
            return Path.Combine(result.ArtifactDirectory, string.Format(LogFilename, LogFileId));
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
