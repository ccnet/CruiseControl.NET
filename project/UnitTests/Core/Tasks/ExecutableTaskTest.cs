using System.IO;
using Exortech.NetReflector;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class ExecutableTaskTest : ProcessExecutorTestFixtureBase
	{
		private const string DefaultExecutable = "run.bat";
		private const string DefaultArgs = "out.txt";
		public const int SUCCESSFUL_EXIT_CODE = 0;
		public const int FAILED_EXIT_CODE = -1;

		private ExecutableTask task;

		[SetUp]
		public void SetUp()
		{
			CreateProcessExecutorMock(DefaultExecutable);
			task = new ExecutableTask((ProcessExecutor) mockProcessExecutor.MockInstance);
			task.Executable = DefaultExecutable;
			task.BuildArgs = DefaultArgs;
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"
    <exec>
    	<executable>mybatchfile.bat</executable>
    	<baseDirectory>C:\</baseDirectory>
		<buildArgs>myarg1 myarg2</buildArgs>
		<buildTimeoutSeconds>123</buildTimeoutSeconds>
    </exec>";

			task = (ExecutableTask) NetReflector.Read(xml);
			Assert.AreEqual(@"C:\", task.ConfiguredBaseDirectory);
			Assert.AreEqual("mybatchfile.bat", task.Executable);
			Assert.AreEqual(123, task.BuildTimeoutSeconds);
			Assert.AreEqual("myarg1 myarg2", task.BuildArgs);
			Verify();
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			string xml = @"
    <exec>
    	<executable>mybatchfile.bat</executable>
    </exec>";

			task = (ExecutableTask) NetReflector.Read(xml);
			Assert.AreEqual("mybatchfile.bat", task.Executable);
			Assert.AreEqual(600, task.BuildTimeoutSeconds);
			Assert.AreEqual("", task.BuildArgs);
			Verify();
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ExpectToExecuteArguments(DefaultArgs);

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(ProcessResultOutput, result.TaskOutput);
			Verify();
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ExpectToExecuteAndReturn(FailedProcessResult());

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			Assert.AreEqual(ProcessResultOutput, result.TaskOutput);
			Verify();
		}

		// TODO - Timeout?
		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			ExpectToExecuteAndThrow();

			task.Run(IntegrationResult());
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), constraint);

			IntegrationResult result = new IntegrationResult();
			result.Label = "1.0";
			result.BuildCondition = BuildCondition.ForceBuild;
			result.WorkingDirectory = @"c:\workingdir\";
			result.ArtifactDirectory = @"c:\artifactdir\";

			task.Executable = "test-exe";
			task.BuildArgs = "test-args";
			task.BuildTimeoutSeconds = 222;
			task.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual("test-exe", info.FileName);
			Assert.AreEqual(222000, info.TimeOut);
			Assert.AreEqual("test-args", info.Arguments);
			Assert.AreEqual("1.0", info.EnvironmentVariables["CCNetLabel"]);
			Assert.AreEqual("ForceBuild", info.EnvironmentVariables["CCNetBuildCondition"]);
			Assert.AreEqual(@"c:\workingdir\", info.EnvironmentVariables["CCNetWorkingDirectory"]);
			Assert.AreEqual(@"c:\artifactdir\", info.EnvironmentVariables["CCNetArtifactDirectory"]);
			Verify();
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotSetUseProjectWorkingDirectoryAsBaseDirectory()
		{
			task.ConfiguredBaseDirectory = null;
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsEmptyUseProjectWorkingDirectoryAsBaseDirectory()
		{
			task.ConfiguredBaseDirectory = "";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotAbsoluteUseProjectWorkingDirectoryAsFirstPartOfBaseDirectory()
		{
			task.ConfiguredBaseDirectory = "relativeBaseDirectory";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("relativeBaseDirectory");
		}

		private void CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart(string relativeDirectory)
		{
			string expectedBaseDirectory = "projectWorkingDirectory";
			if (relativeDirectory != "")
			{
				expectedBaseDirectory = Path.Combine(expectedBaseDirectory, relativeDirectory);
			}
			CheckBaseDirectory(new IntegrationResult("project", "projectWorkingDirectory"), expectedBaseDirectory);
			Verify();
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
			task.ConfiguredBaseDirectory = @"c:\my\base\directory";
			CheckBaseDirectory(new IntegrationResult("project", "projectWorkingDirectory"), @"c:\my\base\directory");
		}

		private void CheckBaseDirectory(IntegrationResult result, string expectedBaseDirectory)
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), constraint);

			task.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
			Verify();
		}
	}
}