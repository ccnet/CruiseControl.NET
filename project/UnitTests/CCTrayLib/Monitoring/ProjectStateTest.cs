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
		public void IsWorseThanIsImplementedCorrectly()
		{
			// broken is worse
			Assert.IsTrue(ProjectState.Broken.IsWorseThan(ProjectState.Building));
			Assert.IsTrue(ProjectState.Broken.IsWorseThan(ProjectState.NotConnected));
			Assert.IsTrue(ProjectState.Broken.IsWorseThan(ProjectState.Success));

			// notconnected is just a bit better
			Assert.IsFalse(ProjectState.NotConnected.IsWorseThan(ProjectState.Broken));
			Assert.IsTrue(ProjectState.NotConnected.IsWorseThan(ProjectState.Building));
			Assert.IsTrue(ProjectState.NotConnected.IsWorseThan(ProjectState.Success));

			// building is not even better
			Assert.IsFalse(ProjectState.Building.IsWorseThan(ProjectState.Broken));
			Assert.IsFalse(ProjectState.Building.IsWorseThan(ProjectState.NotConnected));
			Assert.IsTrue(ProjectState.Building.IsWorseThan(ProjectState.Success));

			// but we really like successful builds the best
			Assert.IsFalse(ProjectState.Success.IsWorseThan(ProjectState.Broken));
			Assert.IsFalse(ProjectState.Success.IsWorseThan(ProjectState.NotConnected));
			Assert.IsFalse(ProjectState.Success.IsWorseThan(ProjectState.Building));
		}

		[Test]
		public void ToStringReturnsStateName()
		{
			Assert.AreEqual(ProjectState.Broken.Name, ProjectState.Broken.ToString());
		}
	}
}
