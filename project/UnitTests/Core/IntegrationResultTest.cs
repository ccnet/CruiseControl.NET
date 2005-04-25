using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationResultTest
	{
		private IntegrationResult result;

		[SetUp]
		protected void CreateIntegrationResult()
		{
			result = new IntegrationResult();			
		}

		[Test]
		public void LastModificationDate()
		{
			Modification earlierModification = new Modification();
			earlierModification.ModifiedTime = new DateTime(0);
			
			Modification laterModification = new Modification();
			laterModification.ModifiedTime = new DateTime(1);

			result.Modifications = new Modification[] {earlierModification, laterModification};
			Assert.AreEqual(laterModification.ModifiedTime, result.LastModificationDate);
		}

		[Test]
		public void LastModificationDateWhenThereAreNoModifications()
		{
			// Project relies on this behavior, but is it really what we want?
			DateTime yesterday = DateTime.Now.AddDays(-1).Date;
			Assert.AreEqual(yesterday, result.LastModificationDate.Date);
		}	
  
		[Test]
		public void VerifyInitialIntegrationResult()
		{
			IntegrationResult initial = IntegrationResult.CreateInitialIntegrationResult("project", @"c:\temp");

			Assert.AreEqual("project", initial.ProjectName);
			Assert.AreEqual(IntegrationStatus.Unknown, initial.LastIntegrationStatus, "last integration status is unknown because no previous integrations exist.");
			Assert.AreEqual(IntegrationStatus.Unknown, initial.Status, "status should be unknown as integration has not run yet.");
			Assert.AreEqual(DateTime.Now.AddDays(-1).Day, initial.StartTime.Day, "assume start date is yesterday in order to detect some modifications.");
			Assert.AreEqual(DateTime.Now.Day, initial.EndTime.Day, "assume end date is today in order to detect some modifications.");
			Assert.AreEqual(@"c:\temp", initial.WorkingDirectory);

			Assert.IsTrue(initial.IsInitial());
		}

		[Test]
		public void ShouldReturnZeroAsLastChangeNumberIfNoModifications()
		{
			Assert.AreEqual(0, result.LastChangeNumber);
		}

		[Test]
		public void ShouldReturnTheMaximumChangeNumberFromAllModificationsForLastChangeNumber()
		{
			Modification mod1 = new Modification();
			mod1.ChangeNumber = 10;

			Modification mod2 = new Modification();
			mod2.ChangeNumber = 20;

			result.Modifications = new Modification[] { mod1 };
			Assert.AreEqual(10, result.LastChangeNumber);
			result.Modifications = new Modification[] { mod1, mod2 };
			Assert.AreEqual(20, result.LastChangeNumber);
			result.Modifications = new Modification[] { mod2, mod1 };
			Assert.AreEqual(20, result.LastChangeNumber);
		}

		[Test] 
		public void ShouldNotRunBuildIfThereAreNoModifications()
		{
			result.Modifications = new Modification[0];
			Assert.IsFalse(result.ShouldRunBuild(0));
		}

		[Test] 
		public void ShouldRunBuildIfThereAreModifications()
		{
			Modification modification = ModificationMother.CreateModification("foo", DateTime.Now.AddSeconds(-2));
			result.Modifications = new Modification[] {modification};
			Assert.IsTrue(result.ShouldRunBuild(0));
		}
		
		[Test] 
		public void ShouldNotRunBuildIfThereAreModificationsWithinModificationDelay()
		{
			Modification modification = ModificationMother.CreateModification("foo", DateTime.Now.AddSeconds(-2));
			result.Modifications = new Modification[] {modification};
			Assert.IsFalse(result.ShouldRunBuild(100));
		}
		
		[Test] 
		public void ShouldRunBuildIfLastModificationOutsideModificationDelay()
		{
			Modification modification = ModificationMother.CreateModification("foo", DateTime.Now.AddMinutes(-2));
			result.Modifications = new Modification[] {modification};
			Assert.IsTrue(result.ShouldRunBuild(100));
		}

		[Test] 
		public void ShouldRunBuildIfInForcedCondition()
		{
			result.BuildCondition = BuildCondition.ForceBuild;
			Assert.IsTrue(result.ShouldRunBuild(0));
		}

		[Test]
		public void TaskOutputShouldAggregateOutputOfTaskResults()
		{
			result.AddTaskResult("<foo/>");
			result.AddTaskResult("<bar/>");
			Assert.AreEqual("<foo/><bar/>", result.TaskOutput);
		}

		[Test]
		public void ShouldBaseRelativePathFromArtifactsDirectory()
		{
			result.ArtifactDirectory = @"c:\";
			Assert.AreEqual(@"c:\hello.bat", result.BaseFromArtifactsDirectory("hello.bat"));
		}

		[Test]
		public void ShouldNotReBaseRelativeToArtifactsDirectoryForAbsolutePath()
		{
			result.ArtifactDirectory = @"c:\";
			Assert.AreEqual(@"d:\hello.bat", result.BaseFromArtifactsDirectory(@"d:\hello.bat"));
		}

		[Test]
		public void ShouldBaseRelativePathFromWorkingDirectory()
		{
			result.WorkingDirectory = @"c:\";
			Assert.AreEqual(@"c:\hello.bat", result.BaseFromWorkingDirectory("hello.bat"));
		}

		[Test]
		public void ShouldNotReBaseRelativeToWorkingDirectoryForAbsolutePath()
		{
			result.WorkingDirectory = @"c:\";
			Assert.AreEqual(@"d:\hello.bat", result.BaseFromWorkingDirectory(@"d:\hello.bat"));
		}
	}
}
