namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Triggers;

    public class ParameterTriggerTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void IntegrationCompletedShouldDelegateToInnerTrigger()
        {
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(() => innerTriggerMock.IntegrationCompleted());
            var trigger = new ParameterTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            mocks.ReplayAll();
            trigger.IntegrationCompleted();
            mocks.VerifyAll();
        }

        [Test]
        public void NextBuildShouldReturnInnerTriggerNextBuildIfUnknown()
        {
            var now = DateTime.Now;
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.NextBuild).Return(now);
            var trigger = new ParameterTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            mocks.ReplayAll();
            var actual = trigger.NextBuild;

            mocks.VerifyAll();
            Assert.AreEqual(now, actual);
        }

        [Test]
        public void FireDoesNothingIfInnerTriggerDoesNotFire()
        {
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.Fire()).Return(null);
            var trigger = new ParameterTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            mocks.ReplayAll();
            var actual = trigger.Fire();

            mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void FirePassesOnParameters()
        {
            var parameters = new[] 
                {
                    new NameValuePair("test", "testValue")
                };
            var request = new IntegrationRequest(BuildCondition.IfModificationExists, "test", null);
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.Fire()).Return(request);
            var trigger = new ParameterTrigger
                {
                    InnerTrigger = innerTriggerMock,
                    Parameters = parameters
                };
            mocks.ReplayAll();
            var actual = trigger.Fire();

            mocks.VerifyAll();
            Assert.AreSame(request, actual);
            Assert.AreEqual(1, request.BuildValues.Count);
            Assert.AreEqual(parameters[0].Value,
                request.BuildValues[parameters[0].Name]);
        }

        [Test]
        public void FireMandlesMissingParameters()
        {
            var request = new IntegrationRequest(BuildCondition.IfModificationExists, "test", null);
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.Fire()).Return(request);
            var trigger = new ParameterTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            mocks.ReplayAll();
            var actual = trigger.Fire();

            mocks.VerifyAll();
            Assert.AreSame(request, actual);
            Assert.AreEqual(0, request.BuildValues.Count);
        }
    }
}
