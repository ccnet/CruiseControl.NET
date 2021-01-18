using System.Diagnostics;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using Moq;
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
			task = new ExecutableTask((ProcessExecutor) mockProcessExecutor.Object);
			task.Executable = DefaultExecutable;
			task.BuildArgs = DefaultArgs;
		}

		[Test]
		public void PopulateFromReflector()
		{
			const string xml = @"
    <exec>
    	<executable>mybatchfile.bat</executable>
    	<baseDirectory>C:\</baseDirectory>
	<buildArgs>myarg1 myarg2</buildArgs>
	<buildTimeoutSeconds>123</buildTimeoutSeconds>
        <environment>
		<variable name=""name1"" value=""value1""/>
		<variable><name>name2</name></variable>
		<variable name=""name3""><value>value3</value></variable>
	</environment>
    <priority>BelowNormal</priority>
	<successExitCodes>0,1,3,5</successExitCodes>
    </exec>";

			task = (ExecutableTask) NetReflector.Read(xml);
			Assert.AreEqual(@"C:\", task.ConfiguredBaseDirectory, "Checking ConfiguredBaseDirectory property.");
			Assert.AreEqual("mybatchfile.bat", task.Executable, "Checking property.");
			Assert.AreEqual(123, task.BuildTimeoutSeconds, "Checking BuildTimeoutSeconds property.");
			Assert.AreEqual("myarg1 myarg2", task.BuildArgs, "Checking BuildArgs property.");
			Assert.AreEqual(3, task.EnvironmentVariables.Length, "Checking environment variable array size.");
			Assert.AreEqual("name1", task.EnvironmentVariables[0].name, "Checking name1 environment variable.");
			Assert.AreEqual("value1", task.EnvironmentVariables[0].value, "Checking name1 environment value.");
			Assert.AreEqual("name2", task.EnvironmentVariables[1].name, "Checking name2 environment variable.");
			Assert.AreEqual("", task.EnvironmentVariables[1].value, "Checking name2 environment value.");
			Assert.AreEqual("name3", task.EnvironmentVariables[2].name, "Checking name3 environment variable.");
			Assert.AreEqual("value3", task.EnvironmentVariables[2].value, "Checking name3 environment value.");
			Assert.AreEqual("0,1,3,5", task.SuccessExitCodes);
            Assert.AreEqual(ProcessPriorityClass.BelowNormal, task.Priority);
			Verify();
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			const string xml = @"
    <exec>
    	<executable>mybatchfile.bat</executable>
    </exec>";

			task = (ExecutableTask) NetReflector.Read(xml);
            Assert.AreEqual("mybatchfile.bat", task.Executable, "Checking property.");
            Assert.AreEqual(600, task.BuildTimeoutSeconds, "Checking BuildTimeoutSeconds property.");
            Assert.AreEqual("", task.BuildArgs, "Checking BuildArgs property.");
            Assert.AreEqual(0, task.EnvironmentVariables.Length, "Checking environment variable array size.");
			Assert.AreEqual("", task.SuccessExitCodes);
            Assert.AreEqual(ProcessPriorityClass.Normal, task.Priority);
			Verify();
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ExpectToExecuteArguments(DefaultArgs);

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

            StringWriter buffer = new StringWriter();
            new ReflectorTypeAttribute("task").Write(new XmlTextWriter(buffer), task);

            Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
            Assert.AreEqual(System.Environment.NewLine + "<buildresults>" + System.Environment.NewLine
                + buffer.ToString() + System.Environment.NewLine + "  <message>" 
                + ProcessResultOutput + "</message>" + System.Environment.NewLine + "</buildresults>" 
                + System.Environment.NewLine, result.TaskOutput);
            Verify();
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ExpectToExecuteAndReturn(FailedProcessResult());

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

            StringWriter buffer = new StringWriter();
            new ReflectorTypeAttribute("task").Write(new XmlTextWriter(buffer), task);

			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
            Assert.AreEqual(System.Environment.NewLine + "<buildresults>" + System.Environment.NewLine
                + buffer.ToString() + System.Environment.NewLine + "  <message>" 
                + ProcessResultOutput + "</message>" + System.Environment.NewLine + "</buildresults>" 
                + System.Environment.NewLine, result.TaskOutput);

			Verify();
		}

		// TODO - Timeout?
		[Test]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			ExpectToExecuteAndThrow();

            Assert.That(delegate { task.Run(IntegrationResult()); },
                        Throws.TypeOf<BuilderException>());
			Verify();
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessInfo info = null;
			mockProcessExecutor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).
				Callback<ProcessInfo>(processInfo => info = processInfo).Returns(SuccessfulProcessResult()).Verifiable();

			IntegrationResult result = (IntegrationResult) IntegrationResult();
			result.Label = "1.0";
			result.BuildCondition = BuildCondition.ForceBuild;
			result.WorkingDirectory = @"c:\workingdir\";
			result.ArtifactDirectory = @"c:\artifactdir\";

			task.Executable = "test-exe";
			task.BuildArgs = "test-args";
			task.BuildTimeoutSeconds = 222;
			task.Run(result);

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
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
            string path = Platform.IsWindows ? @"c:\my\base\directory" : @"/my/base/directory";
        
			task.ConfiguredBaseDirectory = path;
			CheckBaseDirectory(IntegrationResultForWorkingDirectoryTest(), path);
		}

		private void CheckBaseDirectory(IIntegrationResult result, string expectedBaseDirectory)
		{
			ProcessInfo info = null;
			mockProcessExecutor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).
				Callback<ProcessInfo>(processInfo => info = processInfo).Returns(SuccessfulProcessResult()).Verifiable();

			task.Run(result);

			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
			Verify();
		}

        [Test]
        public void ExecutableOutputShouldBeBuildResults()
        {
            ExecutableTask xmlTestTask = new ExecutableTask((ProcessExecutor)mockProcessExecutor.Object);
            xmlTestTask.Executable = DefaultExecutable;
            xmlTestTask.BuildArgs = DefaultArgs;
            ExpectToExecuteArguments(DefaultArgs);

            IIntegrationResult result = IntegrationResult();
            xmlTestTask.Run(result);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(IntegrationStatus.Success, result.Status);

            StringWriter buffer = new StringWriter();
            new ReflectorTypeAttribute("task").Write(new XmlTextWriter(buffer), xmlTestTask);
            
            // TODO: The following only works correctly when ProcessResultOutput is a single non-empty line.
            // That is always the case, courtesy of our superclass' initialization.  If that should ever
            // change, this test needs to be adjusted accordingly.
            Assert.AreEqual(System.Environment.NewLine + "<buildresults>" + System.Environment.NewLine
                + buffer.ToString() + System.Environment.NewLine + "  <message>" 
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
		public void ShouldThrowExceptionOnInvalidSuccessExitCodes()
		{
			Assert.That(delegate { task.SuccessExitCodes = "0, 1, GOOD"; },
                        Throws.TypeOf<System.FormatException>());
		}

		[Test]
		public void ShouldPassSuccessExitCodesToProcessExecutor()
		{
			ProcessInfo info = null;
			mockProcessExecutor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).
				Callback<ProcessInfo>(processInfo => info = processInfo).Returns(SuccessfulProcessResult()).Verifiable();

			IntegrationResult result = (IntegrationResult)IntegrationResult();
			result.Label = "1.0";
			result.BuildCondition = BuildCondition.ForceBuild;
			result.WorkingDirectory = @"c:\workingdir\";
			result.ArtifactDirectory = @"c:\artifactdir\";

			task.SuccessExitCodes = "0,1,3,5";
			task.Run(result);

			Assert.IsTrue(info.ProcessSuccessful(0));
			Assert.IsTrue(info.ProcessSuccessful(1));
			Assert.IsFalse(info.ProcessSuccessful(2));
			Assert.IsTrue(info.ProcessSuccessful(3));
			Assert.IsFalse(info.ProcessSuccessful(4));
			Assert.IsTrue(info.ProcessSuccessful(5));
			Assert.IsFalse(info.ProcessSuccessful(6));

			Verify();
		}

		[Test]
		public void ShouldFailIfProcessTimesOut()
		{
			ExpectToExecuteAndReturn(TimedOutProcessResult());

			IIntegrationResult result = IntegrationResult();
			task.Run(result);

			Assert.That(result.Status, Is.EqualTo(IntegrationStatus.Failure));
			Assert.That(result.TaskOutput, Does.Match("Command line '.*' timed out after \\d+ seconds"));

			Verify();
		}
	}
}
