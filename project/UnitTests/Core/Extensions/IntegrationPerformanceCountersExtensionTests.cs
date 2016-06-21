namespace ThoughtWorks.CruiseControl.UnitTests.Core.Extensions
{
    using NUnit.Framework;
    using Rhino.Mocks;
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
            this.mocks = new MockRepository();
        }

        [Test]
        public void StartAndStopDoesNothing()
        {
            var counters = this.InitialiseCounters();
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
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
            var counters = this.InitialiseCounters();
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
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

        [Test]
        public void SuccessfulIntegrationUpdatesSuccessCounter()
        {
            var counters = this.InitialiseCounters();
            Expect.Call(() => counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberCompletedCounter));
            Expect.Call(() => counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberTotalCounter));
            Expect.Call(() => counters.IncrementCounter(
                Arg<string>.Is.Equal(IntegrationPerformanceCountersExtension.CategoryName),
                Arg<string>.Is.Equal(IntegrationPerformanceCountersExtension.AverageTimeCounter), 
                Arg<long>.Is.Anything));
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            var completedRaiser = Expect.Call(() => serverMock
                .IntegrationCompleted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var startRaiser = Expect
                .Call(() => serverMock.IntegrationStarted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var request = new IntegrationRequest(BuildCondition.ForceBuild, "Testing", null);

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            startRaiser.Raise(serverMock, new IntegrationStartedEventArgs(request, "TestProject"));
            completedRaiser.Raise(serverMock, new IntegrationCompletedEventArgs(request, "TestProject", IntegrationStatus.Success));
            
            this.mocks.VerifyAll();
        }

        [Test]
        public void FailedIntegrationUpdatesFailureCounter()
        {
            var counters = this.InitialiseCounters();
            Expect.Call(() => counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberFailedCounter));
            Expect.Call(() => counters.IncrementCounter(
                IntegrationPerformanceCountersExtension.CategoryName,
                IntegrationPerformanceCountersExtension.NumberTotalCounter));
            Expect.Call(() => counters.IncrementCounter(
                Arg<string>.Is.Equal(IntegrationPerformanceCountersExtension.CategoryName),
                Arg<string>.Is.Equal(IntegrationPerformanceCountersExtension.AverageTimeCounter),
                Arg<long>.Is.Anything));
            var extension = new IntegrationPerformanceCountersExtension
                {
                    PerformanceCounters = counters
                };
            var serverMock = this.mocks.StrictMock<ICruiseServer>();
            var config = new ExtensionConfiguration();
            var completedRaiser = Expect.Call(() => serverMock
                .IntegrationCompleted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var startRaiser = Expect
                .Call(() => serverMock.IntegrationStarted += null)
                .IgnoreArguments()
                .GetEventRaiser();
            var request = new IntegrationRequest(BuildCondition.ForceBuild, "Testing", null);

            this.mocks.ReplayAll();
            extension.Initialise(serverMock, config);
            startRaiser.Raise(serverMock, new IntegrationStartedEventArgs(request, "TestProject"));
            completedRaiser.Raise(serverMock, new IntegrationCompletedEventArgs(request, "TestProject", IntegrationStatus.Failure));

            this.mocks.VerifyAll();
        }

        private IPerformanceCounters InitialiseCounters()
        {
            var counters = this.mocks.StrictMock<IPerformanceCounters>();
            Expect.Call(() => counters.EnsureCategoryExists(string.Empty, string.Empty)).IgnoreArguments();
            return counters;
        }
    }
}
