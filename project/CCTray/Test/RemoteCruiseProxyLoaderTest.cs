using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebServiceProxy;

namespace ThoughtWorks.CruiseControl.CCTray.Test
{
	[TestFixture]
	public class RemoteCruiseProxyLoaderTest
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
			Assert.IsNotNull((CCNetManagementProxy) _loader.LoadProxy(_settings));		    
		}
		
		[Test]
		public void ShouldUseCCNetManagementProxyWhenUsingRemotingInSettings()
		{
			_settings.ConnectionMethod = ConnectionMethod.Remoting;
			_settings.RemoteServerUrl = "tcp://localhost:1234/foo.rem";
			Assert.IsNotNull( _loader.LoadProxy(_settings));		    
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
