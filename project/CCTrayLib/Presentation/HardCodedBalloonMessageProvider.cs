using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class HardCodedBalloonMessageProvider : IBalloonMessageProvider
	{
		public CaptionAndMessage GetCaptionAndMessageForBuildTransition(BuildTransition buildTransition)
		{
			if (buildTransition == BuildTransition.Broken)
			{
				return new CaptionAndMessage("Broken build", "Recent checkins have broken the build");
			}
			
			if (buildTransition == BuildTransition.Fixed)
			{
				return new CaptionAndMessage("Fixed build", "Recent checkins have fixed the build");
			}

			if (buildTransition == BuildTransition.StillFailing)
			{
				return new CaptionAndMessage("Build still failing", "The build is still broken...");
			}
			
			if (buildTransition == BuildTransition.StillSuccessful)
			{
				return new CaptionAndMessage("Build successful", "Yet another successful build!");
			}
			
			throw new InvalidOperationException("Unexpected build transition value: " + buildTransition);
		}
	}
}