namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class CommentTaskTests
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }
        #endregion

        #region Tests
        [Test]
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            const string xml = @"<commentTask><message>Test Message</message></commentTask>";
            var task = NetReflector.Read(xml) as CommentTask;
            Assert.AreEqual("Test Message", task.Message);
        }

        [Test]
        public void ShouldLoadExtendedValuesFromConfiguration()
        {
            const string xml = @"<commentTask failure=""true""><message>Test Message</message></commentTask>";
            var task = NetReflector.Read(xml) as CommentTask;
            Assert.AreEqual("Test Message", task.Message);
            Assert.IsTrue(task.FailTask);
        }

        [Test]
        public void ExecuteLogsMessage()
        {
            var buildInfo = this.mocks.StrictMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = this.mocks.StrictMock<IIntegrationResult>();
            var logger = this.mocks.StrictMock<ILogger>();
            SetupResult.For(result.Status).PropertyBehavior();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            Expect.Call(() => buildInfo.SignalStartRunTask("Adding a comment to the log"));
            Expect.Call(() => logger.Debug("Logging message: Test Message"));
            GeneralTaskResult loggedResult = null;
            Expect.Call(() => result.AddTaskResult(
                Arg<GeneralTaskResult>.Matches<GeneralTaskResult>(arg =>
                {
                    loggedResult = arg;
                    return true;
                })));
            var task = new CommentTask
                {
                    Message = "Test Message",
                    Logger = logger
                };
            result.Status = IntegrationStatus.Unknown;

            this.mocks.ReplayAll();
            task.Run(result);

            this.mocks.VerifyAll();
            Assert.IsNotNull(loggedResult);
            Assert.IsTrue(loggedResult.CheckIfSuccess());
            Assert.AreEqual("Test Message", loggedResult.Data);
        }

        [Test]
        public void ExecuteLogsFailMessage()
        {
            var buildInfo = this.mocks.StrictMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = this.mocks.StrictMock<IIntegrationResult>();
            var logger = this.mocks.StrictMock<ILogger>();
            SetupResult.For(result.Status).PropertyBehavior();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            Expect.Call(() => buildInfo.SignalStartRunTask("Adding a comment to the log"));
            Expect.Call(() => logger.Debug("Logging error message: Test Message"));
            GeneralTaskResult loggedResult = null;
            Expect.Call(() => result.AddTaskResult(
                Arg<GeneralTaskResult>.Matches<GeneralTaskResult>(arg =>
                {
                    loggedResult = arg;
                    return true;
                })));
            var task = new CommentTask
            {
                FailTask = true,
                Message = "Test Message",
                Logger = logger
            };
            result.Status = IntegrationStatus.Unknown;

            this.mocks.ReplayAll();
            task.Run(result);

            this.mocks.VerifyAll();
            Assert.IsNotNull(loggedResult);
            Assert.IsFalse(loggedResult.CheckIfSuccess());
            Assert.AreEqual("Test Message", loggedResult.Data);
        }
        #endregion
    }
}
