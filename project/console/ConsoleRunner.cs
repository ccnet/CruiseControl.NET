using System;
using System.Diagnostics;

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
			ArgumentParser parser = new ArgumentParser(args);
			Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));

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
					// server.WaitForExit();
				}
				else
				{
					LogUtil.Log("Starting CruiseControl.NET Project: " + _parser.Project);
					server.ForceBuild(_parser.Project);
					// server.CruiseManager.ForceBuild(_parser.Project);
					// server.CruiseManager.WaitForExit(_parser.Project);
					LogUtil.Log("Hit Ctrl-C when integration is complete."); // HACK: better to join thread.
				}
				_timeout.Wait();
			}
			finally
			{
				server.Abort();
			}
		}
	}
}