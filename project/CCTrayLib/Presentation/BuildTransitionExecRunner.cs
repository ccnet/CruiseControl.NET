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
                string fileName = string.Empty;
                string arguments = string.Empty;
                try
                {
                    ProcessStartInfo lProc = new ProcessStartInfo();
                    lProc.UseShellExecute = false;
                    lProc.CreateNoWindow = true;
                    if (command.StartsWith("\""))
                    {
                        command = command.TrimStart(new[] { '\"' });
                        fileName = command.Substring(0, command.IndexOf('\"'));
                        arguments = command.Substring(command.IndexOf('\"')).Trim();
                    }

                    else if (command.Contains(" "))
                    {
                        fileName = command.Substring(0, command.IndexOf(' '));
                        arguments = command.Substring(command.IndexOf(' '));
                    }
                    else
                    {
                        fileName = command;
                    }
                    lProc.FileName = fileName;
                    lProc.Arguments = arguments;
                    Process.Start(lProc);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    string message = string.Format(
                                    "Unable to execute command \"{0}\" FileName: \"{1}\" Arguments: \"{2}\" {3}",
                                    command, fileName, arguments, e);
                    MessageBox.Show(message);
                }
            }
        }
    }
}
