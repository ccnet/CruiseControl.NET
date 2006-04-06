using System.Collections;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{

	internal class Statistic
	{
		public string Name
		{
			get { return name; }
		}

		protected readonly string name;
		protected readonly string xpath;

		public Statistic(DictionaryEntry entry) :this (entry.Key.ToString(), entry.Value.ToString())
		{
			
		}
		public Statistic(string name, string xpath)
		{
			this.name = name;
			this.xpath = xpath;
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