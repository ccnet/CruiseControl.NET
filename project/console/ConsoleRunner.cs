using System;
using System.Diagnostics;
using System.IO;

using tw.ccnet.core;
using tw.ccnet.core.configuration;

namespace tw.ccnet.console
{
	/// <summary>
	/// Runs CruiseControl.NET from the console.
	/// </summary>
	class ConsoleRunner
	{
		public static readonly string DEFAULT_CONFIG_PATH = @".\ccnet.config";
		static readonly string HELP_OPTION = "-help";

		static CruiseManager _manager;

		[STAThread]
		internal static void Main(string[] args)
		{
			try
			{
				string configFile = GetConfigFileName(args);
				if (configFile==null)
				{
					DisplayUsage();
					return;
				}
				
				FileInfo configFileInfo = new FileInfo(configFile);
				if (!configFileInfo.Exists)
				{
					Console.WriteLine("Config file {0} does not exist - exiting application", configFileInfo.FullName);
					return;
				}

				Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

				bool useRemoting = GetOption("remoting", args)=="on";
				if (useRemoting)
					StartRemoteCC(configFile);
				else
					StartLocalCC(configFile);

				Console.WriteLine("Server started");
				BlockForUserInput();
			}
			catch (ConfigurationException ex)
			{
				Console.WriteLine("There was an error loading the configuration file: ");
				Console.WriteLine("  " + ex.Message);
			}
			catch (CruiseControlException ex)
			{
				Console.WriteLine("Cruise Exception: " + ex);
				Console.WriteLine("Cruise Stacktrace: " + ex.StackTrace);
			}
		}

		static void StartRemoteCC(string configFile)
		{
			_manager = new CruiseManager(configFile);
			_manager.RegisterForRemoting();
			_manager.StartCruiseControl();
		}

		static void StartLocalCC(string configFile)
		{
			ConfigurationLoader configLoader = new ConfigurationLoader(configFile);
			ICruiseControl cruiseControl = new CruiseControl(configLoader);
			cruiseControl.Start();
		}

		static void BlockForUserInput()
		{
			Console.WriteLine("Press <enter> to exit");
			Console.ReadLine();

			if (_manager!=null)
				_manager.StopCruiseControlNow();
		}

		public static string GetConfigFileName(string[] args)
		{
			for (int i=0; i<args.Length; i++)
			{
				if (!args[i].StartsWith("-"))
					return args[i];
			}

			if (args.Length==0)
				return DEFAULT_CONFIG_PATH;

			if (HELP_OPTION.Equals(args[0]))
				return null;

			return DEFAULT_CONFIG_PATH;
		}

		internal static string GetOption(string optionRequired, string[] args)
		{
			try
			{
				for (int i=0; i<args.Length; i++)
				{
					string option = args[i].Substring(1, args[i].IndexOf(":") - 1);
					if (option.Equals(optionRequired))
					{
						return args[i].Substring(args[i].IndexOf(":") + 1);
					}
				}
			}
			catch
			{
				return null;
			}
			return null;
		}

		static void DisplayUsage()
		{
			Console.WriteLine("CruiseControl [options] <configFile>");
			Console.WriteLine("Options:");
			Console.WriteLine("  -remoting:[on/off] default:off");
			Console.WriteLine("  -help  Help");
		}
	}
}