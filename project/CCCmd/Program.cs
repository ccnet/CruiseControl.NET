using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Mono;
using ThoughtWorks.CruiseControl.Remote.Parameters;

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
        private static string string_params;
        private static string params_filename;
        private static CommandType command;
        private static List<string> extra = new List<string>();
        private static string userName;
        private static string password;
        private static bool xml;
        private static string volunteer_name = Environment.UserName;
        private static BuildCondition condition = BuildCondition.ForceBuild;

        static void Main(string[] args)
        {
           	OptionSet opts = new OptionSet();
            opts.Add("h|?|help", "display this help screen", delegate(string v) { help = v != null; })
                .Add("s|server=", "the CruiseControl.Net server to send the commands to (required for all actions except help)", delegate(string v) { server = v; })
                .Add("t|target=", "the target server for all messages", delegate(string v) { target = v; })
                .Add("p|project=", "the project to use (required for all actions except help and retrieve)", delegate(string v) { project = v; })
                .Add("a|all", "lists all the projects (only valid for retrieve)", delegate(string v) { all = v != null; })
                .Add("q|quiet", "run in quiet mode (do not print messages)", delegate(string v) { quiet = v != null; })
                .Add("ime|ifmodificationexists", "only force the build if modification exist", delegate(string v) { condition = v != null ? BuildCondition.IfModificationExists : BuildCondition.ForceBuild; })
                .Add("r|params=", "a semicolon separated list of name value pairs", delegate(string v) { string_params = v; })
                .Add("f|params_file=", "the name of a XML file containing the parameters values to use when forcing a build. If specified at the same time as this flag, the values from the command line are ignored", delegate(string v) { params_filename = v; })             
 				.Add("x|xml", "outputs the details in XML format instead of plain text (only valid for retrieve)", delegate(string v) { xml = v != null; })
                .Add("user=", "the user of the user account to use", v => { userName = v; })
                .Add("pwd=", "the password to use for the user", v => { password = v;})
                .Add("volunteer_name=", "the name to use when volunteering (defaults to Environment.UserName)", v => { volunteer_name = v; });
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
                    case CommandType.Volunteer:
                        RunVolunteer();
                        break;
                    default:
                        throw new CruiseControlException("Unknown action: " + command.ToString());
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
            if (!string.IsNullOrEmpty(userName))
            {
                client.Login(new { UserName = userName, Password = password });
            }

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
                    Dictionary<string, string> userParameters = new Dictionary<string, string>();
                    if (File.Exists(params_filename))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(params_filename);
                        XmlNodeList names = xmlDoc.GetElementsByTagName("name");
                        XmlNodeList values = xmlDoc.GetElementsByTagName("value");

                        for (int i = 0; i < names.Count; ++i)
                            userParameters.Add(names[i].InnerText, values[i].InnerText);
                    }
                    else if (!String.IsNullOrEmpty(string_params))
                    {
                        string[] splittedParams = string_params.Split(';');
                        foreach (string nameValuePair in splittedParams)
                        {
                            string[] splittedNameValuePair = nameValuePair.Split('=');
                            userParameters.Add(splittedNameValuePair[0], splittedNameValuePair[1]);
                        }
                    } 


                    using (var client = GenerateClient())
                    {
                        List<ParameterBase> parameters = client.ListBuildParameters(project);
                        List<NameValuePair> buildParameters = new List<NameValuePair>();

                        foreach (ParameterBase parameter in parameters)
                        {
                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Parameter: {0}", parameter.Name), ConsoleColor.Gray);

                            bool required = false;
                            PropertyInfo propInfos = parameter.GetType().GetProperty("IsRequired");
                            if (propInfos == null)
                                WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"propInfos is null, considering that parameter {0} is not required", parameter.Name), ConsoleColor.DarkYellow);
                            else
                                required = (bool)propInfos.GetValue(parameter, null);

                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," Kind: {0}", parameter.ToString()), ConsoleColor.Gray);
                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," Data type: {0}", parameter.DataType), ConsoleColor.Gray);
                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," Required: {0}", required), ConsoleColor.Gray);
                            string userProvidedValue;
                            userParameters.TryGetValue(parameter.Name, out userProvidedValue);
                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," User provided value: {0}", userProvidedValue), ConsoleColor.Gray);

                            var convertedValue = parameter.Convert(userProvidedValue);
                            string convertedValueString = userProvidedValue;
                            if (convertedValue != null)
                            {
                                convertedValueString = convertedValue.ToString();
                            }
                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," Converted value: {0}", convertedValueString), ConsoleColor.Gray);

                            if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," Default value: {0}", parameter.DefaultValue), ConsoleColor.Gray);
                            if (parameter.AllowedValues == null)
                            {
                                if (!quiet) WriteLine(" Allowed values: null", ConsoleColor.Gray);
                            }
                            else
                            {
                                if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture," Allowed values: {0}", parameter.AllowedValues), ConsoleColor.Gray);

                                bool isAllowedValue = false;
                                int i = 0;
                                while (!isAllowedValue && (i < parameter.AllowedValues.Length))
                                {
                                    isAllowedValue = parameter.AllowedValues[i].Equals(userProvidedValue);
                                    ++i;
                                }

                                if (!isAllowedValue)
                                    throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Parameter {0} was given value {1} which is not one of the allowed ones ({2})", parameter.Name, userProvidedValue, string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}", parameter.AllowedValues)));
                            }

                            if (required && String.IsNullOrEmpty(convertedValueString) && String.IsNullOrEmpty(parameter.DefaultValue))
                                throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Parameter {0} is required but was not provided and does not have a default value", parameter.Name));

                            buildParameters.Add(new NameValuePair(parameter.Name, convertedValueString));
                        } 

                        if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Sending ForceBuild request for '{0}'", project), ConsoleColor.White);
                        client.ForceBuild(project, buildParameters, condition);
                        if (!quiet) WriteLine("ForceBuild request sent", ConsoleColor.White);
                    }
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
                    using (var client = GenerateClient())
                    {
                        if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Sending AbortBuild request for '{0}'", project), ConsoleColor.White);
                        client.AbortBuild(project);
                        if (!quiet) WriteLine("AbortBuild request sent", ConsoleColor.White);
                    }
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
                    using (var client = GenerateClient())
                    {
                        if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Sending StartProject request for '{0}'", project), ConsoleColor.White);
                        client.StartProject(project);
                        if (!quiet) WriteLine("StartProject request sent", ConsoleColor.White);
                    }
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
                    using (var client = GenerateClient())
                    {
                        if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Sending StopProject request for '{0}'", project), ConsoleColor.White);
                        client.StopProject(project);
                        if (!quiet) WriteLine("StopProject request sent", ConsoleColor.White);
                    }
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to send ForceBuild request", error);
                }
            }
        }

        private static void RunVolunteer()
        {
            if (ValidateParameter(server, "--server") &&
                ValidateNotAll() &&
                ValidateParameter(project, "--project"))
            {
                try
                {
                    using (var client = GenerateClient())
                    {
                        if (!quiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Volunteering to fix '{0}'", project), ConsoleColor.White);
                        string message = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} is fixing the build.", volunteer_name);
                        client.SendMessage(project, new Message(message, Message.MessageKind.Fixer));
                        if (!quiet) WriteLine("Volunteer message sent", ConsoleColor.White);
                    }
                }
                catch (Exception error)
                {
                    WriteError("ERROR: Unable to volunteer", error);
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
                    using (var client = GenerateClient())
                    {
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
        }

        private static void DisplayServerStatus(CruiseServerClientBase client, bool isQuiet)
        {
            try
            {
                if (!isQuiet) WriteLine("Retrieving snapshot from " + client.TargetServer, ConsoleColor.Gray);
                CruiseServerSnapshot snapShot = client.GetCruiseServerSnapshot();
                if (xml)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CruiseServerSnapshot));
                    Stream console = Console.OpenStandardOutput();
                    serializer.Serialize(console, snapShot);
                    console.Close();
                }
                else
                {
                    foreach (ProjectStatus prj in snapShot.ProjectStatuses)
                    {
                        DisplayProject(prj);
                    }
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
                if (!isQuiet) WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Retrieving project '{0}' on server {1}", projectName, client.TargetServer), ConsoleColor.Gray);
                CruiseServerSnapshot snapShot = client.GetCruiseServerSnapshot();
                var wasFound = false;
                foreach (ProjectStatus project in snapShot.ProjectStatuses)
                {
                    if (string.Equals(project.Name, projectName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (xml)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ProjectStatus));
                            Stream console = Console.OpenStandardOutput();
                            serializer.Serialize(console, project);
                            console.Close();
                        }
                        else
                        {
                            DisplayProject(project);
                        } 
                        
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                {
                    WriteError("Project '" + projectName + "' was not found", null);
                }
            }
            catch (Exception error)
            {
                WriteError("ERROR: Unable to retrieve project details", error);
            }
        }

        private static void DisplayProject(ProjectStatus project)
        {
            WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}: {1}", project.Name, project.Status), ConsoleColor.White);
            WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tActivity: {0}", project.Activity), ConsoleColor.White);
            WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tBuild Status: {0}", project.BuildStatus), ConsoleColor.White);
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
                        WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tBuild Stage: {0} ({1})", stageData, stageTime), ConsoleColor.White);
                    }
                }
                catch
                {
                    WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tBuild Stage: {0}", project.BuildStage), ConsoleColor.White);
                }
            }
            WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tLast Build: {0:G}", project.LastBuildDate), ConsoleColor.White);
            WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tNext Build: {0:G}", project.NextBuildTime), ConsoleColor.White);
        }

        private static bool ValidateParameter(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteError(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Input parameter '{0}' is missing", name), null);
                return false;
            }
            return true;
        }

        private static bool ValidateNotAll()
        {
            if (all)
            {
                WriteError(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Input parameter '--all' is not valid for {0}", command), null);
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
                WriteLine(error.ToString(), ConsoleColor.Red);
                Environment.ExitCode = 2;
            }
            else
            {
                Environment.ExitCode = 1;
            }
        }

        private static void DisplayHelp(OptionSet opts)
        {
            string xmlParamFileLayout =
@"<parameters>
  <parameter>
    <name>buildType</name>
    <value>Development</value>
  </parameter>
  <parameter>
    <name>DeployMsg</name>
    <value>This is a test</value>
  </parameter>
</parameters> ";

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

            Console.WriteLine();
            WriteLine("Layout of the parameter file :", ConsoleColor.Yellow);
            WriteLine(xmlParamFileLayout,ConsoleColor.DarkGray);
            Console.WriteLine();
            WriteLine(" Examples :",ConsoleColor.Yellow);
            WriteLine("-==========-", ConsoleColor.Yellow);
            WriteLine("    CCCmd.exe retrieve   -s=tcp://localhost:21234/CruiseManager.rem -a", ConsoleColor.White);
            WriteLine("    CCCmd.exe retrieve   -s=tcp://localhost:21234/CruiseManager.rem -a -x", ConsoleColor.White);
            WriteLine("    CCCmd.exe retrieve   -s=tcp://localhost:21234/CruiseManager.rem -p=ccnet", ConsoleColor.White);
            WriteLine("    CCCmd.exe forcebuild -s=tcp://localhost:21234/CruiseManager.rem -p=ccnet", ConsoleColor.White);
            WriteLine("    CCCmd.exe forcebuild -s=tcp://localhost:21234/CruiseManager.rem -p=ccnet -r=buildtype=fulltest", ConsoleColor.White);
            WriteLine("    CCCmd.exe forcebuild -s=tcp://localhost:21234/CruiseManager.rem -p=ccnet -r=buildtype=fulltest;makepackage=true", ConsoleColor.White);

 


        }
    }
}
