namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Remote;

    public class StatusTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void EvaluateReturnsTrueIfConditionIsMatched()
        {
            var condition = new StatusTaskCondition
                {
                    Status = IntegrationStatus.Success
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.Status).Return(IntegrationStatus.Success);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfConditionIsNotMatched()
        {
            var condition = new StatusTaskCondition
            {
                Status = IntegrationStatus.Success,
                Description = "Not equal test"
            };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.Status).Return(IntegrationStatus.Failure);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }
    }
}
