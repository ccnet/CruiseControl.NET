using System;
using CookComputing.XmlRpc;
using System.Configuration;
using System.Runtime.Remoting;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections;
using ThoughtWorks.CruiseControl.XMLRPCWebService.XMLRPC;

namespace ThoughtWorks.CruiseControl.XMLRPCWebService
{
	public class ServerWebServiceImpl : XmlRpcService, Server
	{
		public string[] GetProjectNames()
		{
			ArrayList projectNames = new ArrayList();
			foreach (ProjectStatus projectStatus in BackingCruiseManager.GetProjectStatus())
			{
			  projectNames.Add(projectStatus.Name);
			}
			return (string[]) projectNames.ToArray (typeof (string));
		}

		public virtual ICruiseManager BackingCruiseManager
		{
			get
			{
				return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), ConfigurationSettings.AppSettings["BackingURL"]);
			}
		}
	}
}
