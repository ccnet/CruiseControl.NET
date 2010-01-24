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
            : base("Unknown server (" + requestedServer + ") - please check the URL is correct")
		{
			this.requestedServer = requestedServer;
		}
	}
}
