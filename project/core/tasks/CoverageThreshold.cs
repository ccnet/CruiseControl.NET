using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// A threshold for a coverage report.
    /// </summary>
    [ReflectorType("coverageThreshold")]
    public class CoverageThreshold
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CoverageThreshold"/>.
        /// </summary>
        public CoverageThreshold()
        {
            ItemType = NCoverItemType.Default;
        }
        #endregion

        #region Public properties
        #region Metric
        /// <summary>
        /// The coverage metric.
        /// </summary>
        [ReflectorProperty("metric")]
        public NCoverMetric Metric { get; set; }
        #endregion

        #region MinValue
        /// <summary>
        /// The minimum coverage value.
        /// </summary>
        [ReflectorProperty("value", Required = false)]
        public int MinValue { get; set; }
        #endregion

        #region ItemType
        /// <summary>
        /// The type of item.
        /// </summary>
        [ReflectorProperty("type", Required = false)]
        public NCoverItemType ItemType { get; set; }
        #endregion

        #region Pattern
        /// <summary>
        /// The matching pattern to use.
        /// </summary>
        [ReflectorProperty("pattern", Required = false)]
        public string Pattern { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region ToParamString()
        /// <summary>
        /// Returns a string that can be used an a parameter to the application.
        /// </summary>
        /// <returns></returns>
        public string ToParamString()
        {
            var builder = new StringBuilder();
            builder.Append(Metric);
            if (MinValue >= 0)
            {
                builder.AppendFormat(":{0}", MinValue);
                if ((ItemType != NCoverItemType.Default) || !string.IsNullOrEmpty(Pattern))
                {
                    builder.AppendFormat(":{0}", ItemType == NCoverItemType.Default ? NCoverItemType.Default : ItemType);
                    if (!string.IsNullOrEmpty(Pattern)) builder.AppendFormat(":{0}", Pattern);
                }
            }
            return builder.ToString();
        }
        #endregion
        #endregion

        #region Enumerations
        #region NCoverMetric
        /// <summary>
        /// The coverage metrics.
        /// </summary>
        public enum NCoverMetric
        {
            SymbolCoverage, 
            BranchCoverage, 
            MethodCoverage, 
            CyclomaticComplexity
        }
        #endregion

        #region NCoverItemType
        /// <summary>
        /// The item types.
        /// </summary>
        public enum NCoverItemType
        {
            Default,
            View, 
            Module, 
            Namespace, 
            Class
        }
        #endregion
        #endregion
    }
}
