namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class UrlHeaderValueTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
        }

        [Test]
        public void EvaluateReturnsTrueIfValuesMatch()
        {
            var webMock = this.mocks.Create<IWebFunctions>(MockBehavior.Strict).Object;
            Mock.Get(webMock).Setup(_webMock => _webMock.PingAndValidateHeaderValue("http://somewhere", "Key", "Value"))
                .Returns(true).Verifiable();
            var condition = new UrlHeaderValueTaskCondition
                {
                    HeaderKey = "Key",
                    HeaderValue = "Value",
                    Url = "http://somewhere",
                    WebFunctions = webMock
                };
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;

            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void ValidatePassesWithUrlAndHeaderKey()
        {
            var processor = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            var condition = new UrlHeaderValueTaskCondition
                {
                    Url = "http://somewhere",
                    HeaderKey = "Key"
                };

            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateThrowsErrorsWithoutUrl()
        {
            var processor = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            var condition = new UrlHeaderValueTaskCondition
            {
                HeaderKey = "Key"
            };
            Mock.Get(processor).Setup(_processor => _processor.ProcessError("URL cannot be empty")).Verifiable();

            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateThrowsErrorsWithoutHeaderKey()
        {
            var processor = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            var condition = new UrlHeaderValueTaskCondition
                {
                    Url = "http://somewhere"
                };
            Mock.Get(processor).Setup(_processor => _processor.ProcessError("Header Key cannot be empty")).Verifiable();

            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }
    }
}
