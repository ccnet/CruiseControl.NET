using System;
using System.Xml;
using NUnit.Framework;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	public class EmailPublisherMother
	{
		public static XmlDocument ConfigurationXml
		{
			get 
			{
				return XmlUtil.CreateDocument(
@"    	<email from=""ccnet@thoughtworks.com"" mailhost=""smtp.telus.net"" projectUrl=""http://localhost/ccnet"" includeDetails=""false"">
    		<users>
    		 	<user name=""buildmaster"" group=""buildmaster"" address=""servid@telus.net""/>
    		 	<user name=""orogers"" group=""developers"" address=""orogers@thoughtworks.com""/>
    		 	<user name=""manders"" group=""developers"" address=""mandersen@thoughtworks.com""/>
    		 	<user name=""dmercier"" group=""developers"" address=""dmercier@thoughtworks.com""/>
    		 	<user name=""rwan"" group=""developers"" address=""rwan@thoughtworks.com""/>
    		</users>
    		<groups>
    			<group name=""developers"" notification=""change""/>
    			<group name=""buildmaster"" notification=""always""/>    			
    		</groups>
    	</email>");
			}
		}

		public static EmailPublisher Create()
		{
			return Create(ConfigurationXml.DocumentElement);
		}

		public static EmailPublisher Create(XmlNode node)
		{
			return NetReflector.Read(node) as EmailPublisher;
		}
	}
}