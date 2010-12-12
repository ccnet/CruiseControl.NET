namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Remote;

    public class BuildConditionTaskConditionTests
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
            var condition = new BuildConditionTaskCondition
            {
                BuildCondition = BuildCondition.ForceBuild
            };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.BuildCondition).Return(BuildCondition.ForceBuild);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfConditionIsNotMatched()
        {
            var condition = new BuildConditionTaskCondition
                {
                    BuildCondition = BuildCondition.ForceBuild,
                    Description = "Not equal test"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.BuildCondition).Return(BuildCondition.IfModificationExists);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }
    }
}
