using System;
using System.Diagnostics;
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// A Process-Monitor receives the currently active process of a specific project
	/// and stores a reference to it.
	/// It can be used to abort a running build.
	/// </summary>
	
	public class ProcessMonitor
	{
		private static readonly IDictionary processMonitors = Hashtable.Synchronized(new Hashtable());
		private Process actProcess;
		
		// Return an existing Processmonitor or create a new one
		public static ProcessMonitor GetProcessMonitorByProject(string projectName)
		{
			ProcessMonitor pc = (ProcessMonitor)processMonitors[projectName];
			
			// Double-checked locking pattern (better performance)
			if(null == pc)
			{
				lock (typeof(ProcessMonitor))
				{
					if (null == pc) pc = new ProcessMonitor();
				}
				processMonitors.Add(projectName, pc);
			}
			return  pc;
		}
		
		public void MonitorNewProcess(Process p)
		{
			actProcess = p;
		}

		// Kill the process
		public string KillProcess()
		{
			try
			{
				actProcess.Kill();
				Log.Info("------------------------------------------------------------------");
				Log.Info("---------The Build Process was successfully aborted---------------");
				Log.Info("------------------------------------------------------------------");
				return "success";
			}
			catch (NullReferenceException e)
			{
				Log.Info(string.Format("System.NullReferenceException: {0}", e));
				Log.Info("The process can't be terminated because it hasn't started yet.");
				return "The process can't be terminated because it hasn't started yet.";
			}
			catch (InvalidOperationException e)
			{
				Log.Info(string.Format("System.InvalidOperationException: {0}", e));
				Log.Info("The process can't be terminated because it has already ended.");
				return "The process can't be terminated because it has already ended.";
			}
			catch (Exception e)
			{
				Log.Info(string.Format("unknown exception: {0}", e));
				Log.Info("!!!!!!!!!!!!!!!!unknown exception!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				return "failure";
			}
		}
	}
}
