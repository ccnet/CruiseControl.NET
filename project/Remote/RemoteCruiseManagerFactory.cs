using System.Runtime.Remoting;

namespace ThoughtWorks.CruiseControl.Remote
{
	public class RemoteCruiseManagerFactory : ICruiseManagerFactory
	{
		public ICruiseManager GetCruiseManager(string url)
		{
			return (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), url);
		}
	}
}