namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class BuildTransition
	{
		public static readonly BuildTransition Broken = new BuildTransition("Broken build", ErrorLevel.Error);
		public static readonly BuildTransition Fixed = new BuildTransition("Fixed build", ErrorLevel.Info);
		public static readonly BuildTransition StillSuccessful = new BuildTransition("Build successful", ErrorLevel.Info);
		public static readonly BuildTransition StillFailing = new BuildTransition("Build still failing", ErrorLevel.Warning);

		private readonly string _message;
		private readonly ErrorLevel _errLevel;

		private BuildTransition(string message, ErrorLevel level)
		{
			_errLevel = level;
			_message = message;
		}

		public string Caption
		{
			get { return _message; }
		}

		public ErrorLevel ErrorLevel
		{
			get { return _errLevel; }
		}
	}
}