namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IBalloonMessageProvider
	{
		CaptionAndMessage GetCaptionAndMessageForBuildTransition(BuildTransition buildTransition);
	}

}