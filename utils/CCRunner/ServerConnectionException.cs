using System;

namespace CCNet.CCRunner
{
	public class ServerConnectionException : ApplicationException
	{
		public ServerConnectionException(string uri, Exception innerException) : base(CreateMessage(uri), innerException)
		{
		}

		private static string CreateMessage(string uri)
		{
			return String.Format("Unable to connect to CruiseControl.NET server.  Please ensure that the server has started and that the url:{0} is correct", uri);
		}
	}
}
