namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class UrlPingsTaskConditionTests
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
            var webMock = this.mocks.StrictMock<IWebFunctions>();
            Expect.Call(webMock.PingUrl("http://somewhere"))
                .Return(true);
            var condition = new UrlPingsTaskCondition
                {
                    Url = "http://somewhere",
                    WebFunctions = webMock
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void ValidatePassesWithUrl()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            var condition = new UrlPingsTaskCondition
                {
                    Url = "http://somewhere"
                };

            this.mocks.ReplayAll();
            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateThrowsErrorsWithoutUrl()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            var condition = new UrlPingsTaskCondition();
            Expect.Call(() => processor.ProcessError("URL cannot be empty"));

            this.mocks.ReplayAll();
            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }
    }
}
