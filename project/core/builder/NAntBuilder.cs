using System;
using System.Collections;
using System.IO;
using System.Text;
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
		public const string DEFAULT_LOGGER = "NAnt.Core.XmlLogger";
		public const bool DEFAULT_NOLOGO = true;

		private ProcessExecutor _executor;

		public NAntBuilder() : this(new ProcessExecutor())
		{
		}

		public NAntBuilder(ProcessExecutor executor)
		{
			_executor = executor;
		}

		private string _executable = DEFAULT_EXECUTABLE;
		private string buildFile = "";
		private string configuredBaseDirectory = "";
		private string buildArgs = "";
		private string logger = DEFAULT_LOGGER;
		private int buildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;
		private bool nologo = DEFAULT_NOLOGO;

		[ReflectorArray("targetList", Required = false)]
		public string[] Targets = new string[0];


		[ReflectorProperty("executable", Required = false)] 
		public string Executable
		{
			get { return _executable; }
			set { _executable = value; }
		}

		[ReflectorProperty("buildFile", Required = false)] 
		public string BuildFile
		{
			get { return buildFile; }
			set { buildFile = value; }
		}

		[ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory
		{
			get { return configuredBaseDirectory; }
			set { configuredBaseDirectory = value; }
		}

		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs
		{
			get { return buildArgs; }
			set { buildArgs = value; }
		}

		[ReflectorProperty("logger", Required = false)]
		public string Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		[ReflectorProperty("nologo", Required = false)]
		public bool NoLogo
		{
			get { return nologo; }
			set { nologo = value; }
		}

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds
		{
			get { return buildTimeoutSeconds; }
			set { buildTimeoutSeconds = value; }
		}

		/// <summary>
		/// Runs the integration using NAnt.  The build number is provided for labelling, build
		/// timeouts are enforced.  The specified targets are used for the specified NAnt build file.
		/// StdOut from nant.exe is redirected and stored.
		/// </summary>
		/// <param name="result">For storing build output.</param>
		public void Run(IIntegrationResult result)
		{
			ProcessResult processResult = AttemptExecute(CreateProcessInfo(result));
			result.Output = processResult.StandardOutput;

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "NAnt process timed out (after " + buildTimeoutSeconds + " seconds)");
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

		private ProcessInfo CreateProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable, CreateArgs(result), BaseDirectory(result));
			info.TimeOut = buildTimeoutSeconds*1000;
			return info;
		}

		private string BaseDirectory(IIntegrationResult result)
		{
			if (StringUtil.IsBlank(configuredBaseDirectory))
			{
				return result.WorkingDirectory;
			}
			else if (Path.IsPathRooted(configuredBaseDirectory))
			{
				return configuredBaseDirectory;
			}
			else
			{
				return Path.Combine(result.WorkingDirectory, configuredBaseDirectory);
			}
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
			get { return string.Format("{0} {1}", Executable, buildArgs); }
		}

		/// <summary>
		/// Creates the command line arguments to the nant.exe executable. These arguments
		/// specify the build-file name, the targets to build to, 
		/// </summary>
		/// <returns></returns>
		private string CreateArgs(IIntegrationResult result)
		{
			StringBuilder buffer = new StringBuilder();
			AppendNoLogoArg(buffer);
			AppendBuildFileArg(buffer);
			AppendLoggerArg(buffer);
			AppendIfNotBlank(buffer, buildArgs);
			AppendIntegrationResultProperties(buffer, result);
			AppendTargets(buffer);
			return buffer.ToString();
		}

		private void AppendNoLogoArg(StringBuilder buffer)
		{
			if (nologo) buffer.Append("-nologo");
		}

		private void AppendBuildFileArg(StringBuilder buffer)
		{
			AppendIfNotBlank(buffer, @"-buildfile:{0}", SurroundInQuotesIfContainsSpace(buildFile));
		}

		private void AppendLoggerArg(StringBuilder buffer)
		{
			AppendIfNotBlank(buffer, "-logger:{0}", logger);
		}

		private void AppendIntegrationResultProperties(StringBuilder buffer, IIntegrationResult result)
		{
			string label = SurroundInQuotesIfContainsSpace(result.Label);
			AppendIfNotBlank(buffer, @"-D:label-to-apply={0}", label);
			AppendIfNotBlank(buffer, @"-D:ccnet.label={0}", label);
			AppendIfNotBlank(buffer, @"-D:ccnet.buildcondition={0}", result.BuildCondition.ToString());
		}

		private void AppendTargets(StringBuilder buffer)
		{
			AppendIfNotBlank(buffer, string.Join(" ", Targets));
		}

		private void AppendIfNotBlank(StringBuilder buffer, string value)
		{
			AppendIfNotBlank(buffer, "{0}", value);
		}

		private void AppendIfNotBlank(StringBuilder buffer, string format, string value)
		{
			if (! StringUtil.IsBlank(value))
			{
				if (buffer.Length > 0) buffer.Append(" ");
				buffer.AppendFormat(format, value);
			}
		}

		private string SurroundInQuotesIfContainsSpace(string value)
		{
			if (! StringUtil.IsBlank(value) && value.IndexOf(' ') >= 0)
				return string.Format(@"""{0}""", value);
			return value;
		}

		public override string ToString()
		{
			string baseDirectory = (configuredBaseDirectory != null ? configuredBaseDirectory : "");
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", baseDirectory, string.Join(", ", Targets), Executable, buildFile);
		}

		public string TargetsForPresentation
		{
			get
			{
				StringBuilder combined = new StringBuilder();
				foreach (string file in Targets)
				{
					if (combined.Length > 0) combined.Append(Environment.NewLine);
					combined.Append(file);
				}
				return combined.ToString();
			}
			set
			{
				if (StringUtil.IsBlank(value))
				{
					Targets = new string[0];
					return;
				}
				ArrayList targets = new ArrayList();
				using (StringReader reader = new StringReader(value))
				{
					while (reader.Peek() >= 0)
					{
						targets.Add(reader.ReadLine());
					}
				}
				Targets = (string[]) targets.ToArray(typeof (string));
			}
		}
	}
}