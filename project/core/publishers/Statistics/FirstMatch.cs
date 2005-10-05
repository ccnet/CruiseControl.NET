using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{

	internal class FirstMatch : Statistic
	{
		public FirstMatch(string name, string xpath) : base(name, xpath)
		{
		}

		protected override object Evaluate(XPathNavigator nav)
		{
			XPathNodeIterator modes = nav.Select(xpath);
			return modes.MoveNext() ? modes.Current.Value : null;
		}
	}
	
}