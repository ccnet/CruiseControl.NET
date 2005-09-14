using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ResourceProjectStateIconProviderTest
	{
		[Test]
		public void CanRetriveIconsForState()
		{
			ResourceProjectStateIconProvider stateIconProvider = new ResourceProjectStateIconProvider();
			Assert.AreEqual(ResourceProjectStateIconProvider.RED, stateIconProvider.GetStatusIconForState(ProjectState.Broken));
			Assert.AreEqual(ResourceProjectStateIconProvider.YELLOW, stateIconProvider.GetStatusIconForState(ProjectState.Building));
			Assert.AreEqual(ResourceProjectStateIconProvider.GRAY, stateIconProvider.GetStatusIconForState(ProjectState.NotConnected));
			Assert.AreEqual(ResourceProjectStateIconProvider.GREEN, stateIconProvider.GetStatusIconForState(ProjectState.Success));
		}
	}

}