using System;
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
	public class NAntTaskTest : ProcessExecutorTestFixtureBase
	{
		private NAntTask builder;
		private IIntegrationResult result;

		[SetUp]
		public void SetUp()
		{
			CreateProcessExecutorMock(NAntTask.DefaultExecutable);
			builder = new NAntTask((ProcessExecutor) mockProcessExecutor.MockInstance);
			result = IntegrationResult();
			result.Label = "1.0";
		}

		[TearDown]
		public void TearDown()
		{
			Verify();
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
			Assert.AreEqual(NAntTask.DefaultExecutable, builder.Executable);
			Assert.AreEqual(0, builder.Targets.Length);
			Assert.AreEqual(NAntTask.DefaultBuildTimeout, builder.BuildTimeoutSeconds);
			Assert.AreEqual(NAntTask.DefaultLogger, builder.Logger);
			Assert.AreEqual(NAntTask.DefaultNoLogo, builder.NoLogo);
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ExpectToExecuteAndReturn(SuccessfulProcessResult());

			builder.Run(result);

			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(SuccessfulProcessResult().StandardOutput, result.TaskOutput);
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ExpectToExecuteAndReturn(FailedProcessResult());

			builder.Run(result);

			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			Assert.AreEqual(FailedProcessResult().StandardOutput, result.TaskOutput);
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessTimesOut()
		{
			ExpectToExecuteAndReturn(TimedOutProcessResult());
			builder.Run(result);
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			ExpectToExecuteAndThrow();
			builder.Run(result);
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			string args = @"-nologo -buildfile:mybuild.build -logger:NAnt.Core.XmlLogger myArgs " + IntegrationProperties(@"C:\temp", @"C:\temp") + " target1 target2";
			ProcessInfo info = NewProcessInfo(args);
			info.TimeOut = 2000;
			ExpectToExecute(info);

			result.Label = "1.0";
			result.WorkingDirectory = @"C:\temp";
			result.ArtifactDirectory = @"C:\temp";

			builder.ConfiguredBaseDirectory = DefaultWorkingDirectory;
			builder.BuildFile = "mybuild.build";
			builder.BuildArgs = "myArgs";
			builder.Targets = new string[] {"target1", "target2"};
			builder.BuildTimeoutSeconds = 2;
			builder.Run(result);
		}

		[Test]
		public void ShouldPassAppropriateDefaultPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ExpectToExecuteArguments(@"-nologo -logger:NAnt.Core.XmlLogger " + IntegrationProperties(@"c:\source", @"c:\source"));
			builder.ConfiguredBaseDirectory = DefaultWorkingDirectory;
			result.ArtifactDirectory = DefaultWorkingDirectory;
			result.WorkingDirectory = DefaultWorkingDirectory;
			builder.Run(result);
		}

		[Test]
		public void ShouldPutQuotesAroundBuildFileIfItContainsASpace()
		{
			ExpectToExecuteArguments(@"-nologo -buildfile:""my project.build"" -logger:NAnt.Core.XmlLogger " + IntegrationProperties(@"c:\source", @"c:\source"));

			builder.BuildFile = "my project.build";
			builder.ConfiguredBaseDirectory = DefaultWorkingDirectory;
			result.ArtifactDirectory = DefaultWorkingDirectory;
			result.WorkingDirectory = DefaultWorkingDirectory;
			builder.Run(result);
		}

		[Test]
		public void ShouldEncloseDirectoriesInQuotesIfTheyContainSpaces()
		{
			DefaultWorkingDirectory = @"c:\dir with spaces";
			ExpectToExecuteArguments(@"-nologo -logger:NAnt.Core.XmlLogger " + IntegrationProperties(@"""c:\dir with spaces""", @"""c:\dir with spaces"""));

			builder.ConfiguredBaseDirectory = DefaultWorkingDirectory;
			result.ArtifactDirectory = DefaultWorkingDirectory;
			result.WorkingDirectory = DefaultWorkingDirectory;
			builder.Run(result);
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
			ProcessResult returnVal = SuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			builder.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
			Verify();
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

		private string IntegrationProperties(string workingDirectory, string artifactDirectory)
		{
			return string.Format(@"-D:CCNetIntegrationStatus=Success -D:CCNetBuildDate={2} -D:CCNetArtifactDirectory={1} -D:CCNetBuildTime={3} -D:CCNetProject=test -D:CCNetLabel=1.0 -D:CCNetWorkingDirectory={0} -D:CCNetLastIntegrationStatus=Unknown -D:CCNetBuildCondition=NoBuild", workingDirectory, artifactDirectory, testDateString, testTimeString);
		}
	}
}