using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	/// <summary>
	/// Tests the messages required by email publisher
	/// </summary>
	[TestFixture]
	public class EmailMessageTest : CustomAssertion
	{
		private static readonly EmailGroup alwaysGroup = new EmailGroup("alwaysGroup", EmailGroup.NotificationType.Always);
		private static readonly EmailGroup failedGroup = new EmailGroup("failedGroup", EmailGroup.NotificationType.Failed);
		private static readonly EmailGroup changedGroup = new EmailGroup("changedGroup", EmailGroup.NotificationType.Change);
        private static readonly EmailGroup successGroup = new EmailGroup("successGroup", EmailGroup.NotificationType.Success);
        private static readonly EmailUser always = new EmailUser("always", alwaysGroup.Name, "always@thoughtworks.com");
		private static readonly EmailUser failed = new EmailUser("failed", failedGroup.Name, "failed@thoughtworks.com");
		private static readonly EmailUser changed = new EmailUser("changed", changedGroup.Name, "changed@thoughtworks.com");
        private static readonly EmailUser success = new EmailUser("success", successGroup.Name, "success@thoughtworks.com");
        private static readonly EmailUser modifier = new EmailUser("modifier", changedGroup.Name, "modifier@thoughtworks.com");

		private EmailPublisher publisher;

		[SetUp]
		protected void CreatePublisher()
		{
			publisher = new EmailPublisher();
			publisher.EmailGroups.Add(alwaysGroup.Name, alwaysGroup);
			publisher.EmailGroups.Add(changedGroup.Name, changedGroup);
			publisher.EmailGroups.Add(failedGroup.Name, failedGroup);
            publisher.EmailGroups.Add(successGroup.Name, successGroup);
            publisher.EmailUsers.Add(always.Name, always);
			publisher.EmailUsers.Add(failed.Name, failed);
			publisher.EmailUsers.Add(changed.Name, changed);
            publisher.EmailUsers.Add(success.Name, success);
            publisher.EmailUsers.Add(modifier.Name, modifier);
		}

		[Test]
		public void VerifyRecipientListForFixedBuild()
		{
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateFixed());
			Assert.AreEqual(ExpectedRecipients(always, changed, modifier, success), new EmailMessage(result, publisher).Recipients);
		}

		[Test]
		public void VerifyRecipientListForFailedBuild()
		{
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateFailed());
            Assert.AreEqual(ExpectedRecipients(always, changed, failed, modifier), new EmailMessage(result, publisher).Recipients);
		}

        [Test]
		public void VerifyRecipientListForStillFailingBuild()
		{
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateStillFailing());
			Assert.AreEqual(ExpectedRecipients(always, failed, modifier), new EmailMessage(result, publisher).Recipients);
		}

        [Test]
        public void VerifyRecipientListForExceptionBuild()
        {
            IIntegrationResult result = AddModification(IntegrationResultMother.CreateExceptioned());
            Assert.AreEqual(ExpectedRecipients(always, changed, failed, modifier), new EmailMessage(result, publisher).Recipients);
        }

        [Test]
        public void VerifyRecipientListForStillExceptionBuild()
        {
            IIntegrationResult result = AddModification(IntegrationResultMother.CreateStillFailing());
            Assert.AreEqual(ExpectedRecipients(always, failed, modifier), new EmailMessage(result, publisher).Recipients);
        }

        [Test]
        public void VerifyRecipientListForSuccessfulBuild()
        {
            IIntegrationResult result = AddModification(IntegrationResultMother.CreateSuccessful());
            Assert.AreEqual(ExpectedRecipients(always, modifier, success), new EmailMessage(result, publisher).Recipients);
        }

        [Test]
        public void VerifyRecipientListForStillSuccessfulBuild()
        {
            IIntegrationResult result = AddModification(IntegrationResultMother.CreateStillSuccessful());
            Assert.AreEqual(ExpectedRecipients(always, modifier, success), new EmailMessage(result, publisher).Recipients);
        }

        [Test]
		public void CreateRecipientListWithNoRecipients()
		{
			EmailMessage emailMessage = new EmailMessage(IntegrationResultMother.CreateFixed(), new EmailPublisher());
			Assert.AreEqual(string.Empty, emailMessage.Recipients);
		}

		[Test]
		public void CreateModifiersListForUnknownUser()
		{
			publisher.EmailUsers.Remove(modifier.Name);
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateStillSuccessful());
			Assert.AreEqual(ExpectedRecipients(always, success), new EmailMessage(result, publisher).Recipients);
		}

		[Test]
		public void CreateModifiersListWithUnspecifiedUser()
		{
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateStillSuccessful());
			result.Modifications[0].UserName = null;
			Assert.AreEqual(ExpectedRecipients(always, success), new EmailMessage(result, publisher).Recipients);
		}

		[Test]
		public void EmailSubject()
		{
			string subject = GetEmailMessage(IntegrationResultMother.CreateStillSuccessful(), true).Subject;
			Assert.AreEqual("Project#9 Build Successful: Build 0", subject);
		}

		[Test]
		public void EmailSubjectForFailedBuild()
		{
			string subject = GetEmailMessage(IntegrationResultMother.CreateFailed(), true).Subject;
			Assert.AreEqual("Project#9 Build Failed", subject);
		}

        [Test]
        public void EmailSubjectForSuccessfulBuild()
        {
            string subject = GetEmailMessage(IntegrationResultMother.CreateSuccessful(), true).Subject;
            Assert.AreEqual("Project#9 Build Successful: Build 0", subject);
        }

        [Test]
		public void EmailSubjectForFixedBuild()
		{
			string subject = GetEmailMessage(IntegrationResultMother.CreateFixed(), true).Subject;
			Assert.AreEqual("Project#9 Build Fixed: Build 0", subject);
		}

		[Test]
		public void EmailSubjectForExceptionedBuild()
		{
			string subject = GetEmailMessage(IntegrationResultMother.CreateExceptioned(), true).Subject;
			Assert.AreEqual("Project#9 Build Failed", subject);
		}

		[Test]
		public void OnlyEmailModifierRecipientsOnBuildFailure()
		{
			publisher = new EmailPublisher();
			publisher.EmailUsers.Add(modifier.Name, modifier);
			publisher.EmailUsers.Add(changed.Name, changed);
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateFailed());
			Assert.AreEqual(ExpectedRecipients(modifier), new EmailMessage(result, publisher).Recipients);
		}

		[Test]
		public void HandleEmailUserWithUnspecifiedGroup()
		{
			publisher = new EmailPublisher();
			publisher.EmailUsers.Add(modifier.Name, modifier);
			publisher.EmailUsers.Add("foo", new EmailUser("foo", null, "x@x.com"));
			IIntegrationResult result = AddModification(IntegrationResultMother.CreateFailed());
			Assert.AreEqual(ExpectedRecipients(modifier), new EmailMessage(result, publisher).Recipients);
		}

		private static EmailMessage GetEmailMessage(IntegrationResult result, bool includeDetails)
		{
			EmailPublisher publisher = EmailPublisherMother.Create();
			publisher.IncludeDetails = includeDetails;
			return new EmailMessage(DecorateIntegrationResult(result), publisher);
		}

		private static IntegrationResult DecorateIntegrationResult(IntegrationResult result)
		{
			result.ProjectName = "Project#9";
			result.Label = "0";
			return result;
		}

		private static IIntegrationResult AddModification(IIntegrationResult result)
		{
			Modification mod = new Modification();
			mod.UserName = modifier.Name;
			result.Modifications = new Modification[1] {mod};
			return result;
		}

		private static string ExpectedRecipients(params EmailUser[] users)
		{
			StringBuilder builder = new StringBuilder();
			foreach (EmailUser user in users)
			{
				if (builder.Length > 0) builder.Append(", ");
				builder.Append(user.Address);
			}
			return builder.ToString();
		}

        /// <summary>
        /// Verify that EmailMessage runs the specified username-to-address converters.
        /// </summary>
        [Test]
        public void ShouldConvertUsernamesToEmailAddresses()
        {
            EmailPublisher myPublisher = new EmailPublisher();

            myPublisher.Converters = new IEmailConverter[1];
            myPublisher.Converters[0] = new EmailRegexConverter("^([^@]*)$", @"$1@example.com");

            IIntegrationResult result = IntegrationResultMother.CreateFailed();
            Modification modification = new Modification();
            modification.UserName = "username";
            result.Modifications = new Modification[1] { modification };

            EmailMessage message = new EmailMessage(result, myPublisher);
            Assert.AreEqual("username@example.com", message.Recipients);

        }

	}
}