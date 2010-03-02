using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using NMock;
using NMock.Remoting;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;

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
		[Ignore("This is intermittently failing, I think due to some evil NMock static nastiness. Do we even want to Unit Test this stuff? Is it not too much of an edge case?")]
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

				ICruiseManager remoteManager = (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), "tcp://localhost:35354/" + RemoteCruiseServer.ManagerUri);
				Assert.IsNotNull(remoteManager, "cruiseserver should be registered on tcp channel");

				remoteManager = (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), "http://localhost:35355/" + RemoteCruiseServer.ManagerUri);
				Assert.IsNotNull(remoteManager, "cruiseserver should be registered on http channel");
			}
			Assert.AreEqual(0, ChannelServices.RegisteredChannels.Length, "all registered channels should be closed.");
			mockCruiseServer.Verify();
			mockCruiseManager.Verify();
		}

		[Test]
		[Ignore("This is intermittently failing, I think due to some evil NMock static nastiness.")]
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

        [Test(Description = "Check to make sure AbortBuildProcessed is correctly wired")]
        public void AbortBuildProcessedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.AbortBuildProcessed += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.AbortBuildProcessed += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new ProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure AbortBuildReceived is correctly wired")]
        public void AbortBuildReceivedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.AbortBuildReceived += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.AbortBuildReceived += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new CancelProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ForceBuildProcessed is correctly wired")]
        public void ForceBuildProcessedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.ForceBuildProcessed += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ForceBuildProcessed += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new ProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ForceBuildReceived is correctly wired")]
        public void ForceBuildReceivedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.ForceBuildReceived += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ForceBuildReceived += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new CancelProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure IntegrationCompleted is correctly wired")]
        public void IntegrationCompletedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.IntegrationCompleted += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.IntegrationCompleted += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new IntegrationCompletedEventArgs(null, "test", IntegrationStatus.Success));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure IntegrationStarted is correctly wired")]
        public void IntegrationStartedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.IntegrationStarted += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.IntegrationStarted += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new IntegrationStartedEventArgs(null, "test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStarted is correctly wired")]
        public void ProjectStartedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.ProjectStarted += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStarted += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new ProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStarting is correctly wired")]
        public void ProjectStartingWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.ProjectStarting += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStarting += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new CancelProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStopped is correctly wired")]
        public void ProjectStoppedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.ProjectStopped += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStopped += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new ProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStopping is correctly wired")]
        public void ProjectStoppingWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.ProjectStopping += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStopping += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new CancelProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure SendMessageProcessed is correctly wired")]
        public void SendMessageProcessedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.SendMessageProcessed += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.SendMessageProcessed += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new ProjectEventArgs<Message>("test", null));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure SendMessageReceived is correctly wired")]
        public void SendMessageReceivedWiredCorrectly()
        {
            var mocks = new MockRepository();
            var innerServer = mocks.DynamicMock<ICruiseServer>();
            var raiser = Expect.Call(() => { innerServer.SendMessageReceived += null; })
                .IgnoreArguments()
                .GetEventRaiser();
            mocks.ReplayAll();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.SendMessageReceived += (o, e) => { eventFired = true; };
                raiser.Raise(innerServer, new CancelProjectEventArgs<Message>("test", null));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
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