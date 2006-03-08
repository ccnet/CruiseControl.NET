using System;
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