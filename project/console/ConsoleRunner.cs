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
				new ConsoleRunner(new ArgumentParser(args), new Timeout()).Run();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}	

		private ArgumentParser _parser;
		private ITimeout _timeout;
		private ICruiseServer _server;

		public ConsoleRunner(ArgumentParser parser, ITimeout timeout) : 
			this(parser, CruiseServerFactory.Create(parser.IsRemote, parser.ConfigFile), timeout) { }

		public ConsoleRunner(ArgumentParser parser, ICruiseServer server, ITimeout timeout)
		{
			_parser = parser;
			_timeout = timeout;
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
			try
			{
				if (_parser.Project == null)
				{
					Log.Info("Starting CruiseControl.NET Server");
					_server.Start();
					// server.WaitForExit();
				}
				else
				{
					Log.Info("Starting CruiseControl.NET Project: " + _parser.Project);
					_server.ForceBuild(_parser.Project);
					// server.CruiseManager.ForceBuild(_parser.Project);
					// server.CruiseManager.WaitForExit(_parser.Project);
				}
				Log.Info("Hit Ctrl-C when integration is complete."); // HACK: better to join thread.
				_timeout.Wait();
			}
			finally
			{
				_server.Abort();
			}
		}
	}
}