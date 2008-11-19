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
	public class DevenvTaskTest : ProcessExecutorTestFixtureBase
	{
		private const string DEVENV_PATH = @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.com";

		private const string DEVENV_2008_PATH = @"C:\Program Files\Microsoft Visual Studio 9\Common7\IDE\";
		private const string DEVENV_2005_PATH = @"C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\";
		private const string DEVENV_2003_PATH = @"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\";
		private const string DEVENV_2002_PATH = @"C:\Program Files\Microsoft Visual Studio .NET\Common7\IDE\";

		private const string SOLUTION_FILE = @"D:\dev\ccnet\ccnet\project\ccnet.sln";
		private const string CONFIGURATION = "Debug";

		private DevenvTask task;
		private IMock mockRegistry;

		[SetUp]
		public void Setup()
		{
			mockRegistry = new DynamicMock(typeof (IRegistry));
			CreateProcessExecutorMock(DEVENV_PATH);
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
	<version>9.0</version>
	<solutionfile>mySolution.sln</solutionfile>
	<configuration>Debug</configuration>
	<buildTimeoutSeconds>4</buildTimeoutSeconds>
	<project>MyProject</project>
	<buildtype>Clean</buildtype>
</devenv>";

			DevenvTask task2 = (DevenvTask) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\vs.net\devenv.com", task2.Executable);
			Assert.AreEqual(@"mySolution.sln", task2.SolutionFile);
			Assert.AreEqual(@"Debug", task2.Configuration);
			Assert.AreEqual(4, task2.BuildTimeoutSeconds);
			Assert.AreEqual(@"Clean", task2.BuildType);
			Assert.AreEqual(@"MyProject", task2.Project);
		}

		[Test]
		public void ShouldLoadMinimalValuesFromConfiguration()
		{
			string xml = @"<devenv solutionfile=""mySolution.sln"" configuration=""Release"" />";
			DevenvTask task2 = (DevenvTask) NetReflector.Read(xml);
			Assert.AreEqual(@"mySolution.sln", task2.SolutionFile);
			Assert.AreEqual(@"Release", task2.Configuration);
			Assert.AreEqual(DevenvTask.DEFAULT_BUILD_TIMEOUT, task2.BuildTimeoutSeconds);
			Assert.AreEqual(@"rebuild", task2.BuildType);
			Assert.AreEqual(@"", task2.Project);
		}

		[Test, ExpectedException(typeof (NetReflectorException))]
		public void ShouldFailToLoadInvalidVersionFromConfiguration()
		{
			string xml = @"<devenv solutionfile=""mySolution.sln"" configuration=""Release"" version=""VSBAD""/>";
			NetReflector.Read(xml);
		}

        [Test]
        public void DefaultVisualStudioShouldBe2008IfInstalled()
        {
            IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));
           
            DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", DEVENV_2008_PATH,
                                         DevenvTask.VS2008_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
            Assert.AreEqual(DEVENV_2008_PATH + "devenv.com", task2.Executable);
            mockRegistry2.Verify();
            mockProcessExecutor.Verify();
        }

		[Test]
		public void SelectVisualStudio2008ExplicitlyUsingVersionNumberWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2008_PATH,
										 DevenvTask.VS2008_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "9.0";

			Assert.AreEqual(DEVENV_2008_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void SelectVisualStudio2008ExplicitlyUsingVersionNameWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2008_PATH,
										 DevenvTask.VS2008_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "VS2008";

			Assert.AreEqual(DEVENV_2008_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

        [Test]
        public void DefaultVisualStudioShouldBe2005IfNothingNewerInstalled()
        {
            IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

            DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2008_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", DEVENV_2005_PATH,
                                         DevenvTask.VS2005_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			Assert.AreEqual(DEVENV_2005_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
            mockProcessExecutor.Verify();
        }

		[Test]
		public void SelectVisualStudio2005ExplicitlyUsingVersionNumberWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2005_PATH,
										 DevenvTask.VS2005_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "8.0";

			Assert.AreEqual(DEVENV_2005_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void SelectVisualStudio2005ExplicitlyUsingVersionNameWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2005_PATH,
										 DevenvTask.VS2005_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "VS2005";

			Assert.AreEqual(DEVENV_2005_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

        [Test]
        public void DefaultVisualStudioShouldBe2003IfNothingNewerInstalled()
		{
            IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));
            
            DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2008_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2005_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", DEVENV_2003_PATH,
										 DevenvTask.VS2003_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
			Assert.AreEqual(DEVENV_2003_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
            mockProcessExecutor.Verify();
        }

		[Test]
		public void SelectVisualStudio2003ExplicitlyUsingVersionNumberWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2003_PATH,
										 DevenvTask.VS2003_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "7.1";

			Assert.AreEqual(DEVENV_2003_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void SelectVisualStudio2003ExplicitlyUsingVersionNameWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2003_PATH,
										 DevenvTask.VS2003_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "VS2003";

			Assert.AreEqual(DEVENV_2003_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
        public void DefaultVisualStudioShouldBe2002IfNothingNewerInstalled()
		{
            IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));
           
            DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2008_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2005_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, DevenvTask.VS2003_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", DEVENV_2002_PATH,
										 DevenvTask.VS2002_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			Assert.AreEqual(DEVENV_2002_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
            mockProcessExecutor.Verify();
        }

		[Test]
		public void SelectVisualStudio2002ExplicitlyUsingVersionNumberWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2002_PATH,
										 DevenvTask.VS2002_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "7.0";

			Assert.AreEqual(DEVENV_2002_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void SelectVisualStudio2002ExplicitlyUsingVersionNameWhenEverythingInstalled()
		{
			IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

			DevenvTask task2 = new DevenvTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
			mockRegistry2.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", DEVENV_2002_PATH,
										 DevenvTask.VS2002_REGISTRY_PATH, DevenvTask.VS_REGISTRY_KEY);

			task2.Version = "VS2002";

			Assert.AreEqual(DEVENV_2002_PATH + "devenv.com", task2.Executable);
			mockRegistry2.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void VerifyDevenvProcessInfo()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			ProcessResult processResult = new ProcessResult("output", "error", 0, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { constraint, new IsAnything() });
			task.Executable = DEVENV_PATH;
			task.SolutionFile = "\"mySolution.sln\"";
			task.Configuration = "Debug";

			task.Run(IntegrationResult());

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(DEVENV_PATH, info.FileName);
			Assert.AreEqual(DevenvTask.DEFAULT_BUILD_TIMEOUT*1000, info.TimeOut);
            CustomAssertion.AssertStartsWith("\"mySolution.sln\" /rebuild \"Debug\"", info.Arguments);
		}

		[Test]
		public void VerifyDevenvProcessInfoWithProjectDefined()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			ProcessResult processResult = new ProcessResult("output", "error", 0, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] {constraint, new IsAnything()});
			task.Executable = DEVENV_PATH;
			task.SolutionFile = "mySolution.sln";
			task.Configuration = "\"Debug\"";
			task.Project = "myProject";

			task.Run(IntegrationResult());

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual(DEVENV_PATH, info.FileName);
			Assert.AreEqual(DevenvTask.DEFAULT_BUILD_TIMEOUT*1000, info.TimeOut);
            CustomAssertion.AssertStartsWith("\"mySolution.sln\" /rebuild \"Debug\" /project \"myProject\"", info.Arguments);
		}

		[Test]
		public void ShouldSetOutputAndIntegrationStatusToSuccessOnSuccessfulBuild()
		{
			ProcessResult processResult = new ProcessResult(@"Rebuild All: 10 succeeded, 0 failed, 0 skipped", string.Empty, ProcessResult.SUCCESSFUL_EXIT_CODE, false);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { new IsAnything(), new IsAnything() });
			task.Executable = DEVENV_PATH;
			task.SolutionFile = SOLUTION_FILE;
			task.Configuration = CONFIGURATION;

			IntegrationResult result = (IntegrationResult)IntegrationResult();
			task.Run(result);

			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			CustomAssertion.AssertMatches(@"Rebuild All: \d+ succeeded, \d+ failed, \d+ skipped", result.TaskOutput);
		}

		[Test]
		public void ShouldSetOutputAndIntegrationStatusToFailedOnFailedBuild()
		{
			ProcessResult processResult = new ProcessResult(@"D:\dev\ccnet\ccnet\project\nosolution.sln could not be found and will not be loaded", string.Empty, 1, false);
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] {constraint, new IsAnything()});

			task.Executable = DEVENV_PATH;
			task.SolutionFile = @"D:\dev\ccnet\ccnet\project\nosolution.sln";
			task.Configuration = CONFIGURATION;

			IIntegrationResult result = Integration("myProject", "myWorkingDirectory", "myArtifactDirectory");
			task.Run(result);

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			Assert.AreEqual("myWorkingDirectory", info.WorkingDirectory);

			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
			CustomAssertion.AssertMatches(@"(\.|\n)*could not be found and will not be loaded", result.TaskOutput);
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessExecutorThrowsAnException()
		{
			mockProcessExecutor.ExpectAndThrow("Execute", new IOException(), new object[] {new IsAnything(), new IsAnything()});
			task.Executable = DEVENV_PATH + ".some.extra.ext.exe"; // file should not exist
			task.SolutionFile = @"D:\dev\ccnet\ccnet\project\nosolution.sln";
			task.Configuration = "Debug";

			task.Run(IntegrationResult());
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessExecutorThrowsAnExceptionUsingUnkownProject()
		{
			mockProcessExecutor.ExpectAndThrow("Execute", new IOException(), new object[] {new IsAnything(), new IsAnything()});
			task.Executable = DEVENV_PATH;
			task.SolutionFile = SOLUTION_FILE;
			task.Configuration = CONFIGURATION;
			task.Project = "unknownproject";

			task.Run(IntegrationResult());
		}

		[Test, ExpectedException(typeof (BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessTimesOut()
		{
			ProcessResult processResult = new ProcessResult(string.Empty, string.Empty, ProcessResult.TIMED_OUT_EXIT_CODE, true);
			mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] {new IsAnything(), new IsAnything()});
			task.BuildTimeoutSeconds = 2;
			task.Executable = DEVENV_PATH;
			task.SolutionFile = SOLUTION_FILE;
			task.Configuration = CONFIGURATION;

			task.Run(IntegrationResult());
		}
	}
}
