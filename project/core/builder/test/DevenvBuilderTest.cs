#define USE_MOCK
using System.ComponentModel;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder.Test
{
	[TestFixture]
	public class DevenvBuilderTest : CustomAssertion
	{
		private const string DEVENV_PATH = @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.com";
		private const string SOLUTION_FILE = @"D:\dev\ccnet\ccnet\project\ccnet.sln";
		private const string CONFIGURATION = "Debug";

		private DevenvBuilder builder;
		private IMock mockRegistry;
		private IMock mockProcessExecutor;
		private IProject project;

		[SetUp]
		protected void CreateBuilder()
		{
			mockRegistry = new DynamicMock(typeof(IRegistry)); 
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); 
			builder = new DevenvBuilder((IRegistry) mockRegistry.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);
			project = (IProject) new DynamicMock(typeof(IProject)).MockInstance;
		}

		[TearDown]
			protected void VerifyMocks()
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

			DevenvBuilder builder = (DevenvBuilder) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\vs.net\devenv.com", builder.Executable);
			Assert.AreEqual(@"mySolution.sln", builder.SolutionFile);
			Assert.AreEqual(@"Debug", builder.Configuration);
			Assert.AreEqual(4, builder.BuildTimeoutSeconds);
		}

		[Test]
		public void ShouldLoadMinimalValuesFromConfiguration()
		{
			string xml = @"<devenv solutionfile=""mySolution.sln"" configuration=""Release"" />";
			DevenvBuilder builder = (DevenvBuilder) NetReflector.Read(xml);
			Assert.AreEqual(@"mySolution.sln", builder.SolutionFile);
			Assert.AreEqual(@"Release", builder.Configuration);
			Assert.AreEqual(DevenvBuilder.DEFAULT_BUILD_TIMEOUT, builder.BuildTimeoutSeconds);
		}

		[Test]
		public void RetrieveExecutableLocationFromRegistryForVS2003()
		{
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\", 
				DevenvBuilder.VS2003_REGISTRY_PATH, DevenvBuilder.VS_REGISTRY_KEY);
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.com", builder.Executable);
		}

		[Test]
		public void RetrieveExecutableLocationFromRegistryForVS2002()
		{
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvBuilder.VS2003_REGISTRY_PATH, DevenvBuilder.VS_REGISTRY_KEY);
			mockRegistry.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\", 
				DevenvBuilder.VS2002_REGISTRY_PATH, DevenvBuilder.VS_REGISTRY_KEY);
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\devenv.com", builder.Executable);
		}

		[Test]
		public void VerifyDevenvProcessInfo()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			ProcessResult processResult = new ProcessResult("output", "error", 0, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, constraint);
			builder.Executable = DEVENV_PATH;
			builder.SolutionFile = "mySolution.sln";
			builder.Configuration = "Debug";

			builder.Run(new IntegrationResult(), project);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(DEVENV_PATH, info.FileName);
			Assert.AreEqual(DevenvBuilder.DEFAULT_BUILD_TIMEOUT * 1000, info.TimeOut);
			AssertStartsWith("mySolution.sln /rebuild Debug", info.Arguments);
		}

		[Test]
		public void ShouldSetOutputAndIntegrationStatusToSuccessOnSuccessfulBuild()
		{
#if USE_MOCK
			ProcessResult processResult = new ProcessResult(@"Rebuild All: 10 succeeded, 0 failed, 0 skipped", string.Empty, ProcessResult.SUCCESSFUL_EXIT_CODE, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new IsAnything());
#else
			DevenvBuilder builder = new DevenvBuilder();
#endif
			builder.Executable = DEVENV_PATH;
			builder.SolutionFile = SOLUTION_FILE;
			builder.Configuration = CONFIGURATION;

			IntegrationResult result = new IntegrationResult();
			builder.Run(result, project);

			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			AssertMatches(@"Rebuild All: \d+ succeeded, \d+ failed, \d+ skipped", result.Output);
			Assert.IsTrue(result.TaskResults[0] is DevenvTaskResult);
		}

		[Test]
		public void ShouldSetOutputAndIntegrationStatusToFailedOnFailedBuild()
		{
#if USE_MOCK
			ProcessResult processResult = new ProcessResult(@"D:\dev\ccnet\ccnet\project\nosolution.sln could not be found and will not be loaded", string.Empty, 1, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new IsAnything());
#else
			DevenvBuilder builder = new DevenvBuilder();
#endif
			builder.Executable = DEVENV_PATH;
			builder.SolutionFile = @"D:\dev\ccnet\ccnet\project\nosolution.sln";
			builder.Configuration = CONFIGURATION;

			IntegrationResult result = new IntegrationResult();
			builder.Run(result, project);

			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			AssertMatches(@"(\.|\n)*could not be found and will not be loaded", result.Output);
			Assert.IsTrue(result.TaskResults[0] is DevenvTaskResult);
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessExecutorThrowsAnException()
		{
#if USE_MOCK
			mockProcessExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());
#else
			DevenvBuilder builder = new DevenvBuilder();
#endif
			builder.Executable = DEVENV_PATH + ".some.extra.ext.exe";	// file should not exist
			builder.SolutionFile = @"D:\dev\ccnet\ccnet\project\nosolution.sln";
			builder.Configuration = "Debug";

			builder.Run(new IntegrationResult(), project);
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void	ShouldThrowBuilderExceptionIfProcessTimesOut()
		{
#if USE_MOCK
			ProcessResult processResult = new ProcessResult(string.Empty, string.Empty, ProcessResult.TIMED_OUT_EXIT_CODE, true);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new IsAnything());
#else
			DevenvBuilder builder = new DevenvBuilder();
#endif
			builder.BuildTimeoutSeconds = 2;
			builder.Executable = DEVENV_PATH;
			builder.SolutionFile = SOLUTION_FILE;
			builder.Configuration = CONFIGURATION;

			builder.Run(new IntegrationResult(), project);
		}
	}
}