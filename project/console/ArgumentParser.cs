using System;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ArgumentParser
	{
		public const string DEFAULT_CONFIG_PATH = @"ccnet.config";
		public const string Usage = 
			@"ccnet [options]
Options:
  -config:[ccnet.config]
  -remoting:[on/off] default:off
  -project:[projectName]
  -help";

		private string[] args;

		public ArgumentParser(string[] args)
		{
			this.args = args;
		}

		public bool IsRemote
		{
			get { return GetOption("remoting") == "on"; }
		}

		public string Project
		{
			get { return GetOption("project"); }
		}

		public string ConfigFile
		{
			get 
			{
				string configFile = GetOption("config");
				return (configFile == null) ? DEFAULT_CONFIG_PATH : configFile; 
			}
		}

		public bool ShowHelp
		{
			get { return GetOption("help") != null; }
		}

		private string GetOption(string optionRequired)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith(string.Format("-{0}", optionRequired)))
				{
					return args[i].Remove(0, optionRequired.Length + 1).TrimStart(':');
				}
			}
			return null;
		}
	}
}
