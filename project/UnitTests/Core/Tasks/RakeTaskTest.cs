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
	public class RakeTaskTest : ProcessExecutorTestFixtureBase
	{
		private RakeTask builder;
		private IIntegrationResult result;

		[SetUp]
		public void SetUp()
		{
			DefaultWorkingDirectory = @"c:\source";
			CreateProcessExecutorMock(RakeTask.DefaultExecutable);
			builder = new RakeTask((ProcessExecutor) mockProcessExecutor.MockInstance);
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
			const string xml = @"
    <rake>
    	<executable>C:\ruby\bin\rake.bat</executable>
    	<baseDirectory>C:\</baseDirectory>
    	<rakefile>Rakefile</rakefile>
		<targetList>
			<target>foo</target>
			<target>bar</target>
		</targetList>
		<buildTimeoutSeconds>123</buildTimeoutSeconds>
		<quiet>true</quiet>
		<silent>true</silent>
		<trace>true</trace>
    </rake>";

			NetReflector.Read(xml, builder);
			Assert.AreEqual(@"C:\", builder.BaseDirectory);
			Assert.AreEqual("Rakefile", builder.Rakefile);
			Assert.AreEqual(@"C:\ruby\bin\rake.bat", builder.Executable);
			Assert.AreEqual(2, builder.Targets.Length);
			Assert.AreEqual("foo", builder.Targets[0]);
			Assert.AreEqual("bar", builder.Targets[1]);
			Assert.AreEqual(123, builder.BuildTimeoutSeconds);
			Assert.AreEqual(true, builder.Quiet);
			Assert.AreEqual(true, builder.Silent);
			Assert.AreEqual(true, builder.Trace);
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			const string xml = @"<rake />";

			NetReflector.Read(xml, builder);
			Assert.AreEqual("", builder.BaseDirectory);
			Assert.AreEqual(RakeTask.DefaultExecutable, builder.Executable);
			Assert.AreEqual(0, builder.Targets.Length);
			Assert.AreEqual(RakeTask.DefaultBuildTimeout, builder.BuildTimeoutSeconds);
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ExpectToExecuteAndReturn(SuccessfulProcessResult());
			
			builder.Run(result);
			
			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(StringUtil.MakeBuildResult(SuccessfulProcessResult().StandardOutput, ""), result.TaskOutput);
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ExpectToExecuteAndReturn(FailedProcessResult());
			
			builder.Run(result);
			
			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			Assert.AreEqual(StringUtil.MakeBuildResult(FailedProcessResult().StandardOutput, ""), result.TaskOutput);
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
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });

			IntegrationResult integrationResult = (IntegrationResult)IntegrationResult();
			integrationResult.ProjectName = "test";
			integrationResult.Label = "1.0";
			integrationResult.BuildCondition = BuildCondition.ForceBuild;
			integrationResult.WorkingDirectory = @"c:\workingdir\";
			integrationResult.ArtifactDirectory = @"c:\artifactdir\";

			builder.Executable = "rake";
			builder.BuildArgs = "myargs";
			builder.BuildTimeoutSeconds = 222;
			builder.Run(integrationResult);

			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("rake", info.FileName);
			Assert.AreEqual(222000, info.TimeOut);
			Assert.AreEqual("myargs", info.Arguments);
			Assert.AreEqual("1.0", info.EnvironmentVariables["CCNetLabel"]);
			Assert.AreEqual("ForceBuild", info.EnvironmentVariables["CCNetBuildCondition"]);
			Assert.AreEqual(@"c:\workingdir\", info.EnvironmentVariables["CCNetWorkingDirectory"]);
			Assert.AreEqual(@"c:\artifactdir\", info.EnvironmentVariables["CCNetArtifactDirectory"]);
		}

		[Test]
		public void ShouldPassAppropriateDefaultPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ExpectToExecuteArguments("");

			builder.Rakefile = "";
			builder.BuildArgs = "";
			builder.BaseDirectory = DefaultWorkingDirectory;
			result.ArtifactDirectory = DefaultWorkingDirectory;
			result.WorkingDirectory = DefaultWorkingDirectory;
			builder.Run(result);
		}

		[Test]
		public void ShouldPutQuotesAroundBuildFileIfItContainsASpace()
		{
			ExpectToExecuteArguments(@"--rakefile ""my project.rake""");

			builder.Rakefile = "my project.rake";
			builder.BuildArgs = "";
			builder.BaseDirectory = DefaultWorkingDirectory;
			result.ArtifactDirectory = DefaultWorkingDirectory;
			result.WorkingDirectory = DefaultWorkingDirectory;
			builder.Run(result);
		}

		[Test]
		public void ShouldEncloseDirectoriesInQuotesIfTheyContainSpaces()
		{
			DefaultWorkingDirectory = @"c:\dir with spaces";
			ExpectToExecuteArguments("");

			builder.Rakefile = "";
			builder.BuildArgs = "";
			builder.BaseDirectory = DefaultWorkingDirectory;
			result.ArtifactDirectory = DefaultWorkingDirectory;
			result.WorkingDirectory = DefaultWorkingDirectory;
			builder.Run(result);
		}
		
		[Test]
		public void ShouldRunWithMultipleTargetsSpecified()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			
			builder.Targets = new string[] { "targeta", "targetb", "targetc" };
			builder.Run(result);
			
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("targeta targetb targetc", info.Arguments);
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotSetUseProjectWorkingDirectoryAsBaseDirectory()
		{
			builder.BaseDirectory = null;
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsEmptyUseProjectWorkingDirectoryAsBaseDirectory()
		{
			builder.BaseDirectory = "";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("");
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsNotAbsoluteUseProjectWorkingDirectoryAsFirstPartOfBaseDirectory()
		{
			builder.BaseDirectory = "relativeBaseDirectory";
			CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart("relativeBaseDirectory");
		}

		private void CheckBaseDirectoryIsProjectDirectoryWithGivenRelativePart(string relativeDirectory)
		{
			string expectedBaseDirectory = "projectWorkingDirectory";
			if (relativeDirectory.Length > 0)
			{
				expectedBaseDirectory = Path.Combine(expectedBaseDirectory, relativeDirectory);
			}
			CheckBaseDirectory(IntegrationResultForWorkingDirectoryTest(), expectedBaseDirectory);
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
			builder.BaseDirectory = @"c:\my\base\directory";
			CheckBaseDirectory(IntegrationResultForWorkingDirectoryTest(), @"c:\my\base\directory");
		}

		private void CheckBaseDirectory(IIntegrationResult integrationResult, string expectedBaseDirectory)
		{
			ProcessResult returnVal = SuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			object[] arr = new object[1];
			arr[0] = constraint;
			mockProcessExecutor.ExpectAndReturn("Execute", returnVal, arr);
			builder.Run(integrationResult);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
		}

		[Test]
		public void ShouldGiveAPresentationValueForTargetsThatIsANewLineSeparatedEquivalentOfAllTargets()
		{
			builder.Targets = new string[] { "target1", "target2" };
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
		
		[Test]
		public void SilentOptionShouldAddSilentArgument()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			builder.Silent = true;
			builder.Run(result);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("--silent", info.Arguments);
		}
		
		[Test]
		public void SilentAndTraceOptionShouldAddSilentAndTraceArgument()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			builder.Silent = true;
			builder.Trace = true;
			builder.Run(result);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("--silent --trace", info.Arguments);
		}
		
		[Test]
		public void QuietOptionShouldAddQuietArgument()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			builder.Quiet = true;
			builder.Run(result);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("--quiet", info.Arguments);
		}
		
		[Test]
		public void QuietAndTraceOptionShouldAddQuietAndTraceArgument()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			builder.Quiet = true;
			builder.Trace = true;
			builder.Run(result);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("--quiet --trace", info.Arguments);
		}
		
		[Test]
		public void TraceOptionShouldAddTraceArgument()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			builder.Trace = true;
			builder.Run(result);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("--trace", info.Arguments);
		}
		
		[Test]
		public void SilentAndQuietOptionShouldOnlyAddSilentArgument()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), new object[] { constraint });
			builder.Silent = true;
			builder.Quiet = true;
			builder.Run(result);
			ProcessInfo info = (ProcessInfo)constraint.Parameter;
			Assert.AreEqual("--silent", info.Arguments);
		}
		
		[Test]
		public void ConstructorShouldNotThrowException()
		{
			new RakeTask();
		}
	}
}
