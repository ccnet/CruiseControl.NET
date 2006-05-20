using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("statistic")]
	public class Statistic
	{
		protected string name;
		protected string xpath;

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

		public object Apply(XPathNavigator nav)
		{
			object value = Evaluate(nav);
			return value;
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
			return base.GetHashCode();
		}
	}
}