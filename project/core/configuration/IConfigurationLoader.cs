using System;
using System.Collections;

namespace tw.ccnet.core.configuration
{
	public delegate void ConfigurationChangedHandler();

	public interface IConfigurationLoader : IConfiguration
	{
		IDictionary LoadProjects();
		void AddConfigurationChangedHandler(ConfigurationChangedHandler handler);
	}
}