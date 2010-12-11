using System.Collections.Generic;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    /// <summary>
    /// A server extension to provide performance counters.
    /// </summary>
    public class IntegrationPerformanceCountersExtension
        : ICruiseServerExtension
    {
        #region Constants
        public const string CategoryName = "CruiseControl.NET Integrations";
        public const string NumberCompletedCounter = "Number of Completed Integrations";
        public const string NumberFailedCounter = "Number of Failed Integrations";
        public const string AverageTimeCounter = "Average Integration Time";
        public const string NumberTotalCounter = "Total Number of Integrations";
        #endregion

        #region Public properties
        #region PerformanceCounters
        /// <summary>
        /// Gets or sets the performance counters.
        /// </summary>
        /// <value>The performance counters.</value>
        public IPerformanceCounters PerformanceCounters { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the extension.
        /// </summary>
        /// <param name="extensionConfig"></param>
        /// <param name="server">The server that this extension is for.</param>
        public void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig)
        {
            var counters = this.PerformanceCounters ?? new DefaultPerformanceCounters();
            counters.EnsureCategoryExists(
                CategoryName,
                "Performance counters for CruiseControl.NET",
                new CounterCreationData(NumberCompletedCounter, string.Empty, PerformanceCounterType.NumberOfItems32),
                new CounterCreationData(NumberFailedCounter, string.Empty, PerformanceCounterType.NumberOfItems32),
                new CounterCreationData(AverageTimeCounter, string.Empty, PerformanceCounterType.AverageTimer32),
                new CounterCreationData(NumberTotalCounter, string.Empty, PerformanceCounterType.AverageBase));

            // Retrieve the counters
            Log.Debug("Initialising performance monitoring - integration requests");
            var stopwatches = new Dictionary<string, Stopwatch>();

            server.IntegrationStarted += (o, e) =>
            {
                Log.Debug(
                    string.Format(CultureInfo.CurrentCulture,"Starting stopwatch for '{0}'", e.ProjectName));

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
                Log.Debug(
                    string.Format(CultureInfo.CurrentCulture,"Performance logging for '{0}'", e.ProjectName));

                // Stop the stopwatch and record the elapsed time
                if (stopwatches.ContainsKey(e.ProjectName))
                {
                    var stopwatch = stopwatches[e.ProjectName];
                    stopwatch.Stop();
                    stopwatches.Remove(e.ProjectName);
                    counters.IncrementCounter(CategoryName, NumberTotalCounter);
                    counters.IncrementCounter(CategoryName, AverageTimeCounter, stopwatch.ElapsedMilliseconds);
                }

                // Record the result
                if (e.Status == IntegrationStatus.Success)
                {
                    counters.IncrementCounter(CategoryName, NumberCompletedCounter);
                }
                else
                {
                    counters.IncrementCounter(CategoryName, NumberFailedCounter);
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
