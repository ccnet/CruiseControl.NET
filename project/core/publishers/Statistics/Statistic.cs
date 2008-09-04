using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// Data object for a build statistic.
    /// </summary>
	[ReflectorType("statistic")]
	public class Statistic
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

	    public Statistic()
		{
		}

        /// <summary>
        /// Create a statistic that extracts all items that match the specifed XML XPath.
        /// </summary>
        /// <param name="name">The name of the statistic.</param>
        /// <param name="xpath">The XML XPath to locate the values.</param>
        public Statistic(string name, string xpath)
		{
			this.name = name;
			this.xpath = xpath;
		}

        /// <summary>
        /// The XML XPath to locate the value of this statistic.
        /// </summary>
        [ReflectorProperty("xpath")]
		public string Xpath
		{
			get { return xpath; }
			set { xpath = value; }
		}

        /// <summary>
        /// The name of this statistic.
        /// </summary>
		[ReflectorProperty("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

        /// <summary>
        /// Should a graph be generated for this statistic?
        /// Default is false.
        /// </summary>
        [ReflectorProperty("generateGraph", Required = false)]
	    public bool GenerateGraph
	    {
            get { return generateGraph; }
            set { generateGraph = value; }
	    }

        /// <summary>
        /// Should this statistic be collected and published?
        /// Default is true.
        /// </summary>
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

		public override bool Equals(object obj)
		{
			Statistic o = (Statistic) obj;
			return name.Equals(o.Name);
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}

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