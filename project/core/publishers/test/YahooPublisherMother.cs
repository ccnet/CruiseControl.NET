using System;
using System.Xml;
using NUnit.Framework;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	public class YahooPublisherMother
	{
		public static XmlDocument ConfigurationXml
		{
			get 
			{
				return XmlUtil.CreateDocument(
					@"    	<yplugin from=""ccnet@thoughtworks.com"" projectUrl=""http://localhost/ccnet"">
    		<yahoousers>
    		 	<yuser ame=""buildmaster"" group=""buildmaster"" id=""itsajey""/>
    		 	<yuser name=""netbuzzme"" group=""developers"" id=""netbuzzme""/>
    		 	<yuser name=""itsajey"" group=""developers"" id=""ajey.gore""/>
    		 	<yuser name=""narsi"" group=""developers"" id=""narsi321""/>
    		 	<yuser name=""asubrama"" group=""developers"" id=""ashok_subramanian""/>
    		</yahoousers>
    		<yahoogroups>
    			<ygroup name=""developers"" notification=""change""/>
    			<ygroup name=""buildmaster"" notification=""always""/>    			
    		</yahoogroups>
    	</yplugin>");
			}
		}

		public static YahooPublisher Create()
		{
			return Create(ConfigurationXml.DocumentElement);
		}

		public static YahooPublisher Create(XmlNode node)
		{
			XmlPopulator populator = new XmlPopulator();
			return (YahooPublisher)populator.Populate(node);
		}
	}
}