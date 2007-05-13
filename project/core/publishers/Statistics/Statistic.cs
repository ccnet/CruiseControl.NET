using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("statistic")]
	public class Statistic
	{
		protected string name;
		protected string xpath;
	    private bool generateGraph;

	    public Statistic()
		{
		}

		public Statistic(string name, string xpath)
		{
			this.name = name;
			this.xpath = xpath;
		}

		[ReflectorProperty("xpath")]
		public string Xpath
		{
			get { return xpath; }
			set { xpath = value; }
		}

		[ReflectorProperty("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

        [ReflectorProperty("generateGraph")]
	    public bool GenerateGraph
	    {
            get { return generateGraph; }
            set { generateGraph = value; }
	    }

		public StatisticResult Apply(XPathNavigator nav)
		{
			object value = Evaluate(nav);
			return new StatisticResult(name, value);
		}

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

	public class StatisticResult
	{
		private readonly string statName;
		private readonly object value;

		public StatisticResult(string statName, object value)
		{
			this.statName = statName;
			this.value = value;
		}

		public string StatName
		{
			get { return statName; }
		}

		public object Value
		{
			get { return value; }
		}
	}
}