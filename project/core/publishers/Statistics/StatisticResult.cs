using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// Data object for the value of a statistic.
    /// </summary>
    public class StatisticResult
    {
        /// <summary>
        /// The name of this statistic.
        /// </summary>
        private readonly string statName;
        /// <summary>
        /// The value of this instance of this statistic.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Create a statistic value data object.
        /// </summary>
        /// <param name="statName">The name of this statistic.</param>
        /// <param name="value">The value of this instance of this statistic.</param>
        public StatisticResult(string statName, object value)
        {
            this.statName = statName;
            this.value = value;
        }

        /// <summary>
        /// The name of this statistic.
        /// </summary>
        public string StatName
        {
            get { return statName; }
        }

        /// <summary>
        /// The value of this instance of this statistic.
        /// </summary>
        public object Value
        {
            get { return value; }
        }
    }
}
