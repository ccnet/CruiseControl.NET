using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	public delegate void ConfigurationChangedHandler();

	public interface IConfigurationLoader
	{
		IConfiguration Load();
		void AddConfigurationChangedHandler(ConfigurationChangedHandler handler);
	}
}