using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
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

		[SetUp]
		public void SetUp()
		{
			_mockExecutor = new DynamicMock(typeof(ProcessExecutor));
			_builder = new CommandLineBuilder((ProcessExecutor) _mockExecutor.MockInstance);
		}

		private void VerifyAll()
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
			Assert.AreEqual(@"C:\", _builder.ConfiguredBaseDirectory);
			Assert.AreEqual("mybatchfile.bat", _builder.Executable);
			Assert.AreEqual(123, _builder.BuildTimeoutSeconds);
			Assert.AreEqual("myarg1 myarg2", _builder.BuildArgs);
			VerifyAll();
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			string xml = @"
    <commandLineBuilder>
    	<executable>mybatchfile.bat</executable>
    </commandLineBuilder>";

			NetReflector.Read(xml, _builder);
			Assert.AreEqual("mybatchfile.bat", _builder.Executable);
			Assert.AreEqual(600, _builder.BuildTimeoutSeconds);
			Assert.AreEqual("", _builder.BuildArgs);
			VerifyAll();
		}

		[Test]
		public void ShouldSetSuccessfulStatusAndBuildOutputAsAResultOfASuccessfulBuild()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);

			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(returnVal.StandardOutput + "\n" + returnVal.StandardError, result.Output);
			VerifyAll();
		}

		[Test]
		public void ShouldSetFailedStatusAndBuildOutputAsAResultOfFailedBuild()
		{
			ProcessResult returnVal = new ProcessResult("output", null, FAILED_EXIT_CODE, false);
			_mockExecutor.ExpectAndReturn("Execute", returnVal, new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);

			Assert.IsTrue(result.Failed);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			Assert.AreEqual(returnVal.StandardOutput + "\n" + returnVal.StandardError, result.Output);
			VerifyAll();
		}

		// TODO - Timeout?
		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			_mockExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);
			VerifyAll();
		}

		[Test]
		public void ShouldPassSpecifiedPropertiesAsProcessInfoArgumentsToProcessExecutor()
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);
			
			IntegrationResult result = new IntegrationResult();
			result.Label = "1.0";

			_builder.Executable = "test-exe";
			_builder.BuildArgs = "test-args";
			_builder.BuildTimeoutSeconds = 222;
			_builder.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual("test-exe", info.FileName);
			Assert.AreEqual(222000, info.TimeOut);
			Assert.AreEqual("test-args", info.Arguments);
			VerifyAll();
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
			string expectedBaseDirectory = "projectWorkingDirectory";
			if (relativeDirectory != "")
			{
				expectedBaseDirectory = Path.Combine(expectedBaseDirectory, relativeDirectory);
			}
			CheckBaseDirectory(new IntegrationResult("project", "projectWorkingDirectory"), expectedBaseDirectory);
			VerifyAll();
		}

		[Test]
		public void IfConfiguredBaseDirectoryIsAbsoluteUseItAtBaseDirectory()
		{
			_builder.ConfiguredBaseDirectory = @"c:\my\base\directory";
			CheckBaseDirectory(new IntegrationResult(), @"c:\my\base\directory");
		}

		private void CheckBaseDirectory(IntegrationResult result, string expectedBaseDirectory)
		{
			ProcessResult returnVal = CreateSuccessfulProcessResult();
			CollectingConstraint constraint = new CollectingConstraint();
			_mockExecutor.ExpectAndReturn("Execute", returnVal, constraint);

			_builder.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(expectedBaseDirectory, info.WorkingDirectory);
			VerifyAll();
		}


		private ProcessResult CreateSuccessfulProcessResult()
		{
			return new ProcessResult("output", null, SUCCESSFUL_EXIT_CODE, false);
		}
	}
}