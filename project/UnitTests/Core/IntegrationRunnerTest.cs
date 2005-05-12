using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationRunnerTest
	{
		private IntegrationRunner runner;

		private DynamicMock resultManagerMock;
		private DynamicMock targetMock;
		private DynamicMock resultMock;
		private DynamicMock lastResultMock;
		private DynamicMock sourceControlMock;

		private IIntegrationResult result;
		private IIntegrationResult lastResult;

		private Modification[] modifications;
		private DateTime time1;
		private DateTime time2;
		private DateTime time3;
		private int modificationDelay = 55;

		[SetUp]
		public void Setup()
		{
			targetMock = new DynamicMock(typeof(IIntegrationRunnerTarget));
			targetMock.Strict = true;

			resultManagerMock = new DynamicMock(typeof(IIntegrationResultManager));
			resultManagerMock.Strict = true;

			runner = new IntegrationRunner((IIntegrationResultManager) resultManagerMock.MockInstance,
				(IIntegrationRunnerTarget) targetMock.MockInstance);

			resultMock = new DynamicMock(typeof(IIntegrationResult));
			resultMock.Strict = true;
			result = (IIntegrationResult) resultMock.MockInstance;

			lastResultMock = new DynamicMock(typeof(IIntegrationResult));
			lastResultMock.Strict = true;
			lastResult = (IIntegrationResult) lastResultMock.MockInstance;

			time1 = new DateTime(2004,1,1);
			time2 = new DateTime(2005,1,1);
			time3 = new DateTime(2005,2,1);
			modifications = new Modification[] { new Modification() };

			targetMock.SetupResult("WorkingDirectory", TempFileUtil.CreateTempDir("workingDir"));
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
			resultMock.ExpectAndReturn("ShouldRunBuild", false, modificationDelay);
			resultMock.Expect("MarkEndTime");
			targetMock.Expect("Activity", ProjectActivity.Sleeping);
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Unknown);
			resultMock.ExpectAndReturn("EndTime", time3);

			IIntegrationResult returnedResult = runner.RunIntegration(BuildCondition.IfModificationExists);

			Assert.AreEqual(result, returnedResult);
			VerifyAll();
		}

		[Test]
		public void ShouldRunBuildIfResultShouldBuild()
		{
			SetupPreambleExpections();
			SetupShouldBuildExpectations();
			SetupBuildPassExpectations();

			IIntegrationResult returnedResult = runner.RunIntegration(BuildCondition.IfModificationExists);

			Assert.AreEqual(result, returnedResult);
			VerifyAll();
		}

		private void SetupPreambleExpections()
		{
			SetupPreambleExpections(typeof(ISourceControl));
		}

		private void SetupPreambleExpections(Type sourceControlType)
		{
			targetMock.Expect("Activity", ProjectActivity.CheckingModifications);
			resultManagerMock.ExpectAndReturn("StartNewIntegration", result, BuildCondition.IfModificationExists);
			resultMock.Expect("MarkStartTime");
			resultManagerMock.ExpectAndReturn("LastIntegrationResult", lastResult);
			lastResultMock.ExpectAndReturn("StartTime", time1);
			resultMock.ExpectAndReturn("StartTime", time2);
			sourceControlMock = new DynamicMock(sourceControlType);
			sourceControlMock.Strict = true;
			targetMock.SetupResult("SourceControl", sourceControlMock.MockInstance);
			sourceControlMock.ExpectAndReturn("GetModifications", modifications, time1, time2);
			resultMock.ExpectAndReturn("Modifications", modifications);
			targetMock.ExpectAndReturn("ModificationDelaySeconds", modificationDelay);
		}

		private void SetupShouldBuildExpectations()
		{
			resultMock.ExpectAndReturn("ShouldRunBuild", true, modificationDelay);

			targetMock.Expect("Activity", ProjectActivity.Building);

			sourceControlMock.Expect("GetSource", result);
			resultMock.ExpectAndReturn("BuildCondition", BuildCondition.IfModificationExists);

			targetMock.Expect("Run", result);
			resultMock.Expect("MarkEndTime");
			resultManagerMock.Expect("FinishIntegration");
			targetMock.Expect("Activity", ProjectActivity.Sleeping);
			resultMock.ExpectAndReturn("EndTime", time3);
		}

		private void SetupBuildPassExpectations()
		{
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Success);
			resultMock.ExpectAndReturn("Status", IntegrationStatus.Success);
			resultMock.ExpectAndReturn("Label", "mylabel");
			sourceControlMock.Expect("LabelSourceControl", "mylabel", result);
			targetMock.Expect("OnIntegrationCompleted", result);
		}
	}
}
