using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder.Test
{
	[TestFixture]
	public class NAntBuilderTest : CustomAssertion
	{
		public const int SUCCESSFUL_EXIT_CODE = 0;
		public const int FAILED_EXIT_CODE = -1;

		private NAntBuilder _builder;
		private IMock _mockExecutor;
		private DynamicMock projectMock;
		private IProject project;

		[SetUp]
		public void SetUp()
		{
			_mockExecutor = new DynamicMock(typeof(ProcessExecutor));
			_builder = new NAntBuilder((ProcessExecutor) _mockExecutor.MockInstance);
			projectMock = new DynamicMock(typeof(IProject));
			project = (IProject) projectMock.MockInstance;
		}

		private void VerifyAll()
		{
			_mockExecutor.Verify();
			projectMock.Verify();
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
    <nant>
    	<executable>NAnt.exe</executable>
    	<baseDirectory>C:\</baseDirectory>
    	<buildFile>mybuild.build</buildFile>
		<targetList>
      		<target>foo</target>
    	</targetList>
		<logger>SourceForge.NAnt.XmlLogger</logger>
		<buildTimeoutSeconds>123</buildTimeoutSeconds>
    </nant>";

			NetReflector.Read(xml, _builder);
			AssertEquals(@"C:\", _builder.ConfiguredBaseDirectory);
			AssertEquals("mybuild.build", _builder.BuildFile);
			AssertEquals("NAnt.exe", _builder.Executable);
			AssertEquals(1, _builder.Targets.Length);
			AssertEquals(123, _builder.BuildTimeoutSeconds);
			AssertEquals("SourceForge.NAnt.XmlLogger", _builder.Logger);
			AssertEquals("foo", _builder.Targets[0]);
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			string xml = @"<nant />";

			NetReflector.Read(xml, _builder);
			AssertEquals(null, _builder.ConfiguredBaseDirectory);
			AssertEquals(NAntBuilder.DEFAULT_EXECUTABLE, _builder.Executable);
			AssertEquals(0, _builder.Targets.Length);
			AssertEquals(NAntBuilder.DEFAULT_BUILD_TIMEOUT, _builder.BuildTimeoutSeconds);
			AssertEquals(NAntBuilder.DEFAULT_LOGGER, _builder.Logger);
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
			AssertEquals(returnVal.StandardOutput, result.Output);
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
			AssertEquals(returnVal.StandardOutput, result.Output);
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessTimesOut()
		{
			ProcessResult returnVal = new ProcessResult("output", null, SUCCESSFUL_EXIT_CODE, true);
			_mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result, project);
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			_mockExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result, project);
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);
			
			IntegrationResult result = new IntegrationResult();
			result.Label = "1.0";

			_builder.ConfiguredBaseDirectory = @"c:\";
			_builder.Executable = "NAnt.exe";
			_builder.BuildFile = "mybuild.build";
			_builder.BuildArgs = "myArgs";
			_builder.Targets = new string[] { "target1", "target2"};
			_builder.BuildTimeoutSeconds = 2;
			_builder.Run(result, project);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertEquals(_builder.Executable, info.FileName);
			AssertEquals(_builder.ConfiguredBaseDirectory, info.WorkingDirectory);
			AssertEquals(2000, info.TimeOut);
			AssertEquals("-buildfile:mybuild.build -logger:" + NAntBuilder.DEFAULT_LOGGER + " myArgs -D:label-to-apply=1.0 target1 target2", info.Arguments);
		}

		[Test]
		public void ShouldPassAppropriateDefaultPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);
			
			_builder.Run(new IntegrationResult(), project);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertEquals(_builder.Executable, NAntBuilder.DEFAULT_EXECUTABLE);
			AssertEquals(NAntBuilder.DEFAULT_BUILD_TIMEOUT * 1000, info.TimeOut);
			AssertEquals("-logger:" + NAntBuilder.DEFAULT_LOGGER + "  -D:label-to-apply=NO-LABEL", info.Arguments);
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotSetUseProjectWorkingDirectoryAsBaseDirectory()
		{
			_builder.ConfiguredBaseDirectory = null;
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsEmptyUseProjectWorkingDirectoryAsBaseDirectory()
		{
			_builder.ConfiguredBaseDirectory = "";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotAbsoluteUseProjectWorkingDirectoryAsFirstPartOfBaseDirectory()
		{
			_builder.ConfiguredBaseDirectory = "relativeBaseDirectory";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("relativeBaseDirectory");
		}

		private void CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart(string relativeDirectory)
		{
			projectMock.ExpectAndReturn("WorkingDirectory", "projectWorkingDirectory");
			string expectedBaseDirectory = "projectWorkingDirectory";
			if (relativeDirectory != "")
			{
				expectedBaseDirectory = Path.Combine(expectedBaseDirectory, relativeDirectory);
			}
			CheckBaseDirectory(expectedBaseDirectory);
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
			projectMock.ExpectNoCall("WorkingDirectory");
			_builder.ConfiguredBaseDirectory = @"c:\my\base\directory";
			CheckBaseDirectory(@"c:\my\base\directory");
		}

		private void CheckBaseDirectory(string expectedBaseDirectory)
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			_builder.Run(new IntegrationResult(), project);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertEquals(expectedBaseDirectory, info.WorkingDirectory);
			VerifyAll();
		}

		[Test]
		public void ShouldRun()
		{
			AssertFalse(_builder.ShouldRun(new IntegrationResult(), project));
			Assert(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Unknown), project));
			Assert(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Success), project));
			AssertFalse(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Failure), project));
			AssertFalse(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Exception), project));
		}

		[Test]
		public void ShouldGiveAPresentationValueForTargetsThatIsANewLineSeparatedEquivalentOfAllTargets()
		{
			_builder.Targets = new string[] {"target1", "target2"};
			AssertEquals ("target1" + Environment.NewLine + "target2", _builder.TargetsForPresentation);
		}

		[Test]
		public void ShouldSplitAtNewLineWhenSettingThroughPresentationValue()
		{
			_builder.TargetsForPresentation = "target1" + Environment.NewLine + "target2";
			AssertEquals("target1", _builder.Targets[0]);
			AssertEquals("target2", _builder.Targets[1]);
			AssertEquals(2, _builder.Targets.Length);
		}

		[Test]
		public void ShouldWorkForEmptyAndNullStringsWhenSettingThroughPresentationValue()
		{
			_builder.TargetsForPresentation = "";
			AssertEquals(0, _builder.Targets.Length);
			_builder.TargetsForPresentation = null;
			AssertEquals(0, _builder.Targets.Length);
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