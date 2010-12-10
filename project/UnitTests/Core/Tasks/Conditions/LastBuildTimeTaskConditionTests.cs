namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class LastBuildTimeTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void EvaluateReturnsTrueIfBeyondTime()
        {
            Func<IIntegrationResult, bool> evalFunc = ir => true;
            var condition = new LastBuildTimeTaskCondition
                {
                    Time = new Timeout(1000)
                };
            var status = new IntegrationSummary(IntegrationStatus.Success, "1", "1", DateTime.Now.AddHours(-1));
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.IsInitial()).Return(false);
            Expect.Call(result.LastIntegration).Return(status);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfWithinTime()
        {
            Func<IIntegrationResult, bool> evalFunc = ir => true;
            var condition = new LastBuildTimeTaskCondition
                {
                    Time = new Timeout(1000),
                    Description = "Not equal test"
                };
            var status = new IntegrationSummary(IntegrationStatus.Success, "1", "1", DateTime.Now);
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.IsInitial()).Return(false);
            Expect.Call(result.LastIntegration).Return(status);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }

        [Test]
        public void EvaluateReturnsTrueIfNoPreviousBuilds()
        {
            Func<IIntegrationResult, bool> evalFunc = ir => true;
            var condition = new LastBuildTimeTaskCondition
                {
                    Time = new Timeout(1000)
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.IsInitial()).Return(true);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }
    }
}
