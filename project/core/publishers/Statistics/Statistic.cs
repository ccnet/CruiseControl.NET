using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class Statistic
	{
		protected readonly string name;
		protected readonly string xpath;

		public Statistic(string name, string xpath)
		{
			this.name = name;
			this.xpath = xpath;
		}

		public string Name
		{
			get { return name; }
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
	}
}