using System;
using System.Collections;
using System.Web.Mail;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class EmailPublisherTest
	{
		private EmailPublisher _publisher;
		private MockEmailGateway _gateway;

		[SetUp]
		public void SetUp()
		{
			_publisher = EmailPublisherMother.Create();
			_gateway = MockEmailGateway.Create();
			_publisher.EmailGateway = _gateway;
		}

		[Test]
		public void SendMessage()
		{
			_publisher.SendMessage("from@foo.com", "to@bar.com", "test subject", "test message");
			Assert.AreEqual(1, _gateway.SentMessages.Count);

			MailMessage message = (MailMessage) _gateway.SentMessages[0];
			Assert.AreEqual("from@foo.com", message.From);
			Assert.AreEqual("to@bar.com", message.To);
			Assert.AreEqual("test subject", message.Subject);
			Assert.AreEqual("test message", message.Body);
		}

		[Test]
		public void ShouldNotSendMessageIfRecipientIsNotSpecifiedAndBuildIsSuccessful()
		{
			_publisher = new EmailPublisher();
			_publisher.EmailGateway = _gateway;
			_publisher.EmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
			_publisher.EmailGroups.Add("foo", new EmailGroup("foo", EmailGroup.NotificationType.Change));
			_publisher.PublishIntegrationResults(IntegrationResultMother.CreateStillSuccessful());
			Assert.AreEqual(0, _gateway.SentMessages.Count);
		}

		[Test]
		public void ShouldSendMessageIfRecipientIsNotSpecifiedAndBuildFailed()
		{
			_publisher = new EmailPublisher();
			_publisher.EmailGateway = _gateway;
			_publisher.EmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
			_publisher.EmailGroups.Add("foo", new EmailGroup("foo", EmailGroup.NotificationType.Change));
			_publisher.PublishIntegrationResults(IntegrationResultMother.CreateFailed());
			Assert.AreEqual(1, _gateway.SentMessages.Count);
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus current, IntegrationStatus last)
		{
			IntegrationResult result = new IntegrationResult();
			result.ProjectName = "Project#9";
			result.Modifications = CreateModifications();
			result.Status = current;
			result.LastIntegrationStatus = last;
			result.Label = "0";
			return result;
		}

		[Test]
		public void EmailMessageWithDetails()
		{
			_publisher.IncludeDetails = true;
			string message = _publisher.CreateMessage(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assert.IsTrue(message.StartsWith("<html>"));
			Assert.IsTrue(message.IndexOf("CruiseControl.NET Build Results for project Project#9") > 0);
			Assert.IsTrue(message.IndexOf("Modifications since last build") > 0);
			Assert.IsTrue(message.EndsWith("</html>"));
		}

		[Test]
		public void Publish()
		{
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();
			_publisher.PublishIntegrationResults(result);
			Assert.AreEqual("mock.gateway.org", _gateway.MailHost);
			Assert.AreEqual(1, _gateway.SentMessages.Count);
		}

		[Test]
		public void UnitTestResultsShouldBeIncludedInEmailMessageWhenIncludesDetailsIsTrue()
		{
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();
			IMock mockTaskResult = new DynamicMock(typeof (ITaskResult));
			mockTaskResult.SetupResult("Data", "<test-results name=\"foo\" total=\"10\" failures=\"0\" not-run=\"0\"><test-suite></test-suite></test-results>");
			result.AddTaskResult((ITaskResult)mockTaskResult.MockInstance);
			_publisher.IncludeDetails = true;
			string message = _publisher.CreateMessage(result);
			Assert.IsTrue(message.IndexOf("Tests run") >= 0);
			mockTaskResult.Verify();
		}

		[Test]
		public void Publish_UnknownIntegrationStatus()
		{
			_publisher.PublishIntegrationResults(new IntegrationResult());
			Assert.AreEqual(0, _gateway.SentMessages.Count);
			// verify that no messages are sent if there were no modifications
		}

		[Test]
		public void HandleIntegrationEvent()
		{
			IntegrationCompletedEventHandler handler = _publisher.IntegrationCompletedEventHandler;
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();
			handler(null, new IntegrationCompletedEventArgs(result));
			Assert.IsTrue(_gateway.SentMessages.Count > 0, "Mail message was not sent!");
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
			_publisher = EmailPublisherMother.Create();

			Assert.AreEqual("smtp.telus.net", _publisher.MailHost);
			Assert.AreEqual("ccnet@thoughtworks.com", _publisher.FromAddress);

			Assert.AreEqual(5, _publisher.EmailUsers.Count);
			ArrayList expected = new ArrayList();
			expected.Add(new EmailUser("buildmaster", "buildmaster", "servid@telus.net"));
			expected.Add(new EmailUser("orogers", "developers", "orogers@thoughtworks.com"));
			expected.Add(new EmailUser("manders", "developers", "mandersen@thoughtworks.com"));
			expected.Add(new EmailUser("dmercier", "developers", "dmercier@thoughtworks.com"));
			expected.Add(new EmailUser("rwan", "developers", "rwan@thoughtworks.com"));
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.IsTrue(_publisher.EmailUsers.ContainsValue(expected[i]));
			}

			Assert.AreEqual(2, _publisher.EmailGroups.Count);
			EmailGroup developers = new EmailGroup("developers", EmailGroup.NotificationType.Change);
			EmailGroup buildmaster = new EmailGroup("buildmaster", EmailGroup.NotificationType.Always);
			Assert.AreEqual(developers, _publisher.EmailGroups["developers"]);
			Assert.AreEqual(buildmaster, _publisher.EmailGroups["buildmaster"]);
		}

		[Test]
		public void VerifyEmailSubjectAndMessageForExceptionIntegrationResult()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Exception, IntegrationStatus.Unknown);
			result.ExceptionResult = new CruiseControlException("test exception");

			Assert.IsTrue(_publisher.CreateMessage(result).StartsWith("CruiseControl.NET Build Results for project Project#9"));

			_publisher.IncludeDetails = true;
			string actual = _publisher.CreateMessage(result);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.Message) > 0);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.GetType().Name) > 0);
			Assert.IsTrue(actual.IndexOf("BUILD COMPLETE") == -1); // verify build complete message is not output
		}
	}
}