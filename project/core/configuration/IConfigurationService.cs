using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public interface IConfigurationService
	{
		IConfiguration Load();
		void Save(IConfiguration configuration);
		void AddConfigurationUpdateHandler(ConfigurationUpdateHandler handler);
	}

	public delegate void ConfigurationUpdateHandler();
}
