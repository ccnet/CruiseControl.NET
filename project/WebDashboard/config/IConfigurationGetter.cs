namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	/// <summary>
	/// Used to (at least) mask out ConfigurationSettings.GetConfig()
	/// </summary>
	public interface IConfigurationGetter
	{
		string GetSimpleConfigSetting(string keyname);
		object GetConfigFromSection(string sectionName);
	}
}
