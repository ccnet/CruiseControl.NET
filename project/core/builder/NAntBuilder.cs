using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder
{
	[ReflectorType("nant")]
	public class NAntBuilder : IBuilder
	{
		private readonly string DEFAULT_BUILDARGS = "-logger:" + Configuration.NAntLogger;

		private string _executable;
		private string _baseDirectory;
		private string _buildArgs;
		private string _buildfile;
		private int _buildTimeoutSeconds;
		private string[] _targets = new string[0];

		public NAntBuilder() 
		{
			_buildArgs = DEFAULT_BUILDARGS;
		}

		#region Reflector properties

		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return _executable; }
			set { _executable = value; }
		}		

		[ReflectorProperty("baseDirectory")]
		public string BaseDirectory
		{
			get { return _baseDirectory; }
			set { _baseDirectory = value; }
		}

		//TODO: can this be optional?
		[ReflectorProperty("buildFile")]
		public string BuildFile
		{
			get { return _buildfile; }
			set { _buildfile = value; }
		}

		[ReflectorProperty("buildArgs", Required=false)]
		public string BuildArgs
		{
			get { return _buildArgs; }
			set { _buildArgs = value; }
		}

		[ReflectorArray("targetList", Required=false)]
		public string[] Targets
		{
			get { return _targets; }
			set { _targets = value; }
		}

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero (or equivalently, omit it from the Xml configuration)
		/// to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required=false)]
		public int BuildTimeoutSeconds
		{
			get { return _buildTimeoutSeconds; }
			set { _buildTimeoutSeconds = value; }
		}

		#endregion

		public string LabelToApply = "NO-LABEL";
		
		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working && result.Modifications.Length > 0;
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
		internal string CreateArgs()
		{
			return string.Format("-buildfile:{0} {1} -D:label-to-apply={2} {3}",
				BuildFile, BuildArgs, LabelToApply, string.Join(" ", Targets));
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
				int exitCode = AttemptExecute(result);
				if (exitCode == 0)
				{
					result.Status = IntegrationStatus.Success;
				}
				else
				{
					result.Status = IntegrationStatus.Failure;
					Log.Info("NAnt build failed - exit code: " + exitCode);
				}
			}
			catch (CruiseControlException)
			{
				throw;
			}
			catch (Exception e) 
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", BuildCommand, e), e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <returns>The process exit code: 0 for successful completion, -1 for an error.</returns>
		/// <exception cref="CruiseControlException">If a BuildTimeout is specified, and the process exceeds it.</exception>
		protected virtual int AttemptExecute(IntegrationResult result)
		{
			using (Process process = ProcessUtil.CreateProcess(Executable, CreateArgs(), BaseDirectory))
			{
				// start the process, and redirect output
				TextReader stdOut = ProcessUtil.ExecuteRedirected(process);

				// read the std output in another thread (otherwise it'll block and we won't be able to do timeout)
				StdOutReader stdOutReader = new StdOutReader(result, stdOut);
				
				// wait for the process
				if (BuildTimeoutSeconds > 0)
				{
					// wait prescribed number of milliseconds
					process.WaitForExit(BuildTimeoutSeconds * 1000);
					
					if (! process.HasExited)
					{
						// the process timed out
						process.Kill();

						// this causes the build to end up in 'exception' state.  the exception message is logged,
						// though currently doesn't appear on the build web page
						throw new BuilderException(this, "NAnt process timed out (after " + BuildTimeoutSeconds + " seconds)");
					}
				}
				else
				{
					// NOTE: in this scenario, if NAnt crashes and displays a modal dialog, CCNet will hang
					// until the user clicks OK...  if this is a remote computer it may go unchecked for some time
					process.WaitForExit();
				}

				// collect output from the process before returning (join thread)
				stdOutReader.BlockUntilFinishedReading();

				// if the process timed out, we'll be unable to read this value -- InvalidOperationException
				return process.ExitCode;
			}
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", 
				BaseDirectory, string.Join(", ", Targets), Executable, BuildFile);
		}

		#region Inner-type: StdOutReader

		/// <summary>
		/// Sets IntegrationResult output text by reading from a TextReader in another thread.
		/// This avoids blocking the main program thread, which prohibits other activities with
		/// the process (specifically, enforcing a build timeout).
		/// </summary>
		public class StdOutReader
		{
			IntegrationResult _result;
			TextReader _reader;
			Thread _thread;

			public StdOutReader(IntegrationResult result, TextReader stdOut)
			{
				_result = result;
				_reader = stdOut;

				_thread = new Thread(new ThreadStart(ReadToEnd));
				_thread.Name = "NAntStdOutReader";
				_thread.Start();
			}

			private void ReadToEnd()
			{
				_result.Output = _reader.ReadToEnd();
			}

			public void BlockUntilFinishedReading()
			{
				_thread.Join();
			}
		}

		#endregion
	}
}