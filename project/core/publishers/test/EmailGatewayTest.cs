using System;
using System.Web.Mail;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class EmailGatewayTest : CustomAssertion
	{
		public void TestMockEmailGateway()
		{
			MockEmailGateway mail = MockEmailGateway.Create();
			mail.MailHost = "partagas.servidium.com";
			mail.Send("orogers@thoughtworks.com", "owen@exortech.com", "test message", "woo-hoo");
			AssertEquals(1, mail.SentMessages.Count);
			AssertEquals("orogers@thoughtworks.com", ((MailMessage)mail.SentMessages[0]).From);
			AssertEquals("owen@exortech.com", ((MailMessage)mail.SentMessages[0]).To);
			AssertEquals("test message", ((MailMessage)mail.SentMessages[0]).Subject);
			AssertEquals("woo-hoo", ((MailMessage)mail.SentMessages[0]).Body);
		}
	}
}
