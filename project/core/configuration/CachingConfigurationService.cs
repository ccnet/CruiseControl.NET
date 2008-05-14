namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class CachingConfigurationService : IConfigurationService
	{
		private readonly IConfigurationService slaveService;
		private IConfiguration cachedConfig;

		public CachingConfigurationService(IConfigurationService slaveService)
		{
			this.slaveService = slaveService;
			this.slaveService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(InvalidateCache));
		}

		public IConfiguration Load()
		{
			if (cachedConfig == null)
			{
				cachedConfig = slaveService.Load();
			}
			return cachedConfig;
		}

		public void Save(IConfiguration configuration)
		{
			slaveService.Save(configuration);
		}

		public void AddConfigurationUpdateHandler(ConfigurationUpdateHandler handler)
		{
			slaveService.AddConfigurationUpdateHandler(handler);
		}

	    public void AddConfigurationSubfileLoadedHandler (
	        ConfigurationSubfileLoadedHandler handler)
	    {
	        slaveService.AddConfigurationSubfileLoadedHandler( handler );
	    }

	    public void InvalidateCache()
		{
			cachedConfig = null;
		}
	}
}