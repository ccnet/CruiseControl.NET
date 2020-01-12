using System;
using System.Runtime.Remoting.Channels;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using ThoughtWorks.CruiseControl.Remote.Messages;

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

        [Test(Description = "Check to make sure AbortBuildProcessed is correctly wired")]
        public void AbortBuildProcessedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.AbortBuildProcessed += It.IsAny<EventHandler<ProjectEventArgs<string>>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.AbortBuildProcessed += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.AbortBuildProcessed += null, new ProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure AbortBuildReceived is correctly wired")]
        public void AbortBuildReceivedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.AbortBuildReceived += It.IsAny<EventHandler<CancelProjectEventArgs<string>>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.AbortBuildReceived += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.AbortBuildReceived += null, new CancelProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ForceBuildProcessed is correctly wired")]
        public void ForceBuildProcessedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.ForceBuildProcessed += It.IsAny<EventHandler<ProjectEventArgs<string>>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ForceBuildProcessed += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.ForceBuildProcessed += null, new ProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ForceBuildReceived is correctly wired")]
        public void ForceBuildReceivedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.ForceBuildReceived += It.IsAny<EventHandler<CancelProjectEventArgs<string>>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ForceBuildReceived += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.ForceBuildReceived += null, new CancelProjectEventArgs<string>("test", "data"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure IntegrationCompleted is correctly wired")]
        public void IntegrationCompletedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.IntegrationCompleted += It.IsAny<EventHandler<IntegrationCompletedEventArgs>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.IntegrationCompleted += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.IntegrationCompleted += null, new IntegrationCompletedEventArgs(null, "test", IntegrationStatus.Success));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure IntegrationStarted is correctly wired")]
        public void IntegrationStartedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.IntegrationStarted += It.IsAny<EventHandler<IntegrationStartedEventArgs>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.IntegrationStarted += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.IntegrationStarted += null, new IntegrationStartedEventArgs(null, "test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStarted is correctly wired")]
        public void ProjectStartedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.ProjectStarted += It.IsAny<EventHandler<ProjectEventArgs>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStarted += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.ProjectStarted += null, new ProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStarting is correctly wired")]
        public void ProjectStartingWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.ProjectStarting += It.IsAny<EventHandler<CancelProjectEventArgs>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStarting += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.ProjectStarting += null, new CancelProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStopped is correctly wired")]
        public void ProjectStoppedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.ProjectStopped += It.IsAny<EventHandler<ProjectEventArgs>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStopped += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.ProjectStopped += null, new ProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure ProjectStopping is correctly wired")]
        public void ProjectStoppingWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.ProjectStopping += It.IsAny<EventHandler<CancelProjectEventArgs>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.ProjectStopping += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.ProjectStopping += null, new CancelProjectEventArgs("test"));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure SendMessageProcessed is correctly wired")]
        public void SendMessageProcessedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.SendMessageProcessed += It.IsAny<EventHandler<ProjectEventArgs<Message>>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.SendMessageProcessed += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.SendMessageProcessed += null, new ProjectEventArgs<Message>("test", null));
            }

            Assert.IsTrue(eventFired);
            mocks.VerifyAll();
        }

        [Test]
        public void GetFinalBuildStatusPassesOnCall()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            var request = new BuildRequest();
            var response = new StatusSnapshotResponse();
            Mock.Get(innerServer).Setup(_innerServer => _innerServer.GetFinalBuildStatus(request))
                .Returns(response).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                var actual = server.GetFinalBuildStatus(request);
                Assert.AreSame(response, actual);
            }

            mocks.VerifyAll();
        }

        [Test(Description = "Check to make sure SendMessageReceived is correctly wired")]
        public void SendMessageReceivedWiredCorrectly()
        {
            var mocks = new MockRepository(MockBehavior.Default);
            var innerServer = mocks.Create<ICruiseServer>().Object;
            Mock.Get(innerServer).SetupAdd(_innerServer => _innerServer.SendMessageReceived += It.IsAny<EventHandler<CancelProjectEventArgs<Message>>>()).Verifiable();

            var configFile = this.CreateTemporaryConfigurationFile();
            var eventFired = false;
            using (var server = new RemoteCruiseServer(innerServer, configFile, true))
            {
                server.SendMessageReceived += (o, e) => { eventFired = true; };
                Mock.Get(innerServer).Raise(_innerServer => _innerServer.SendMessageReceived += null, new CancelProjectEventArgs<Message>("test", null));
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