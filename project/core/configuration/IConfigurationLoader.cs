using System;
using System.Collections;

namespace tw.ccnet.core
{
	public delegate void ConfigurationChangedHandler();

	public interface IConfigurationLoader
	{
		IDictionary LoadProjects();
		void AddConfigurationChangedHandler(ConfigurationChangedHandler handler);
	}
}
