using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.Remoting;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ConsoleMain
	{
        private static object lockObject = new object();

		[STAThread]
		internal static int Main(string[] args)
		{
            bool restart = true;
            int result = 0;
            DateTime restartTime = DateTime.MinValue;
            using (FileSystemWatcher watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            {
                AppRunner runner = null;

                // Start monitoring file changes
                watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                {
                    if (!restart)
                    {
                        lock (lockObject)
                        {
                            try
                            {
                                runner.Stop("One or more DLLs have changed");
                            }
                            catch (RemotingException)
                            {
                                // Sometimes this exception occurs - the lock statement should catch it, but...
                            }
                        }
                    }
                    restart = true;
                    restartTime = DateTime.Now.AddSeconds(10);
                };
                watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;
                watcher.EnableRaisingEvents = true;

                // Begin the main application loop
                while (restart)
                {
                    restart = false;

                    // Load the domain and start the runner
                    // Allow the user to turn shadow-copying off
                    var setting = ConfigurationManager.AppSettings["ShadowCopy"] ?? string.Empty;
                    var useShadowCopying = !(string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(setting, "false", StringComparison.OrdinalIgnoreCase));
                    AppDomain runnerDomain;
                    try
                    {
                        runnerDomain = CreateNewDomain(useShadowCopying);
                    }
                    catch (FileLoadException)
                    {
                        // Unable to use shadow-copying (no user profile?), therefore turn off shadow-copying
                        useShadowCopying = false;
                        runnerDomain = CreateNewDomain(useShadowCopying);
                    }
                    runner = runnerDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                        typeof(AppRunner).FullName) as AppRunner;
                    result = runner.Run(args, useShadowCopying);
                    AppDomain.Unload(runnerDomain);

                    // Allow any change events to finish (i.e. if multiple files are copied)
                    while (DateTime.Now < restartTime)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the new runner domain.
        /// </summary>
        /// <param name="useShadowCopying">If set to <c>true</c> shadow copying will be used.</param>
        /// <returns>The new <see cref="AppDomain"/>.</returns>
        private static AppDomain CreateNewDomain(bool useShadowCopying)
        {
            return AppDomain.CreateDomain(
                "CC.Net",
                null,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath,
                useShadowCopying);
        }
    }
}
