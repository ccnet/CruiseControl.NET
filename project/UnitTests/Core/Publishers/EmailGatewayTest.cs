using System.Web.Mail;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class EmailGatewayTest : CustomAssertion
	{
		public void TestMockEmailGateway()
		{
			MockEmailGateway mail = MockEmailGateway.Create();
			mail.MailHost = "partagas.servidium.com";
			mail.Send("orogers@thoughtworks.com", "owen@exortech.com", "ssschiel@starnet.com", "test message", "woo-hoo");
			Assert.AreEqual(1, mail.SentMessages.Count);
			Assert.AreEqual("orogers@thoughtworks.com", ((MailMessage) mail.SentMessages[0]).From);
			Assert.AreEqual("owen@exortech.com", ((MailMessage) mail.SentMessages[0]).To);
			Assert.AreEqual("ssschiel@starnet.com", ((MailMessage) mail.SentMessages[0]).Headers["Reply-To"]);
			Assert.AreEqual("test message", ((MailMessage) mail.SentMessages[0]).Subject);
			Assert.AreEqual("woo-hoo", ((MailMessage) mail.SentMessages[0]).Body);
			Assert.AreEqual(null, ((MailMessage) mail.SentMessages[0]).Fields[MockEmailGateway.AuthenticationUsernameUrl]);
			Assert.AreEqual(null, ((MailMessage) mail.SentMessages[0]).Fields[MockEmailGateway.AuthenticationPasswordUrl]);
		}

		public void TestMockEmailGatewayWithAuthentication()
		{
			MockEmailGateway mail = MockEmailGateway.Create();
			mail.MailHost = "partagas.servidium.com";
			mail.MailHostUsername = "mailhostUser";
			mail.MailHostPassword = "mailhostPassword";
			mail.Send("orogers@thoughtworks.com", "owen@exortech.com", "ssschiel@starnet.com", "test message", "woo-hoo");
			Assert.AreEqual(1, mail.SentMessages.Count);
			Assert.AreEqual("orogers@thoughtworks.com", ((MailMessage) mail.SentMessages[0]).From);
			Assert.AreEqual("owen@exortech.com", ((MailMessage) mail.SentMessages[0]).To);
			Assert.AreEqual("ssschiel@starnet.com", ((MailMessage) mail.SentMessages[0]).Headers["Reply-To"]);
			Assert.AreEqual("test message", ((MailMessage) mail.SentMessages[0]).Subject);
			Assert.AreEqual("woo-hoo", ((MailMessage) mail.SentMessages[0]).Body);
			Assert.AreEqual("mailhostUser",
			                ((MailMessage) mail.SentMessages[0]).Fields[MockEmailGateway.AuthenticationUsernameUrl]);
			Assert.AreEqual("mailhostPassword",
			                ((MailMessage) mail.SentMessages[0]).Fields[MockEmailGateway.AuthenticationPasswordUrl]);
		}
	}
}