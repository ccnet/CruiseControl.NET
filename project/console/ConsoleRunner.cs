using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Console
{
	/// <summary>
	/// Runs CruiseControl.NET from the console.
	/// </summary>
	class ConsoleRunner
	{
		[STAThread]
		internal static void Main(string[] args)
		{
			try
			{
				new ConsoleRunner(new ArgumentParser(args)).Run();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}	

		private ArgumentParser _parser;
		private ICruiseServer _server;

		public ConsoleRunner(ArgumentParser parser) : 
			this(parser, CruiseServerFactory.Create(parser.IsRemote, parser.ConfigFile)) { }

		public ConsoleRunner(ArgumentParser parser, ICruiseServer server)
		{
			_parser = parser;
			_server = server;
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
			
				if (_parser.Project == null)
				{
					_server.Start();
					_server.WaitForExit();
				}
				else
				{
					_server.ForceBuild(_parser.Project);
					_server.WaitForExit(_parser.Project);
				}
			}
		}

		private void HandleControlEvent(object sender, EventArgs args)
		{
			ConsoleEventHandler handler = (ConsoleEventHandler)sender;
			handler.OnConsoleEvent -= new EventHandler(HandleControlEvent);	// remove handler to prevent event from being called again
			_server.Dispose();
		}
	}
}