namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	/// <summary>
	/// Wraps ConfigurationSettings (to make other code more testable / componentized)
	/// </summary>
	public class ConfigurationSettingsConfigGetter : IConfigurationGetter
	{
		public object GetConfig(string sectionName)
		{
			return System.Configuration.ConfigurationSettings.GetConfig(sectionName);
		}
	}
}
