using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	// ToDo - make disposable?
	public class FileConfigurationService : IConfigurationService
	{
		private readonly FileInfo configFile;
		private readonly IFileWatcher fileWatcher;
		private readonly IConfigurationFileSaver saver;
		private readonly IConfigurationFileLoader loader;

		private ConfigurationUpdateHandler updateHandler;

		public FileConfigurationService(IConfigurationFileLoader loader, IConfigurationFileSaver saver, IFileWatcher fileWatcher, FileInfo configFile)
		{
			this.loader = loader;
			this.saver = saver;
			this.fileWatcher = fileWatcher;
			fileWatcher.OnFileChanged += new FileSystemEventHandler(HandleConfigurationFileChanged);
			this.configFile = configFile;
		}

		public IConfiguration Load()
		{
			lock (configFile)
			{
				return loader.Load(configFile);	
			}
		}

		public void Save(IConfiguration configuration)
		{
			lock (configFile)
			{
				saver.Save(configuration, configFile);
			}
		}

		public void AddConfigurationUpdateHandler(ConfigurationUpdateHandler handler)
		{
			updateHandler += handler;
		}

		private void HandleConfigurationFileChanged(object source, FileSystemEventArgs args)
		{
			try
			{
				lock (configFile)
				{
					if (updateHandler != null)
					{
						updateHandler();
					}
				}
			}
			catch (Exception ex) 
			{
				Log.Error(ex);
			}
		}
	}
}
