using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public interface IRemoteCruiseProxyLoader
	{
	    ICruiseManager LoadProxy(Settings settings);
	}
}
