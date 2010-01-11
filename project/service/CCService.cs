using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceProcess;

namespace ThoughtWorks.CruiseControl.Service
{
    public class CCService : ServiceBase
    {
        private AppRunner runner;
        private const int restartTime = 10;
        public const string DefaultServiceName = "CCService";
        private object lockObject = new object();
        private FileSystemWatcher watcher;
        private AppDomain runnerDomain;
        private System.Timers.Timer waitTimer = new System.Timers.Timer(restartTime * 1000);

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
                    if (runner == null) RunApplication("File change delay finished");
                }
            };
            ServiceName = LookupServiceName();
        }

        protected override void OnStart(string[] args)
        {
            RunApplication("SCM start");
        }

        private void RunApplication(string action)
        {
            if (watcher == null)
            {
                watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                {
                    StopRunner(string.Format("One or more DLLs have changed - waiting {0}s", restartTime));
                    waitTimer.Stop();
                    waitTimer.Start();
                };
                watcher.EnableRaisingEvents = true;
                watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;
            }

            // Allow the user to turn shadow-copying off
            var setting = ConfigurationManager.AppSettings["ShadowCopy"] ?? string.Empty;
            var useShadowCopying = !(string.Equals(setting, "off", StringComparison.InvariantCultureIgnoreCase) ||
                !string.Equals(setting, "false", StringComparison.InvariantCultureIgnoreCase));
            try
            {
                this.runnerDomain = CreateNewDomain(useShadowCopying);
            }
            catch (FileLoadException)
            {
                // Unable to use shadow-copying (no user profile?), therefore turn off shadow-copying
                useShadowCopying = false;
                this.runnerDomain = CreateNewDomain(useShadowCopying);
            }
            runner = runnerDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                typeof(AppRunner).FullName) as AppRunner;
            try
            {
                runner.Run(action, useShadowCopying);
            }
            catch (SerializationException)
            {
                var configFilename = ConfigurationManager.AppSettings["ccnet.config"];
                configFilename = string.IsNullOrEmpty(configFilename) ? Path.Combine(Environment.CurrentDirectory, "ccnet.log") : configFilename;
                throw new ApplicationException(
                    string.Format("A fatal error has occurred while starting CCService. Please check '{0}' for any details.", configFilename));
            }
        }

        /// <summary>
        /// Creates the new runner domain.
        /// </summary>
        /// <param name="useShadowCopying">If set to <c>true</c> shadow copying will be used.</param>
        /// <returns>The new <see cref="AppDomain"/>.</returns>
        private AppDomain CreateNewDomain(bool useShadowCopying)
        {
            return AppDomain.CreateDomain(
                "CC.Net",
                null,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath,
                useShadowCopying);
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
            AppRunner runnerToStop = null;

            // Retrieve the runner in a thread-safe block and then clear it so we are not holding up otherwise processing
            lock (lockObject)
            {
                if (runner != null)
                {
                    runnerToStop = runner;
                    runner = null;
                }
            }

            // If a runner needs to be stopped, do it here
            if (runnerToStop != null)
            {
                runnerToStop.Stop(reason);
                AppDomain.Unload(runnerDomain);
            }
        }

        protected override void OnContinue()
        {
            RunApplication("SCM continue");
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
                // mono returns 128 when running on linux, .NET 2.0 returns 4
                // see http://www.mono-project.com/FAQ:_Technical
                int platform = (int)Environment.OSVersion.Platform;
                return ((platform != 4) && (platform != 128));
            }
        }
    }
}