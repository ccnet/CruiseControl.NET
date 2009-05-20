using System;

namespace ThoughtWorks.CruiseControl.CCCmd
{
    public class CommandParameters
    {
        public CommandType Command = CommandType.Help;
        public string ServerUrl;
        public string TargetServer;
        public string ProjectName;
        public bool IsAll = false;
        public bool QuietMode = false;

        public static CommandParameters Parse(string[] args)
        {
            CommandParameters parameters = new CommandParameters();

            foreach (string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    string[] argParts = arg.Substring(1).Split('=');
                    switch (argParts[0].ToLower())
                    {
                        case "server":
                            parameters.ServerUrl = argParts[1];
                            break;
                        case "target":
                            parameters.TargetServer = argParts[1];
                            break;
                        case "project":
                            parameters.ProjectName = argParts[1];
                            break;
                        case "all":
                            parameters.IsAll = true;
                            break;
                        case "quiet":
                            parameters.QuietMode = true;
                            break;
                    }
                }
                else
                {
                    parameters.Command = (CommandType)Enum.Parse(typeof(CommandType), arg, true);
                }
            }

            return parameters;
        }
    }
}
