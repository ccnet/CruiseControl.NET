using System;
using System.Collections;

using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core 
{
	/// <summary>
	/// Manages an instance of the CruiseControl.NET main process, and exposes
	/// this interface via remoting.  The CCTray is one such example of an
	/// application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager
	{
		public const int TCP_PORT = 1234;

		private ICruiseControl _cruiseControl; 

		public CruiseManager(ICruiseControl cruiseControl)
		{
			_cruiseControl = cruiseControl;
		}

		public CruiseControlStatus GetStatus()
		{
			return _cruiseControl.Status;
		}				

		public ProjectStatus [] GetProjectStatus()
		{
			ArrayList projects = new ArrayList();
			foreach (Project project in _cruiseControl.Configuration) 
			{
				projects.Add(new ProjectStatus(GetStatus(), 
					project.GetLatestBuildStatus(), 
					project.CurrentActivity, 
					project.Name, 
					project.WebURL, 
					project.LastIntegrationResult.StartTime, 
					project.LastIntegrationResult.Label));
			}

			return (ProjectStatus []) projects.ToArray(typeof(ProjectStatus));
		}

		public void ForceBuild(string projectName)
		{
			((ICruiseServer)_cruiseControl).ForceBuild(projectName);
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}