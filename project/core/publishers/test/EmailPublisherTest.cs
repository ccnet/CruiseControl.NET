using System;
using System.Collections;
using System.Web.Mail;
using System.Xml;
using NUnit.Framework;
using tw.ccnet.core.test;
using tw.ccnet.core.util;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core.publishers.test
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

		public void TestSendMessage()
		{
			_publisher.SendMessage("from@foo.com", "to@bar.com", "test subject", "test message");
			Assertion.AssertEquals(1, _gateway.SentMessages.Count);

			MailMessage message = (MailMessage)_gateway.SentMessages[0];
			Assertion.AssertEquals("from@foo.com", message.From);
			Assertion.AssertEquals("to@bar.com", message.To);
			Assertion.AssertEquals("test subject", message.Subject);
			Assertion.AssertEquals("test message", message.Body);
		}

		public void TestEmailSubject()
		{
			string subject = _publisher.CreateSubject(
				CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assertion.AssertEquals("Project#9 Build Successful: Build 0", subject);
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

		public void TestEmailSubjectFailedBuild()
		{
			string subject = _publisher.CreateSubject(
				CreateIntegrationResult(IntegrationStatus.Failure, IntegrationStatus.Success));
			Assertion.AssertEquals("Project#9 Build Failed", subject);
		}

		public void TestEmailSubjectFixedBuild()
		{
			string subject = _publisher.CreateSubject(
				CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Failure));
			Assertion.AssertEquals("Project#9 Build Fixed: Build 0", subject);
		}

		public void TestEmailMessageWithoutDetails()
		{
			_publisher.IncludeDetails = false;
			string message = _publisher.CreateMessage(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assertion.AssertEquals(@"CC.NET Build Results for Project#9: http://localhost/ccnet?log=log19741224023000Lbuild.0.xml", message);
		}
		
		public void TestEmailMessageWithDetails() {
			_publisher.IncludeDetails = true;
			string message = _publisher.CreateMessage(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assertion.Assert(message.StartsWith("<html>"));
			Assertion.Assert(message.EndsWith("</html>"));
		}		

		public void TestCreateRecipientList_BuildStateChanged()
		{
			string expected = "dmercier@thoughtworks.com, mandersen@thoughtworks.com, orogers@thoughtworks.com, rwan@thoughtworks.com, servid@telus.net";
			string actual = _publisher.CreateRecipientList(CreateIntegrationResult(IntegrationStatus.Failure, IntegrationStatus.Success));
			Assertion.AssertEquals(expected, actual);
		}

		public void TestCreateRecipientList_BuildStateNotChanged()
		{
			string expected = "orogers@thoughtworks.com, servid@telus.net";
			string actual = _publisher.CreateRecipientList(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assertion.AssertEquals(expected, actual);
		}

		public void TestCreateRecipientList_NoRecipients()
		{
			_publisher.EmailUsers.Clear();
//			Modification[] modifications = new Modification[] {};
			IntegrationResult IntegrationResult = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Failure);

			string expected = String.Empty;
			string actual = _publisher.CreateRecipientList(IntegrationResult);
			Assertion.AssertEquals(expected, actual);
		}

		public void TestCreateModifiersList()
		{			
			Modification[] modifications = CreateModifications();
			string[] modifiers = _publisher.CreateModifiersList(modifications);
			Assertion.AssertEquals("expected 2 modifiers", 2, modifications.Length);
			Assertion.AssertEquals("servid@telus.net", modifiers[0]);
			Assertion.AssertEquals("orogers@thoughtworks.com", modifiers[1]);
		}

		public void TestCreateModifiersList_unknownUser()
		{
			Modification[] modifications = new Modification[1];
			modifications[0] = new Modification();
			modifications[0].UserName = "nosuchuser";

			string[] modifiers = _publisher.CreateModifiersList(modifications);
			Assertion.AssertEquals("expected 0 modifier", 0, modifiers.Length);
		}

		public void TestCreateNotifyList()
		{			
			string[] always = _publisher.CreateNotifyList(EmailGroup.NotificationType.Always);
			Assertion.AssertEquals(1, always.Length);
			Assertion.AssertEquals("servid@telus.net", always[0]);

			string[] change = _publisher.CreateNotifyList(EmailGroup.NotificationType.Change);
			Assertion.AssertEquals(4, change.Length);
		}

		public void TestPublish()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			_publisher.Publish(null, result);
			Assertion.AssertEquals("mock.gateway.org", _gateway.MailHost);
			Assertion.AssertEquals(1, _gateway.SentMessages.Count);
		}

		public void TestPublish_noModifications()
		{
			_publisher.Publish(null, new IntegrationResult());
			Assertion.AssertEquals(1, _gateway.SentMessages.Count);
		}

		public void TestHandleIntegrationCompletedEvent()
		{
			IntegrationCompletedEventHandler handler = _publisher.IntegrationCompletedEventHandler;
			handler(this, CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assertion.Assert("Mail message was not sent!", _gateway.SentMessages.Count > 0);
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

		public void TestPopulateFromConfiguration()
		{
			XmlPopulator populator = new XmlPopulator();
			populator.Reflector.AddReflectorTypes(System.Reflection.Assembly.GetExecutingAssembly());
			populator.Populate(EmailPublisherMother.ConfigurationXml.DocumentElement, _publisher);

			Assertion.AssertEquals("smtp.telus.net", _publisher.MailHost);
			Assertion.AssertEquals("ccnet@thoughtworks.com", _publisher.FromAddress);

			Assertion.AssertEquals(5, _publisher.EmailUsers.Count);
			ArrayList expected = new ArrayList();
			expected.Add(new EmailUser("buildmaster", "buildmaster", "servid@telus.net"));
			expected.Add(new EmailUser("orogers", "developers", "orogers@thoughtworks.com"));
			expected.Add(new EmailUser("manders", "developers", "mandersen@thoughtworks.com"));
			expected.Add(new EmailUser("dmercier", "developers", "dmercier@thoughtworks.com"));
			expected.Add(new EmailUser("rwan", "developers", "rwan@thoughtworks.com"));
			for (int i = 0; i < expected.Count; i++)
			{
				Assertion.Assert("EmailUser was not loaded from config: " + expected[i], 
					_publisher.EmailUsers.ContainsValue(expected[i]));
			}

			Assertion.AssertEquals(2, _publisher.EmailGroups.Count);
			EmailGroup developers = new EmailGroup("developers", EmailGroup.NotificationType.Change);
			EmailGroup buildmaster = new EmailGroup("buildmaster", EmailGroup.NotificationType.Always);
			Assertion.AssertEquals(developers, _publisher.EmailGroups["developers"]);
			Assertion.AssertEquals(buildmaster, _publisher.EmailGroups["buildmaster"]);
		}
	}
}