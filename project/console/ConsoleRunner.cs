using System;
using System.Diagnostics;
using System.IO;

using tw.ccnet.core;
using tw.ccnet.core.configuration;
using tw.ccnet.remote;
using tw.ccnet.core.util;

namespace tw.ccnet.console
{
	/// <summary>
	/// Runs CruiseControl.NET from the console.
	/// </summary>
	class ConsoleRunner
	{
		[STAThread]
		internal static void Main(string[] args)
		{
			ArgumentParser parser = new ArgumentParser(args);
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

			new ConsoleRunner(parser, new Timeout()).Run();
		}	

		private ArgumentParser _parser;
		private ITimeout _timeout;

		public ConsoleRunner(ArgumentParser parser, ITimeout timeout)
		{
			_parser = parser;
			_timeout = timeout;
		}

		public void Run()
		{
			if (_parser.ShowHelp)
			{
				LogUtil.Log(ArgumentParser.Usage);
				return;
			}

			LaunchServer();
		}

		private void LaunchServer()
		{
			ICruiseServer server = CruiseServerFactory.Create(_parser.IsRemote, _parser.ConfigFile);
			try
			{
				if (_parser.Project == null)
				{
					LogUtil.Log("Starting CruiseControl.NET Server");
					server.Start();
					_timeout.Wait();
				}
				else
				{
					LogUtil.Log("Starting CruiseControl.NET Project: " + _parser.Project);
					server.ForceBuild(_parser.Project);
				}
			}
			finally
			{
				server.Stop();
			}
		}
	}
}