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
			Assert.AreEqual(1, mail.SentMessages.Count);
			Assert.AreEqual("orogers@thoughtworks.com", ((MailMessage)mail.SentMessages[0]).From);
			Assert.AreEqual("owen@exortech.com", ((MailMessage)mail.SentMessages[0]).To);
			Assert.AreEqual("test message", ((MailMessage)mail.SentMessages[0]).Subject);
			Assert.AreEqual("woo-hoo", ((MailMessage)mail.SentMessages[0]).Body);
		}
	}
}
