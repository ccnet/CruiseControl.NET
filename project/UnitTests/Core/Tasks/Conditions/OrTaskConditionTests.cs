namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Core;
    using System;

    public class OrTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void ValidateRaisesAnErrorIfNullChildConditionsDefined()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() => processor.ProcessError(
                "Validation failed for orCondition - at least one child condition must be supplied"));

            this.mocks.ReplayAll();
            var condition = new OrTaskCondition();
            condition.Validate(null, null, processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateRaisesAnErrorIfNoChildConditionsDefined()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() => processor.ProcessError(
                "Validation failed for orCondition - at least one child condition must be supplied"));

            this.mocks.ReplayAll();
            var condition = new OrTaskCondition
            {
                Conditions = new ITaskCondition[0]
            };
            condition.Validate(null, null, processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateCallsChildrenValidation()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            var validateCalled = false;
            var childCondition = new MockCondition
            {
                ValidateAction = (c, t, ep) => validateCalled = true
            };

            this.mocks.ReplayAll();
            var condition = new OrTaskCondition
            {
                Conditions = new[] { childCondition }
            };
            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
            Assert.IsTrue(validateCalled);
        }

        [Test]
        public void EvaluateReturnsFalseIfAllChildrenAreFalse()
        {
            Func<IIntegrationResult, bool> evalFunc = ir => false;
            var condition = new OrTaskCondition
            {
                Conditions = new[] 
                        {
                            new MockCondition { EvalFunction = evalFunc },
                            new MockCondition { EvalFunction = evalFunc }
                        }
            };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }

        [Test]
        public void EvaluateReturnsTrueIfOneChildIsTrue()
        {
            var count = 0;
            Func<IIntegrationResult, bool> evalFunc = ir => (count++) % 2 == 0;
            var condition = new OrTaskCondition
            {
                Conditions = new[] 
                        {
                            new MockCondition { EvalFunction = evalFunc },
                            new MockCondition { EvalFunction = evalFunc }
                        }
            };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }
    }
}
