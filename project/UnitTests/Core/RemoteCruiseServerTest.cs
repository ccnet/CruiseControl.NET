using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using NMock;
using NMock.Remoting;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class RemoteCruiseServerTest
	{
		[SetUp]
		public void SetUp()
		{
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				ChannelServices.UnregisterChannel(channel);
			}
			Assert.AreEqual(0, ChannelServices.RegisteredChannels.Length);
		}

		[TearDown]
		public void DeleteTempFiles()
		{
			TempFileUtil.DeleteTempDir("RemoteCruiseServerTest");
		}

		[Test]
		public void SetupAndTeardownRemotingInfrastructure()
		{
			string configFile = CreateTemporaryConfigurationFile();

			IMock mockCruiseManager = new RemotingMock(typeof (ICruiseManager));
			IMock mockCruiseServer = new DynamicMock(typeof (ICruiseServer));
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.Expect("Dispose");

			using (new RemoteCruiseServer((ICruiseServer) mockCruiseServer.MockInstance, configFile))
			{
				Assert.AreEqual(2, ChannelServices.RegisteredChannels.Length);
				Assert.IsNotNull(ChannelServices.GetChannel("ccnet"), "ccnet channel is missing");
				Assert.IsNotNull(ChannelServices.GetChannel("ccnet2"), "ccnet2 channel is missing");

				ICruiseManager remoteManager = (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), "tcp://localhost:35354/" + RemoteCruiseServer.URI);
				Assert.IsNotNull(remoteManager, "cruiseserver should be registered on tcp channel");

				remoteManager = (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), "http://localhost:35355/" + RemoteCruiseServer.URI);
				Assert.IsNotNull(remoteManager, "cruiseserver should be registered on http channel");
			}
			Assert.AreEqual(0, ChannelServices.RegisteredChannels.Length, "all registered channels should be closed.");
			mockCruiseServer.Verify();
			mockCruiseManager.Verify();
		}

		[Test]
		public void ShouldOnlyDisposeOnce()
		{
			string configFile = CreateTemporaryConfigurationFile();
			IMock mockCruiseManager = new RemotingMock(typeof (ICruiseManager));
			IMock mockCruiseServer = new DynamicMock(typeof (ICruiseServer));
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.Expect("Dispose");

			RemoteCruiseServer server = new RemoteCruiseServer((ICruiseServer) mockCruiseServer.MockInstance, configFile);
			((IDisposable)server).Dispose();

			mockCruiseServer.ExpectNoCall("Dispose");
			((IDisposable)server).Dispose();
			mockCruiseServer.Verify();
		}

		private string CreateTemporaryConfigurationFile()
		{
			string configXml = @"<configuration>
	<system.runtime.remoting>
		<application>
			<channels>
				<channel ref=""tcp"" port=""35354"" name=""ccnet"">
					<serverProviders>
						<formatter ref=""binary"" typeFilterLevel=""Full"" />
					</serverProviders>
				</channel>
				<channel ref=""http"" port=""35355"" name=""ccnet2"">
					<serverProviders>
						<formatter ref=""binary"" typeFilterLevel=""Full"" />
					</serverProviders>
				</channel>
			</channels>
		</application>
	</system.runtime.remoting>
</configuration>";
	
	
			TempFileUtil.CreateTempDir("RemoteCruiseServerTest");
			return TempFileUtil.CreateTempXmlFile("RemoteCruiseServerTest", "remote.config", configXml);
		}
	}
}