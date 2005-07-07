using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// The ProcessExecutor executes a new <see cref="Process"/> using the properties specified in the input <see cref="ProcessInfo" />.
	/// ProcessExecutor is designed specifically to deal with processes that redirect the results of both
	/// the standard output and the standard error streams.  Reading from these streams is performed in
	/// a separate thread using the <see cref="ProcessReader"/> class, in order to prevent deadlock while 
	/// blocking on <see cref="Process.WaitForExit(int)"/>.
	/// If the process does not complete executing within the specified timeout period, the ProcessExecutor will attempt to kill the process.
	/// As process termination is asynchronous, the ProcessExecutor needs to wait for the process to die.  Under certain circumstances, 
	/// the process does not terminate gracefully after being killed, causing the ProcessExecutor to throw an exception.
	/// </summary>
	public class ProcessExecutor
	{
		private const int WAIT_FOR_KILLED_PROCESS_TIMEOUT = 5000;

		public virtual ProcessResult Execute(ProcessInfo processInfo)
		{
			using (Process process = Start(processInfo))
			{
				using (ProcessReader standardOutput = new ProcessReader(process.StandardOutput), standardError = new ProcessReader(process.StandardError))
				{
					WriteToStandardInput(process, processInfo);

					bool hasExited = process.WaitForExit(processInfo.TimeOut);
					if (hasExited)
					{
						standardOutput.WaitForExit();
						standardError.WaitForExit();
					}
					else
					{
						Kill(process, processInfo, standardOutput, standardError);
					}
					return new ProcessResult(standardOutput.Output, standardError.Output, process.ExitCode, ! hasExited);
				}
			}
		}

		private Process Start(ProcessInfo processInfo)
		{
			Process process = processInfo.CreateProcess();
			Log.Debug(string.Format("Attempting to start process [{0}] in working directory [{1}] with arguments [{2}]", process.StartInfo.FileName, process.StartInfo.WorkingDirectory, process.StartInfo.Arguments));

			try
			{
				bool isNewProcess = process.Start();
				if (! isNewProcess) Log.Debug("Reusing existing process...");
			}
			catch (Win32Exception e)
			{
				string filename = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
				string msg = string.Format("Unable to execute file [{0}].  The file may not exist or may not be executable.", filename);
				throw new IOException(msg, e);
			}
			return process;
		}

		private void Kill(Process process, ProcessInfo processInfo, ProcessReader standardOutput, ProcessReader standardError)
		{
			Log.Warning(string.Format("Process timed out: {0} {1}.  Process id: {2}.  This process will now be killed.", processInfo.FileName, processInfo.Arguments, process.Id));
			Log.Debug(string.Format("Process stdout: {0}", standardOutput.Output));
			Log.Debug(string.Format("Process stderr: {0}", standardError.Output));
			try
			{
				process.Kill();
				if (! process.WaitForExit(WAIT_FOR_KILLED_PROCESS_TIMEOUT)) 
					throw new CruiseControlException(string.Format(@"The killed process {0} did not terminate within the allotted timeout period {1}.  The process or one of its child processes may not have died.  This may create problems when trying to re-execute the process.  It may be necessary to reboot the server to recover.", process.Id, WAIT_FOR_KILLED_PROCESS_TIMEOUT, process));
			}
			catch (InvalidOperationException)
			{
				Log.Warning(string.Format("Process has already exited before getting killed: {0}", process.Id));				
			}
			Log.Warning(string.Format("The timed out process has been killed: {0}", process.Id));
		}

		private void WriteToStandardInput(Process process, ProcessInfo processInfo)
		{
			// TODO - not tested yet - any ideas? -- Mike R
			// TODO - maybe we actually need to do this line-by-line. In that case we should probably extract this 
			//   to a 'ProcessWriter' and do the thread stuff like the Readers do. -- Mike R
			if (process.StartInfo.RedirectStandardInput)
			{
				process.StandardInput.Write(processInfo.StandardInputContent);
				process.StandardInput.Flush();
				process.StandardInput.Close();
			}
		}
	}
}