using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServerFactory : ICruiseServerFactory
	{
		private static readonly string RemotingConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

		private ICruiseServer CreateLocal(string configFile)
		{
			return new CruiseServer(
				new CachingConfigurationService(
					new FileConfigurationService(
						new DefaultConfigurationFileLoader(),
						new DefaultConfigurationFileSaver(new NetReflectorProjectSerializer()),
						new FileChangedWatcher(configFile),
						new FileInfo(configFile))),
				new ProjectIntegratorListFactory(),
				new NetReflectorProjectSerializer());
		}

		private ICruiseServer CreateRemote(string configFile)
		{
			return new RemoteCruiseServer(CreateLocal(configFile), RemotingConfigurationFile);
		}

		public ICruiseServer Create(bool remote, string configFile)
		{
			return (remote) ? CreateRemote(configFile) : CreateLocal(configFile);
		}
	}
}
