using System;
using ThoughtWorks.CruiseControl.XMLRPCWebService.XMLRPC;
using CookComputing.XmlRpc;

namespace ThoughtWorks.CruiseControl.XMLRPCWebService.Client
{
	public class ServerProxy : XmlRpcClientProtocol, 	Server
	{
		[XmlRpcMethod("server.get_project_names")]
		public string[] GetProjectNames()
		{
			return (string[]) Invoke("GetProjectNames");
		}
	}
}
