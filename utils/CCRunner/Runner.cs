using System;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Activation;
using System.Configuration;
using System.Text.RegularExpressions;
using tw.ccnet.remote;

namespace CCNet.CCRunner
{
	public class Runner
	{
		private string url;
		public const string URL_CONFIG = "cc.net.url";
		public const string NULL_PROJECT_MSG = "Please specify the name of the CruiseControl.NET project to run";
		public const string HELP =
@"usage: ccnet.runner.exe [options] projectName
options:
	-url:<url>		Location of server.  If omitted this will be loaded from 'ccnet.runner.exe.config'.
	-help			Display command-line help and options.";

		public Runner()
		{
			Url = LoadURLFromConfig();
		}

		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		public void Run(string projectName)
		{
			if (projectName == null) throw new ArgumentNullException("projectName", NULL_PROJECT_MSG);
			try 
			{
				GetRemoteManager().Run(projectName, new Schedule());
			}
			catch (SocketException ex)
			{
				throw new ServerConnectionException(Url, ex);
			}
			catch (RemotingException ex)
			{
				throw new ServerConnectionException(Url, ex);
			}
		}

		private ICruiseManager GetRemoteManager()
		{			
			 return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), Url);
		}

		private string LoadURLFromConfig()
		{
			 return ConfigurationSettings.AppSettings[URL_CONFIG];
		}

		public static System.IO.TextWriter Out = Console.Out;

		[STAThread]
		public static void Main(string[] args)
		{
			if (HasHelp(args))
			{
				ShowHelp();
				return;
			}

			Runner runner = new Runner();
			runner.Url = ParseUrl(args);
			try
			{
				runner.Run(ParseProject(args));
			}
			catch (Exception ex)
			{
				Out.WriteLine(ex.Message);
				Out.WriteLine();
				ShowHelp();
			}
		}

		private static bool HasHelp(string[] args)
		{
			foreach (string arg in args)
			{
				if (arg == "-help")
				{
					return true;
				}
			}
			return false;
		}

		private static void ShowHelp()
		{
			Out.WriteLine(HELP);
			Environment.ExitCode = 1;
		}

		public static string ParseUrl(string[] args)
		{
			foreach (string arg in args)
			{
				if (arg.StartsWith("-url:"))
				{
					return arg.Substring(5);
				}
			}
			return null;
		}

		public static string ParseProject(string[] args)
		{
			foreach (string arg in args)
			{
				if (! arg.StartsWith("-"))
				{
					return arg;
				}
			}
			return null;
		}
	}
}
