using System;
using tw.ccnet.core.configuration;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public class CruiseServerFactory
	{
		public static ICruiseServer CreateLocal(string configFile)
		{
			return new CruiseControl(new ConfigurationLoader(configFile));
		}

		public static ICruiseServer CreateRemote(string configFile)
		{
			return new RemoteCruiseServer(CreateLocal(configFile));
		}

		public static ICruiseServer Create(bool remote, string configFile)
		{
			return (remote) ? CreateRemote(configFile) : CreateLocal(configFile);
		}
	}
}
