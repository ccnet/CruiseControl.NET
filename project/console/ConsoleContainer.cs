using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ConsoleContainer : IDisposable
	{
		private ICruiseServer server;
		private ConfigurationContainer configuration;
		private IFileWatcher watcher;

		public ConsoleContainer(string configFile, bool isRemote)
		{
			configuration = CreateConfiguration(configFile);
			server = CreateServer(isRemote);
		}

		private ConfigurationContainer CreateConfiguration(string configFile)
		{
			ConfigurationLoader loader = new ConfigurationLoader(configFile);	// todo: configuration loader should load file && report error if it is missing!
			watcher = new FileChangedWatcher(configFile);
			return new ConfigurationContainer(loader, watcher);
		}

		public ICruiseServer CreateServer(bool isRemote)
		{
			server = new CruiseServer(configuration);
			if (isRemote)
			{
				server = new RemoteCruiseServer(server);
			}
			return server;
		}

		public void Dispose()
		{
			watcher.Dispose();
			server.Dispose();
		}
	}
}