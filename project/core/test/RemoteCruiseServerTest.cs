using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using NMock;
using NMock.Remoting;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class RemoteCruiseServerTest : Assertion
	{
		[SetUp]
		public void SetUp()
		{
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				ChannelServices.UnregisterChannel(channel);
			}			
		}

		[TearDown]
		public void DeleteTempFiles()
		{
			TempFileUtil.DeleteTempDir("RemoteCruiseServerTest");	
		}

		[Test]
		public void SetupAndTeardownRemotingInfrastructure()
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

			AssertEquals("no registered channels should be open.", 0, ChannelServices.RegisteredChannels.Length);

			TempFileUtil.CreateTempDir("RemoteCruiseServerTest");
			string configFile = TempFileUtil.CreateTempXmlFile("RemoteCruiseServerTest", "remote.config", configXml);

			IMock mockCruiseManager = new RemotingMock(typeof(ICruiseManager));
			IMock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.Expect("Dispose");

			using(RemoteCruiseServer server = new RemoteCruiseServer((ICruiseServer) mockCruiseServer.MockInstance, configFile))
			{
				AssertEquals(2, ChannelServices.RegisteredChannels.Length);
				AssertNotNull("ccnet channel is missing", ChannelServices.GetChannel("ccnet"));
				AssertNotNull("ccnet2 channel is missing", ChannelServices.GetChannel("ccnet2"));

				ICruiseManager remoteManager = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), "tcp://localhost:35354/" + RemoteCruiseServer.URI);
				AssertNotNull("cruiseserver should be registered on tcp channel", remoteManager);

				remoteManager = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), "http://localhost:35355/" + RemoteCruiseServer.URI);
				AssertNotNull("cruiseserver should be registered on http channel", remoteManager);
			}
			AssertEquals("all registered channels should be closed.", 0, ChannelServices.RegisteredChannels.Length);
			mockCruiseServer.Verify();
			mockCruiseManager.Verify();
		}
	}
}
