namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System.Diagnostics;
using System.Collections.Generic;

    /// <summary>
    /// Default implementation of <see cref="IPerformanceCounters"/>.
    /// </summary>
    public class DefaultPerformanceCounters
        : IPerformanceCounters
    {
        #region Private fields
        private readonly Dictionary<string, PerformanceCounter> counters = new Dictionary<string, PerformanceCounter>();
        #endregion

        #region Public methods
        #region EnsureCategoryExists()
        /// <summary>
        /// Ensures the category exists.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        /// <param name="counters">The counters.</param>
        public void EnsureCategoryExists(string category, string description, params CounterCreationData[] counters)
        {
            if (!PerformanceCounterCategory.Exists(category))
            {
                Log.Info("Initialising new performance counters: " + category);
                var collection = new CounterCreationDataCollection(counters);

                // Create the group
                PerformanceCounterCategory.Create(category,
                    description, 
                    PerformanceCounterCategoryType.SingleInstance,
                    collection);
            }
        }
        #endregion

        #region IncrementCounter()
        /// <summary>
        /// Increments the counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        public void IncrementCounter(string category, string name)
        {
            this.RetrieveCounter(category, name).Increment();
        }

        /// <summary>
        /// Increments the counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="amount">The amount.</param>
        public void IncrementCounter(string category, string name, long amount)
        {
            this.RetrieveCounter(category, name).IncrementBy(amount);
        }
        #endregion
        #endregion

        #region Private methods
        #region RetrieveCounter()
        /// <summary>
        /// Retrieves the counter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private PerformanceCounter RetrieveCounter(string category, string name)
        {
            PerformanceCounter counter;
            var key = category + ":" + name;
            if (!this.counters.TryGetValue(key, out counter))
            {
                counter = new PerformanceCounter(category, name, false);
                this.counters.Add(key, counter);
            }

            return counter;
        }
        #endregion
        #endregion
    }
}
