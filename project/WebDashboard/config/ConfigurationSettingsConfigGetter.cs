using System;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	/// <summary>
	/// Wraps ConfigurationSettings (to make other code more testable / componentized)
	/// </summary>
	public class ConfigurationSettingsConfigGetter : IConfigurationGetter
	{
		public string GetSimpleConfigSetting(string keyname)
		{
			return ConfigurationSettings.AppSettings[keyname];
		}

		public object GetConfigFromSection(string sectionName)
		{
			return ConfigurationSettings.GetConfig(sectionName);
		}
	}
}
