using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ThoughtWorks.CruiseControl.Service
{
    public class CCService : ServiceBase
    {
        private AppRunner runner;
        public const string DefaultServiceName = "CCService";
        private object lockObject = new object();
        private FileSystemWatcher watcher;
        private AppDomain runnerDomain;
        private System.Timers.Timer waitTimer = new System.Timers.Timer(15000);

        public CCService()
        {
            if (string.Equals(ConfigurationManager.AppSettings["DebugCCService"], "yes", StringComparison.InvariantCultureIgnoreCase))
            {
                Debugger.Launch();
            }
            // Initialise the wait timer
            waitTimer.AutoReset = false;
            waitTimer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
            {
                lock (lockObject)
                {
                    if (runner == null) RunApplication();
                }
            };
            ServiceName = LookupServiceName();
        }

        protected override void OnStart(string[] args)
        {
            RunApplication();
        }

        private void RunApplication()
        {
            if (watcher == null)
            {
                watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                {
                    StopRunner("One or more DLLs have changed");
                    waitTimer.Stop();
                    waitTimer.Start();
                };
                watcher.EnableRaisingEvents = true;
                watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;
            }

            runnerDomain = AppDomain.CreateDomain("CC.Net",
                null,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath,
                true);
            runner = runnerDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                typeof(AppRunner).FullName) as AppRunner;
            runner.Run();
        }

        // Should this be stop or abort?
        protected override void OnStop()
        {
            StopRunner("Service is stopped");
        }

        protected override void OnPause()
        {
            StopRunner("Service is paused");
        }

        private void StopRunner(string reason)
        {
            lock (lockObject)
            {
                if (runner != null)
                {
                    runner.Stop(reason);
                    AppDomain.Unload(runnerDomain);
                    runner = null;
                }
            }
        }

        protected override void OnContinue()
        {
            RunApplication();
        }

        private static string LookupServiceName()
        {
            string serviceName = ConfigurationManager.AppSettings["service.name"];
            return string.IsNullOrEmpty(serviceName) ? DefaultServiceName : serviceName;
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
            if (IsRunningOnWindows) AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private static bool IsRunningOnWindows
        {
            get
            {
				int platform = (int)Environment.OSVersion.Platform;
				return ((platform != 4) // PlatformID.Unix
					&& (platform != 6) // PlatformID.MacOSX
					&& (platform != 128)); // Mono compability value for PlatformID.Unix on .NET 1.x profile
            }
        }
    }
}