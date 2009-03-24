using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ConsoleMain
	{
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
                    if (!restart) runner.Stop(e.Name + " has changed");
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
                    AppDomain newDomain = AppDomain.CreateDomain("CC.Net",
                        null,
                        AppDomain.CurrentDomain.BaseDirectory,
                        AppDomain.CurrentDomain.RelativeSearchPath,
                        true);
                    runner = newDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                        "ThoughtWorks.CruiseControl.Console.AppRunner") as AppRunner;
                    result = runner.Run(args);
                    AppDomain.Unload(newDomain);

                    // Allow any change events to finish (i.e. if multiple files are copied)
                    while (DateTime.Now < restartTime)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
            return result;
        }
	}
}
