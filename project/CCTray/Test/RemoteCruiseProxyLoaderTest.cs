using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebServiceProxy;

namespace ThoughtWorks.CruiseControl.CCTray.Test
{
	[NUnit.Framework.TestFixture]
	public class RemoteCruiseProxyLoaderTest : Assertion
	{

		private RemoteCruiseProxyLoader _loader;
		private Settings _settings;

		[SetUp]
		public void Init()
		{
			_settings = new Settings();
		    _loader = new RemoteCruiseProxyLoaderExtension();
		}
		
		[Test]
		public void ShouldUseCCNetManagementProxyWhenUsingWebServicesInSettings()
		{
			_settings.ConnectionMethod = ConnectionMethod.WebService;
			AssertNotNull((CCNetManagementProxy) _loader.LoadProxy(_settings));		    
		}
		
		[Test]
		public void ShouldUseCCNetManagementProxyWhenUsingRemotingInSettings()
		{
			_settings.ConnectionMethod = ConnectionMethod.Remoting;
			_settings.RemoteServerUrl = "tcp://localhost:1234/foo.rem";
			AssertNotNull( _loader.LoadProxy(_settings));		    
		}
	}

	public class RemoteCruiseProxyLoaderExtension : RemoteCruiseProxyLoader
	{
	    public RemoteCruiseProxyLoaderExtension() : base()
	    {

	    }

	    protected override ICruiseManager GetRemoteObject(Settings s)
	    {
	        return (ICruiseManager) new DynamicMock(typeof(ICruiseManager)).MockInstance;
	    }

	}
}
