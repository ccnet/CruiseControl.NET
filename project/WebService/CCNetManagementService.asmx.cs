using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Services;

using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebService
{
	public class CCNetManagementService : System.Web.Services.WebService
	{
		public virtual ICruiseManager BackingCruiseManager
		{
			get
			{
				return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), ConfigurationSettings.AppSettings["BackingURL"]);
			}
		}

		[WebMethod]
		public ProjectStatus [] GetProjectStatus()
		{
			return BackingCruiseManager.GetProjectStatus();
		}

		[WebMethod]
		public void ForceBuild(string projectName)
		{
			BackingCruiseManager.ForceBuild(projectName);
		}
	}
}
