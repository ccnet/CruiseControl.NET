using System;
using System.Collections;
using System.Web.Mail;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Publishers;
using NMock;

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
			_publisher.PublishIntegrationResults(null, IntegrationResultMother.CreateStillSuccessful());
			Assert.AreEqual(0, _gateway.SentMessages.Count);
		}

		[Test]
		public void ShouldSendMessageIfRecipientIsNotSpecifiedAndBuildFailed()
		{
			_publisher = new EmailPublisher();
			_publisher.EmailGateway = _gateway;
			_publisher.EmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
			_publisher.EmailGroups.Add("foo", new EmailGroup("foo", EmailGroup.NotificationType.Change));
			_publisher.PublishIntegrationResults(null, IntegrationResultMother.CreateFailed());
			Assert.AreEqual(1, _gateway.SentMessages.Count);
		}

		[Test]
		public void EmailSubject()
		{
			string subject = _publisher.CreateSubject(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assert.AreEqual("Project#9 Build Successful: Build 0", subject);
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
		public void EmailSubjectFailedBuild()
		{
			string subject = _publisher.CreateSubject(CreateIntegrationResult(IntegrationStatus.Failure, IntegrationStatus.Success));
			Assert.AreEqual("Project#9 Build Failed", subject);
		}

		[Test]
		public void EmailSubjectFixedBuild()
		{
			string subject = _publisher.CreateSubject(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Failure));
			Assert.AreEqual("Project#9 Build Fixed: Build 0", subject);
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
		public void CreateRecipientList_BuildStateChanged()
		{
			string expected = "dmercier@thoughtworks.com, mandersen@thoughtworks.com, orogers@thoughtworks.com, rwan@thoughtworks.com, servid@telus.net";
			string actual = _publisher.CreateRecipientList(CreateIntegrationResult(IntegrationStatus.Failure, IntegrationStatus.Success));
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateRecipientList_BuildStateNotChanged()
		{
			string expected = "orogers@thoughtworks.com, servid@telus.net";
			string actual = _publisher.CreateRecipientList(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateRecipientList_NoRecipients()
		{
			_publisher.EmailUsers.Clear();
			IntegrationResult IntegrationResult = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Failure);

			string expected = String.Empty;
			string actual = _publisher.CreateRecipientList(IntegrationResult);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateModifiersList()
		{
			Modification[] modifications = CreateModifications();
			string[] modifiers = _publisher.CreateModifiersList(modifications);
			Assert.AreEqual(2, modifications.Length);
			Assert.AreEqual("servid@telus.net", modifiers[0]);
			Assert.AreEqual("orogers@thoughtworks.com", modifiers[1]);
		}

		[Test]
		public void CreateModifiersList_unknownUser()
		{
			Modification[] modifications = new Modification[1];
			modifications[0] = new Modification();
			modifications[0].UserName = "nosuchuser";

			string[] modifiers = _publisher.CreateModifiersList(modifications);
			Assert.AreEqual(0, modifiers.Length);
		}

		[Test]
		public void CreateModifiersListWithUnspecifiedUser()
		{
			Modification[] modifications = new Modification[1];
			modifications[0] = new Modification();
			modifications[0].UserName = null;

			string[] modifiers = _publisher.CreateModifiersList(modifications);
			Assert.AreEqual(0, modifiers.Length);
		}

		[Test]
		public void CreateNotifyList()
		{
			string[] always = _publisher.CreateNotifyList(EmailGroup.NotificationType.Always);
			Assert.AreEqual(1, always.Length);
			Assert.AreEqual("servid@telus.net", always[0]);

			string[] change = _publisher.CreateNotifyList(EmailGroup.NotificationType.Change);
			Assert.AreEqual(4, change.Length);
		}

		[Test]
		public void Publish()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			_publisher.PublishIntegrationResults(null, result);
			Assert.AreEqual("mock.gateway.org", _gateway.MailHost);
			Assert.AreEqual(1, _gateway.SentMessages.Count);
		}

		[Test]
		public void UnitTestResultsShouldBeIncludedInEmailMessageWhenIncludesDetailsIsTrue()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			IMock mockTaskResult = new DynamicMock(typeof(ITaskResult));
			mockTaskResult.SetupResult("Data", "<test-results name=\"foo\" total=\"10\" failures=\"0\" not-run=\"0\"><test-suite></test-suite></test-results>");
			result.TaskResults.Add((ITaskResult) mockTaskResult.MockInstance);
			_publisher.IncludeDetails = true;
			string message = _publisher.CreateMessage(result);
			Assert.IsTrue(message.IndexOf("Tests run") >= 0);
			mockTaskResult.Verify();
		}

		[Test]
		public void Publish_UnknownIntegrationStatus()
		{
			_publisher.PublishIntegrationResults(null, new IntegrationResult());
			Assert.AreEqual(0, _gateway.SentMessages.Count);
			// verify that no messages are sent if there were no modifications
		}

		[Test]
		public void HandleIntegrationEvent()
		{
			IntegrationCompletedEventHandler handler = _publisher.IntegrationCompletedEventHandler;
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
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

			Assert.AreEqual("Project#9 Build Failed", _publisher.CreateSubject(result));
			Assert.IsTrue(_publisher.CreateMessage(result).StartsWith("CruiseControl.NET Build Results for project Project#9"));

			_publisher.IncludeDetails = true;
			string actual = _publisher.CreateMessage(result);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.Message) > 0);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.GetType().Name) > 0);
			Assert.IsTrue(actual.IndexOf("BUILD COMPLETE") == -1); // verify build complete message is not output
		}
	}
}