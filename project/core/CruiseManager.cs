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

		public ProjectStatus GetProjectStatus()
		{
			IEnumerator e =_cruiseControl.Configuration.GetEnumerator();
			e.MoveNext();
			Project p = (Project)e.Current;
			return new ProjectStatus(GetStatus(), p.GetLatestBuildStatus(), p.CurrentActivity, p.Name, p.WebURL, p.LastIntegrationResult.StartTime, p.LastIntegrationResult.Label); 
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