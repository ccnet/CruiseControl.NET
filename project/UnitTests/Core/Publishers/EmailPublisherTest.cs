using System;
using System.Collections;
using System.Web.Mail;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class EmailPublisherTest : CustomAssertion
	{
		private EmailPublisher publisher;
		private MockEmailGateway gateway;

		[SetUp]
		public void SetUp()
		{
			publisher = EmailPublisherMother.Create();
			gateway = MockEmailGateway.Create();
			publisher.EmailGateway = gateway;
		}

		[Test]
		public void SendMessage()
		{
			publisher.SendMessage("from@foo.com", "to@bar.com", "replyto@bar.com", "test subject", "test message");
			Assert.AreEqual(1, gateway.SentMessages.Count);

			MailMessage message = (MailMessage) gateway.SentMessages[0];
			Assert.AreEqual("from@foo.com", message.From);
			Assert.AreEqual("to@bar.com", message.To);
			Assert.AreEqual("replyto@bar.com", message.Headers["Reply-To"]);
			Assert.AreEqual("test subject", message.Subject);
			Assert.AreEqual("test message", message.Body);
		}

		[Test]
		public void ShouldNotSendMessageIfRecipientIsNotSpecifiedAndBuildIsSuccessful()
		{
			publisher = new EmailPublisher();
			publisher.EmailGateway = gateway;
			publisher.EmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
			publisher.EmailGroups.Add("foo", new EmailGroup("foo", EmailGroup.NotificationType.Change));
			publisher.Run(IntegrationResultMother.CreateStillSuccessful());
			Assert.AreEqual(0, gateway.SentMessages.Count);
		}

		[Test]
		public void ShouldSendMessageIfRecipientIsNotSpecifiedAndBuildFailed()
		{
			publisher = new EmailPublisher();
			publisher.EmailGateway = gateway;
			publisher.EmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
			publisher.EmailGroups.Add("foo", new EmailGroup("foo", EmailGroup.NotificationType.Change));
			publisher.Run(IntegrationResultMother.CreateFailed());
			Assert.AreEqual(1, gateway.SentMessages.Count);
		}

		[Test]
		public void ShouldSendMessageIfBuildFailed()
		{
			publisher = new EmailPublisher();
			publisher.EmailGateway = gateway;
			publisher.EmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
			publisher.EmailGroups.Add("foo", new EmailGroup("foo", EmailGroup.NotificationType.Failed));
			publisher.Run(IntegrationResultMother.CreateFailed() );
			
			Assert.AreEqual(1, gateway.SentMessages.Count);
			Assert.AreEqual(1, gateway.Recipients.Count);
		}

		[Test]
		public void ShouldSendMessageIfBuildFailedAndPreviousFailed()
		{
			publisher = new EmailPublisher();
			publisher.EmailGateway = gateway;
			
			publisher.EmailUsers.Add("dev", new EmailUser("dev", "changing", "dev@foo.com"));
			publisher.EmailUsers.Add("admin", new EmailUser("admin", "failing", "bar@foo.com"));

			publisher.EmailGroups.Add("changing", new EmailGroup("changing", EmailGroup.NotificationType.Change));
			publisher.EmailGroups.Add("failing", new EmailGroup("failing", EmailGroup.NotificationType.Failed));

			publisher.Run(IntegrationResultMother.CreateFailed(IntegrationStatus.Failure) );
			
			Assert.AreEqual(1, gateway.SentMessages.Count);
			Assert.AreEqual(1, gateway.Recipients.Count);
		}

		[Test]
		public void ShouldSendMessageIfBuildFailedAndPreviousOK()
		{
			publisher = new EmailPublisher();
			publisher.EmailGateway = gateway;
			
			publisher.EmailUsers.Add("dev", new EmailUser("dev", "changing", "dev@foo.com"));
			publisher.EmailUsers.Add("admin", new EmailUser("admin", "failing", "bar@foo.com"));

			publisher.EmailGroups.Add("changing", new EmailGroup("changing", EmailGroup.NotificationType.Change));
			publisher.EmailGroups.Add("failing", new EmailGroup("failing", EmailGroup.NotificationType.Failed));

			publisher.Run(IntegrationResultMother.CreateFailed(IntegrationStatus.Success) );

			Assert.AreEqual(1, gateway.SentMessages.Count);
			Assert.AreEqual(2, gateway.Recipients.Count);
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus current, IntegrationStatus last)
		{
			IntegrationResult result = IntegrationResultMother.Create(current, last, new DateTime(1980, 1, 1));
			result.ProjectName = "Project#9";
			result.Label = "0";
			return result;
		}

		[Test]
		public void EmailMessageWithDetails()
		{
			publisher.IncludeDetails = true;
			string message = publisher.CreateMessage(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assert.IsTrue(message.StartsWith("<html>"));
			Assert.IsTrue(message.IndexOf("CruiseControl.NET Build Results for project Project#9") > 0);
			Assert.IsTrue(message.IndexOf("Modifications since last build") > 0);
			Assert.IsTrue(message.EndsWith("</html>"));
		}

		[Test]
		public void IfThereIsAnExceptionBuildMessageShouldPublishExceptionMessage()
		{
			DynamicMock mock = new DynamicMock(typeof(IMessageBuilder));
			mock.ExpectAndThrow("BuildMessage", new Exception("oops"), new IsAnything());
			publisher = new EmailPublisher((IMessageBuilder) mock.MockInstance);
			string message = publisher.CreateMessage(new IntegrationResult());
			AssertContains("oops", message);
		}

		[Test]
		public void Publish()
		{
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();
			publisher.Run(result);
			Assert.AreEqual("mock.gateway.org", gateway.MailHost);
			Assert.AreEqual(1, gateway.SentMessages.Count);
		}

		[Test]
		public void UnitTestResultsShouldBeIncludedInEmailMessageWhenIncludesDetailsIsTrue()
		{
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();
			string results = "<test-results name=\"foo\" total=\"10\" failures=\"0\" not-run=\"0\"><test-suite></test-suite></test-results>";
			result.AddTaskResult(results);
			publisher.IncludeDetails = true;
			string message = publisher.CreateMessage(result);
			Assert.IsTrue(message.IndexOf("Tests run") >= 0);
		}

		[Test]
		public void Publish_UnknownIntegrationStatus()
		{
			publisher.Run(new IntegrationResult());
			Assert.AreEqual(0, gateway.SentMessages.Count);
			// verify that no messages are sent if there were no modifications
		}

		private Modification[] CreateModifications()
		{
			// use one from 'change' group and one from 'always' group to test non-duplication
			Modification[] modifications = new Modification[2];
			Modification mod0 = new Modification();
			mod0.UserName = "buildmaster";
			mod0.ModifiedTime = new DateTime(1974, 12, 24, 2, 30, 00);
			modifications[0] = mod0;

			Modification mod1 = new Modification();
			mod1.UserName = "orogers";
			mod1.ModifiedTime = new DateTime(1973, 12, 24, 2, 30, 00);
			modifications[1] = mod1;

			return modifications;
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			publisher = EmailPublisherMother.Create();

			Assert.AreEqual("smtp.telus.net", publisher.MailHost);
			Assert.AreEqual("mailuser", publisher.MailhostUsername);
			Assert.AreEqual("mailpassword", publisher.MailhostPassword);
			Assert.AreEqual("ccnet@thoughtworks.com", publisher.FromAddress);

			Assert.AreEqual(5, publisher.EmailUsers.Count);
			ArrayList expected = new ArrayList();
			expected.Add(new EmailUser("buildmaster", "buildmaster", "servid@telus.net"));
			expected.Add(new EmailUser("orogers", "developers", "orogers@thoughtworks.com"));
			expected.Add(new EmailUser("manders", "developers", "mandersen@thoughtworks.com"));
			expected.Add(new EmailUser("dmercier", "developers", "dmercier@thoughtworks.com"));
			expected.Add(new EmailUser("rwan", "developers", "rwan@thoughtworks.com"));
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.IsTrue(publisher.EmailUsers.ContainsValue(expected[i]));
			}

			Assert.AreEqual(2, publisher.EmailGroups.Count);
			EmailGroup developers = new EmailGroup("developers", EmailGroup.NotificationType.Change);
			EmailGroup buildmaster = new EmailGroup("buildmaster", EmailGroup.NotificationType.Always);
			Assert.AreEqual(developers, publisher.EmailGroups["developers"]);
			Assert.AreEqual(buildmaster, publisher.EmailGroups["buildmaster"]);
		}

		[Test]
		public void SerializeToXml()
		{
			publisher = EmailPublisherMother.Create();
			string xml = NetReflector.Write(publisher);
			XmlUtil.VerifyXmlIsWellFormed(xml);
		}

		[Test]
		public void VerifyEmailSubjectAndMessageForExceptionIntegrationResult()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Exception, IntegrationStatus.Unknown);
			result.ExceptionResult = new CruiseControlException("test exception");

			Assert.IsTrue(publisher.CreateMessage(result).StartsWith("CruiseControl.NET Build Results for project Project#9"));

			publisher.IncludeDetails = true;
			string actual = publisher.CreateMessage(result);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.Message) > 0);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.GetType().Name) > 0);
			Assert.IsTrue(actual.IndexOf("BUILD COMPLETE") == -1); // verify build complete message is not output
		}
	}
}