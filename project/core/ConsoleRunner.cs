using System;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ConsoleRunner
	{
		private readonly ArgumentParser _parser;
		private readonly ICruiseServerFactory _serverFactory;
		private ICruiseServer server;

		public ConsoleRunner(ArgumentParser parser, ICruiseServerFactory serverFactory)
		{
			_parser = parser;
			_serverFactory = serverFactory;
		}

		public void Run()
		{
			Console.WriteLine("CruiseControl.NET Server {0} -- .NET Continuous Integration Server", Assembly.GetExecutingAssembly().GetName().Version);
			Console.WriteLine("Copyright (C) 2003-2006 ThoughtWorks Inc.  All Rights Reserved.");
			Console.WriteLine(".NET Runtime Version: {0}\tImage Runtime Version: {1}", Environment.Version, Assembly.GetExecutingAssembly().ImageRuntimeVersion);
			Console.WriteLine("OS Version: " + Environment.OSVersion);
			Console.WriteLine();

			if (_parser.ShowHelp)
			{
				Log.Warning(ArgumentParser.Usage);
				return;
			}
			LaunchServer();
		}

		private void LaunchServer()
		{
			using (ConsoleEventHandler handler = new ConsoleEventHandler())
			{
				handler.OnConsoleEvent += new EventHandler(HandleControlEvent);

				using (server = _serverFactory.Create(_parser.UseRemoting, _parser.ConfigFile))
				{
					if (_parser.Project == null)
					{
						server.Start();
						server.WaitForExit();
					}
					else
					{
						server.ForceBuild(_parser.Project);
						server.WaitForExit(_parser.Project);
					}
				}
			}
		}

		private void HandleControlEvent(object sender, EventArgs args)
		{
			server.Dispose();
		}
	}
}