using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core 
{
	/// <summary>
	/// Exposes project management functionality (start, stop, status) via remoting.  
	/// The CCTray is one such example of an application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager
	{
		private readonly ICruiseServer cruiseServer;

		public CruiseManager(ICruiseServer cruiseServer)
		{
			this.cruiseServer = cruiseServer;
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return cruiseServer.GetProjectStatus();
		}

		public void ForceBuild(string project)
		{
			cruiseServer.ForceBuild(project);
		}

		public void WaitForExit(string project)
		{
			cruiseServer.WaitForExit(project);
		}

		public string GetLatestBuildName(string projectName)
		{
			return cruiseServer.GetLatestBuildName(projectName);
		}

		public string[] GetBuildNames(string projectName)
		{
			return cruiseServer.GetBuildNames(projectName);
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return cruiseServer.GetMostRecentBuildNames(projectName, buildCount);
		}

		public string GetLog(string projectName, string buildName)
		{
			return cruiseServer.GetLog(projectName, buildName);
		}

		public string GetServerLog()
		{
			return cruiseServer.GetServerLog();
		}

		public void AddProject(string serializedProject)
		{
			cruiseServer.AddProject(serializedProject);
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}