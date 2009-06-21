using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Runtime.Remoting;

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
        public int Run(string[] args)
        {
            ArgumentParser parsedArgs = new ArgumentParser(args);
            try
            {
                runner = new ConsoleRunner(parsedArgs, new CruiseServerFactory());
                runner.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (!parsedArgs.NoPauseOnError)
                {
                    System.Console.WriteLine("An unexpected error has caused the console to crash, please press any key to continue...");
                    System.Console.ReadKey();
                }
                return 1;
            }
            finally
            {
                runner = null;
            }
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
                // Perform the actual stop
                Log.Info("Stopping console: " + reason);
                try
                {
                    runner.Stop();
                }
                catch (RemotingException)
                {
                    // Sometimes this exception gets thrown and not sure why. 
                }
            }
        }
    }
}
