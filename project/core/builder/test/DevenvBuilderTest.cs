#define USE_MOCK

using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Publishers;
using System.Text.RegularExpressions;
using System.ComponentModel;

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

		[SetUp]
		protected void CreateBuilder()
		{
			mockRegistry = new DynamicMock(typeof(IRegistry)); 
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); 
			builder = new DevenvBuilder((IRegistry) mockRegistry.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);
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
			AssertEquals(@"c:\vs.net\devenv.com", builder.Executable);
			AssertEquals(@"mySolution.sln", builder.SolutionFile);
			AssertEquals(@"Debug", builder.Configuration);
			AssertEquals(4, builder.BuildTimeoutSeconds);
		}

		[Test]
		public void ShouldLoadMinimalValuesFromConfiguration()
		{
			string xml = @"<devenv solutionfile=""mySolution.sln"" configuration=""Release"" />";
			DevenvBuilder builder = (DevenvBuilder) NetReflector.Read(xml);
			AssertEquals(@"mySolution.sln", builder.SolutionFile);
			AssertEquals(@"Release", builder.Configuration);
			AssertEquals(DevenvBuilder.DEFAULT_BUILD_TIMEOUT, builder.BuildTimeoutSeconds);
		}

		[Test]
		public void RetrieveExecutableLocationFromRegistryForVS2003()
		{
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\", 
				DevenvBuilder.VS2003_REGISTRY_PATH, DevenvBuilder.VS_REGISTRY_KEY);
			AssertEquals(@"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.com", builder.Executable);
		}

		[Test]
		public void RetrieveExecutableLocationFromRegistryForVS2002()
		{
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvBuilder.VS2003_REGISTRY_PATH, DevenvBuilder.VS_REGISTRY_KEY);
			mockRegistry.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\", 
				DevenvBuilder.VS2002_REGISTRY_PATH, DevenvBuilder.VS_REGISTRY_KEY);
			AssertEquals(@"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\devenv.com", builder.Executable);
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

			builder.Run(new IntegrationResult());

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertEquals(DEVENV_PATH, info.FileName);
			AssertEquals(DevenvBuilder.DEFAULT_BUILD_TIMEOUT * 1000, info.TimeOut);
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
			builder.Run(result);

			AssertEquals(IntegrationStatus.Success, result.Status);
			AssertMatches(@"Rebuild All: \d+ succeeded, \d+ failed, \d+ skipped", result.Output);
			AssertEquals(typeof(DevenvTaskResult), result.TaskResults[0]);
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
			builder.Run(result);

			AssertEquals(IntegrationStatus.Failure, result.Status);
			AssertMatches(@"(\.|\n)*could not be found and will not be loaded", result.Output);
			AssertEquals(typeof(DevenvTaskResult), result.TaskResults[0]);
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

			builder.Run(new IntegrationResult());
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

			builder.Run(new IntegrationResult());
		}
	}
}