using System;
using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationRunnerTest : IntegrationFixture
	{
		private IntegrationRunner runner;

		private DynamicMock resultManagerMock;
		private DynamicMock targetMock;
		private DynamicMock resultMock;
		private DynamicMock lastResultMock;
		private DynamicMock sourceControlMock;
		private IMock quietPeriodMock;

		private IIntegrationResult result;
		private IIntegrationResult lastResult;

		private Modification[] modifications;
		private DateTime time3;
		private IntegrationRequest request;

		[SetUp]
		public void Setup()
		{
			targetMock = new DynamicMock(typeof (IIntegrationRunnerTarget));
			targetMock.Strict = true;

			resultManagerMock = new DynamicMock(typeof (IIntegrationResultManager));
			resultManagerMock.Strict = true;
			request = ModificationExistRequest();

			quietPeriodMock = new DynamicMock(typeof (IQuietPeriod));

			runner = new IntegrationRunner((IIntegrationResultManager) resultManagerMock.MockInstance,
			                               (IIntegrationRunnerTarget) targetMock.MockInstance,
			                               (IQuietPeriod) quietPeriodMock.MockInstance);

			resultMock = new DynamicMock(typeof (IIntegrationResult));
			resultMock.Strict = true;
			result = (IIntegrationResult) resultMock.MockInstance;

			lastResultMock = new DynamicMock(typeof (IIntegrationResult));
			lastResultMock.Strict = true;
			lastResult = (IIntegrationResult) lastResultMock.MockInstance;

			time3 = new DateTime(2005, 2, 1);
			modifications = new Modification[] {new Modification()};

			resultMock.SetupResult("WorkingDirectory", TempFileUtil.GetTempPath("workingDir"));
			resultMock.SetupResult("ArtifactDirectory", TempFileUtil.GetTempPath("artifactDir"));
		}

		[TearDown]
		public void Teardown()
		{
			TempFileUtil.DeleteTempDir("workingDir");
		}

		private void VerifyAll()
		{
			resultManagerMock.Verify();
			targetMock.Verify();
			resultMock.Verify();
			lastResultMock.Verify();
			sourceControlMock.Verify();
		}

		// ToDo - Move more tests over from ProjectTest

		[Test]
		public void ShouldNotRunBuildIfResultShouldNotBuild()
		{
			SetupPreambleExpections();
			resultMock.ExpectAndReturn("ShouldRunBuild", false);
			resultMock.Expect("MarkEndTime");
			targetMock.Expect("Activity", ProjectActivity.Sleeping);
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Unknown);
			resultMock.ExpectAndReturn("EndTime", time3);

			IIntegrationResult returnedResult = runner.Integrate(request);

			Assert.AreEqual(result, returnedResult);
			Assert.IsTrue(Directory.Exists(result.WorkingDirectory));
			Assert.IsTrue(Directory.Exists(result.ArtifactDirectory));
			VerifyAll();
		}

		[Test]
		public void ShouldRunBuildIfResultShouldBuild()
		{
			SetupPreambleExpections();
			SetupShouldBuildExpectations();
			SetupBuildPassExpectations();

			IIntegrationResult returnedResult = runner.Integrate(request);

			Assert.AreEqual(result, returnedResult);
			VerifyAll();
		}

		[Test]
		public void ShouldStillPublishResultsIfLabellingThrowsException()
		{
			SetupPreambleExpections();
			SetupShouldBuildExpectations();
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Success);
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Success);
			sourceControlMock.ExpectAndThrow("LabelSourceControl", new Exception(), result);
			targetMock.Expect("PublishResults", result);
			resultManagerMock.Expect("FinishIntegration");

			IIntegrationResult returnedResult = runner.Integrate(request);

			Assert.AreEqual(result, returnedResult);
			VerifyAll();
		}

		private void SetupPreambleExpections()
		{
			SetupPreambleExpections(typeof (ISourceControl));
		}

		private void SetupPreambleExpections(Type sourceControlType)
		{
			targetMock.Expect("Activity", ProjectActivity.CheckingModifications);
			resultManagerMock.ExpectAndReturn("StartNewIntegration", result, request);
			resultMock.Expect("MarkStartTime");
			resultManagerMock.ExpectAndReturn("LastIntegrationResult", lastResult);
			sourceControlMock = new DynamicMock(sourceControlType);
			sourceControlMock.Strict = true;
			targetMock.SetupResult("SourceControl", sourceControlMock.MockInstance);
			quietPeriodMock.ExpectAndReturn("GetModifications", modifications, sourceControlMock.MockInstance, lastResult, result);
			resultMock.ExpectAndReturn("Modifications", modifications);
		}

		private void SetupShouldBuildExpectations()
		{
			resultMock.ExpectAndReturn("ShouldRunBuild", true);
			targetMock.Expect("Activity", ProjectActivity.Building);
			sourceControlMock.Expect("GetSource", result);
			targetMock.Expect("Run", result);
			resultMock.Expect("MarkEndTime");
			targetMock.Expect("Activity", ProjectActivity.Sleeping);
			resultMock.ExpectAndReturn("EndTime", time3);
		}

		private void SetupBuildPassExpectations()
		{
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Success);
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Success);
			sourceControlMock.Expect("LabelSourceControl", result);
			targetMock.Expect("PublishResults", result);
			resultManagerMock.Expect("FinishIntegration");
		}
	}
}