namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	public interface IDashboardConfiguration
	{
		IRemoteServicesConfiguration RemoteServices { get; }
		IPluginConfiguration PluginConfiguration { get; }
	}
}
