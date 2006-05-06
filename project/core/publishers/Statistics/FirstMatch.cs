using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class FirstMatch : Statistic
	{
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