using System.Collections;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ConfigurableBalloonMessageProvider : IBalloonMessageProvider
	{
		private readonly IDictionary messages = new HybridDictionary();

		public ConfigurableBalloonMessageProvider(BalloonMessages balloonMessages)
		{
			if (balloonMessages != null){
			messages.Add(BuildTransition.Broken, balloonMessages.BrokenBuildMessage);
			messages.Add(BuildTransition.Fixed, balloonMessages.FixedBuildMessage);
			messages.Add(BuildTransition.StillFailing, balloonMessages.StillFailingBuildMessage);
			messages.Add(BuildTransition.StillSuccessful, balloonMessages.StillSuccessfulBuildMessage);
			}
		}

		public CaptionAndMessage GetCaptionAndMessageForBuildTransition(BuildTransition buildTransition)
		{
			return (CaptionAndMessage) messages[buildTransition];
		}
	}
}
