using System;
using System.Collections;
using System.Web.Mail;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class EmailPublisherTest : Assertion
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
			AssertEquals(1, _gateway.SentMessages.Count);

			MailMessage message = (MailMessage)_gateway.SentMessages[0];
			AssertEquals("from@foo.com", message.From);
			AssertEquals("to@bar.com", message.To);
			AssertEquals("test subject", message.Subject);
			AssertEquals("test message", message.Body);
		}

		public void TestEmailSubject()
		{
			string subject = _publisher.CreateSubject(
				CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			AssertEquals("Project#9 Build Successful: Build 0", subject);
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
			AssertEquals("Project#9 Build Failed", subject);
		}

		public void TestEmailSubjectFixedBuild()
		{
			string subject = _publisher.CreateSubject(
				CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Failure));
			AssertEquals("Project#9 Build Fixed: Build 0", subject);
		}

		public void TestEmailMessageWithoutDetails()
		{
			_publisher.IncludeDetails = false;
			string message = _publisher.CreateMessage(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			AssertEquals(@"CruiseControl.NET Build Results for project Project#9: http://localhost/ccnet?log=log19741224023000Lbuild.0.xml", message);
		}
		
		public void TestEmailMessageWithDetails() 
		{
			_publisher.IncludeDetails = true;
			string message = _publisher.CreateMessage(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			Assert(message.StartsWith("<html>"));
			Assert(message.IndexOf("CruiseControl.NET Build Results for project Project#9") > 0);
			Assert(message.IndexOf("Modifications since last build:") > 0);
			Assert(message.EndsWith("</html>"));
		}		

		public void TestCreateRecipientList_BuildStateChanged()
		{
			string expected = "dmercier@thoughtworks.com, mandersen@thoughtworks.com, orogers@thoughtworks.com, rwan@thoughtworks.com, servid@telus.net";
			string actual = _publisher.CreateRecipientList(CreateIntegrationResult(IntegrationStatus.Failure, IntegrationStatus.Success));
			AssertEquals(expected, actual);
		}

		public void TestCreateRecipientList_BuildStateNotChanged()
		{
			string expected = "orogers@thoughtworks.com, servid@telus.net";
			string actual = _publisher.CreateRecipientList(CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success));
			AssertEquals(expected, actual);
		}

		public void TestCreateRecipientList_NoRecipients()
		{
			_publisher.EmailUsers.Clear();
			IntegrationResult IntegrationResult = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Failure);

			string expected = String.Empty;
			string actual = _publisher.CreateRecipientList(IntegrationResult);
			AssertEquals(expected, actual);
		}

		public void TestCreateModifiersList()
		{			
			Modification[] modifications = CreateModifications();
			string[] modifiers = _publisher.CreateModifiersList(modifications);
			AssertEquals("expected 2 modifiers", 2, modifications.Length);
			AssertEquals("servid@telus.net", modifiers[0]);
			AssertEquals("orogers@thoughtworks.com", modifiers[1]);
		}

		public void TestCreateModifiersList_unknownUser()
		{
			Modification[] modifications = new Modification[1];
			modifications[0] = new Modification();
			modifications[0].UserName = "nosuchuser";

			string[] modifiers = _publisher.CreateModifiersList(modifications);
			AssertEquals("expected 0 modifier", 0, modifiers.Length);
		}

		public void TestCreateNotifyList()
		{			
			string[] always = _publisher.CreateNotifyList(EmailGroup.NotificationType.Always);
			AssertEquals(1, always.Length);
			AssertEquals("servid@telus.net", always[0]);

			string[] change = _publisher.CreateNotifyList(EmailGroup.NotificationType.Change);
			AssertEquals(4, change.Length);
		}

		public void TestPublish()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			_publisher.PublishIntegrationResults(null, result);
			AssertEquals("mock.gateway.org", _gateway.MailHost);
			AssertEquals(1, _gateway.SentMessages.Count);
		}

		public void TestPublish_UnknownIntegrationStatus()
		{
			_publisher.PublishIntegrationResults(null, new IntegrationResult());
			AssertEquals(0, _gateway.SentMessages.Count);
			// verify that no messages are sent if there were no modifications
		}

		[Test]
		public void TestHandleIntegrationEvent()
		{
			IntegrationCompletedEventHandler handler = _publisher.IntegrationCompletedEventHandler;
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			handler(null, new IntegrationCompletedEventArgs(result));
			Assert("Mail message was not sent!", _gateway.SentMessages.Count > 0);
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

			AssertEquals("smtp.telus.net", _publisher.MailHost);
			AssertEquals("ccnet@thoughtworks.com", _publisher.FromAddress);

			AssertEquals(5, _publisher.EmailUsers.Count);
			ArrayList expected = new ArrayList();
			expected.Add(new EmailUser("buildmaster", "buildmaster", "servid@telus.net"));
			expected.Add(new EmailUser("orogers", "developers", "orogers@thoughtworks.com"));
			expected.Add(new EmailUser("manders", "developers", "mandersen@thoughtworks.com"));
			expected.Add(new EmailUser("dmercier", "developers", "dmercier@thoughtworks.com"));
			expected.Add(new EmailUser("rwan", "developers", "rwan@thoughtworks.com"));
			for (int i = 0; i < expected.Count; i++)
			{
				Assert("EmailUser was not loaded from config: " + expected[i], 
					_publisher.EmailUsers.ContainsValue(expected[i]));
			}

			AssertEquals(2, _publisher.EmailGroups.Count);
			EmailGroup developers = new EmailGroup("developers", EmailGroup.NotificationType.Change);
			EmailGroup buildmaster = new EmailGroup("buildmaster", EmailGroup.NotificationType.Always);
			AssertEquals(developers, _publisher.EmailGroups["developers"]);
			AssertEquals(buildmaster, _publisher.EmailGroups["buildmaster"]);
		}

		[Test]
		public void VerifyEmailSubjectAndMessageForExceptionIntegrationResult()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Exception, IntegrationStatus.Unknown);
			result.ExceptionResult = new CruiseControlException("test exception");

			AssertEquals("Project#9 Build Failed", _publisher.CreateSubject(result));
			Assert(_publisher.CreateMessage(result).StartsWith("CruiseControl.NET Build Results for project Project#9"));

			_publisher.IncludeDetails = true;
			string actual = _publisher.CreateMessage(result);
			Assert(actual.IndexOf(result.ExceptionResult.Message) > 0);
			Assert(actual.IndexOf(result.ExceptionResult.GetType().Name) > 0);
			Assert(actual.IndexOf("BUILD COMPLETE") == -1);			// verify build complete message is not output
		}
	}
}