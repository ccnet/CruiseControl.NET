using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;
using System.Runtime.Remoting;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class LocalCruiseManagerAggregator
	{
		private ArrayList connectionExceptions = new ArrayList();
		private IList projectDetails = new ArrayList();
		private IDictionary urlsForProjects = new Hashtable();

		public LocalCruiseManagerAggregator(IList urls) 
		{
			ConnectToRemoteServers(urls);
		}

		private void ConnectToRemoteServers(IList urls)
		{
			foreach (string url in urls)
			{
				try
				{
					ICruiseManager remoteCC = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), url);
					foreach (ProjectStatus status in remoteCC.GetProjectStatus())
					{
						projectDetails.Add(status);
						urlsForProjects[status.Name] = url;
					}
				}
				catch (Exception ex)
				{
					connectionExceptions.Add(new ConnectionException(url, ex));
				}
			}
		}

		public IList ProjectDetails
		{
			get { return projectDetails; }
		}

		public IList ConnectionExceptions
		{
			get { return connectionExceptions; }
		}

		public void ForceBuild(string projectName)
		{
			ICruiseManager remoteCC = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), (string)urlsForProjects[projectName]);
			remoteCC.ForceBuild(projectName);
		}
	}
}
