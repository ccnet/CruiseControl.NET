using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	/// <summary>
	/// Tests the messages required by email publisher
	/// </summary>
	[TestFixture]
	public class EmailMessageTest : CustomAssertion
	{
		[Test]
		public void CreateRecipientListForFixedBuild()
		{
			string expected = "dmercier@thoughtworks.com, mandersen@thoughtworks.com, orogers@thoughtworks.com, rwan@thoughtworks.com, servid@telus.net";
			string actual = new EmailMessage(IntegrationResultMother.CreateFixed(),
			                                 EmailPublisherMother.Create()).Recipients;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateRecipientListForBuildStillSuccessful()
		{
			string expected = "orogers@thoughtworks.com, servid@telus.net";
			IntegrationResult integrationResult = IntegrationResultMother.CreateStillSuccessful();
			integrationResult.Modifications = CreateModifications();
			string actual = new EmailMessage(integrationResult, EmailPublisherMother.Create()).Recipients;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateRecipientListWithNoRecipients()
		{
			string expected = String.Empty;
			EmailPublisher publisher = EmailPublisherMother.Create();
			publisher.EmailUsers.Clear();
			EmailMessage emailMessage = new EmailMessage(IntegrationResultMother.CreateFixed(), publisher);
			string actual = emailMessage.Recipients;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateModifiersList()
		{
			Modification[] modifications = CreateModifications();
			string[] modifiers = GetEmailMessage(modifications).Modifiers;
			Assert.AreEqual(2, modifications.Length);
			Assert.AreEqual("servid@telus.net", modifiers[0]);
			Assert.AreEqual("orogers@thoughtworks.com", modifiers[1]);
		}

		[Test]
		public void CreateModifiersListForUnknownUser()
		{
			Modification[] modifications = new Modification[1] {ModificationMother.CreateModification("nosuchuser", DateTime.Now)};
			string[] modifiers = GetEmailMessage(modifications).Modifiers;
			Assert.AreEqual(0, modifiers.Length);
		}

		[Test]
		public void CreateModifiersListWithUnspecifiedUser()
		{
			Modification[] modifications = new Modification[1] {ModificationMother.CreateModification(null, DateTime.Now)};
			string[] modifiers = GetEmailMessage(modifications).Modifiers;
			Assert.AreEqual(0, modifiers.Length);
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
		
/*
		[Test]
		public void CreateNotifyList()
		{
			string[] always = _publisher.CreateNotifyList(EmailGroup.NotificationType.Always);
			Assert.AreEqual(1, always.Length);
			Assert.AreEqual("servid@telus.net", always[0]);

			string[] change = _publisher.CreateNotifyList(EmailGroup.NotificationType.Change);
			Assert.AreEqual(4, change.Length);
		}
*/

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

		private EmailMessage GetEmailMessage(Modification[] modifications)
		{
			IntegrationResult integrationResult = IntegrationResultMother.CreateSuccessful(modifications);
			return new EmailMessage(integrationResult, EmailPublisherMother.Create());
		}

		private Modification[] CreateModifications()
		{
			return new Modification[]
				{
					ModificationMother.CreateModification("buildmaster", new DateTime(2004, 1, 1, 10, 0, 0)),
					ModificationMother.CreateModification("orogers", new DateTime(2004, 1, 1, 10, 0, 0))
				};
		}
	}
}