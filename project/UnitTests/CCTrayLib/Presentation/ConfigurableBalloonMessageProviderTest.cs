using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ConfigurableBalloonMessageProviderTest
	{
		[Test]
		public void ReturnsTheMessagesAndCaptionsDefinedInTheConfiguration()
		{
			BalloonMessages messages = new BalloonMessages();
			ConfigurableBalloonMessageProvider provider = new ConfigurableBalloonMessageProvider(messages);
			
			Assert.AreSame(messages.BrokenBuildMessage, provider.GetCaptionAndMessageForBuildTransition(BuildTransition.Broken));			
			Assert.AreSame(messages.FixedBuildMessage, provider.GetCaptionAndMessageForBuildTransition(BuildTransition.Fixed));			
			Assert.AreSame(messages.StillFailingBuildMessage, provider.GetCaptionAndMessageForBuildTransition(BuildTransition.StillFailing));			
			Assert.AreSame(messages.StillSuccessfulBuildMessage, provider.GetCaptionAndMessageForBuildTransition(BuildTransition.StillSuccessful));			
		}
		
	}
}