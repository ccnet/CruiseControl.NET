using System;

using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServerFactory
	{
		public static ICruiseServer CreateLocal(string configFile)
		{
			return new CruiseServer(new ConfigurationContainer(configFile));
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
