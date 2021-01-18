using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Xml;
using Exortech.NetReflector;
using Moq;
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
		private Mock<EmailGateway> mockGateway;

		[SetUp]
		public void SetUp()
		{
			publisher = EmailPublisherMother.Create();
			mockGateway = new Mock<EmailGateway>();
			publisher.EmailGateway = (EmailGateway) mockGateway.Object;
		}

		[Test]
		public void SendMessage()
		{
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual("from@foo.com", message.From.Address);
                    Assert.AreEqual("to@bar.com", message.To[0].Address);
                    Assert.AreEqual("replyto@bar.com", message.ReplyTo.Address);
                    Assert.AreEqual("test subject", message.Subject);
                    Assert.AreEqual("test message", message.Body);
                }).Verifiable();

			publisher.SendMessage("from@foo.com", "to@bar.com", "replyto@bar.com", "test subject", "test message", "workingDir");
            mockGateway.Verify();
		}

	    [Test]
		public void ShouldNotSendMessageIfRecipientIsNotSpecifiedAndBuildIsSuccessful()
		{
			publisher = new EmailPublisher();
			publisher.EmailGateway = (EmailGateway) mockGateway.Object;
            publisher.IndexedEmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
            publisher.IndexedEmailGroups.Add("foo", new EmailGroup("foo", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change }));
			publisher.Run(IntegrationResultMother.CreateStillSuccessful());
            mockGateway.Verify();
            mockGateway.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldSendMessageIfRecipientIsNotSpecifiedAndBuildFailed()
		{
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(1, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
		    publisher.FromAddress = "from@foo.com";
			publisher.EmailGateway = (EmailGateway) mockGateway.Object;
            publisher.IndexedEmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
            publisher.IndexedEmailGroups.Add("foo", new EmailGroup("foo", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change }));
			publisher.Run(IntegrationResultMother.CreateFailed());
            mockGateway.Verify();
		}

	    [Test]
		public void ShouldSendMessageIfBuildFailed()
		{
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(1, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
			publisher.EmailGateway = (EmailGateway) mockGateway.Object;
            publisher.IndexedEmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
            publisher.IndexedEmailGroups.Add("foo", new EmailGroup("foo", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Failed }));
			publisher.Run(IntegrationResultMother.CreateFailed() );
            mockGateway.Verify();
        }


        [Test]
		public void ShouldSendMessageIfBuildFailedAndPreviousFailed()
		{
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(1, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;

            publisher.IndexedEmailUsers.Add("dev", new EmailUser("dev", "changing", "dev@foo.com"));
            publisher.IndexedEmailUsers.Add("admin", new EmailUser("admin", "failing", "bar@foo.com"));

            publisher.IndexedEmailGroups.Add("changing", new EmailGroup("changing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change }));
            publisher.IndexedEmailGroups.Add("failing", new EmailGroup("failing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Failed }));

			publisher.Run(IntegrationResultMother.CreateFailed(IntegrationStatus.Failure) );
            mockGateway.Verify();
		}

		[Test]
		public void ShouldSendMessageIfBuildFailedAndPreviousOK()
		{
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(2, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;

            publisher.IndexedEmailUsers.Add("dev", new EmailUser("dev", "changing", "dev@foo.com"));
            publisher.IndexedEmailUsers.Add("admin", new EmailUser("admin", "failing", "bar@foo.com"));

            publisher.IndexedEmailGroups.Add("changing", new EmailGroup("changing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change }));
            publisher.IndexedEmailGroups.Add("failing", new EmailGroup("failing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Failed }));

			publisher.Run(IntegrationResultMother.CreateFailed(IntegrationStatus.Success) );
            mockGateway.Verify();
        }

        [Test]
        public void ShouldSendMessageIfBuildSuccessful()
        {
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(1, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;
            publisher.IndexedEmailUsers.Add("bar", new EmailUser("bar", "foo", "bar@foo.com"));
            publisher.IndexedEmailGroups.Add("foo", new EmailGroup("foo", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Success }));
            publisher.Run(IntegrationResultMother.CreateSuccessful());
            mockGateway.Verify();
        }

        [Test]
        public void ShouldSendMessageIfBuildSuccessfulAndPreviousFailed()
        {
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(2, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;

            publisher.IndexedEmailUsers.Add("dev", new EmailUser("dev", "changing", "dev@foo.com"));
            publisher.IndexedEmailUsers.Add("admin", new EmailUser("admin", "succeeding", "bar@foo.com"));
            publisher.IndexedEmailUsers.Add("fixer", new EmailUser("fixer", "fixing", "bar@foo.com"));

            publisher.IndexedEmailGroups.Add("changing", new EmailGroup("changing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change }));
            publisher.IndexedEmailGroups.Add("succeeding", new EmailGroup("succeeding", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Success }));
            publisher.IndexedEmailGroups.Add("fixing", new EmailGroup("fixing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Fixed }));

            publisher.Run(IntegrationResultMother.CreateSuccessful(IntegrationStatus.Failure));
            mockGateway.Verify();
        }

        [Test]
        public void ShouldSendMessageIfBuildSuccessfulAndPreviousSuccessful()
        {
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(1, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;

            publisher.IndexedEmailUsers.Add("dev", new EmailUser("dev", "changing", "dev@foo.com"));
            publisher.IndexedEmailUsers.Add("admin", new EmailUser("admin", "succeeding", "bar@foo.com"));

            publisher.IndexedEmailGroups.Add("changing", new EmailGroup("changing", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change }));
            publisher.IndexedEmailGroups.Add("succeeding", new EmailGroup("succeeding", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Success }));

            publisher.Run(IntegrationResultMother.CreateSuccessful(IntegrationStatus.Success));
            mockGateway.Verify();
        }

        [Test]
        public void ShouldSendToModifiersAndFailureUsers()
        {
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(2, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;

            publisher.IndexedEmailUsers.Add("user1", new EmailUser("user1", null, "user1@foo.com"));
            publisher.IndexedEmailUsers.Add("user2", new EmailUser("user2", null, "user2@foo.com"));

            IntegrationResult result;
            Modification modification;

            result = IntegrationResultMother.CreateFailed();
            result.FailureUsers.Add("user1");

            modification = new Modification();
            modification.UserName = "user2";
            modification.ModifiedTime = new DateTime(1973, 12, 24, 2, 30, 00);
            result.Modifications = new Modification[] { modification };

            publisher.Run(result);
            mockGateway.Verify();
        }

        [Test]
        public void ShouldSendFixedMailToFailureUsersWithModificationNotificationSetToFailedAndFixed()
        {
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).
                Callback<MailMessage>(message => {
                    Assert.AreEqual(2, message.To.Count);
                }).Verifiable();

            publisher = new EmailPublisher();
            publisher.FromAddress = "from@foo.com";
            publisher.EmailGateway = (EmailGateway)mockGateway.Object;
            publisher.ModifierNotificationTypes = new EmailGroup.NotificationType[2];
            publisher.ModifierNotificationTypes[0] = EmailGroup.NotificationType.Failed;
            publisher.ModifierNotificationTypes[1] = EmailGroup.NotificationType.Fixed;

            publisher.IndexedEmailUsers.Add("user1", new EmailUser("user1", null, "user1@foo.com"));
            publisher.IndexedEmailUsers.Add("user2", new EmailUser("user2", null, "user2@foo.com"));

            IntegrationResult result;
            Modification modification;

            result = IntegrationResultMother.CreateFixed();
            result.FailureUsers.Add("user1");

            modification = new Modification();
            modification.UserName = "user2";
            modification.ModifiedTime = new DateTime(1973, 12, 24, 2, 30, 00);
            result.Modifications = new Modification[] { modification };

            publisher.Run(result);
            mockGateway.Verify();
        }

        private static IntegrationResult CreateIntegrationResult(IntegrationStatus current, IntegrationStatus last)
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
			Assert.IsTrue(message.StartsWith("<html>"), "message does not start with html: " + message);
			Assert.IsTrue(message.IndexOf("CruiseControl.NET Build Results for project Project#9") > 0, "message does not contain header: " + message);
			Assert.IsTrue(message.IndexOf("Modifications since last build") > 0, "message does not contain modifications: " + message);
			Assert.IsTrue(message.EndsWith("</html>"), "message does not end with html: " + message);
		}

		[Test]
		public void IfThereIsAnExceptionBuildMessageShouldPublishExceptionMessage()
		{
			var mock = new Mock<IMessageBuilder>();
			mock.Setup(builder => builder.BuildMessage(It.IsAny<IIntegrationResult>())).Throws(new Exception("oops")).Verifiable();
			publisher = new EmailPublisher((IMessageBuilder) mock.Object);
			string message = publisher.CreateMessage(new IntegrationResult());
			AssertContains("oops", message);
		}

		[Test]
		public void Publish()
		{
//            mockGateway.Expect("MailHost", "mock.gateway.org");
            mockGateway.Setup(gateway => gateway.Send(It.IsAny<MailMessage>())).Verifiable();
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();
			publisher.Run(result);
            mockGateway.Verify();
		}

		[Test]
		public void UnitTestResultsShouldBeIncludedInEmailMessageWhenIncludesDetailsIsTrue()
		{
			IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();

            System.IO.StreamReader reader = System.IO.File.OpenText("UnitTestResults.xml");
            string results = reader.ReadToEnd();
            reader.Close();

			result.AddTaskResult(results);
			publisher.IncludeDetails = true;

			string message = publisher.CreateMessage(result);
			Assert.IsTrue(message.IndexOf("Tests run") >= 0, "message does not contain 'Tests run': " + message);
		}

        [Test]
        public void UnitTestResultsShouldNotBeIncludedInEmailMessageWhenIncludesDetailsIsTrueAndNoUnitTestXslIsDefined()
        {
            IntegrationResult result = IntegrationResultMother.CreateStillSuccessful();

            System.IO.StreamReader reader = System.IO.File.OpenText("UnitTestResults.xml");
            string results = reader.ReadToEnd();
            reader.Close();

            result.AddTaskResult(results);
            publisher.IncludeDetails = true;

            string[] xslFiles = { @"xsl\NCover.xsl" };

            publisher.XslFiles = xslFiles;

            string message = publisher.CreateMessage(result);
            Assert.IsFalse(message.IndexOf("Tests run") >= 0);
        }



		[Test]
		public void Publish_UnknownIntegrationStatus()
		{
			publisher.Run(new IntegrationResult());
			// verify that no messages are sent if there were no modifications
            mockGateway.Verify();
            mockGateway.VerifyNoOtherCalls();
		}

	    [Test]
		public void PopulateFromConfiguration()
		{
			publisher = EmailPublisherMother.Create();

			Assert.AreEqual("smtp.telus.net", publisher.MailHost);
            Assert.AreEqual(26, publisher.MailPort);
			Assert.AreEqual("mailuser", publisher.MailhostUsername);
            Assert.IsNotNull(publisher.MailhostPassword);
			Assert.AreEqual("mailpassword", publisher.MailhostPassword.PrivateValue);
			Assert.AreEqual("ccnet@thoughtworks.com", publisher.FromAddress);
            Assert.AreEqual(2, publisher.ModifierNotificationTypes.Length);
            Assert.AreEqual(EmailGroup.NotificationType.Failed, publisher.ModifierNotificationTypes[0]);
            Assert.AreEqual(EmailGroup.NotificationType.Fixed, publisher.ModifierNotificationTypes[1]);
            
            Assert.AreEqual(1, publisher.Converters.Length);
            Assert.AreEqual("$", ((EmailRegexConverter) publisher.Converters[0]).Find);
            Assert.AreEqual("@TheCompany.com", ((EmailRegexConverter) publisher.Converters[0]).Replace);

            Assert.AreEqual(6, publisher.IndexedEmailUsers.Count);
			var expected = new List<EmailUser>();
			expected.Add(new EmailUser("buildmaster", "buildmaster", "servid@telus.net"));
			expected.Add(new EmailUser("orogers", "developers", "orogers@thoughtworks.com"));
			expected.Add(new EmailUser("manders", "developers", "mandersen@thoughtworks.com"));
			expected.Add(new EmailUser("dmercier", "developers", "dmercier@thoughtworks.com"));
			expected.Add(new EmailUser("rwan", "developers", "rwan@thoughtworks.com"));
            expected.Add(new EmailUser("owjones", "successdudes", "oliver.wendell.jones@example.com"));
			for (int i = 0; i < expected.Count; i++)
			{
                Assert.IsTrue(publisher.IndexedEmailUsers.ContainsValue(expected[i]));
			}

            Assert.AreEqual(3, publisher.IndexedEmailGroups.Count);
            EmailGroup developers = new EmailGroup("developers", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Change });
            EmailGroup buildmaster = new EmailGroup("buildmaster", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Always });
            EmailGroup successdudes = new EmailGroup("successdudes", new EmailGroup.NotificationType[] { EmailGroup.NotificationType.Success });
			Assert.AreEqual(developers, publisher.IndexedEmailGroups["developers"]);
            Assert.AreEqual(buildmaster, publisher.IndexedEmailGroups["buildmaster"]);
            Assert.AreEqual(successdudes, publisher.IndexedEmailGroups["successdudes"]);
		}

        [Test]
        public void ShouldPopulateFromMinimalConfiguration()
        {
            string configXml = @"<email from=""ccnet@example.com"" mailhost=""smtp.example.com""> <users/> <groups/> </email>";
            XmlDocument configXmlDocument = XmlUtil.CreateDocument(configXml);
            publisher = EmailPublisherMother.Create(configXmlDocument.DocumentElement);

            Assert.AreEqual("smtp.example.com", publisher.MailHost);
            Assert.AreEqual(25, publisher.MailPort);
            Assert.AreEqual(null, publisher.MailhostUsername);
            Assert.AreEqual(null, publisher.MailhostPassword);
            Assert.AreEqual(null, publisher.ReplyToAddress);
            Assert.AreEqual(false, publisher.IncludeDetails);
            Assert.AreEqual("ccnet@example.com", publisher.FromAddress);
            Assert.AreEqual(1, publisher.ModifierNotificationTypes.Length);
            Assert.AreEqual(EmailGroup.NotificationType.Always, publisher.ModifierNotificationTypes[0]);

            Assert.AreEqual(0, publisher.Converters.Length);
            Assert.AreEqual(0, publisher.IndexedEmailUsers.Count);
            Assert.AreEqual(0, publisher.IndexedEmailGroups.Count);
        }



        [Test]
        public void ShouldPopulateXslFiles()
        {
            string configXml = @"<email from=""ccnet@example.com"" mailhost=""smtp.example.com""> <users/> <groups/>
					  <xslFiles> 
                        <file>xsl\NCover.xsl</file>
					   <file>xsl\NCoverExplorer.xsl</file>
					</xslFiles>
                </email>";
            XmlDocument configXmlDocument = XmlUtil.CreateDocument(configXml);
            publisher = EmailPublisherMother.Create(configXmlDocument.DocumentElement);

            Assert.AreEqual(2, publisher.XslFiles.Length);
            Assert.AreEqual(@"xsl\NCover.xsl", publisher.XslFiles[0]);
            Assert.AreEqual(@"xsl\NCoverExplorer.xsl", publisher.XslFiles[1]);
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

			string message = publisher.CreateMessage(result);
            
            Assert.IsTrue(message.StartsWith("CruiseControl.NET Build Results for project Project#9"));

			publisher.IncludeDetails = true;
			string actual = publisher.CreateMessage(result);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.Message) > 0, "message does not contain exception result message: " + actual);
			Assert.IsTrue(actual.IndexOf(result.ExceptionResult.GetType().Name) > 0, "message does not contain exception result type name: " + actual);
			Assert.IsTrue(actual.IndexOf("BUILD COMPLETE") == -1, "message does not contain 'BUILD COMPLETE': " + actual); // verify build complete message is not output
		}
	}
}