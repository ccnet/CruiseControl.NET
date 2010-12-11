namespace ThoughtWorks.CruiseControl.UnitTests.Core.Extensions
{
    using System.Xml;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Extensions;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Events;

    [TestFixture]
    public class IntegrationRequestThrottleExtensionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void InitialiseLoadsNumberOfRequestsAllowed()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            config.Items = new[] 
                {
                    GenerateElement("limit", "10")
                };
            Expect.Call(() => serverMock.IntegrationCompleted += null).IgnoreArguments();
            Expect.Call(() => serverMock.IntegrationStarted += null).IgnoreArguments();

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);

            this.mocks.VerifyAll();
            Assert.AreEqual(10, extension.NumberOfRequestsAllowed);
        }

        [Test]
        public void IntegrationStartIsAllowedWhenWithinLimit()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            Expect.Call(() => serverMock.IntegrationCompleted += null).IgnoreArguments();
            var startRaiser = Expect
                .Call(() => serverMock.IntegrationStarted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var eventArgs = new IntegrationStartedEventArgs(null, "TestProject");

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            startRaiser.Raise(serverMock, eventArgs);

            this.mocks.VerifyAll();
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Continue, eventArgs.Result);
        }

        [Test]
        public void IntegrationStartIsDelayedBeyondLimit()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            Expect.Call(() => serverMock.IntegrationCompleted += null).IgnoreArguments();
            var startRaiser = Expect
                .Call(() => serverMock.IntegrationStarted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var eventArgs = new IntegrationStartedEventArgs(null, "TestProject");

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            extension.NumberOfRequestsAllowed = 1;
            startRaiser.Raise(serverMock, new IntegrationStartedEventArgs(null, "First"));
            startRaiser.Raise(serverMock, eventArgs);

            this.mocks.VerifyAll();
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Delay, eventArgs.Result);
        }

        [Test]
        public void IntegrationStartAllowedAtTopOfQueue()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            var completeRaiser = Expect
                .Call(() => serverMock.IntegrationCompleted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var startRaiser = Expect
                .Call(() => serverMock.IntegrationStarted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var eventArgs = new IntegrationStartedEventArgs(null, "TestProject");

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            extension.NumberOfRequestsAllowed = 1;
            startRaiser.Raise(serverMock, new IntegrationStartedEventArgs(null, "First"));
            startRaiser.Raise(serverMock, eventArgs);
            startRaiser.Raise(serverMock, new IntegrationStartedEventArgs(null, "Third"));
            completeRaiser.Raise(
                serverMock,
                new IntegrationCompletedEventArgs(null, "First", IntegrationStatus.Success));
            startRaiser.Raise(serverMock, eventArgs);

            this.mocks.VerifyAll();
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Continue, eventArgs.Result);
        }

        [Test]
        public void IntegrationCompleteClearsSlot()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            var completeRaiser = Expect
                .Call(() => serverMock.IntegrationCompleted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var startRaiser = Expect
                .Call(() => serverMock.IntegrationStarted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var eventArgs = new IntegrationStartedEventArgs(null, "TestProject");

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            extension.NumberOfRequestsAllowed = 1;
            startRaiser.Raise(serverMock, new IntegrationStartedEventArgs(null, "First"));
            startRaiser.Raise(serverMock, eventArgs);
            completeRaiser.Raise(
                serverMock,
                new IntegrationCompletedEventArgs(null, "First", IntegrationStatus.Success));
            startRaiser.Raise(serverMock, eventArgs);

            this.mocks.VerifyAll();
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Continue, eventArgs.Result);
        }

        [Test]
        public void StartAndStopDoesNothing()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            Expect.Call(() => serverMock.IntegrationCompleted += null).IgnoreArguments();
            Expect.Call(() => serverMock.IntegrationStarted += null).IgnoreArguments();

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            extension.Start();
            extension.Stop();

            this.mocks.VerifyAll();
        }

        [Test]
        public void StartAndAbortDoesNothing()
        {
            var extension = new IntegrationRequestThrottleExtension();
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            Expect.Call(() => serverMock.IntegrationCompleted += null).IgnoreArguments();
            Expect.Call(() => serverMock.IntegrationStarted += null).IgnoreArguments();

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            extension.Start();
            extension.Abort();

            this.mocks.VerifyAll();
        }

        private XmlElement GenerateElement(string name, string value)
        {
            var xmlDoc = new XmlDocument();
            var element = xmlDoc.CreateElement(name);
            element.InnerXml = value;
            return element;
        }
    }
}
