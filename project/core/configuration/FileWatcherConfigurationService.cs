using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	// ToDo - make disposable?  need to ensure that the filewatcher is disposed.  should be done in container.
	public class FileWatcherConfigurationService : IConfigurationService
	{
		private readonly IConfigurationService decoratedService;
		private readonly IFileWatcher fileWatcher;
		private ConfigurationUpdateHandler updateHandler;
	    private ConfigurationSubfileLoadedHandler subfileHandler;

		public FileWatcherConfigurationService(IConfigurationService decoratedService, IFileWatcher fileWatcher)
		{
			this.decoratedService = decoratedService;
			this.fileWatcher = fileWatcher;
			this.fileWatcher.OnFileChanged += new FileSystemEventHandler(HandleConfigurationFileChanged);
		    decoratedService.AddConfigurationSubfileLoadedHandler( SubfileLoaded );
		}

	    public IConfiguration Load()
		{
			return decoratedService.Load();
		}

	    private void SubfileLoaded (string path)
	    {
	        fileWatcher.AddWatcher( path );
	    }

	    public void Save(IConfiguration configuration)
		{
			decoratedService.Save(configuration);
		}

		public void AddConfigurationUpdateHandler(ConfigurationUpdateHandler handler)
		{
			updateHandler += handler;
		}

	    public void AddConfigurationSubfileLoadedHandler (
	        ConfigurationSubfileLoadedHandler handler)
	    {
	        subfileHandler += handler;	        
	    }

	    private void HandleConfigurationFileChanged(object source, FileSystemEventArgs args)
		{
			try
			{
				lock (this)
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