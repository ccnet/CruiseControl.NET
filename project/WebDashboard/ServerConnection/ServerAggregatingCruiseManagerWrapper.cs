using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ServerAggregatingCruiseManagerWrapper : ICruiseManagerWrapper
	{
		public ServerAggregatingCruiseManagerWrapper()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string GetLatestLogName(string serverName, string projectName)
		{
			throw new NotImplementedException();
		}

		public string GetLog(string serverName, string projectName, string logName)
		{
			throw new NotImplementedException();
		}
	}
}
