using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core 
{
	/// <summary>
	/// Exposes project management functionality (start, stop, status) via remoting.  
	/// The CCTray is one such example of an application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager
	{
		private IConfiguration _config; 

		public CruiseManager(IConfiguration config)
		{
			_config = config;
		}

		public ProjectStatus [] GetProjectStatus()
		{
			ArrayList projects = new ArrayList();
			foreach (Project project in _config.Projects) 
			{
				projects.Add(new ProjectStatus(GetStatus(), 
					project.LatestBuildStatus, 
					project.CurrentActivity, 
					project.Name, 
					project.WebURL, 
					project.LastIntegrationResult.StartTime, 
					project.LastIntegrationResult.Label));
			}

			return (ProjectStatus []) projects.ToArray(typeof(ProjectStatus));
		}

		private IProjectIntegrator GetIntegrator(string project)
		{
			IProjectIntegrator integrator = _config.Integrators[project];
			if (integrator == null)
			{
				throw new CruiseControlException("Specified project does not exist: " + project);
			}
			return integrator;
		}

		public void ForceBuild(string project)
		{
			GetIntegrator(project).ForceBuild();
		}

		public void WaitForExit(string project)
		{
			GetIntegrator(project).WaitForExit();
		}

		/// <summary>
		/// TODO: deprecate this
		/// </summary>
		/// <returns></returns>
		private CruiseControlStatus GetStatus()
		{
			return CruiseControlStatus.Unknown;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}