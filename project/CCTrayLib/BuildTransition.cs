namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class BuildTransition
	{
		public static readonly BuildTransition Broken = 
			new BuildTransition("Broken build", ErrorLevel.Error, "Recent checkins have broken the build");
		public static readonly BuildTransition Fixed = 
			new BuildTransition("Fixed build", ErrorLevel.Info, "Recent checkins have fixed the build");
		public static readonly BuildTransition StillSuccessful = 
			new BuildTransition("Build successful", ErrorLevel.Info, "Yet another successful build!");
		public static readonly BuildTransition StillFailing = 
			new BuildTransition("Build still failing", ErrorLevel.Warning, "The build is still broken...");

		private readonly string caption;
		private readonly ErrorLevel errorLevel;
		private readonly string message;

		private BuildTransition(string caption, ErrorLevel level, string message)
		{
			errorLevel = level;
			this.message = message;
			this.caption = caption;
		}

		public string Caption
		{
			get { return caption; }
		}

		public ErrorLevel ErrorLevel
		{
			get { return errorLevel; }
		}

		public string Message
		{
			get { return message; }
		}

		public override string ToString()
		{
			return Caption;
		}

	}
}