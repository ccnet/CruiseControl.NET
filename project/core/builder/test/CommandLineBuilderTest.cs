using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Builder;

namespace ThoughtWorks.CruiseControl.Core.Builder.Test
{
	[TestFixture]
	public class CommandLineBuilderTest : CustomAssertion
	{
		public const int SUCCESSFUL_EXIT_CODE = 0;
		public const int FAILED_EXIT_CODE = -1;

		private CommandLineBuilder _builder;
		private IMock _mockExecutor;
		private IProject project;

		[SetUp]
		public void SetUp()
		{
			_mockExecutor = new DynamicMock(typeof(ProcessExecutor));
			_builder = new CommandLineBuilder((ProcessExecutor) _mockExecutor.MockInstance);
			project = (IProject) new DynamicMock(typeof(IProject)).MockInstance;
		}

		[TearDown]
		public void TearDown()
		{
			_mockExecutor.Verify();
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"
    <commandLineBuilder>
    	<executable>mybatchfile.bat</executable>
    	<baseDirectory>C:\</baseDirectory>
		<buildArgs>myarg1 myarg2</buildArgs>
		<buildTimeoutSeconds>123</buildTimeoutSeconds>
    </commandLineBuilder>";

			NetReflector.Read(xml, _builder);
			AssertEquals(@"C:\", _builder.BaseDirectory);
			AssertEquals("mybatchfile.bat", _builder.Executable);
			AssertEquals(123, _builder.BuildTimeoutSeconds);
			AssertEquals("myarg1 myarg2", _builder.BuildArgs);
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			string xml = @"
    <commandLineBuilder>
    	<executable>mybatchfile.bat</executable>
    </commandLineBuilder>";

			NetReflector.Read(xml, _builder);
			AssertEquals(".", _builder.BaseDirectory);
			AssertEquals("mybatchfile.bat", _builder.Executable);
			AssertEquals(600, _builder.BuildTimeoutSeconds);
			AssertEquals("", _builder.BuildArgs);
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result, project);

			Assert("build should have succeeded", result.Succeeded);
			AssertEquals(IntegrationStatus.Success, result.Status);
			AssertEquals(returnVal.StandardOutput + "\n" + returnVal.StandardError, result.Output);
			_mockExecutor.Verify();
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ProcessResult returnVal = new ProcessResult("output", null, FAILED_EXIT_CODE, false);
			_mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result, project);

			Assert("build should have failed", result.Failed);
			AssertEquals(IntegrationStatus.Failure, result.Status);
			AssertEquals(returnVal.StandardOutput + "\n" + returnVal.StandardError, result.Output);
			_mockExecutor.Verify();
		}

		// TODO - Timeout?

		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			_mockExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result, project);
			_mockExecutor.Verify();
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);
			
			IntegrationResult result = new IntegrationResult();
			result.Label = "1.0";

			_builder.BaseDirectory = "test-base-dir";
			_builder.Executable = "test-exe";
			_builder.BuildArgs = "test-args";
			_builder.BuildTimeoutSeconds = 222;
			_builder.Run(result, project);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertEquals("test-exe", info.FileName);
			AssertEquals("test-base-dir", info.WorkingDirectory);
			AssertEquals(222000, info.TimeOut);
			AssertEquals("test-args", info.Arguments);
			_mockExecutor.Verify();
		}

		[Test]
		public void ShouldRun()
		{
			AssertFalse(_builder.ShouldRun(new IntegrationResult(), new Project()));
			Assert(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Unknown), project));
			Assert(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Success), project));
			AssertFalse(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Failure), project));
			AssertFalse(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Exception), project));
		}

		private ProcessResult CreateSuccessfulProcessResult()
		{
			return new ProcessResult("output", null, SUCCESSFUL_EXIT_CODE, false);
		}

		private IntegrationResult CreateIntegrationResultWithModifications (IntegrationStatus status)
		{
			IntegrationResult result = new IntegrationResult ();
			result.Status = status;
			result.Modifications = new Modification[] { new Modification () };
			return result;
		}
	}
}