namespace ThoughtWorks.CruiseControl.UnitTests.Core.Extensions
{
    using System;
    using System.Diagnostics;
    using Moq;
    using NUnit.Framework;
    using CruiseControl.Core.Extensions;
    using CruiseControl.Remote;
    using CruiseControl.Core.Util;
    using CruiseControl.Remote.Events;

    [TestFixture]
    public class IntegrationPerformanceCountersExtensionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository(MockBehavior.Default);
        }

        [Test]
        public void StartAndStopDoesNothing()
        {
            var counters = this.InitialiseCounters();
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
            var serverMock = this.mocks.Create<ICruiseServer>(MockBehavior.Strict).Object;
            var config = new ExtensionConfiguration();
            Mock.Get(serverMock)
                .SetupAdd(_serverMock => _serverMock.IntegrationCompleted += It.IsAny<EventHandler<IntegrationCompletedEventArgs>>()).Verifiable();
            Mock.Get(serverMock)
                .SetupAdd(_serverMock => _serverMock.IntegrationStarted += It.IsAny<EventHandler<IntegrationStartedEventArgs>>()).Verifiable();

            extension.Initialise(serverMock, config);
            extension.Start();
            extension.Stop();

            mocks.VerifyAll();
        }

        [Test]
        public void StartAndAbortDoesNothing()
        {
            var counters = this.InitialiseCounters();
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
            var serverMock = this.mocks.Create<ICruiseServer>(MockBehavior.Strict).Object;
            var config = new ExtensionConfiguration();
            Mock.Get(serverMock)
                .SetupAdd(_serverMock => _serverMock.IntegrationCompleted += It.IsAny<EventHandler<IntegrationCompletedEventArgs>>()).Verifiable();
            Mock.Get(serverMock)
                .SetupAdd(_serverMock => _serverMock.IntegrationStarted += It.IsAny<EventHandler<IntegrationStartedEventArgs>>()).Verifiable();

            extension.Initialise(serverMock, config);
            extension.Start();
            extension.Abort();

            mocks.VerifyAll();
        }

        [Test]
        public void SuccessfulIntegrationUpdatesSuccessCounter()
        {
            var counters = this.InitialiseCounters();
            Mock.Get(counters).Setup(_counters => _counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberCompletedCounter)).Verifiable();
            Mock.Get(counters).Setup(_counters => _counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberTotalCounter)).Verifiable();
            Mock.Get(counters).Setup(_counters => _counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.AverageTimeCounter,
                It.IsAny<long>())).Verifiable();
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
            var serverMock = this.mocks.Create<ICruiseServer>(MockBehavior.Strict).Object;
            var config = new ExtensionConfiguration();
            Mock.Get(serverMock).SetupAdd(_serverMock => _serverMock.IntegrationCompleted += It.IsAny<EventHandler<IntegrationCompletedEventArgs>>()).Verifiable();
            Mock.Get(serverMock).SetupAdd(_serverMock => _serverMock.IntegrationStarted += It.IsAny<EventHandler<IntegrationStartedEventArgs>>()).Verifiable();
            var request = new IntegrationRequest(BuildCondition.ForceBuild, "Testing", null);

            extension.Initialise(serverMock, config);
            Mock.Get(serverMock).Raise(_serverMock => _serverMock.IntegrationStarted += null, new IntegrationStartedEventArgs(request, "TestProject"));
            Mock.Get(serverMock).Raise(_serverMock => _serverMock.IntegrationCompleted += null, new IntegrationCompletedEventArgs(request, "TestProject", IntegrationStatus.Success));

            mocks.VerifyAll();
        }

        [Test]
        public void FailedIntegrationUpdatesFailureCounter()
        {
            var counters = InitialiseCounters();
            Mock.Get(counters).Setup(_counters => _counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberFailedCounter)).Verifiable();
            Mock.Get(counters).Setup(_counters => _counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberTotalCounter)).Verifiable();
            Mock.Get(counters).Setup(_counters => _counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.AverageTimeCounter,
                It.IsAny<long>())).Verifiable();
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
            var serverMock = this.mocks.Create<ICruiseServer>(MockBehavior.Strict).Object;
            var config = new ExtensionConfiguration();
            Mock.Get(serverMock).SetupAdd(_serverMock => _serverMock.IntegrationCompleted += It.IsAny<EventHandler<IntegrationCompletedEventArgs>>()).Verifiable();
            Mock.Get(serverMock).SetupAdd(_serverMock => _serverMock.IntegrationStarted += It.IsAny<EventHandler<IntegrationStartedEventArgs>>()).Verifiable();
            var request = new IntegrationRequest(BuildCondition.ForceBuild, "Testing", null);

            extension.Initialise(serverMock, config);
            Mock.Get(serverMock).Raise(_serverMock => _serverMock.IntegrationStarted += null, new IntegrationStartedEventArgs(request, "TestProject"));
            Mock.Get(serverMock).Raise(_serverMock => _serverMock.IntegrationCompleted += null, new IntegrationCompletedEventArgs(request, "TestProject", IntegrationStatus.Failure));

            mocks.VerifyAll();
        }

        private IPerformanceCounters InitialiseCounters()
        {
            var counters = this.mocks.Create<IPerformanceCounters>(MockBehavior.Strict).Object;
            Mock.Get(counters)
                .Setup(_counters => _counters.EnsureCategoryExists(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CounterCreationData[]>()))
                .Verifiable();

            return counters;
        }
    }
}
