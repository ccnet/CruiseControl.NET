using System.Collections;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	internal class FirstMatch : Statistic
	{
		public FirstMatch(DictionaryEntry entry) : base(entry)
		{
			
		}
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