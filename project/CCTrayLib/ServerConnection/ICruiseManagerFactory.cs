using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
	// So yes, this is copied from the dashboard.  Ideally we should
	// create a shard client assembly that both dashboard and cctray should
	// use.  But this was delayed until we find out exactly how much really
	// is shared.
	public interface ICruiseManagerFactory
	{
		ICruiseManager GetCruiseManager(string url);
	}
}
