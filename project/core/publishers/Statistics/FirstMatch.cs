using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// A statistic that extracts the first item that matches the specifed XML XPath.
    /// </summary>
    /// <title>FirstMatch</title>
    /// <version>1.0</version>
    [ReflectorType("firstMatch")]
    public class FirstMatch 
        : StatisticBase
	{
		public FirstMatch()
		{
		}

        /// <summary>
        /// Create a statistic that extracts the first item that matches the specifed XML XPath.
        /// </summary>
        /// <param name="name">The name of the statistic.</param>
        /// <param name="xpath">The XML XPath to locate the value.</param>
		public FirstMatch(string name, string xpath) : base(name, xpath)
		{
		}

        /// <summary>
        /// Extract the value of the statistic from the specified XML data.
        /// </summary>
        /// <param name="nav">A navigator into an XML document containing the statistic data.</param>
        /// <returns>The statistic value.</returns>
		protected override object Evaluate(XPathNavigator nav)
		{
			XPathNodeIterator nodeIterator = nav.Select(xpath);
			return nodeIterator.MoveNext() ? nodeIterator.Current.Value : null;
		}
	}	
}