using System;
using NUnit.Framework;

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
	}
}
