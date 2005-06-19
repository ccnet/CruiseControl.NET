using System;
using System.Collections;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nant")]
	public class NAntTask : ITask
	{
		public const int DefaultBuildTimeout = 600;
		public const string DefaultExecutable = "nant.exe";
		public const string DefaultLogger = "NAnt.Core.XmlLogger";
		public const bool DefaultNoLogo = true;

		private ProcessExecutor executor;

		public NAntTask() : this(new ProcessExecutor())
		{
		}

		public NAntTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		[ReflectorArray("targetList", Required = false)]
		public string[] Targets = new string[0];

		[ReflectorProperty("executable", Required = false)] 
		public string Executable = DefaultExecutable;

		[ReflectorProperty("buildFile", Required = false)] 
		public string BuildFile = string.Empty;

		[ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory = string.Empty;

		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs = string.Empty;

		[ReflectorProperty("logger", Required = false)]
		public string Logger = DefaultLogger;

		[ReflectorProperty("nologo", Required = false)]
		public bool NoLogo = DefaultNoLogo;

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds = DefaultBuildTimeout;

		/// <summary>
		/// Runs the integration using NAnt.  The build number is provided for labelling, build
		/// timeouts are enforced.  The specified targets are used for the specified NAnt build file.
		/// StdOut from nant.exe is redirected and stored.
		/// </summary>
		/// <param name="result">For storing build output.</param>
		public void Run(IIntegrationResult result)
		{
			ProcessResult processResult = AttemptExecute(CreateProcessInfo(result));
			result.AddTaskResult(new ProcessTaskResult(processResult));

			// is this right?? or should this break the build
			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "NAnt process timed out (after " + BuildTimeoutSeconds + " seconds)");
			}
		}

		private ProcessInfo CreateProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable, CreateArgs(result), BaseDirectory(result));
			info.TimeOut = BuildTimeoutSeconds*1000;
			return info;
		}

		private string BaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
		}

		protected ProcessResult AttemptExecute(ProcessInfo info)
		{
			try
			{
				return executor.Execute(info);
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

		/// TODO: refactor to use ProcessArgumentBuilder
		private string CreateArgs(IIntegrationResult result)
		{
			StringBuilder buffer = new StringBuilder();
			AppendNoLogoArg(buffer);
			AppendBuildFileArg(buffer);
			AppendLoggerArg(buffer);
			AppendIfNotBlank(buffer, BuildArgs);
			AppendIntegrationResultProperties(buffer, result);
			AppendTargets(buffer);
			return buffer.ToString();
		}

		private void AppendNoLogoArg(StringBuilder buffer)
		{
			if (NoLogo) buffer.Append("-nologo");
		}

		private void AppendBuildFileArg(StringBuilder buffer)
		{
			AppendIfNotBlank(buffer, @"-buildfile:{0}", SurroundInQuotesIfContainsSpace(BuildFile));
		}

		private void AppendLoggerArg(StringBuilder buffer)
		{
			AppendIfNotBlank(buffer, "-logger:{0}", Logger);
		}

		private void AppendIntegrationResultProperties(StringBuilder buffer, IIntegrationResult result)
		{
			string label = SurroundInQuotesIfContainsSpace(result.Label);
			AppendIfNotBlank(buffer, @"-D:label-to-apply={0}", label);
			AppendIfNotBlank(buffer, @"-D:ccnet.label={0}", label);
			AppendIfNotBlank(buffer, @"-D:ccnet.buildcondition={0}", result.BuildCondition.ToString());
			AppendIfNotBlank(buffer, @"-D:ccnet.working.directory={0}", SurroundInQuotesIfContainsSpace(RemoveTrailingSlash(result.WorkingDirectory)));
			AppendIfNotBlank(buffer, @"-D:ccnet.artifact.directory={0}", SurroundInQuotesIfContainsSpace(RemoveTrailingSlash(result.ArtifactDirectory)));
		}

		private string RemoveTrailingSlash(string directory)
		{			
			return StringUtil.IsBlank(directory) ? string.Empty : directory.TrimEnd(Path.DirectorySeparatorChar);
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
			string baseDirectory = (ConfiguredBaseDirectory != null ? ConfiguredBaseDirectory : "");
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", baseDirectory, string.Join(", ", Targets), Executable, BuildFile);
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