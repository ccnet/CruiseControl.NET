using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class FileConfigurationService : IConfigurationService
	{
		private readonly FileInfo configFile;
		private readonly IConfigurationFileSaver saver;
		private readonly IConfigurationFileLoader loader;	    

		public FileConfigurationService(IConfigurationFileLoader loader, IConfigurationFileSaver saver, FileInfo configFile)
		{
			this.loader = loader;
			this.saver = saver;
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
		{}

	    public void AddConfigurationSubfileLoadedHandler (
	        ConfigurationSubfileLoadedHandler handler)
	    {
	        loader.AddSubfileLoadedHandler( handler );
	    }
	}
}