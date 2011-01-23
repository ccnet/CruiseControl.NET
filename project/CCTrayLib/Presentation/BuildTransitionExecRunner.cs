using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class BuildTransitionExecRunner
	{
		private IProjectMonitor monitor;
		private ExecCommands configuration;
		private ProjectState status = null;

		public BuildTransitionExecRunner(IProjectMonitor monitor, ExecCommands configuration)
		{
			this.monitor = monitor;
			this.configuration = configuration;

			monitor.Polled += Monitor_Polled;
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			Status = monitor.ProjectState;
		}

		private ProjectState Status
		{
			set
			{
				if (status != value)
				{
					if (value == ProjectState.Broken)
					{
						Exec(configuration.BrokenCommand);
					}
					else if (value == ProjectState.BrokenAndBuilding)
					{
						Exec(configuration.BrokenAndBuildingCommand);
					}
					else if (value == ProjectState.Building)
					{
						Exec(configuration.BuildingCommand);
					}
					else if (value == ProjectState.NotConnected)
					{
						Exec(configuration.NotConnectedCommand);
					}
					else if (value == ProjectState.Success)
					{
						Exec(configuration.SuccessCommand);
					}
				}
				status = value;
			}
		}

		private static void Exec(string command)
		{
			if (!string.IsNullOrEmpty(command))
			{
				try
				{
					ProcessStartInfo lProc = new ProcessStartInfo();
					lProc.UseShellExecute = false;
					lProc.CreateNoWindow = true;

					if (command.Contains(" "))
					{
						lProc.FileName = command.Substring(0, command.IndexOf(' '));
						lProc.Arguments = command.Substring(command.IndexOf(' '));
					}
					else
					{
						lProc.FileName = command;
					}
					Process.Start(lProc);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					MessageBox.Show("Unable to execute command \"" + command + "\": " + e);
				}
			}
		}
	}
}
