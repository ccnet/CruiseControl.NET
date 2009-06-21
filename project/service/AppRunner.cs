using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Service
{
    public class AppRunner
        : MarshalByRefObject
    {
        private const string DefaultConfigFileName = "ccnet.config";
        private readonly string DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private ICruiseServer server;
        private bool isStopping = false;
        private object lockObject = new object();

        private string ConfigFilename
        {
            get
            {
                string configFilename = ConfigurationManager.AppSettings["ccnet.config"];
                return string.IsNullOrEmpty(configFilename) ? DefaultConfigFilePath() : configFilename;
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

        private static string Remoting
        {
            get { return ConfigurationManager.AppSettings["remoting"]; }
        }

        public void Run(string action)
        {
            try
            {
                // Set working directory to service executable's home directory.
                Directory.SetCurrentDirectory(DefaultDirectory);

                // Announce our presence
                Log.Info(string.Format("CruiseControl.NET Server {0} -- .NET Continuous Integration Server", Assembly.GetExecutingAssembly().GetName().Version));
                // Find out our copyright claim, if any, and display it.
                AssemblyCopyrightAttribute[] copyrightAttributes = (AssemblyCopyrightAttribute[])Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (copyrightAttributes.Length > 0)
                {
                    Log.Info(string.Format("{0}  All Rights Reserved.", copyrightAttributes[0].Copyright));
                }
                Log.Info(string.Format(".NET Runtime Version: {0}{2}\tImage Runtime Version: {1}", Environment.Version, Assembly.GetExecutingAssembly().ImageRuntimeVersion, GetRuntime()));
                Log.Info(string.Format("OS Version: {0}\tServer locale: {1}", Environment.OSVersion, CultureInfo.CurrentUICulture.NativeName));
                if (!string.IsNullOrEmpty(action)) Log.Info(string.Format("Reason: {0}", action));

                VerifyConfigFileExists();
                CreateAndStartCruiseServer();
            }
            catch (Exception error)
            {
                Log.Error("A fatal error occurred while starting the CruiseControl.NET server");
                Log.Error(error);
                throw;
            }
        }

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
                Log.Info("Stopping service: " + reason);
                server.Stop();
                server.WaitForExit();
                Log.Debug("Service has been stopped");
            }
        }

        private string DefaultConfigFilePath()
        {
            return Path.Combine(DefaultDirectory, DefaultConfigFileName);
        }

        private void VerifyConfigFileExists()
        {
            FileInfo configFileInfo = new FileInfo(ConfigFilename);
            if (!configFileInfo.Exists)
            {
                throw new Exception(
                    string.Format("CruiseControl.NET configuration file {0} does not exist.", 
                        configFileInfo.FullName));
            }
        }

        private void CreateAndStartCruiseServer()
        {
            server = new CruiseServerFactory().Create(UseRemoting(), ConfigFilename);
            server.Start();
        }

        private static bool UseRemoting()
        {
            return (Remoting != null && Remoting.Trim().ToLower() == "on");
        }

        private static string GetRuntime()
        {
            if (Type.GetType("Mono.Runtime") != null)
                return " [Mono]";
            return string.Empty;
        }
    }
}
