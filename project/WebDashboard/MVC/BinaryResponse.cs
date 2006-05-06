namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class BinaryResponse : IResponse
	{
		private byte[] content;

		public BinaryResponse(byte[] content)
		{
			this.content = content;
		}

		public byte[] Content
		{
			get { return content; }
		}
	}
}
