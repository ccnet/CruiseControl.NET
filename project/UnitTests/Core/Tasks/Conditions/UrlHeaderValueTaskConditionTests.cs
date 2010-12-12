namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using NUnit.Framework;
    using Rhino.Mocks;
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
            this.mocks = new MockRepository();
        }

        [Test]
        public void EvaluateReturnsTrueIfValuesMatch()
        {
            var webMock = this.mocks.StrictMock<IWebFunctions>();
            Expect.Call(webMock.PingAndValidateHeaderValue("http://somewhere", "Key", "Value"))
                .Return(true);
            var condition = new UrlHeaderValueTaskCondition
                {
                    HeaderKey = "Key",
                    HeaderValue = "Value",
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
        public void ValidatePassesWithUrlAndHeaderKey()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            var condition = new UrlHeaderValueTaskCondition
                {
                    Url = "http://somewhere",
                    HeaderKey = "Key"
                };

            this.mocks.ReplayAll();
            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateThrowsErrorsWithoutUrl()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            var condition = new UrlHeaderValueTaskCondition
            {
                HeaderKey = "Key"
            };
            Expect.Call(() => processor.ProcessError("URL cannot be empty"));

            this.mocks.ReplayAll();
            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateThrowsErrorsWithoutHeaderKey()
        {
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            var condition = new UrlHeaderValueTaskCondition
                {
                    Url = "http://somewhere"
                };
            Expect.Call(() => processor.ProcessError("Header Key cannot be empty"));

            this.mocks.ReplayAll();
            condition.Validate(null, ConfigurationTrace.Start(this), processor);

            this.mocks.VerifyAll();
        }
    }
}
