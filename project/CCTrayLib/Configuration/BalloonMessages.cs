namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class BalloonMessages
	{
		public BalloonMessages()
		{
				BrokenBuildMessage = new CaptionAndMessage("Broken build", "Recent checkins have broken the build");
				FixedBuildMessage = new CaptionAndMessage("Fixed build", "Recent checkins have fixed the build");
				StillFailingBuildMessage = new CaptionAndMessage("Build still failing", "The build is still broken...");
				StillSuccessfulBuildMessage = new CaptionAndMessage("Build successful", "Yet another successful build!");
		}

		public CaptionAndMessage BrokenBuildMessage;
		public CaptionAndMessage FixedBuildMessage;
		public CaptionAndMessage StillFailingBuildMessage;
		public CaptionAndMessage StillSuccessfulBuildMessage;
	}
}