using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// A generic statistic.
    /// </summary>
    /// <title>Statistic</title>
    /// <version>1.0</version>
    [ReflectorType("statistic")]
	public class Statistic
        : StatisticBase
	{
        public Statistic()
        {
        }

        /// <summary>
        /// Create a statistic that extracts all items that match the specifed XML XPath.
        /// </summary>
        /// <param name="name">The name of the statistic.</param>
        /// <param name="xpath">The XML XPath to locate the values.</param>
        public Statistic(string name, string xpath)
            : base(name, xpath)
        {
        }
	}
}