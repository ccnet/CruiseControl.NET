using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.WebServiceProxy;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	[TestFixture]
	public class SettingsTest
	{
		private Settings _settings;

		[SetUp]
		public void Init()
		{
			_settings = new Settings();
		}

		[Test]
		public void ShouldUseCCNetManagementProxyWhenUsingWebServicesInSettings()
		{
			_settings.ConnectionMethod = ConnectionMethod.WebService;
			Assert.IsNotNull((CCNetManagementProxy) _settings.CruiseManager);
		}

		[Test]
		public void ShouldUseCCNetManagementProxyWhenUsingRemotingInSettings()
		{
			_settings.ConnectionMethod = ConnectionMethod.Remoting;
			_settings.RemoteServerUrl = "tcp://localhost:1234/foo.rem";
			Assert.IsNotNull(_settings.CruiseManager);
		}

		[Test]
		public void ShouldTrimURL()
		{
			_settings.RemoteServerUrl = "tcp://localhost:1234/foo.rem ";
			Assert.AreEqual("tcp://localhost:1234/foo.rem", _settings.RemoteServerUrl);
		}
	}
}