using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Cache
{
	public class LocalFileCacheManager : ICacheManager
	{
		public LocalFileCacheManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void AddContent(string serverName, string projectName, string directory, string fileName, string content)
		{
			throw new NotImplementedException();
		}

		public string GetURLForFile(string serverName, string projectName, string directory, string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
