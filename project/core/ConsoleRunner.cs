using System;
using System.Globalization;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ConsoleRunner
	{
		private readonly ArgumentParser parser;
		private readonly ICruiseServerFactory serverFactory;
		private ICruiseServer server;

		public ConsoleRunner(ArgumentParser parser, ICruiseServerFactory serverFactory)
		{
			this.parser = parser;
			this.serverFactory = serverFactory;
		}

		public void Run()
		{
			Console.WriteLine("CruiseControl.NET Server {0} -- .NET Continuous Integration Server", Assembly.GetExecutingAssembly().GetName().Version);
            // Find out our copyright claim, if any, and display it.
		    AssemblyCopyrightAttribute[] copyrightAttributes = (AssemblyCopyrightAttribute[]) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
            if (copyrightAttributes.Length > 0)
            {
                Console.WriteLine("{0}  All Rights Reserved.", copyrightAttributes[0].Copyright);
            }
			Console.WriteLine(".NET Runtime Version: {0}{2}\tImage Runtime Version: {1}", Environment.Version, Assembly.GetExecutingAssembly().ImageRuntimeVersion, GetRuntime());
			Console.WriteLine("OS Version: {0}\tServer locale: {1}", Environment.OSVersion, CultureInfo.CurrentCulture);
			Console.WriteLine();

            // In DEBUG builds, give the developer a chance to debug our execution
            if (parser.LaunchDebugger)
                System.Diagnostics.Debugger.Launch();

			if (parser.ShowHelp)
			{
				Log.Warning(ArgumentParser.Usage);
				return;
			}

            if (parser.NoLogging)
            {
                Log.Warning("Logging has been disabled - no information (except errors) will be written to the log");
                Log.DisableLogging();
            }

            if (parser.ValidateConfigOnly)
            {
                serverFactory.Create(false, parser.ConfigFile);
                return;
            }
			LaunchServer();
		}

        public void Stop()
        {
            server.Stop();
            server.WaitForExit();
        }

		private string GetRuntime()
		{
			if (Type.GetType ("Mono.Runtime") != null)
				return " [Mono]";
			return string.Empty;
		}

		private void LaunchServer()
		{
			using (ConsoleEventHandler handler = new ConsoleEventHandler())
			{
				handler.OnConsoleEvent += new EventHandler(HandleControlEvent);

				using (server = serverFactory.Create(parser.UseRemoting, parser.ConfigFile))
				{
					if (parser.Project == null)
					{
						server.Start();
						server.WaitForExit();
					}
					else
					{
                        // Force the build
                        ValidateResponse(
                            server.ForceBuild(
                                new ProjectRequest(null, parser.Project)));

                        // Tell the server to stop as soon as the build has finished and then wait for it
                        ValidateResponse(
                            server.Stop(
                                new ProjectRequest(null, parser.Project)));
						server.WaitForExit(
                            new ProjectRequest(null, parser.Project));
					}
				}
			}
		}

		private void HandleControlEvent(object sender, EventArgs args)
		{
			server.Dispose();
		}

        /// <summary>
        /// Validates that the request processed ok.
        /// </summary>
        /// <param name="value">The response to check.</param>
        private void ValidateResponse(Response value)
        {
            if (value.Result == ResponseResult.Failure)
            {
                string message = "Request has failed on the server:" + Environment.NewLine +
                    value.ConcatenateErrors();
                throw new CruiseControlException(message);
	}
        }
    }
}
