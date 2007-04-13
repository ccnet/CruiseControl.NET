using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class BinaryResponse : IResponse
	{
		private byte[] content;

		public BinaryResponse(byte[] content)
		{
			this.content = content;
		}

		public void Process(HttpResponse response)
		{
			response.BinaryWrite(content);
		}
	}
}