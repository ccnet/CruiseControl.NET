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

			_watcher = watcher;
			_watcher.OnFileChanged += new FileSystemEventHandler(HandleConfigurationFileChanged);
		}

		public void AddConfigurationChangedHandler(ConfigurationChangedHandler handler)
		{
			_handler += handler;
		}

		private void InstanciateConfig()
		{
			if (_config == null)
			{
				try
				{
					_config = _loader.Load();
				}
				catch (ConfigurationFileMissingException)
				{
					throw;
				}
				catch (ConfigurationException ce)
				{
					Log.Error(ce);
				}
			}
		}

		public IProjectList Projects 
		{ 
			get 
			{
				lock(this)
				{
					InstanciateConfig();
					if (_config != null)
					{
						return _config.Projects; 
					}
					else
					{
						return new ProjectList();
					}
				}
			} 
		}

		public IProjectIntegratorList Integrators 
		{ 
			get 
			{ 
				lock(this)
				{
					InstanciateConfig();
					if (_config != null)
					{
						return _config.Integrators;
					}
					else
					{
						return new ProjectIntegratorList();
					}
				}
			} 
		}

		private void HandleConfigurationFileChanged(object source, FileSystemEventArgs args)
		{
			try
			{
				lock(this)
				{
					IConfiguration newConfig = _loader.Load();
					_handler(newConfig);
					_config = newConfig;
				}
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
