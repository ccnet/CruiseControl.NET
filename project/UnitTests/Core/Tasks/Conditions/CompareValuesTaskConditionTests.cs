namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;

    public class CompareValuesTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void EvaluateReturnsTrueIfValuesMatch()
        {
            var condition = new CompareValuesTaskCondition
                {
                    Value1 = "test",
                    Value2 = "test"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfValuesDontMatch()
        {
            var condition = new CompareValuesTaskCondition
                {
                    Value1 = "test1",
                    Value2 = "test2"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }

        [Test]
        public void EvaluateReturnsTrueIfValuesDontMatchAndTypeIsNotEqual()
        {
            var condition = new CompareValuesTaskCondition
                {
                    Value1 = "test1",
                    Value2 = "test2",
                    EvaluationType = CompareValuesTaskCondition.Evaluation.NotEqual
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateFailsIfTypeIsUnknown()
        {
            var condition = new CompareValuesTaskCondition
            {
                Value1 = "test1",
                Value2 = "test2",
                EvaluationType = (CompareValuesTaskCondition.Evaluation)99
            };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            Assert.Throws<ArgumentOutOfRangeException>(() => condition.Eval(result));

            this.mocks.VerifyAll();
        }
    }
}
