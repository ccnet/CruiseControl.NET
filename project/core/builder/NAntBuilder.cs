using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder
{
	[ReflectorType("nant")]
	public class NAntBuilder : IBuilder
	{
		public const int DEFAULT_BUILD_TIMEOUT = 600;
		public const string DEFAULT_EXECUTABLE = "nant.exe";
		public const string DEFAULT_BASEDIRECTORY = ".";
		public const string DEFAULT_LABEL = "NO-LABEL";
		public const string DEFAULT_LOGGER = "NAnt.Core.XmlLogger";

		private ProcessExecutor _executor;

		public NAntBuilder() : this(new ProcessExecutor()) { }

		public NAntBuilder(ProcessExecutor executor)
		{
			_executor = executor;
		}

		[ReflectorProperty("executable", Required = false)] 
		public string Executable = DEFAULT_EXECUTABLE;

		[ReflectorProperty("baseDirectory", Required = false)] 
		public string BaseDirectory = DEFAULT_BASEDIRECTORY;

		[ReflectorProperty("buildFile", Required = false)] 
		public string BuildFile;

		[ReflectorProperty("buildArgs", Required = false)] 
		public string BuildArgs;

		[ReflectorProperty("logger", Required = false)]
		public string Logger = DEFAULT_LOGGER;

		[ReflectorArray("targetList", Required = false)] 
		public string[] Targets = new string[0];

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;

		/// <summary>
		/// Runs the integration using NAnt.  The build number is provided for labelling, build
		/// timeouts are enforced.  The specified targets are used for the specified NAnt build file.
		/// StdOut from nant.exe is redirected and stored.
		/// </summary>
		/// <param name="result">For storing build output.</param>
		public void Run(IntegrationResult result)
		{
			ProcessResult processResult = AttemptExecute(CreateProcessInfo(result));
			result.Output = processResult.StandardOutput;

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "NAnt process timed out (after " + BuildTimeoutSeconds + " seconds)");
			}

			if (processResult.ExitCode == 0)
			{
				result.Status = IntegrationStatus.Success;
			}
			else
			{
				result.Status = IntegrationStatus.Failure;
				Log.Info("NAnt build failed: " + processResult.StandardError);
			}
		}

		private ProcessInfo CreateProcessInfo(IntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable, CreateArgs(result), BaseDirectory);
			info.TimeOut = BuildTimeoutSeconds*1000;
			return info;
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

		/// <summary>
		/// Creates the command line arguments to the nant.exe executable. These arguments
		/// specify the build-file name, the targets to build to, 
		/// </summary>
		/// <returns></returns>
		internal string CreateArgs(IntegrationResult result)
		{
			return string.Format("{0} {1} {2} {3} {4}", CreateBuildFileArg(), CreateLoggerArg(), BuildArgs, CreateLabelToApplyArg(result), string.Join(" ", Targets)).Trim();
		}

		private string CreateBuildFileArg()
		{
			if (StringUtil.IsBlank(BuildFile)) return string.Empty;

			return "-buildfile:" + BuildFile;
		}

		private string CreateLoggerArg()
		{
			if (StringUtil.IsBlank(Logger)) return string.Empty;

			return "-logger:" + Logger;
		}

		private string CreateLabelToApplyArg(IntegrationResult result)
		{
			string label = StringUtil.IsBlank(result.Label) ? DEFAULT_LABEL : result.Label;
			return "-D:label-to-apply=" + label;
		}

		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working && result.HasModifications();
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", BaseDirectory, string.Join(", ", Targets), Executable, BuildFile);
		}
	}
}