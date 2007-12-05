using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	public class EmailPublisherMother
	{
		public static XmlDocument ConfigurationXml
		{
			get 
			{
				return XmlUtil.CreateDocument(
@"    	<email from=""ccnet@thoughtworks.com"" mailhost=""smtp.telus.net"" mailport=""26""
                mailhostUsername=""mailuser"" mailhostPassword=""mailpassword""
                projectUrl=""http://localhost/ccnet"" includeDetails=""false"">
            <modifierNotificationTypes>
                <NotificationType>failed</NotificationType>
                <NotificationType>fixed</NotificationType>
            </modifierNotificationTypes>
    		<users>
    		 	<user name=""buildmaster"" group=""buildmaster"" address=""servid@telus.net""/>
    		 	<user name=""orogers"" group=""developers"" address=""orogers@thoughtworks.com""/>
    		 	<user name=""manders"" group=""developers"" address=""mandersen@thoughtworks.com""/>
    		 	<user name=""dmercier"" group=""developers"" address=""dmercier@thoughtworks.com""/>
    		 	<user name=""rwan"" group=""developers"" address=""rwan@thoughtworks.com""/>
                <user name=""owjones"" group=""successdudes"" address=""oliver.wendell.jones@example.com""/>
    		</users>
    		<groups>
    			<group name=""developers"" notification=""change""/>
    			<group name=""buildmaster"" notification=""always""/>
                <group name=""successdudes"" notification=""success""/>
    		</groups>
			<converters>
				<converter find=""$"" replace=""@TheCompany.com""/>
			</converters>
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