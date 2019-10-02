using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Runtime.Remoting;
using ThoughtWorks.CruiseControl.Remote.Mono;

namespace ThoughtWorks.CruiseControl.Console
{
    /// <summary>
    /// Runs the application in a remoting context.
    /// </summary>
    public class AppRunner
        : MarshalByRefObject
    {
        private ConsoleRunner runner;
        private bool isStopping = false;
        private object lockObject = new object();

        /// <summary>
        /// Start the application.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Run(string[] args, bool usesShadowCopying)
        {
            ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();

            if (consoleArgs.ShowHelp)
            {
                DisplayHelp(GetPopulatedOptionSet(consoleArgs));
                return 0;
            }

            return SetUpConsoleRunner(usesShadowCopying, consoleArgs);
        }

        private int SetUpConsoleRunner(bool usesShadowCopying, ConsoleRunnerArguments consoleArgs)
        {
            try
            {
                StartConsoleRunner(usesShadowCopying, consoleArgs);
                return 0;
            }
            catch (Exception ex)
            {
                HandleAppRunnerException(consoleArgs.PauseOnError, ex);
                return 1;
            }
            finally
            {
                runner = null;
            }
        }

        private void HandleAppRunnerException(bool pauseOnError, Exception ex)
        {
            Log.Error(ex);
            if (pauseOnError)
            {
                System.Console.WriteLine("An unexpected error has caused the console to crash, please press any key to continue...");
                System.Console.ReadKey();
            }
        }

        private void StartConsoleRunner(bool usesShadowCopying, ConsoleRunnerArguments consoleArgs)
        {
            runner = new ConsoleRunner(consoleArgs, new CruiseServerFactory());
            if (!usesShadowCopying)
            {
                Log.Warning("Shadow-copying has been turned off - hot-swapping will not work!");
            }

            runner.Run();
        }

        private OptionSet GetPopulatedOptionSet(ConsoleRunnerArguments consoleArgs)
        {
            OptionSet opts = new OptionSet();
            opts.Add("h|?|help", "display this help screen", delegate (string v) { consoleArgs.ShowHelp = v != null; })
                .Add("c|config=", "the configuration file to use (defaults to ccnet.conf)", delegate (string v) { consoleArgs.ConfigFile = v; })
                .Add("r|remoting=", "turn remoting on/off (defaults to on)", delegate (string v) { consoleArgs.UseRemoting = v == "on"; })
                .Add("p|project=", "the project to integrate (???)", delegate (string v) { consoleArgs.Project = v; })
                .Add("v|validate", "validate the configuration file and exit", delegate (string v) { consoleArgs.ValidateConfigOnly = v != null; })
                .Add("l|logging=", "turn logging on/off (defaults to on)", delegate (string v) { consoleArgs.Logging = v == "on"; })
                .Add("e|errorpause=", "turn pause on error on/off (defaults to on)", delegate (string v) { consoleArgs.PauseOnError = v == "on"; });
            return opts;
        }

        #region InitializeLifetimeService()
        /// <summary>
        /// Initialise the lifetime service.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion

        /// <summary>
        /// Stop the application.
        /// </summary>
        /// <param name="reason"></param>
        public void Stop(string reason)
        {
            // Since there may be a race condition around stopping the runner, check if it should be stopped
            // within a lock statement
            bool stopRunner = false;
            lock (lockObject)
            {
                if (!isStopping)
                {
                    stopRunner = true;
                    isStopping = true;
                }
            }
            if (stopRunner)
            {
                StopAppRunner(reason);
            }
        }
        private void StopAppRunner(string reason)
        {
            try
            {
                Log.Info("Stopping console: " + reason);
                runner.Stop();
            }
            catch (RemotingException)
            {
                // Sometimes this exception gets thrown and not sure why. 
            }
        }

        private void DisplayHelp(OptionSet opts)
        {
            Assembly thisApp = Assembly.GetExecutingAssembly();
            Stream helpStream = thisApp.GetManifestResourceStream("ThoughtWorks.CruiseControl.Console.Help.txt");
            try
            {
                WriteHelpToConsole(helpStream);
            }
            finally
            {
                helpStream.Close();
            }
            opts.WriteOptionDescriptions(System.Console.Out);
        }

        private void WriteHelpToConsole(Stream helpStream)
        {
            StreamReader reader = new StreamReader(helpStream);
            string data = reader.ReadToEnd();
            reader.Close();
            System.Console.Write(data);
        }
    }
}
