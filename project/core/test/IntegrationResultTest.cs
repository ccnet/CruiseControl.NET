using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class IntegrationResultTest
	{
		[Test]
		public void LastModificationDate()
		{
			IntegrationResult integrationResult = new IntegrationResult();
			
			Modification earlierModification = new Modification();
			earlierModification.ModifiedTime = new DateTime(0);
			
			Modification laterModification = new Modification();
			laterModification.ModifiedTime = new DateTime(1);

			integrationResult.Modifications = new Modification[] {earlierModification, laterModification};
			Assert.AreEqual(laterModification.ModifiedTime, integrationResult.LastModificationDate);
		}

		[Test]
		public void LastModificationDateWhenThereAreNoModifications()
		{
			// Project relies on this behavior, but is it really what we want?
			IntegrationResult integrationResult = new IntegrationResult();
			DateTime yesterday = DateTime.Now.AddDays(-1).Date;
			Assert.AreEqual(yesterday, integrationResult.LastModificationDate.Date);
		}	
  
		[Test]
		public void VerifyInitialIntegrationResult()
		{
			IntegrationResult initial = IntegrationResult.CreateInitialIntegrationResult("project");

			Assert.AreEqual("project", initial.ProjectName);
			Assert.AreEqual(IntegrationStatus.Unknown, initial.LastIntegrationStatus, "last integration status is unknown because no previous integrations exist.");
			Assert.AreEqual(IntegrationStatus.Unknown, initial.Status, "status should be unknown as integration has not run yet.");
			Assert.AreEqual(DateTime.Now.AddDays(-1).Day, initial.StartTime.Day, "assume start date is yesterday in order to detect some modifications.");
			Assert.AreEqual(DateTime.Now.Day, initial.EndTime.Day, "assume end date is today in order to detect some modifications.");

			Assert.IsTrue(initial.IsInitial());
		}
	}
}
