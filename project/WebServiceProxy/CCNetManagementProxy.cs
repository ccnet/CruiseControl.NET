using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebServiceProxy
{
	/// <summary>
	/// Summary description for CCNetManagementProxy.
	/// </summary>
	public class CCNetManagementProxy : ICruiseManager
	{
		private string _serverUrl;

		public CCNetManagementProxy(string serverUrl)
		{
			_serverUrl = serverUrl;
		}

		#region ICruiseManager Members

		public void StopCruiseControl()
		{
			Service.StopCruiseControl();
		}

		public string Configuration
		{
			get
			{
				throw new NotSupportedException("Configuration property not implemented on web service");
			}
			set
			{
				throw new NotSupportedException("Configuration property not implemented on web service");
			}
		}

		public ProjectStatus [] GetProjectStatus()
		{
			Generated.ProjectStatus serviceStatus = Service.GetProjectStatus();
			return new ProjectStatus [] {new ProjectStatus((CruiseControlStatus) serviceStatus.Status, 
											(IntegrationStatus) serviceStatus.BuildStatus, 
											(ProjectActivity) serviceStatus.Activity,
											serviceStatus.Name,
											serviceStatus.WebURL,
											serviceStatus.LastBuildDate,
											serviceStatus.LastBuildLabel)};
		}

		public void ForceBuild(string projectName)
		{
			Service.ForceBuild(projectName);
		}

		public void WaitForExit(string projectName)
		{
			
		}

		public string GetLatestBuildName(string projectName)
		{
			throw new NotImplementedException();
		}

		public string[] GetBuildNames(string projectName)
		{
			throw new NotImplementedException();
		}

		public string GetLog(string projectName, string buildName)
		{
			throw new NotImplementedException();
		}

		public string GetServerLog ()
		{
			throw new NotImplementedException ();
		}

		public void StartCruiseControl()
		{
			Service.StartCruiseControl();
		}

		public CruiseControlStatus GetStatus()
		{
			return (CruiseControlStatus) Service.GetStatus();
		}

		public void StopCruiseControlNow()
		{
			Service.StopCruiseControlNow();
		}

		#endregion

		public Generated.CCNetManagementService Service
		{
			get
			{
				Generated.CCNetManagementService service = new Generated.CCNetManagementService();
				service.Url = _serverUrl;
				return service;
			}
		}
	}
}
