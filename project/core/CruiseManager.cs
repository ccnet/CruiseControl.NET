using System;
using System.Collections;
using System.IO;
using ThoughtWorks.Core.Log;
using ThoughtWorks.CruiseControl.Core.Publishers;
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

		public string GetLatestBuildName(string projectName)
		{
			return GetBuildNames(projectName)[0];
		}

		public string[] GetBuildNames(string projectName)
		{
			// TODO - this is a hack - I'll tidy it up later - promise! :) MR
			foreach (Project project in _config.Projects) 
			{
				if (project.Name == projectName)
				{
					foreach (IIntegrationCompletedEventHandler publisher in project.Publishers)
					{
						if (publisher is XmlLogPublisher)
						{
							// ToDo - check these are sorted?
							return LogFileUtil.GetLogFileNames(((XmlLogPublisher) publisher).LogDir);
						}
					}
					throw new CruiseControlException("Unable to find Log Publisher for project so can't find log file");
				}
			}

			throw new NoSuchProjectException(projectName);
		}

		public string GetLog(string projectName, string buildName)
		{
			// TODO - this is a hack - I'll tidy it up later - promise! :) MR
			foreach (Project project in _config.Projects) 
			{
				if (project.Name == projectName)
				{
					foreach (IIntegrationCompletedEventHandler publisher in project.Publishers)
					{
						if (publisher is XmlLogPublisher)
						{
							using (StreamReader sr = new StreamReader(Path.Combine(((XmlLogPublisher) publisher).LogDir, buildName)))
							{
								return sr.ReadToEnd();
							}
						}
					}
					throw new CruiseControlException("Unable to find Log Publisher for project so can't find log file");
				}
			}

			throw new NoSuchProjectException(projectName);
		}

		// ToDo - test
		public string GetServerLog ()
		{
			return new ServerLogFileReader().Read();
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