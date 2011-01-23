using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// Provides the base functionality for statistics.
    /// </summary>
    /// <title>Statistics</title>
    public abstract class StatisticBase
    {
        /// <summary>
        /// The name of this statistic.
        /// </summary>
        protected string name;
        /// <summary>
        /// The XML XPath to locate the value of this statistic.
        /// </summary>
        protected string xpath;
        /// <summary>
        /// Should a graph be generated for this statistic?
        /// </summary>
        private bool generateGraph;
        /// <summary>
        /// Should this statistic be collected and published?
        /// </summary>
        private bool include = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticBase" /> class.	
        /// </summary>
        /// <remarks></remarks>
        protected StatisticBase()
        {
        }

        /// <summary>
        /// Create a statistic that extracts all items that match the specifed XML XPath.
        /// </summary>
        /// <param name="name">The name of the statistic.</param>
        /// <param name="xpath">The XML XPath to locate the values.</param>
        protected StatisticBase(string name, string xpath)
        {
            this.name = name;
            this.xpath = xpath;
        }

        /// <summary>
        /// The XML XPath to locate the value of this statistic.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.0</version>
        [ReflectorProperty("xpath")]
        public string Xpath
        {
            get { return xpath; }
            set { xpath = value; }
        }

        /// <summary>
        /// The name of the statistic.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.0</version>
        [ReflectorProperty("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Should a graph be generated for this statistic?
        /// </summary>
        /// <default>false</default>
        /// <version>1.3</version>
        [ReflectorProperty("generateGraph", Required = false)]
        public bool GenerateGraph
        {
            get { return generateGraph; }
            set { generateGraph = value; }
        }

        /// <summary>
        /// Should this statistic be collected and published?
        /// </summary>
        /// <default>true</default>
        /// <version>1.3</version>
        [ReflectorProperty("include", Required = false)]
        public bool Include
        {
            get { return include; }
            set { include = value; }
        }

        /// <summary>
        /// Extract the value of the statistic from the specified XML data.
        /// </summary>
        /// <param name="nav">A navigator into an XML document containing the statistic data.</param>
        /// <returns>The statistic value.</returns>
        public StatisticResult Apply(XPathNavigator nav)
        {
            object value = Evaluate(nav);
            return new StatisticResult(name, value);
        }

        /// <summary>
        /// Extract the value of the statistic from the specified XML data.
        /// </summary>
        /// <param name="nav">A navigator into an XML document containing the statistic data.</param>
        /// <returns>The statistic value.</returns>
        protected virtual object Evaluate(XPathNavigator nav)
        {
            return nav.Evaluate(xpath);
        }

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool Equals(object obj)
        {
            var o = (StatisticBase)obj;
            return name.Equals(o.Name);
        }

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
