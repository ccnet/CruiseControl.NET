using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class CachingConfigurationService : IConfigurationService
	{
		private readonly IConfigurationService slaveService;
		private IConfiguration cachedConfig;

		public CachingConfigurationService(IConfigurationService slaveService)
		{
			this.slaveService = slaveService;
			// Lazy load - will be instantiated on first call;
			this.cachedConfig = null;

			slaveService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(InvalidateCache));
		}

		public IConfiguration Load()
		{
			// Don't bother synchronizing here - 2 concurrent calls are safe since this class doesn't have any 'real' state
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

		public void InvalidateCache()
		{
			cachedConfig = null;
		}
	}
}
