using System;
using System.Web.Mail;
using NUnit.Framework;

namespace tw.ccnet.core.publishers.test
{
	[TestFixture]
	public class EmailGatewayTest
	{
		public void TestMockEmailGateway()
		{
			MockEmailGateway mail = MockEmailGateway.Create();
			mail.MailHost = "partagas.servidium.com";
			mail.Send("orogers@thoughtworks.com", "owen@exortech.com", "test message", "woo-hoo");
			Assertion.AssertEquals(1, mail.SentMessages.Count);
			Assertion.AssertEquals("orogers@thoughtworks.com", ((MailMessage)mail.SentMessages[0]).From);
			Assertion.AssertEquals("owen@exortech.com", ((MailMessage)mail.SentMessages[0]).To);
			Assertion.AssertEquals("test message", ((MailMessage)mail.SentMessages[0]).Subject);
			Assertion.AssertEquals("woo-hoo", ((MailMessage)mail.SentMessages[0]).Body);
		}
	}
}
