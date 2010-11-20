using System.Collections.Generic;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    /// <summary>
    /// A server extension to provide performance counters.
    /// </summary>
    public class IntegrationPerformanceCountersExtension
        : ICruiseServerExtension
    {
        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the extension.
        /// </summary>
        /// <param name="extensionConfig"></param>
        /// <param name="server">The server that this extension is for.</param>
        public void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig)
        {
            if (!PerformanceCounterCategory.Exists("CruiseControl.NET"))
            {
                Log.Info("Initialising new performance counters for integration requests");
                var collection = new CounterCreationDataCollection();

                // Number of integrations completed counter
                var numberOfCompletedIntegrations = new CounterCreationData();
                numberOfCompletedIntegrations.CounterType = PerformanceCounterType.NumberOfItems32;
                numberOfCompletedIntegrations.CounterName = "Number of Completed Integrations";
                collection.Add(numberOfCompletedIntegrations);

                // Number of integrations failed counter
                var numberOfFailedIntegrations = new CounterCreationData();
                numberOfFailedIntegrations.CounterType = PerformanceCounterType.NumberOfItems32;
                numberOfFailedIntegrations.CounterName = "Number of Failed Integrations";
                collection.Add(numberOfFailedIntegrations);

                // Integration time counter
                var integrationElapsedTime = new CounterCreationData();
                integrationElapsedTime.CounterType = PerformanceCounterType.AverageTimer32;
                integrationElapsedTime.CounterName = "Integration Time";
                collection.Add(integrationElapsedTime);

                // Integration count counter
                var averageIntegrations = new CounterCreationData();
                averageIntegrations.CounterType = PerformanceCounterType.AverageBase;
                averageIntegrations.CounterName = "Average number of integrations";
                collection.Add(averageIntegrations);

                // Create the category
                PerformanceCounterCategory.Create("CruiseControl.NET",
                    "Performance counters for CruiseControl.NET",
                    collection);
            }

            // Retrieve the counters
            Log.Debug("Initialising performance monitoring - integration requests");
            var numberOfCompletedIntegrationsCounter = new PerformanceCounter("CruiseControl.NET", "Number of Completed Integrations", false);
            var numberOfFailedIntegrationsCounter = new PerformanceCounter("CruiseControl.NET", "Number of Failed Integrations", false);
            var integrationElapsedTimeCounter = new PerformanceCounter("CruiseControl.NET", "Integration Time", false);
            var averageIntegrationsCounter = new PerformanceCounter("CruiseControl.NET", "Average number of integrations", false);
            var stopwatches = new Dictionary<string, Stopwatch>();

            server.IntegrationStarted += (o, e) =>
            {
                Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Starting stopwatch for '{0}'", e.ProjectName));

                // Start a stopwatch for the project
                if (stopwatches.ContainsKey(e.ProjectName))
                {
                    stopwatches[e.ProjectName].Reset();
                }
                else
                {
                    var stopwatch = new Stopwatch();
                    stopwatches.Add(e.ProjectName, stopwatch);
                    stopwatch.Start();
                }
            };
            server.IntegrationCompleted += (o, e) =>
            {
                Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Performance logging for '{0}'", e.ProjectName));

                // Stop the stopwatch and record the elapsed time
                if (stopwatches.ContainsKey(e.ProjectName))
                {
                    var stopwatch = stopwatches[e.ProjectName];
                    stopwatch.Stop();
                    stopwatches.Remove(e.ProjectName);
                    averageIntegrationsCounter.Increment();
                    integrationElapsedTimeCounter.IncrementBy(stopwatch.ElapsedTicks);
                }

                // Record the result
                if (e.Status == IntegrationStatus.Success)
                {
                    numberOfCompletedIntegrationsCounter.Increment();
                }
                else
                {
                    numberOfFailedIntegrationsCounter.Increment();
                }
            };
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts the extension.
        /// </summary>
        public void Start()
        {
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops the extension.
        /// </summary>
        public void Stop()
        {
        }
        #endregion

        #region Abort()
        /// <summary>
        /// Terminates the extension immediately.
        /// </summary>
        public void Abort()
        {
        }
        #endregion
        #endregion
    }
}
