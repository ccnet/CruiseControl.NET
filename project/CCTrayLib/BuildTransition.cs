namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class BuildTransition
	{
		public static readonly BuildTransition Broken = 
			new BuildTransition("Broken build", ErrorLevel.Error);
		public static readonly BuildTransition Fixed = 
			new BuildTransition("Fixed build", ErrorLevel.Info);
		public static readonly BuildTransition StillSuccessful = 
			new BuildTransition("Build successful", ErrorLevel.Info);
		public static readonly BuildTransition StillFailing = 
			new BuildTransition("Build still failing", ErrorLevel.Warning);

		private readonly string caption;
		private readonly ErrorLevel errorLevel;

		private BuildTransition(string caption, ErrorLevel errorLevel)
		{
			this.caption = caption;
			this.errorLevel = errorLevel;
		}

		public ErrorLevel ErrorLevel
		{
			get { return errorLevel; }
		}

		public override string ToString()
		{
			return caption;
		}

	}
}