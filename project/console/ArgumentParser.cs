using System;
using System.Collections;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ArgumentParser
	{
		public const string DEFAULT_CONFIG_PATH = @"ccnet.config";
		public const string Usage = 
			@"ccnet [options]
Options:
  -config:[ccnet.config]
  -remoting:[on/off] default:on
  -project:[projectName]
  -help";

		private string[] validOptions = new string[] { "config", "remoting", "project", "help" };
		private Hashtable options = new Hashtable();
		private Regex regex = new Regex("-(?<option>[^:]*)(:(?<value>.*))?");

		public ArgumentParser(string[] args)
		{
			InitialiseOptions();
			Parse(args);
		}

		private void InitialiseOptions()
		{
			foreach (string option in validOptions)
			{
				options.Add(option, null);
			}
		}

		private void Parse(string[] args)
		{
			foreach (string arg in args)
			{
				Match match = regex.Match(arg);
				if (match.Success && options.ContainsKey(match.Groups["option"].Value))
				{
					options[match.Groups["option"].Value] = match.Groups["value"].Value;
				}
				else
				{
					Log.Warning(string.Format("Invalid argument: {0}", arg));
					options["help"] = true;
				}
			}
		}

		public bool IsRemote
		{
			get { return GetOption("remoting") != "off"; }
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
			return options[optionRequired] == null ? null : options[optionRequired].ToString();
		}
	}
}
