namespace ThoughtWorks.CruiseControl.Core.Config
{
	public interface IConfigurationPersister
	{
		IConfiguration Load();
		void Save(IConfiguration config);
	}
}