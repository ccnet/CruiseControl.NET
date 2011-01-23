using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IBalloonMessageProvider
	{
		CaptionAndMessage GetCaptionAndMessageForBuildTransition(BuildTransition buildTransition);
	}

}