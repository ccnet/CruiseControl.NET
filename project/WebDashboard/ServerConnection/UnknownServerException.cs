using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class UnknownServerException : ApplicationException
	{
		private readonly string requestedServer;

		public string RequestedServer
		{
			get { return requestedServer; }
		}

		public UnknownServerException(string requestedServer)
		{
			this.requestedServer = requestedServer;
		}
	}
}
