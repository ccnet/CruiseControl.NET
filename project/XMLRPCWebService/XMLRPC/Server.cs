using System;
using CookComputing.XmlRpc;

namespace ThoughtWorks.CruiseControl.XMLRPCWebService.XMLRPC
{
	public interface Server
	{
		[XmlRpcMethod("server.get_project_names")]
		string[] GetProjectNames();
	}
}
