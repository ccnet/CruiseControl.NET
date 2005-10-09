namespace ThoughtWorks.CruiseControl.Remote
{
	public interface ICruiseManagerFactory
	{
		ICruiseManager GetCruiseManager(string url);
	}
}