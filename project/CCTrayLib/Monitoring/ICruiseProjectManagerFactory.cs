namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ICruiseProjectManagerFactory
	{
		ICruiseProjectManager Create( string serverUrl, string projectName );
	}
}