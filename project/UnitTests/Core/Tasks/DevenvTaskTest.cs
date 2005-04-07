using System.ComponentModel;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class DevenvTaskTest
	{
		private const string DEVENV_PATH = @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.com";
		private const string SOLUTION_FILE = @"D:\dev\ccnet\ccnet\project\ccnet.sln";
		private const string CONFIGURATION = "Debug";

		private DevenvTask task;
		private IMock mockRegistry;
		private IMock mockProcessExecutor;

		[SetUp]
		public void Setup()
		{
			mockRegistry = new DynamicMock(typeof(IRegistry)); 
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); 
			task = new DevenvTask((IRegistry) mockRegistry.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);
		}

		[TearDown]
		public void VerifyMocks()
		{
			mockRegistry.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void ShouldLoadAllValuesFromConfiguration()
		{
			string xml = @"
<devenv>
	<executable>c:\vs.net\devenv.com</executable>
	<solutionfile>mySolution.sln</solutionfile>
	<configuration>Debug</configuration>
	<buildTimeoutSeconds>4</buildTimeoutSeconds>
</devenv>";

			DevenvTask task = (DevenvTask) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\vs.net\devenv.com", task.Executable);
			Assert.AreEqual(@"mySolution.sln", task.SolutionFile);
			Assert.AreEqual(@"Debug", task.Configuration);
			Assert.AreEqual(4, task.BuildTimeoutSeconds);
		}

		[Test]
		public void ShouldLoadMinimalValuesFromConfiguration()
		{
			string xml = @"<devenv solutionfile=""mySolution.sln"" configuration=""Release"" />";
			DevenvTask task = (DevenvTask) NetReflector.Read(xml);
			Assert.AreEqual(@"mySolution.sln", task.SolutionFile);
			Assert.AreEqual(@"Release", task.Configuration);
			Assert.AreEqual(DevenvTask.DEFAULT_BUILD_TIMEOUT, task.BuildTimeoutSeconds);
		}

		[Test]
		public void RetrieveExecutableLocationFromRegistryForVS2003()
		{
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\", 
			                             DevenvTask.VS2003_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.com", task.Executable);
		}

		[Test]
		public void RetrieveExecutableLocationFromRegistryForVS2002()
		{
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2003_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			mockRegistry.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\", 
			                             DevenvTask.VS2002_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\devenv.com", task.Executable);
		}

		[Test]
		public void VerifyDevenvProcessInfo()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			ProcessResult processResult = new ProcessResult("output", "error", 0, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, constraint);
			task.Executable = DEVENV_PATH;
			task.SolutionFile = "mySolution.sln";
			task.Configuration = "Debug";

			task.Run(new IntegrationResult());

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(DEVENV_PATH, info.FileName);
			Assert.AreEqual(DevenvTask.DEFAULT_BUILD_TIMEOUT * 1000, info.TimeOut);
			CustomAssertion.AssertStartsWith("mySolution.sln /rebuild Debug", info.Arguments);
		}

		[Test]
		public void ShouldSetOutputAndIntegrationStatusToSuccessOnSuccessfulBuild()
		{
			ProcessResult processResult = new ProcessResult(@"Rebuild All: 10 succeeded, 0 failed, 0 skipped", string.Empty, ProcessResult.SUCCESSFUL_EXIT_CODE, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new IsAnything());
			task.Executable = DEVENV_PATH;
			task.SolutionFile = SOLUTION_FILE;
			task.Configuration = CONFIGURATION;

			IntegrationResult result = new IntegrationResult();
			task.Run(result);

			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			CustomAssertion.AssertMatches(@"Rebuild All: \d+ succeeded, \d+ failed, \d+ skipped", result.TaskOutput);
		}

		[Test]
		public void ShouldSetOutputAndIntegrationStatusToFailedOnFailedBuild()
		{
			ProcessResult processResult = new ProcessResult(@"D:\dev\ccnet\ccnet\project\nosolution.sln could not be found and will not be loaded", string.Empty, 1, false);
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, constraint);

			task.Executable = DEVENV_PATH;
			task.SolutionFile = @"D:\dev\ccnet\ccnet\project\nosolution.sln";
			task.Configuration = CONFIGURATION;

			IntegrationResult result = new IntegrationResult("myProject", "myWorkingDirectory");
			task.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual("myWorkingDirectory", info.WorkingDirectory);

			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			CustomAssertion.AssertMatches(@"(\.|\n)*could not be found and will not be loaded", result.TaskOutput);
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessExecutorThrowsAnException()
		{
			mockProcessExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());
			task.Executable = DEVENV_PATH + ".some.extra.ext.exe";	// file should not exist
			task.SolutionFile = @"D:\dev\ccnet\ccnet\project\nosolution.sln";
			task.Configuration = "Debug";

			task.Run(new IntegrationResult());
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void	ShouldThrowBuilderExceptionIfProcessTimesOut()
		{
			ProcessResult processResult = new ProcessResult(string.Empty, string.Empty, ProcessResult.TIMED_OUT_EXIT_CODE, true);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new IsAnything());
			task.BuildTimeoutSeconds = 2;
			task.Executable = DEVENV_PATH;
			task.SolutionFile = SOLUTION_FILE;
			task.Configuration = CONFIGURATION;

			task.Run(new IntegrationResult());
		}
	}
}