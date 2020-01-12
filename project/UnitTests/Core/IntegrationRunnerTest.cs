using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class IntegrationRunnerTest : IntegrationFixture
    {
        private MockRepository mocks;

        private IntegrationRunner runner;

        private Mock<IIntegrationResultManager> resultManagerMock;
        private Mock<IIntegrationRunnerTarget> targetMock;
        private Mock<IIntegrationResult> resultMock;
        private Mock<IIntegrationResult> lastResultMock;
        private Mock<ISourceControl> sourceControlMock;
        private Mock<IQuietPeriod> quietPeriodMock;

        private IIntegrationResult result;
        private IIntegrationResult lastResult;

        private Modification[] modifications;
        private DateTime endTime;
        private IntegrationRequest request;
        private Mockery mockery;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);

            mockery = new Mockery();
            targetMock = mockery.NewDynamicMock<IIntegrationRunnerTarget>();
            resultManagerMock = mockery.NewDynamicMock<IIntegrationResultManager>();
            quietPeriodMock = mockery.NewDynamicMock<IQuietPeriod>();

            runner = new IntegrationRunner((IIntegrationResultManager)resultManagerMock.Object,
                                           (IIntegrationRunnerTarget)targetMock.Object,
                                           (IQuietPeriod)quietPeriodMock.Object);


            request = ModificationExistRequest();
            endTime = new DateTime(2005, 2, 1);
            modifications = new Modification[] { new Modification() };

            resultMock = mockery.NewDynamicMock<IIntegrationResult>();
            resultMock.SetupGet(_result => _result.WorkingDirectory).Returns(TempFileUtil.GetTempPath("workingDir"));
            resultMock.SetupGet(_result => _result.ArtifactDirectory).Returns(TempFileUtil.GetTempPath("artifactDir"));
            resultMock.SetupGet(_result => _result.BuildProgressInformation).Returns(new ThoughtWorks.CruiseControl.Core.Util.BuildProgressInformation("",string.Empty));
            resultMock.SetupGet(_result => _result.IntegrationProperties).Returns(new Dictionary<string, string>());
            result = (IIntegrationResult)resultMock.Object;

            lastResultMock = mockery.NewDynamicMock<IIntegrationResult>();
            lastResult = (IIntegrationResult)lastResultMock.Object;
        }

        [TearDown]
        public void Teardown()
        {
            TempFileUtil.DeleteTempDir("workingDir");
        }

        // ToDo - Move more tests over from ProjectTest

        [Test]
        public void ShouldNotRunBuildIfResultShouldNotBuild()
        {
            SetupPreambleExpections();
            resultMock.Setup(_result => _result.ShouldRunBuild()).Returns(false).Verifiable();
            targetMock.SetupSet(target => target.Activity = ProjectActivity.Sleeping).Verifiable();

            IIntegrationResult returnedResult = runner.Integrate(request);

            Assert.AreEqual(result, returnedResult);
            Assert.IsTrue(Directory.Exists(result.WorkingDirectory));
            Assert.IsTrue(Directory.Exists(result.ArtifactDirectory));
            mockery.Verify();
        }

        [Test]
        public void ShouldRunBuildIfResultShouldBuild()
        {
            SetupPreambleExpections();
            SetupShouldBuildExpectations();
            SetupBuildPassExpectations();

            IIntegrationResult returnedResult = runner.Integrate(request);

            Assert.AreEqual(result, returnedResult);
            mockery.Verify();
        }

        [Test]
        public void ShouldStillPublishResultsIfPrebuildThrowsException()
        {
            SetupPreambleExpections();

            resultMock.Setup(_result => _result.ShouldRunBuild()).Returns(true).Verifiable();
            resultMock.SetupGet(_result => _result.LastIntegrationStatus).Returns(IntegrationStatus.Success).Verifiable();

            targetMock.SetupSet(target => target.Activity = ProjectActivity.Building).Verifiable();
            sourceControlMock.Setup(sourceControl => sourceControl.GetSource(result)).Verifiable();
            targetMock.Setup(target => target.Prebuild(result)).Throws(new CruiseControlException("expected exception")).Verifiable();
            resultMock.SetupSet(_result => _result.ExceptionResult = It.IsAny<Exception>()).Verifiable();
            resultMock.Setup(_result => _result.MarkEndTime()).Verifiable();
            targetMock.SetupSet(target => target.Activity = ProjectActivity.Sleeping).Verifiable();
            resultMock.SetupGet(_result => _result.EndTime).Returns(endTime).Verifiable();
            resultMock.SetupGet(_result => _result.Status).Returns(IntegrationStatus.Exception).Verifiable();
            targetMock.Setup(target => target.PublishResults(result)).Verifiable();
            resultManagerMock.Setup(_manager => _manager.FinishIntegration()).Verifiable();

            runner.Integrate(ModificationExistRequest());
            mockery.Verify();
        }

        private void SetupPreambleExpections()
        {
            targetMock.SetupSet(target => target.Activity = ProjectActivity.CheckingModifications).Verifiable();
            resultManagerMock.Setup(_manager => _manager.StartNewIntegration(request)).Returns(result).Verifiable();
            resultMock.Setup(_result => _result.MarkStartTime()).Verifiable();
            resultManagerMock.SetupGet(_manager => _manager.LastIntegrationResult).Returns(lastResult).Verifiable();
            sourceControlMock = new Mock<ISourceControl>(MockBehavior.Strict);
            targetMock.SetupGet(target => target.SourceControl).Returns(sourceControlMock.Object).Verifiable();
            quietPeriodMock.Setup(period => period.GetModifications(sourceControlMock.Object, lastResult, result)).Returns(modifications).Verifiable();
            resultMock.SetupSet(_result => _result.Modifications = modifications).Verifiable();
        }

        private void SetupShouldBuildExpectations()
        {
            resultMock.Setup(_result => _result.ShouldRunBuild()).Returns(true).Verifiable();
            resultMock.SetupGet(_result => _result.LastIntegrationStatus).Returns(IntegrationStatus.Success).Verifiable();
            targetMock.SetupSet(target => target.Activity = ProjectActivity.Building).Verifiable();
            sourceControlMock.Setup(sourceControl => sourceControl.GetSource(result)).Verifiable();
            targetMock.Setup(target => target.Prebuild(result)).Verifiable();
            resultMock.SetupGet(_result => _result.Failed).Returns(false).Verifiable();
            targetMock.Setup(_result => _result.Run(result)).Verifiable();
            resultMock.Setup(_result => _result.MarkEndTime()).Verifiable();
            targetMock.SetupSet(target => target.Activity = ProjectActivity.Sleeping).Verifiable();
            resultMock.SetupGet(_result => _result.EndTime).Returns(endTime).Verifiable();
        }

        private void SetupBuildPassExpectations()
        {
            resultMock.SetupGet(_result => _result.Status).Returns(IntegrationStatus.Success).Verifiable();
            sourceControlMock.Setup(sourceControl => sourceControl.LabelSourceControl(result)).Verifiable();
            targetMock.Setup(target => target.PublishResults(result)).Verifiable();
            resultManagerMock.Setup(_manager => _manager.FinishIntegration()).Verifiable();           
        }

        #region GenerateSystemParameterValues() tests
        [Test]
        public void GenerateSystemParameterValuesShouldAddNewParameters()
        {
            // Initialise the test
            var runner = new IntegrationRunner(null, null, null);
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            var integrationProperties = new Dictionary<string, string>
            {
                {
                    "CCNetUser",
                    "John Doe"
                },
                {
                    "CCNetLabel",
                    "1.1"
                }
            };
            Mock.Get(result).SetupGet(_result => _result.IntegrationProperties).Returns(integrationProperties);
            var request = new IntegrationRequest(BuildCondition.ForceBuild, "Test", "John Doe");
            Mock.Get(result).SetupGet(_result => _result.IntegrationRequest).Returns(request);
            var parameters = new List<NameValuePair>();
            Mock.Get(result).SetupGet(_result => _result.Parameters).Returns(parameters);

            // Run the test
            runner.GenerateSystemParameterValues(result);

            // Check the results
            this.mocks.VerifyAll();
            Assert.AreEqual(2, request.BuildValues.Count);
            Assert.IsTrue(request.BuildValues.ContainsKey("$CCNetUser"));
            Assert.AreEqual("John Doe", request.BuildValues["$CCNetUser"]);
            Assert.IsTrue(request.BuildValues.ContainsKey("$CCNetLabel"));
            Assert.AreEqual("1.1", request.BuildValues["$CCNetLabel"]);
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("$CCNetUser", parameters[0].Name);
            Assert.AreEqual("John Doe", parameters[0].Value);
            Assert.AreEqual("$CCNetLabel", parameters[1].Name);
            Assert.AreEqual("1.1", parameters[1].Value);
        }

        [Test]
        public void GenerateSystemParameterValuesShouldUpdateExistingParameters()
        {
            // Initialise the test
            var runner = new IntegrationRunner(null, null, null);
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            var integrationProperties = new Dictionary<string, string>
            {
                {
                    "CCNetUser",
                    "John Doe"
                },
                {
                    "CCNetLabel",
                    "1.1"
                }
            };
            Mock.Get(result).SetupGet(_result => _result.IntegrationProperties).Returns(integrationProperties);
            var request = new IntegrationRequest(BuildCondition.ForceBuild, "Test", "John Doe");
            Mock.Get(result).SetupGet(_result => _result.IntegrationRequest).Returns(request);
            var parameters = new List<NameValuePair>();
            Mock.Get(result).SetupGet(_result => _result.Parameters).Returns(parameters);

            // Run the test
            runner.GenerateSystemParameterValues(result);
            integrationProperties["CCNetLabel"] = "1.2";
            runner.GenerateSystemParameterValues(result);

            // Check the results
            this.mocks.VerifyAll();
            Assert.AreEqual(2, request.BuildValues.Count);
            Assert.IsTrue(request.BuildValues.ContainsKey("$CCNetUser"));
            Assert.AreEqual("John Doe", request.BuildValues["$CCNetUser"]);
            Assert.IsTrue(request.BuildValues.ContainsKey("$CCNetLabel"));
            Assert.AreEqual("1.2", request.BuildValues["$CCNetLabel"]);
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("$CCNetUser", parameters[0].Name);
            Assert.AreEqual("John Doe", parameters[0].Value);
            Assert.AreEqual("$CCNetLabel", parameters[1].Name);
            Assert.AreEqual("1.2", parameters[1].Value);
        }
        #endregion
    }
}