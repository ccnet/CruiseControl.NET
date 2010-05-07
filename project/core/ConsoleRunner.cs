namespace ThoughtWorks.CruiseControl.Core
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    public class ConsoleRunner
	{
		private readonly ConsoleRunnerArguments args;
		private readonly ICruiseServerFactory serverFactory;
		private ICruiseServer server;

		public ConsoleRunner(ConsoleRunnerArguments args, ICruiseServerFactory serverFactory)
		{
			this.args = args;
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
            if (args.LaunchDebugger)
                System.Diagnostics.Debugger.Launch();

			if (args.ShowHelp)
			{
				//Log.Warning(ConsoleRunnerArguments.Usage);
				return;
			}

            if (!args.Logging)
            {
                Log.Warning("Logging has been disabled - no information (except errors) will be written to the log");
                Log.DisableLogging();
            }

            if (args.ValidateConfigOnly)
            {
                serverFactory.Create(false, args.ConfigFile);
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

				using (server = serverFactory.Create(args.UseRemoting, args.ConfigFile))
				{
					if (args.Project == null)
					{
						server.Start();
						server.WaitForExit();
					}
					else
					{
                        // Force the build
                        var actualServer = ((server as RemoteCruiseServer).CruiseManager as CruiseManager).ActualServer as CruiseServer;
                        ValidateResponse(
                            server.ForceBuild(
                                new ProjectRequest(actualServer.OverrideSession, args.Project)));

                        // Tell the server to stop as soon as the build has finished and then wait for it
                        ValidateResponse(
                            server.Stop(
                                new ProjectRequest(actualServer.OverrideSession, args.Project)));
						server.WaitForExit(
                            new ProjectRequest(actualServer.OverrideSession, args.Project));
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
