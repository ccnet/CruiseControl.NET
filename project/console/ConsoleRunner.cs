using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using tw.ccnet.core;
using tw.ccnet.core.configuration;

namespace tw.ccnet.console
{
	class ConsoleRunner
	{
		private static readonly string HELP_OPTION = "-help";
		public static readonly string DEFAULT_CONFIG_PATH = ".\\ccnet.config";
		private static CruiseManager manager;

		[STAThread]
		internal static void Main(string[] args)
		{
			try
			{
				string configFile = GetConfigFileName(args);
				if (configFile == null)
				{
					Usage();
					return;
				}
				
				FileInfo configFileInfo = (new FileInfo(configFile));

				if (! configFileInfo.Exists)
				{
					Console.WriteLine("Config file {0} does not exist - exiting application", configFileInfo.FullName);
					return;
				}

				Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

				if (GetOption("remoting", args) != null && GetOption("remoting", args).Equals("on"))
				{
					StartRemoteCC(configFile);
				}
				else
				{
					StartLocalCC(configFile);
				}
				BlockForUserInput();
			}
			catch (ConfigurationException ex)
			{
				Console.WriteLine("There was an error loading the configuration file: ");
				Console.WriteLine("  " + ex.InnerException.Message);
			}
			catch (CruiseControlException ex)
			{
				Console.WriteLine("Cruise Exception: " + ex.ToString());
				Console.WriteLine("Cruise Stacktrace: " + ex.StackTrace);
			}
		}

		private static void StartRemoteCC(String configFile)
		{
			RemotingConfiguration.Configure("ccnet.exe.config");
			manager = new CruiseManager();
			manager.InitializeCruiseControl(configFile);
			manager.StartCruiseControl();
			RemotingServices.Marshal(manager, "CruiseManager.rem");
			Console.WriteLine("Server Started.........");

		}

		private static void StartLocalCC(String configFile)
		{
			ConfigurationLoader configLoader = new ConfigurationLoader(configFile);
			CruiseControl cruiseControl = new CruiseControl(configLoader);
			cruiseControl.Start();
		}

		private static void BlockForUserInput()
		{
			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();
			if (manager != null)
			{
				manager.StopCruiseControlNow();
			}
		}

		public static string GetConfigFileName(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (! args[i].StartsWith("-"))
				{
					return args[i];
				}
			}

			if (args.Length == 0)
			{
				return DEFAULT_CONFIG_PATH;
			}

			if (HELP_OPTION.Equals(args[0]))
			{
				return null;
			}

			return 	DEFAULT_CONFIG_PATH;
		}

		internal static String GetOption(String optionRequired,String[] args)
		{
			try
			{
				for (int i = 0; i < args.Length; i++)
				{
					String option = args[i].Substring(1, args[i].IndexOf(":") - 1);
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

		private static void Usage()
		{
			Console.WriteLine("CruiseControl [options] <configFile>");
			Console.WriteLine("Options:");
			Console.WriteLine("  -remoting:[on/off] default:off");
			Console.WriteLine("  -help  Help");
		}
	}
}