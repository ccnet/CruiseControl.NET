using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote;
using Mono.Options;

namespace ThoughtWorks.CruiseControl.CCCmd
{
    class Program
    {
    	private static bool help;
        private static string server;
        private static string target;
        private static string project;
        private static bool all;
        private static bool quiet;
        private static CommandType command;
        private static List<string> extra = new List<string>();
        
        static void Main(string[] args)
        {
           	OptionSet opts = new OptionSet();
        	opts.Add("h|?|help", "display this help screen", delegate (string v) { help = v != null; })
        		.Add("s|server=", "the CruiseControl.Net server to send the commands to (required for all actions except help)", delegate (string v) { server = v; })
        		.Add("t|target=", "the target server for all messages", delegate (string v) { target = v; })
        		.Add("p|project=", "the project to use (required for all actions except help and retrieve)", delegate (string v) { project = v; })
        		.Add("a|all", "lists all the projects (only valid for retrieve)", delegate (string v) { all = v != null; })
        		.Add("q|quiet", "run in quiet mode (do not print messages)", delegate (string v) { quiet = v != null; });
        	
        	try
        	{
        		extra = opts.Parse(args);
        	}
        	catch (OptionException e)
        	{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				return;
			}
        	
        	if((extra.Count == 1) && !help)
        	{
        		command = (CommandType) Enum.Parse(typeof(CommandType), extra[0], true);
        	}
        	else
        	{
        		DisplayHelp(opts);
        		return;
        	}
        	
            try
            {                
                switch (command)
                {
                    case CommandType.Help:
                        DisplayHelp(opts);
                        break;
                    case CommandType.Retrieve:
                        RunRetrive();
                        break;
                    case CommandType.ForceBuild:
                        RunForceBuild();
                        break;
                    case CommandType.AbortBuild:
                        RunAbortBuild();
                        break;
                    case CommandType.StartProject:
                        RunStartProject();
                        break;
                    case CommandType.StopProject:
                        RunStopProject();
                        break;
                    default:
                        throw new Exception("Unknown action: " + command.ToString());
                }
            }
            catch (Exception error)
            {
                WriteError("ERROR: An unknown error has occurred!", error);
            }
        }

        private static CruiseServerClientBase GenerateClient()
        {
            var client = new CruiseServerClientFactory().GenerateClient(server, target);
            return client;
        }

        private static void RunForceBuild()
        {
            if (ValidateParameter(server, "--server") &&
        	    ValidateNotAll() &&
                ValidateParameter(project, "--project"))
            {
                try
                {
                    var client = GenerateClient();
                    if (!quiet) WriteLine(string.Format("Sending ForceBuild request for '{0}'", project), ConsoleColor.White);
                    client.ForceBuild(project);
                    if (!quiet) WriteLine("ForceBuild request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunAbortBuild()
        {
            if (ValidateParameter(server, "--server") &&
                ValidateNotAll() &&
                ValidateParameter(project, "--project"))
            {
                try
                {
                    var client = GenerateClient();
                    if (!quiet) WriteLine(string.Format("Sending AbortBuild request for '{0}'", project), ConsoleColor.White);
                    client.AbortBuild(project);
                    if (!quiet) WriteLine("AbortBuild request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunStartProject()
        {
            if (ValidateParameter(server, "--server") &&
                ValidateNotAll() &&
                ValidateParameter(project, "--project"))
            {
                try
                {
                    var client = GenerateClient();
                    if (!quiet) WriteLine(string.Format("Sending StartProject request for '{0}'", project), ConsoleColor.White);
                    client.StartProject(project);
                    if (!quiet) WriteLine("StartProject request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunStopProject()
        {
            if (ValidateParameter(server, "--server") &&
                ValidateNotAll() &&
                ValidateParameter(project, "--project"))
            {
                try
                {
                    var client = GenerateClient();
                    if (!quiet) WriteLine(string.Format("Sending StopProject request for '{0}'", project), ConsoleColor.White);
                    client.StopProject(project);
                    if (!quiet) WriteLine("StopProject request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunRetrive()
        {
            if (ValidateParameter(server, "--server"))
            {
            	if (string.IsNullOrEmpty(project) && !all)
                {
                    WriteLine("Must specify either a project or use the '-all' option", ConsoleColor.Red);
                }
                else
                {
                    var client = GenerateClient();
                    if (all)
                    {
                        DisplayServerStatus(client, quiet);
                    }
                    else
                    {
                        DisplayProjectStatus(client, project, quiet);
                    }
                }
            }
        }

        private static void DisplayServerStatus(CruiseServerClientBase client, bool isQuiet)
        {
            try
            {
                if (!isQuiet) WriteLine("Retrieving snapshot from " + client.TargetServer, ConsoleColor.Gray);
                CruiseServerSnapshot snapShot = client.GetCruiseServerSnapshot();
                foreach (ProjectStatus project in snapShot.ProjectStatuses)
                {
                    DisplayProject(project);
                }
            }
            catch (Exception error)
            {
                WriteError("ERROR: Unable to retrieve server details", error);
            }
        }

        private static void DisplayProjectStatus(CruiseServerClientBase client, string projectName, bool isQuiet)
        {
            try
            {
                if (!isQuiet) WriteLine(string.Format("Retrieving project '{0}' on server {1}", projectName, client.TargetServer), ConsoleColor.Gray);
                CruiseServerSnapshot snapShot = client.GetCruiseServerSnapshot();
                foreach (ProjectStatus project in snapShot.ProjectStatuses)
                {
                    if (string.Equals(project.Name, projectName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        DisplayProject(project);
                    }
                }
            }
            catch (Exception error)
            {
                WriteError("ERROR: Unable to retrieve project details", error);
            }
        }

        private static void DisplayProject(ProjectStatus project)
        {
            WriteLine(string.Format("{0}: {1}", project.Name, project.Status), ConsoleColor.White);
            WriteLine(string.Format("\tActivity: {0}", project.Activity), ConsoleColor.White);
            WriteLine(string.Format("\tBuild Status: {0}", project.BuildStatus), ConsoleColor.White);
            if (!string.IsNullOrEmpty(project.BuildStage))
            {
                XmlDocument stageXml = new XmlDocument();
                try
                {
                    stageXml.LoadXml(project.BuildStage);
                    foreach (XmlElement stageItem in stageXml.SelectNodes("/data/Item"))
                    {
                        string stageTime = stageItem.GetAttribute("Time");
                        string stageData = stageItem.GetAttribute("Data");
                        WriteLine(string.Format("\tBuild Stage: {0} ({1})", stageData, stageTime), ConsoleColor.White);
                    }
                }
                catch
                {
                    WriteLine(string.Format("\tBuild Stage: {0}", project.BuildStage), ConsoleColor.White);
                }
            }
            WriteLine(string.Format("\tLast Build: {0:G}", project.LastBuildDate), ConsoleColor.White);
            WriteLine(string.Format("\tNext Build: {0:G}", project.NextBuildTime), ConsoleColor.White);
        }

        private static bool ValidateParameter(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteError(string.Format("Input parameter '{0}' is missing", name), null);
                return false;
            }
            return true;
        }

        private static bool ValidateNotAll()
        {
            if (all)
            {
                WriteError(string.Format("Input parameter '--all' is not valid for {0}", command), null);
                return false;
            }
            return true;
        }

        private static void WriteLine(string line, ConsoleColor colour)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(line);
            Console.ForegroundColor = currentColor;
        }

        private static void WriteError(string message, Exception error)
        {
            WriteLine(message, ConsoleColor.Red);
            if (error != null)
            {
                WriteLine(error.Message, ConsoleColor.Red);
                Environment.ExitCode = 2;
            }
            else
            {
                Environment.ExitCode = 1;
            }
        }

        private static void DisplayHelp(OptionSet opts)
        {
            Assembly thisApp = Assembly.GetExecutingAssembly();
            Stream helpStream = thisApp.GetManifestResourceStream("ThoughtWorks.CruiseControl.CCCmd.Help.txt");
            try
            {
                StreamReader reader = new StreamReader(helpStream);
                string data = reader.ReadToEnd();
                Console.Write(data);
            }
            finally
            {
                helpStream.Close();
            }
            opts.WriteOptionDescriptions (Console.Out);
        }
    }
}
