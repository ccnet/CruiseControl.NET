using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ConfigurableProjectStateIconProviderTest
	{
		[Test]
		public void WhenTheValuesInTheConfigurationAreNullOrEmptyTheDefaultIconsAreUsed()
		{
			Icons icons = new Icons();
			icons.BrokenIcon = string.Empty;
			icons.BuildingIcon = null;
			
			ConfigurableProjectStateIconProvider stateIconProvider = new ConfigurableProjectStateIconProvider(icons);
			Assert.AreSame(ResourceProjectStateIconProvider.RED, stateIconProvider.GetStatusIconForState(ProjectState.Broken));
			Assert.AreSame(ResourceProjectStateIconProvider.YELLOW, stateIconProvider.GetStatusIconForState(ProjectState.Building));
			Assert.AreSame(ResourceProjectStateIconProvider.GRAY, stateIconProvider.GetStatusIconForState(ProjectState.NotConnected));
			Assert.AreSame(ResourceProjectStateIconProvider.GREEN, stateIconProvider.GetStatusIconForState(ProjectState.Success));
		}
		
	}
}