using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Service
{
    public class CCService : ServiceBase
    {
        AppRunner runner;
        public const string DefaultServiceName = "CCService";

        public CCService()
        {
            ServiceName = LookupServiceName();
        }

        protected override void OnStart(string[] args)
        {
            RunApplication();
        }

        private void RunApplication()
        {
            bool restart = true;
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
                    runner = newDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                        "ThoughtWorks.CruiseControl.Console.AppRunner") as AppRunner;

                    watcher.Changed += delegate(object sender, FileSystemEventArgs e) { restart = true; runner.Stop(e.Name + " has changed"); };
                    watcher.EnableRaisingEvents = true;
                    watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;

                    runner.Run();
                    AppDomain.Unload(newDomain);
                }
            }
        }

        // Should this be stop or abort?
        protected override void OnStop()
        {
            runner.Stop("Service is stopped");
        }

        protected override void OnPause()
        {
            runner.Stop("Service is paused");
        }

        protected override void OnContinue()
        {
            RunApplication();
        }

        private static string LookupServiceName()
        {
            string serviceName = ConfigurationManager.AppSettings["service.name"];
            return StringUtil.IsBlank(serviceName) ? DefaultServiceName : serviceName;
        }

        private static void Main()
        {
            AllocateWin32Console();
            Run(new ServiceBase[] { new CCService() });
        }

        // Allocates a Win32 console if needed since Windows does not provide
        // one to Services by default. Normally that's okay, but we will be
        // launching console applications and they may fail unless the parent
        // process supplies them with a console.
        private static void AllocateWin32Console()
        {
            if (new ExecutionEnvironment().IsRunningOnWindows)
                AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();
    }
}