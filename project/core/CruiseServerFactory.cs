using System.IO;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServerFactory
	{
		public static ICruiseServer CreateLocal(string configFile)
		{
			return new CruiseServer(
				new CachingConfigurationService(
					new FileConfigurationService(
						new DefaultConfigurationFileLoader(),
						new DefaultConfigurationFileSaver(new NetReflectorProjectSerializer()),
						new FileChangedWatcher(configFile),
						new FileInfo(configFile))),
				new ProjectIntegratorListFactory());
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
