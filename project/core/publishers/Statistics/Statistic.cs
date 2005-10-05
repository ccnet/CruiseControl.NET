using System.Collections;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{

	internal class Statistic
	{
		public string Name
		{
			get { return name; }
		}

		protected readonly string name;
		protected readonly string xpath;

		public Statistic(string name, string xpath)
		{
			this.name = name;
			this.xpath = xpath;
		}

		public void Apply(XPathNavigator nav, Hashtable results)
		{
			results[name] = Evaluate(nav);
		}

		protected virtual object Evaluate(XPathNavigator nav)
		{
			return nav.Evaluate(xpath);
		}
	}
}