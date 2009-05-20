using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandParameters parameters = CommandParameters.Parse(args);
                switch (parameters.Command)
                {
                    case CommandType.Help:
                        DisplayHelp();
                        break;
                    case CommandType.Retrieve:
                        RunRetrive(parameters);
                        break;
                    case CommandType.ForceBuild:
                        RunForceBuild(parameters);
                        break;
                    case CommandType.AbortBuild:
                        RunAbortBuild(parameters);
                        break;
                    case CommandType.StartProject:
                        RunStartProject(parameters);
                        break;
                    case CommandType.StopProject:
                        RunStopProject(parameters);
                        break;
                    default:
                        throw new Exception("Unknown action: " + parameters.Command.ToString());
                }
            }
            catch (Exception error)
            {
                WriteError("ERROR: An unknown error has occurred!", error);
            }
        }

        private static CruiseServerClientBase GenerateClient(CommandParameters parameters)
        {
            var client = CruiseServerClientFactory.GenerateClient(parameters.ServerUrl,
                parameters.TargetServer);
            return client;
        }

        private static void RunForceBuild(CommandParameters parameters)
        {
            if (ValidateParameter(parameters.ServerUrl, "-server") &&
                ValidateNotAll(parameters) && 
                ValidateParameter(parameters.ProjectName, "-project"))
            {
                try
                {
                    var client = GenerateClient(parameters);
                    if (!parameters.QuietMode) WriteLine(string.Format("Sending ForceBuild request for '{0}'", parameters.ProjectName), ConsoleColor.White);
                    client.ForceBuild(parameters.ProjectName);
                    if (!parameters.QuietMode) WriteLine("ForceBuild request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunAbortBuild(CommandParameters parameters)
        {
            if (ValidateParameter(parameters.ServerUrl, "-server") &&
                ValidateNotAll(parameters) &&
                ValidateParameter(parameters.ProjectName, "-project"))
            {
                try
                {
                    var client = GenerateClient(parameters);
                    if (!parameters.QuietMode) WriteLine(string.Format("Sending AbortBuild request for '{0}'", parameters.ProjectName), ConsoleColor.White);
                    client.AbortBuild(parameters.ProjectName);
                    if (!parameters.QuietMode) WriteLine("AbortBuild request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunStartProject(CommandParameters parameters)
        {
            if (ValidateParameter(parameters.ServerUrl, "-server") &&
                ValidateNotAll(parameters) &&
                ValidateParameter(parameters.ProjectName, "-project"))
            {
                try
                {
                    var client = GenerateClient(parameters);
                    if (!parameters.QuietMode) WriteLine(string.Format("Sending StartProject request for '{0}'", parameters.ProjectName), ConsoleColor.White);
                    client.StartProject(parameters.ProjectName);
                    if (!parameters.QuietMode) WriteLine("StartProject request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunStopProject(CommandParameters parameters)
        {
            if (ValidateParameter(parameters.ServerUrl, "-server") &&
                ValidateNotAll(parameters) &&
                ValidateParameter(parameters.ProjectName, "-project"))
            {
                try
                {
                    var client = GenerateClient(parameters);
                    if (!parameters.QuietMode) WriteLine(string.Format("Sending StopProject request for '{0}'", parameters.ProjectName), ConsoleColor.White);
                    client.StopProject(parameters.ProjectName);
                    if (!parameters.QuietMode) WriteLine("StopProject request sent", ConsoleColor.White);
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunRetrive(CommandParameters parameters)
        {
            if (ValidateParameter(parameters.ServerUrl, "-server"))
            {
                if (string.IsNullOrEmpty(parameters.ProjectName) && !parameters.IsAll)
                {
                    WriteLine("Must specify either a project or use the '-all' option", ConsoleColor.Red);
                }
                else
                {
                    var client = GenerateClient(parameters);
                    if (parameters.IsAll)
                    {
                        DisplayServerStatus(client, parameters.QuietMode);
                    }
                    else
                    {
                        DisplayProjectStatus(client, parameters.ProjectName, parameters.QuietMode);
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

        private static bool ValidateNotAll(CommandParameters parameters)
        {
            if (parameters.IsAll)
            {
                WriteError(string.Format("Input parameter '-all' is not valid for {0}", parameters.Command), null);
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

        private static void DisplayHelp()
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
        }
    }
}
