using System;
using System.Collections;
using System.Runtime.Remoting;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class LocalCruiseManagerAggregator
	{
		private ArrayList connectionExceptions = new ArrayList();
		private ArrayList projectDetails = new ArrayList();
		private IDictionary urlsForProjects = new Hashtable();

		public LocalCruiseManagerAggregator(ServerSpecification[] servers) 
		{
			ConnectToRemoteServers(servers);
		}

		private void ConnectToRemoteServers(ServerSpecification[] servers)
		{
			foreach (ServerSpecification server in servers)
			{
				try
				{
					ICruiseManager remoteCC = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), server.Url);
					foreach (ProjectStatus status in remoteCC.GetProjectStatus())
					{
						projectDetails.Add(status);
						urlsForProjects[status.Name] = server.Url;
					}
				}
				catch (Exception ex)
				{
					connectionExceptions.Add(new ConnectionException(server.Url, ex));
				}
			}
		}

		public ArrayList ProjectDetails
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

	public struct ConnectionException
	{
		private string _url;
		private Exception _exception;

		public ConnectionException(string URL, Exception exception)
		{
			this._url = URL;
			this._exception = exception;
		}

		public string URL
		{
			get { return _url; }
		}

		public string Message
		{
			get { return _exception.Message; }
		}

		public Exception Exception
		{
			get { return _exception; }
		}
	}
}
