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

		public ConsoleRunner(ArgumentParser parser, ITimeout timeout)
		{
			_parser = parser;
			_timeout = timeout;
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
			ICruiseServer server = CruiseServerFactory.Create(_parser.IsRemote, _parser.ConfigFile);
			try
			{
				if (_parser.Project == null)
				{
					Log.Info("Starting CruiseControl.NET Server");
					server.Start();
					// server.WaitForExit();
				}
				else
				{
					Log.Info("Starting CruiseControl.NET Project: " + _parser.Project);
					server.ForceBuild(_parser.Project);
					// server.CruiseManager.ForceBuild(_parser.Project);
					// server.CruiseManager.WaitForExit(_parser.Project);
				}
				Log.Info("Hit Ctrl-C when integration is complete."); // HACK: better to join thread.
				_timeout.Wait();
			}
			finally
			{
				server.Abort();
			}
		}
	}
}