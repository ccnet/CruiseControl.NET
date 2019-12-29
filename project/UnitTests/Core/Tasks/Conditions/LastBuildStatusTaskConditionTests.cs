namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Remote;

    public class LastBuildStatusTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
        }

        [Test]
        public void EvaluateReturnsTrueIfConditionIsMatched()
        {
            var condition = new LastBuildStatusTaskCondition
                {
                    Status = IntegrationStatus.Success
                };
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.IsInitial()).Returns(false).Verifiable();
            Mock.Get(result).SetupGet(_result => _result.LastBuildStatus).Returns(IntegrationStatus.Success).Verifiable();

            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfConditionIsNotMatched()
        {
            var condition = new LastBuildStatusTaskCondition
            {
                Status = IntegrationStatus.Success,
                Description = "Not equal test"
            };
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.IsInitial()).Returns(false).Verifiable();
            Mock.Get(result).SetupGet(_result => _result.LastBuildStatus).Returns(IntegrationStatus.Failure).Verifiable();

            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfNoPreviousBuilds()
        {
            var condition = new LastBuildStatusTaskCondition
                {
                    Status = IntegrationStatus.Success
                };
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.IsInitial()).Returns(true).Verifiable();

            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }
    }
}
