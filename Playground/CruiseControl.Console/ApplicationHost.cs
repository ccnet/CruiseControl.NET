namespace CruiseControl.Console
{
    using System;
    using CruiseControl.Core;
    using CruiseControl.Core.Utilities;
    using CruiseControl.Core.Xaml;

    /// <summary>
    /// Host for running the application.
    /// </summary>
    public class ApplicationHost
        : MarshalByRefObject
    {
        #region Public methods
        #region Run()
        /// <summary>
        /// Runs the application.
        /// </summary>
        public void Run()
        {
            // TODO: Make the configuration service configurable
            var application = new Application
                                  {
                                      ConfigurationService = new ConfigurationService(),
                                      FileSystem = new FileSystem(),
                                      ValidationLog = new LoggingValidationLog()
                                  };
            application.LoadConfiguration();
            application.Start();
            var runApplication = true;
            while (runApplication)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.End)
                {
                    application.Stop();
                    runApplication = false;
                }
            }
        }
        #endregion
        #endregion
    }
}
