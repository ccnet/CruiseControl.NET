namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class CaptionAndMessage
	{
		public CaptionAndMessage(string caption, string message)
		{
			Caption = caption;
			Message = message;
		}

		public readonly string Caption;
		public readonly string Message;
	}
}