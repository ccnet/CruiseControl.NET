using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// Publisher that provides for executing an arbitrary command.
	/// Written by Garrett Smith, gsmith@northwestern.edu.
	/// </summary>
	[ReflectorType("executable")]
	public class ExecutablePublisher : PublisherBase
	{
		private StreamReader _standardError;
		private StreamReader _standardOutput;
		private volatile bool _stopReading = false;

		private static readonly string _LABEL_ENVIRONMENT_VARIABLE = "ccnet.label";

		/// <summary>
		/// The arguments that are passed to the executable.  Defaults to empty.
		/// </summary>
		[ReflectorProperty("arguments", Required = false)] public string Arguments = string.Empty;

		/// <summary>
		/// The filename of the executable to be run.
		/// </summary>
		[ReflectorProperty("executable", Required = true)] public string Executable = null;

		/// <summary>
		/// The maximum amount of time that the publisher should wait for the executable to exit.  Measured
		/// in milliseconds; defaults to 60 seconds.
		/// </summary>
		[ReflectorProperty("timeout", Required = false)] public int Timeout = 60*1000; // 60 seconds

		/// <summary>
		/// The working directory of the executable; defaults to the current directory.
		/// </summary>
		[ReflectorProperty("workingDirectory", Required = false)] public string WorkingDirectory = Environment.CurrentDirectory;

		/// <summary>
		/// Whether a nonzero exit status should be considered fatal (throw a CruiseControlException).
		/// </summary>
		[ReflectorProperty("nonzeroExitFatal", Required = false)] public bool NonzeroExitFatal = false;

		/// <summary>
		/// The standard output from the process, if any.
		/// </summary>
		public string StandardOutput;

		/// <summary>
		/// The standard error from the process, if any.
		/// </summary>
		public string StandardError;

		public override void PublishIntegrationResults(IProject project, IIntegrationResult result)
		{
			Thread standardOutput = null;
			Thread standardError = null;
			Process process = null;
			try
			{
				process = CreateProcess(result);
				standardOutput = new Thread(new ThreadStart(ReadFromStandardOutput));
				standardOutput.Name = "StandardOutput";
				standardError = new Thread(new ThreadStart(ReadFromStandardError));
				standardError.Name = "StandardError";
				process.Start();
				_standardError = process.StandardError;
				_standardOutput = process.StandardOutput;
				standardOutput.Start();
				standardError.Start();
				process.WaitForExit(Timeout);
				HandleProcessExit(process);
			}
			catch (CruiseControlException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new CruiseControlException("unknown error", e);
			}
			finally
			{
				KillProcessIfNeeded(process);
				_stopReading = true;
				standardOutput.Join(50);
				standardError.Join(50);
				process.Close();
			}
		}

		private Process CreateProcess(IIntegrationResult result)
		{
			Process process = new Process();
			process.StartInfo.FileName = Executable;
			process.StartInfo.Arguments = Arguments;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.ErrorDialog = false;
			process.StartInfo.EnvironmentVariables.Add(_LABEL_ENVIRONMENT_VARIABLE, result.Label);
			process.StartInfo.WorkingDirectory = WorkingDirectory;
			return process;
		}

		private void HandleProcessExit(Process process)
		{
			if (process.HasExited && NonzeroExitFatal && process.ExitCode != 0)
			{
				throw new CruiseControlException(string.Format("{0} {1} returned nonzero exit code: {2}", Executable, Arguments, process.ExitCode));
			}
			else if (! process.HasExited)
			{
				throw new CruiseControlException(string.Format("\"{0} {1}\" didn't exit before {2} millisecond timeout", Executable, Arguments, Timeout));
			}
		}

		private void KillProcessIfNeeded(Process process)
		{
			if (process != null && !process.HasExited)
			{
				process.Kill();
			}
		}

		private void ReadFromStandardOutput()
		{
			StandardOutput = ReadFromReader(_standardOutput);
		}

		private void ReadFromStandardError()
		{
			StandardError = ReadFromReader(_standardError);
		}

		private string ReadFromReader(StreamReader input)
		{
			StringWriter output = new StringWriter();
			try
			{
				string line;
				while (! _stopReading)
				{
					line = input.ReadLine();
					if (line != null)
					{
						output.WriteLine(line);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			finally
			{
				input.Close();
				output.Close();
			}
			return output.ToString();
		}
	}
}