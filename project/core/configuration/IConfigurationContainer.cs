namespace ThoughtWorks.CruiseControl.Core.Config
{
	public delegate void ConfigurationChangedHandler(IConfiguration configuration);

	public interface IConfigurationContainer : IConfiguration
	{
		void AddConfigurationChangedHandler(ConfigurationChangedHandler handler);
	}
}
