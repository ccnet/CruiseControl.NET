using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class ConfigurationContainer : IConfigurationContainer, IDisposable
	{
		private IConfigurationLoader _loader;
		private IFileWatcher _watcher;
		private IConfiguration _config;
		private ConfigurationChangedHandler _handler;

		public ConfigurationContainer(string filename) 
			: this(new ConfigurationLoader(filename), new FileChangedWatcher(filename)) { }

		public ConfigurationContainer(IConfigurationLoader loader, IFileWatcher watcher)
		{
			_loader = loader;
			_config = _loader.Load();

			_watcher = watcher;
			_watcher.OnFileChanged += new FileSystemEventHandler(HandleConfigurationFileChanged);
		}

		public void AddConfigurationChangedHandler(ConfigurationChangedHandler handler)
		{
			_handler += handler;
		}

		public IProjectList Projects { get { return _config.Projects; } }

		public IProjectIntegratorList Integrators { get { return _config.Integrators; } }

		// lock?
		private void HandleConfigurationFileChanged(object source, FileSystemEventArgs args)
		{
			try
			{
				IConfiguration newConfig = _loader.Load();
				_handler(newConfig);
				_config = newConfig;
			}
			catch (Exception ex) 
			{
				Log.Error(ex);
			}
		}

		void IDisposable.Dispose() 
		{
			_watcher.Dispose();
		}
	}
}
