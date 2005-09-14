namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class CaptionAndMessage
	{
		public CaptionAndMessage()
		{
		}

		public CaptionAndMessage(string caption, string message)
		{
			Caption = caption;
			Message = message;
		}

		public string Caption;
		public string Message;
	}
}