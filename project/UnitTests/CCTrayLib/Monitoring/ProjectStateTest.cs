using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectStateTest
	{
		[Test]
		public void EqualityIsImplementedCorrectly()
		{
			Assert.AreEqual(ProjectState.Broken, ProjectState.Broken);
			Assert.AreEqual(ProjectState.Success, ProjectState.Success);
			Assert.AreEqual(ProjectState.NotConnected, ProjectState.NotConnected);
			Assert.AreEqual(ProjectState.Building, ProjectState.Building);

			Assert.IsFalse(ProjectState.Broken.Equals(ProjectState.Success));
			Assert.IsFalse(ProjectState.Broken.Equals(ProjectState.NotConnected));
			Assert.IsFalse(ProjectState.Broken.Equals(ProjectState.Building));
		}

		[Test]
		public void IsMoreImportantThanIsImplementedCorrectly()
		{
			// broken is most important state to know about
			Assert.IsTrue(ProjectState.Broken.IsMoreImportantThan(ProjectState.Building));
			Assert.IsTrue(ProjectState.Broken.IsMoreImportantThan(ProjectState.NotConnected));
			Assert.IsTrue(ProjectState.Broken.IsMoreImportantThan(ProjectState.Success));

			// building is slightly less important
			Assert.IsFalse(ProjectState.Building.IsMoreImportantThan(ProjectState.Broken));
			Assert.IsTrue(ProjectState.Building.IsMoreImportantThan(ProjectState.NotConnected));
			Assert.IsTrue(ProjectState.Building.IsMoreImportantThan(ProjectState.Success));

			// not connected is next
			Assert.IsFalse(ProjectState.NotConnected.IsMoreImportantThan(ProjectState.Broken));
			Assert.IsFalse(ProjectState.NotConnected.IsMoreImportantThan(ProjectState.Building));
			Assert.IsTrue(ProjectState.NotConnected.IsMoreImportantThan(ProjectState.Success));

			// successful builds "least" important -- i.e. only show this if all projects
			// are in the successful state
			Assert.IsFalse(ProjectState.Success.IsMoreImportantThan(ProjectState.Broken));
			Assert.IsFalse(ProjectState.Success.IsMoreImportantThan(ProjectState.NotConnected));
			Assert.IsFalse(ProjectState.Success.IsMoreImportantThan(ProjectState.Building));
		}

		[Test]
		public void ToStringReturnsStateName()
		{
			Assert.AreEqual(ProjectState.Broken.Name, ProjectState.Broken.ToString());
		}
	}
}
