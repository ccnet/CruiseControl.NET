using System;
using System.Configuration;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServerFactory : ICruiseServerFactory
	{
		private static readonly string RemotingConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		private static bool WatchConfigFile
		{
			get
			{
				string value = ConfigurationSettings.AppSettings["WatchConfigFile"];
				return value == null || StringUtil.EqualsIngnoreCase(value, Boolean.TrueString);
			}
		}

		private ICruiseServer CreateLocal(string configFile)
		{
			return new CruiseServer(
				NewConfigurationService(configFile),
				new ProjectIntegratorListFactory(),
				new NetReflectorProjectSerializer());
		}

		private IConfigurationService NewConfigurationService(string configFile)
		{
			IConfigurationService service = new FileConfigurationService(
				new DefaultConfigurationFileLoader(),
				new DefaultConfigurationFileSaver(
					new NetReflectorProjectSerializer()), 
				new FileInfo(configFile));

			if (WatchConfigFile)
				service = new FileWatcherConfigurationService(service, new FileChangedWatcher(configFile));
			
			return new CachingConfigurationService(service);
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