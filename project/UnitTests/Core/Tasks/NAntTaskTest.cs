using System;
using System.ComponentModel;
using System.IO;
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
	public class NAntTaskTest : CustomAssertion
	{
		public const int SUCCESSFUL_EXIT_CODE = 0;
		public const int FAILED_EXIT_CODE = -1;

		private NAntTask builder;
		private IMock mockExecutor;

		[SetUp]
		public void SetUp()
		{
			mockExecutor = new DynamicMock(typeof (ProcessExecutor));
			builder = new NAntTask((ProcessExecutor) mockExecutor.MockInstance);
		}

		private void VerifyAll()
		{
			mockExecutor.Verify();
		}

		[TearDown]
		public void TearDown()
		{
			mockExecutor.Verify();
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
		<nologo>FALSE</nologo>
    </nant>";

			NetReflector.Read(xml, builder);
			Assert.AreEqual(@"C:\", builder.ConfiguredBaseDirectory);
			Assert.AreEqual("mybuild.build", builder.BuildFile);
			Assert.AreEqual("NAnt.exe", builder.Executable);
			Assert.AreEqual(1, builder.Targets.Length);
			Assert.AreEqual(123, builder.BuildTimeoutSeconds);
			Assert.AreEqual("SourceForge.NAnt.XmlLogger", builder.Logger);
			Assert.AreEqual("foo", builder.Targets[0]);
			Assert.AreEqual(false, builder.NoLogo);
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			string xml = @"<nant />";

			NetReflector.Read(xml, builder);
			Assert.AreEqual("", builder.ConfiguredBaseDirectory);
			Assert.AreEqual(NAntTask.DEFAULT_EXECUTABLE, builder.Executable);
			Assert.AreEqual(0, builder.Targets.Length);
			Assert.AreEqual(NAntTask.DEFAULT_BUILD_TIMEOUT, builder.BuildTimeoutSeconds);
			Assert.AreEqual(NAntTask.DEFAULT_LOGGER, builder.Logger);
			Assert.AreEqual(NAntTask.DEFAULT_NOLOGO, builder.NoLogo);
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			builder.Run(result);

			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(returnVal.StandardOutput, result.TaskOutput);
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ProcessResult returnVal = new ProcessResult("output", null, FAILED_EXIT_CODE, false);
			mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			builder.Run(result);

			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			Assert.AreEqual(returnVal.StandardOutput, result.TaskOutput);
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessTimesOut()
		{
			ProcessResult returnVal = new ProcessResult("output", null, SUCCESSFUL_EXIT_CODE, true);
			mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			builder.Run(result);
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			mockExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());

			IntegrationResult result = new IntegrationResult();
			builder.Run(result);
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			IntegrationResult result = new IntegrationResult();
			result.Label = "1.0";
			result.WorkingDirectory = @"C:\temp";
			result.ArtifactDirectory = @"C:\temp";

			builder.ConfiguredBaseDirectory = @"c:\";
			builder.Executable = "NAnt.exe";
			builder.BuildFile = "mybuild.build";
			builder.BuildArgs = "myArgs";
			builder.Targets = new string[] {"target1", "target2"};
			builder.BuildTimeoutSeconds = 2;
			builder.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(builder.Executable, info.FileName);
			Assert.AreEqual(builder.ConfiguredBaseDirectory, info.WorkingDirectory);
			Assert.AreEqual(2000, info.TimeOut);
			Assert.AreEqual(@"-nologo -buildfile:mybuild.build -logger:NAnt.Core.XmlLogger myArgs -D:label-to-apply=1.0 -D:ccnet.label=1.0 -D:ccnet.buildcondition=NoBuild -D:ccnet.working.directory=""C:\temp"" -D:ccnet.artifact.directory=""C:\temp"" target1 target2", info.Arguments);
		}

		[Test]
		public void ShouldPassAppropriateDefaultPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			builder.Run(new IntegrationResult());

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(builder.Executable, NAntTask.DEFAULT_EXECUTABLE);
			Assert.AreEqual(NAntTask.DEFAULT_BUILD_TIMEOUT*1000, info.TimeOut);
			Assert.AreEqual("-nologo -logger:NAnt.Core.XmlLogger -D:ccnet.buildcondition=NoBuild", info.Arguments);
		}

		[Test]
		public void ShouldPutQuotesAroundBuildFileIfItContainsASpace()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			builder.BuildFile = "my project.build";
			builder.Run(new IntegrationResult());

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(@"-nologo -buildfile:""my project.build"" -logger:NAnt.Core.XmlLogger -D:ccnet.buildcondition=NoBuild", info.Arguments);
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotSetUseProjectWorkingDirectoryAsBaseDirectory()
		{
			builder.ConfiguredBaseDirectory = null;
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsEmptyUseProjectWorkingDirectoryAsBaseDirectory()
		{
			builder.ConfiguredBaseDirectory = "";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotAbsoluteUseProjectWorkingDirectoryAsFirstPartOfBaseDirectory()
		{
			builder.ConfiguredBaseDirectory = "relativeBaseDirectory";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("relativeBaseDirectory");
		}

		private void CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart(string relativeDirectory)
		{
			string expectedBaseDirectory = "projectWorkingDirectory";
			if (relativeDirectory.Length > 0)
			{
				expectedBaseDirectory = Path.Combine(expectedBaseDirectory, relativeDirectory);
			}
			CheckBaseDirectory(new IntegrationResult("project", "projectWorkingDirectory"), expectedBaseDirectory);
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
			builder.ConfiguredBaseDirectory = @"c:\my\base\directory";
			CheckBaseDirectory(new IntegrationResult("project", "projectWorkingDirectory"), @"c:\my\base\directory");
		}

		private void CheckBaseDirectory(IntegrationResult result, string expectedBaseDirectory)
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			builder.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
			VerifyAll();
		}

		[Test]
		public void ShouldGiveAPresentationValueForTargetsThatIsANewLineSeparatedEquivalentOfAllTargets()
		{
			builder.Targets = new string[] {"target1", "target2"};
			Assert.AreEqual("target1" + Environment.NewLine + "target2", builder.TargetsForPresentation);
		}

		[Test]
		public void ShouldWorkForSingleTargetWhenSettingThroughPresentationValue()
		{
			builder.TargetsForPresentation = "target1";
			Assert.AreEqual("target1", builder.Targets[0]);
			Assert.AreEqual(1, builder.Targets.Length);
		}

		[Test]
		public void ShouldSplitAtNewLineWhenSettingThroughPresentationValue()
		{
			builder.TargetsForPresentation = "target1" + Environment.NewLine + "target2";
			Assert.AreEqual("target1", builder.Targets[0]);
			Assert.AreEqual("target2", builder.Targets[1]);
			Assert.AreEqual(2, builder.Targets.Length);
		}

		[Test]
		public void ShouldWorkForEmptyAndNullStringsWhenSettingThroughPresentationValue()
		{
			builder.TargetsForPresentation = "";
			Assert.AreEqual(0, builder.Targets.Length);
			builder.TargetsForPresentation = null;
			Assert.AreEqual(0, builder.Targets.Length);
		}

		private ProcessResult CreateSuccessfulProcessResult()
		{
			return new ProcessResult("output", null, SUCCESSFUL_EXIT_CODE, false);
		}
	}
}