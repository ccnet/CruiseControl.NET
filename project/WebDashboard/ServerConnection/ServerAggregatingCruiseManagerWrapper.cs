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

		public string GetLog(string serverName, string projectName, ILogSpecifier logSpecifier)
		{
			throw new NotImplementedException();
		}
	}
}
