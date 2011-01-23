namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System.Diagnostics;

    /// <summary>
    /// Exposes functionality for working with performance counters.
    /// </summary>
    public interface IPerformanceCounters
    {
        #region Public methods
        #region EnsureCategoryExists()
        /// <summary>
        /// Ensures the category exists.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        /// <param name="counters">The counters.</param>
        void EnsureCategoryExists(string category, string description, params CounterCreationData[] counters);
        #endregion

        #region IncrementCounter()
        /// <summary>
        /// Increments the counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        void IncrementCounter(string category, string name);

        /// <summary>
        /// Increments the counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="amount">The amount.</param>
        void IncrementCounter(string category, string name, long amount);
        #endregion
        #endregion
    }
}
