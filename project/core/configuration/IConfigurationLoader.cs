using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public delegate void ConfigurationChangedHandler(IConfiguration configuration);

	public interface IConfigurationLoader
	{
		IConfiguration Load();
		void AddConfigurationChangedHandler(ConfigurationChangedHandler handler);
	}
}