using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	/// <summary>
	/// Used to (at least) mask out ConfigurationSettings.GetConfig()
	/// </summary>
	public interface IConfigurationGetter
	{
		object GetConfig(string sectionName);
	}
}
