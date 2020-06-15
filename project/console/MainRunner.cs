using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.Remoting;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.Console
{
    public class MainRunner
    {
        private AppRunner runner;
        private DateTime restartTime = DateTime.MinValue;
        private object lockObject;
        private string[] args;
        private bool restart = true;

        public MainRunner(string[] _args, object _lockObject)
        {
            lockObject = _lockObject;
            args = _args;
        }

        private FileSystemWatcher GetWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            watcher.Changed += delegate (object sender, FileSystemEventArgs e)
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

            return watcher;
        }

        public int Run()
        {
            int result = 0;
            using (FileSystemWatcher watcher = GetWatcher())
            {
                // Begin the main application loop
                while (restart)
                {
                    restart = false;
                    result = GenerateResult();

                }
            }
            return result;
        }

        private int GenerateResult()
        {
            AppDomain runnerDomain = GetRunnerDomain(ShouldUseShadowCopying());
            int result = GetRunResult(runnerDomain, ShouldUseShadowCopying());
            AppDomain.Unload(runnerDomain);
            AllowChangeEventsToFinish();
            return result;
        }

        private void AllowChangeEventsToFinish()
        {
            // Allow any change events to finish (i.e. if multiple files are copied)
            while (DateTime.Now < restartTime)
            {
                Thread.Sleep(500);
            }
        }

        private int GetRunResult(AppDomain runnerDomain, bool useShadowCopying)
        {
            var result = GetRunner(runnerDomain).Run(args, useShadowCopying);
            return result;
        }

        private AppRunner GetRunner(AppDomain runnerDomain)
        {
            if (runner != null)
            {
                return runner;
            }
            else
            {
                runner = runnerDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(AppRunner).FullName) as AppRunner;
                return runner;
            }
        }

        private AppDomain GetRunnerDomain(bool useShadowCopying)
        {
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

            return runnerDomain;
        }

        private bool ShouldUseShadowCopying()
        {
            string setting = ConfigurationManager.AppSettings["ShadowCopy"] ?? string.Empty;

            return !(string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(setting, "false", StringComparison.OrdinalIgnoreCase));
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
    }
}
