using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	public delegate void ConfigurationChangedHandler();

	public interface IConfigurationLoader : IConfiguration
	{
		IDictionary LoadProjects();
		void AddConfigurationChangedHandler(ConfigurationChangedHandler handler);
	}
}