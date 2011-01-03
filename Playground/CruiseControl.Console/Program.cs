namespace CruiseControl.Console
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Runs the application.
    /// </summary>
    public class Program
    {
        #region Public methods
        #region Main()
        /// <summary>
        /// Main application.
        /// </summary>
        public static void Main()
        {
            ApplicationHost host;
            var domain = CreateDomain(false);
            host = domain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(ApplicationHost).FullName) as ApplicationHost;
            host.Run();
        }
        #endregion

        #region CreateDomain()
        /// <summary>
        /// Creates a new instance of the application domain.
        /// </summary>
        /// <param name="shadowCopy">if set to <c>true</c> [shadow copy].</param>
        public static AppDomain CreateDomain(bool shadowCopy)
        {
            var domain = AppDomain.CreateDomain(
                "CruiseControl.Net",
                null,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath,
                shadowCopy);
            return domain;
        }
        #endregion
        #endregion
    }
}
