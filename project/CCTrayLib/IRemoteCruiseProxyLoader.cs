using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public interface IRemoteCruiseProxyLoader
	{
		ICruiseManager LoadProxy (Settings settings);
	}
}