using System;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ConsoleMain
	{
		[STAThread]
		internal static int Main(string[] args)
		{
            bool restart = true;
            int result = 0;
            while (restart)
            {
                using (FileSystemWatcher watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "ThoughtWorks.CruiseControl.Core.dll"))
                {
                    restart = false;

                    AppDomain newDomain = AppDomain.CreateDomain("CC.Net",
                        null,
                        AppDomain.CurrentDomain.BaseDirectory,
                        AppDomain.CurrentDomain.RelativeSearchPath,
                        true);
                    AppRunner runner = newDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                        "ThoughtWorks.CruiseControl.Console.AppRunner") as AppRunner;

                    watcher.Changed += delegate(object sender, FileSystemEventArgs e) { restart = true; runner.Stop(e.Name + " has changed"); };
                    watcher.EnableRaisingEvents = true;
                    watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;

                    result = runner.Run(args);
                    AppDomain.Unload(newDomain);
                }
            }
            return result;
        }
	}
}
