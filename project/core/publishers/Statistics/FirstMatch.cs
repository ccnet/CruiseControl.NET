using System.Xml.XPath;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("firstMatch")]
	public class FirstMatch : Statistic
	{
		public FirstMatch()
		{
		}

		public FirstMatch(string name, string xpath) : base(name, xpath)
		{
		}

		protected override object Evaluate(XPathNavigator nav)
		{
			XPathNodeIterator nodeIterator = nav.Select(xpath);
			return nodeIterator.MoveNext() ? nodeIterator.Current.Value : null;
		}
	}	
}