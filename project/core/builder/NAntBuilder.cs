using Exortech.NetReflector;
using System;
using System.Threading;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder
{
	[ReflectorType("nant")]
	public class NAntBuilder : IBuilder
	{
		private const int DEFAULT_BUILD_TIMEOUT = 600;
		private readonly string DEFAULT_BUILDARGS = "-logger:" + Configuration.NAntLogger;

		private string _executable;
		private string _baseDirectory;
		private string _buildArgs;
		private string _buildfile;
		private int _buildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;
		private string[] _targets = new string[0];

		public NAntBuilder()
		{
			_buildArgs = DEFAULT_BUILDARGS;
		}

		#region Reflector properties

		[ReflectorProperty("executable")]
		public string Executable
		{
			get
			{
				return _executable;
			}
			set
			{
				_executable = value;
			}
		}

		[ReflectorProperty("baseDirectory")]
		public string BaseDirectory
		{
			get
			{
				return _baseDirectory;
			}
			set
			{
				_baseDirectory = value;
			}
		}

		//TODO: can this be optional?

		[ReflectorProperty("buildFile")]
		public string BuildFile
		{
			get
			{
				return _buildfile;
			}
			set
			{
				_buildfile = value;
			}
		}

		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs
		{
			get
			{
				return _buildArgs;
			}
			set
			{
				_buildArgs = value;
			}
		}

		[ReflectorArray("targetList", Required = false)]
		public string[] Targets
		{
			get
			{
				return _targets;
			}
			set
			{
				_targets = value;
			}
		}

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero (or equivalently, omit it from the Xml configuration)
		/// to disable process timeouts.
		/// </summary>


		[ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds
		{
			get
			{
				return _buildTimeoutSeconds;
			}
			set
			{
				_buildTimeoutSeconds = value;
			}
		}

		#endregion

		public string LabelToApply = "NO-LABEL";

		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working && result.Modifications.Length > 0;
		}

		private string BuildCommand
		{
			get
			{
				return string.Format("{0} {1}", Executable, BuildArgs);
			}
		}

		/// <summary>
		/// Creates the command line arguments to the nant.exe executable. These arguments
		/// specify the build-file name, the targets to build to, 
		/// </summary>
		/// <returns></returns>


		internal string CreateArgs()
		{
			return string.Format("-buildfile:{0} {1} -D:label-to-apply={2} {3}", BuildFile, BuildArgs, LabelToApply, string.Join(" ", Targets));
		}

		/// <summary>
		/// Runs the integration, using NAnt.  The build number is provided for labelling, build
		/// timeouts are enforced.  The specified targets are used for the specified NAnt build file.
		/// StdOut from nant.exe is redirected and stored.
		/// </summary>
		/// <param name="result">For storing build output.</param>


		public void Run(IntegrationResult result)
		{
			if (result.Label != null && result.Label.Trim().Length > 0)
			{
				LabelToApply = result.Label;
			}

			try
			{
				ProcessResult processResult = AttemptExecute();
				result.Output = processResult.StandardOutput;
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
			catch(CruiseControlException)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", BuildCommand, e), e);
			}
		}

		protected virtual ProcessResult AttemptExecute()
		{
			ProcessExecutor executor = new ProcessExecutor();
			executor.Timeout = BuildTimeoutSeconds*1000;
			ProcessResult processResult = executor.Execute(Executable, CreateArgs(), BaseDirectory);
			if (processResult.TimedOut)
			{
				// this causes the build to end up in 'exception' state.  the exception message is logged,
				// though currently doesn't appear on the build web page
				throw new BuilderException(this, "NAnt process timed out(after " + BuildTimeoutSeconds + " seconds)");
			}
			return processResult;
		}

		protected virtual ProcessInfo CreateProcess(string filename, string arguments, string workingDirectory)
		{
			return new ProcessInfo(filename, arguments, workingDirectory);
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", BaseDirectory, string.Join(", ", Targets), Executable, BuildFile);
		}
	}
}