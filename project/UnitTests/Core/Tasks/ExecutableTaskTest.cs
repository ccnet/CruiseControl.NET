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
		<successExitCodes>0,1,3,5</successExitCodes>
    </exec>";

			task = (ExecutableTask) NetReflector.Read(xml);
			Assert.AreEqual(@"C:\", task.ConfiguredBaseDirectory);
			Assert.AreEqual("mybatchfile.bat", task.Executable);
			Assert.AreEqual(123, task.BuildTimeoutSeconds);
			Assert.AreEqual("myarg1 myarg2", task.BuildArgs);
			Assert.AreEqual("0,1,3,5", task.SuccessExitCodes);
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
			Assert.AreEqual("", task.SuccessExitCodes);
			Verify();
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ExpectToExecuteArgumentsWithMonitor(DefaultArgs);

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
            Assert.AreEqual(System.Environment.NewLine + "<buildresults>" + System.Environment.NewLine + "  <message>" 
                + ProcessResultOutput + "</message>" + System.Environment.NewLine + "</buildresults>" 
                + System.Environment.NewLine, result.TaskOutput);
            Verify();
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ExpectToExecuteAndReturnWithMonitor(FailedProcessResult(), new ProcessMonitor());

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
            Assert.AreEqual(System.Environment.NewLine + "<buildresults>" + System.Environment.NewLine + "  <message>" 
                + ProcessResultOutput + "</message>" + System.Environment.NewLine + "</buildresults>" 
                + System.Environment.NewLine, result.TaskOutput);

			Verify();
		}

		// TODO - Timeout?
		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			ExpectToExecuteAndThrowWithMonitor();

			task.Run(IntegrationResult());
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] {constraint, new IsAnything()});

			IntegrationResult result = (IntegrationResult) IntegrationResult();
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
			CheckBaseDirectory(IntegrationResultForWorkingDirectoryTest(), expectedBaseDirectory);
			Verify();
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
			task.ConfiguredBaseDirectory = @"c:\my\base\directory";
			CheckBaseDirectory(IntegrationResultForWorkingDirectoryTest(), @"c:\my\base\directory");
		}

		private void CheckBaseDirectory(IIntegrationResult result, string expectedBaseDirectory)
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] {constraint, new IsAnything()});

			task.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
			Verify();
		}

        [Test]
        public void ExecutableOutputShouldBeBuildResults()
        {
            ExecutableTask xmlTestTask = new ExecutableTask((ProcessExecutor)mockProcessExecutor.MockInstance);
            xmlTestTask.Executable = DefaultExecutable;
            xmlTestTask.BuildArgs = DefaultArgs;
            ExpectToExecuteArgumentsWithMonitor(DefaultArgs);

            IIntegrationResult result = IntegrationResult();
            xmlTestTask.Run(result);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(IntegrationStatus.Success, result.Status);

            // TODO: The following only works correctly when ProcessResultOutput is a single non-empty line.
            // That is always the case, courtesy of our superclass' initialization.  If that should ever
            // change, this test needs to be adjusted accordingly.
            Assert.AreEqual(System.Environment.NewLine + "<buildresults>" + System.Environment.NewLine + "  <message>" 
                + ProcessResultOutput + "</message>" + System.Environment.NewLine + "</buildresults>"
                + System.Environment.NewLine, result.TaskOutput);
            Verify();
        }

		[Test]
		public void ShouldParseValidSuccessExitCodes()
		{
			task.SuccessExitCodes = "0,1,3,5";

			task.SuccessExitCodes = "300,500,-1";
		}

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void ShouldThrowExceptionOnInvalidSuccessExitCodes()
		{
			task.SuccessExitCodes = "0, 1, GOOD";
		}

		[Test]
		public void ShouldPassSuccessExitCodesToProcessExecutor()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint, new IsAnything() });

			IntegrationResult result = (IntegrationResult)IntegrationResult();
			result.Label = "1.0";
			result.BuildCondition = BuildCondition.ForceBuild;
			result.WorkingDirectory = @"c:\workingdir\";
			result.ArtifactDirectory = @"c:\artifactdir\";

			task.SuccessExitCodes = "0,1,3,5";
			task.Run(result);

			ProcessInfo info = (ProcessInfo)constraint.Parameter;

			Assert.IsTrue(info.ProcessSuccessful(0));
			Assert.IsTrue(info.ProcessSuccessful(1));
			Assert.IsFalse(info.ProcessSuccessful(2));
			Assert.IsTrue(info.ProcessSuccessful(3));
			Assert.IsFalse(info.ProcessSuccessful(4));
			Assert.IsTrue(info.ProcessSuccessful(5));
			Assert.IsFalse(info.ProcessSuccessful(6));

			Verify();
		}
	}
}
