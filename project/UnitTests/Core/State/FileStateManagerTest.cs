using System;
using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
	[TestFixture]
	[Ignore("Ignored until someone with Rhino.Mocks knowlege fixed this.")]
	public class FileStateManagerTest : CustomAssertion
	{
		private const string ProjectName = IntegrationResultMother.DefaultProjectName;
		private const string DefaultStateFilename = "Test.state";
		private string applicationDataPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
						 Path.Combine("CruiseControl.NET", "Server"));

		private FileStateManager state;
		private IntegrationResult result;
		private MockRepository mocks;
        private IFileSystem fileSystem;
		private IExecutionEnvironment executionEnvironment;

		[SetUp]
		public void SetUp()
		{
            mocks = new MockRepository();
			fileSystem = mocks.DynamicMock<IFileSystem>();
			executionEnvironment = mocks.DynamicMock<IExecutionEnvironment>();

			SetupResult.For(executionEnvironment.IsRunningOnWindows).Return(true);
			SetupResult.For(executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server)).Return(applicationDataPath);
			SetupResult.For(fileSystem.DirectoryExists(applicationDataPath)).Return(true);
			Expect.Call(delegate { fileSystem.EnsureFolderExists(applicationDataPath); });
			mocks.ReplayAll();

			state = new FileStateManager(fileSystem, executionEnvironment);
			result = IntegrationResultMother.CreateSuccessful();
			result.ProjectName = ProjectName;
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"<state><directory>c:\temp</directory></state>";
            mocks.ReplayAll();
            state = (FileStateManager)NetReflector.Read(xml);
			Assert.AreEqual(@"c:\temp", state.StateFileDirectory);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void LoadShouldThrowExceptionIfStateFileDoesNotExist()
		{
            Expect.Call(fileSystem.Load(StateFilename())).Throw(new FileNotFoundException());
            mocks.ReplayAll();
			state.LoadState(ProjectName);
		}

		[Test]
		public void HasPreviousStateIsTrueIfStateFileExists()
		{
            Expect.Call(fileSystem.FileExists(StateFilename())).Return(true);
            mocks.ReplayAll();
            Assert.IsTrue(state.HasPreviousState(ProjectName));			
		}

        [Test]
        public void SaveWithInvalidDirectory()
        {
            string foldername = @"c:\CCNet_remove_invalid";

            Expect.Call(() => fileSystem.EnsureFolderExists(foldername));
            mocks.ReplayAll();
            state.StateFileDirectory = foldername;
        }

		[Test]
		public void AttemptToSaveWithInvalidXml()
		{
            Expect.Call(() => fileSystem.AtomicSave(string.Empty, string.Empty)).
                Constraints(
                    new Equal(StateFilename()),
                    new Anything());
            mocks.ReplayAll();

			result.Label = "<&/<>";
			result.AddTaskResult("<badxml>>");
			state.SaveState(result);
		}

        [Test]
        public void LoadStateFileWithValid144Data()
        {
            var data = @"<?xml version=""1.0"" encoding=""utf-8""?>
<IntegrationResult xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ProjectName>ccnetlive</ProjectName>
  <ProjectUrl>http://CRAIG-PC/ccnet</ProjectUrl>
  <BuildCondition>ForceBuild</BuildCondition>
  <Label>7</Label>
  <Parameters />
  <WorkingDirectory>e:\sourcecontrols\sourceforge\ccnetlive</WorkingDirectory>
  <ArtifactDirectory>e:\download-area\CCNetLive-Builds</ArtifactDirectory>
  <Status>Success</Status>
  <StartTime>2009-06-17T13:28:35.7652391+12:00</StartTime>
  <EndTime>2009-06-17T13:29:13.7824391+12:00</EndTime>
  <LastIntegrationStatus>Success</LastIntegrationStatus>
  <LastSuccessfulIntegrationLabel>7</LastSuccessfulIntegrationLabel>
  <FailureUsers />
</IntegrationResult>";

            Expect.Call(fileSystem.Load(StateFilename()))
                .Return(new StringReader(data));
            mocks.ReplayAll();
            state.LoadState(ProjectName);
        }

        [Test]
        [ExpectedException(typeof(CruiseControlException))]
        public void LoadStateThrowsAnExceptionWithInvalidData()
        {
            var data = @"<?xml version=""1.0"" encoding=""utf-8""?>
<garbage />";

            Expect.Call(fileSystem.Load(StateFilename()))
                .Return(new StringReader(data));
            mocks.ReplayAll();
            state.LoadState(ProjectName);
        }

		[Test]
		public void SaveProjectWithSpacesInName()
		{
            Expect.Call(() => fileSystem.AtomicSave(string.Empty, string.Empty)).
                Constraints(
					new Equal(Path.Combine(executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server), "MyProject.state")),
                    new Anything());
            mocks.ReplayAll();

			result.ProjectName = "my project";
			state.SaveState(result);
		}

		[Test]
		public void ShouldWriteXmlUsingUTF8Encoding()
		{
            Expect.Call(() => fileSystem.AtomicSave(string.Empty, string.Empty)).
                Constraints(
                    new Equal(StateFilename()),
                    new StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));
            mocks.ReplayAll();

			result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			state.SaveState(result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void HandleExceptionSavingStateFile()
		{
            Expect.Call(() => fileSystem.AtomicSave(string.Empty, string.Empty)).
                Constraints(
                    new Equal(StateFilename()),
                    new Anything())
                .Throw(new SystemException());
            mocks.ReplayAll();
            state.SaveState(result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void HandleExceptionLoadingStateFile()
		{
            Expect.Call(fileSystem.Load(StateFilename()))
                .Throw(new SystemException());
            mocks.ReplayAll();
            state.LoadState(ProjectName);
		}

		[Test]
		public void LoadStateFromVersionedXml()
		{
			string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<IntegrationResult xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ProjectName>NetReflector</ProjectName>
  <ProjectUrl>http://localhost/ccnet</ProjectUrl>
  <BuildCondition>ForceBuild</BuildCondition>
  <Label>1.0.0.7</Label>
  <WorkingDirectory>C:\dev\ccnet\integrationTests\netreflector</WorkingDirectory>
  <ArtifactDirectory>C:\dev\ccnet\trunk4\build\server\NetReflector\Artifacts</ArtifactDirectory>
  <StatisticsFile>report.xml</StatisticsFile>
  <Status>Success</Status>
  <LastIntegrationStatus>Success</LastIntegrationStatus>
  <LastSuccessfulIntegrationLabel>1.0.0.7</LastSuccessfulIntegrationLabel>
  <StartTime>2006-12-10T14:41:50-08:00</StartTime>
  <EndTime>2006-12-10T14:42:12-08:00</EndTime>
</IntegrationResult>";

            mocks.ReplayAll();
            result = (IntegrationResult)state.LoadState(new StringReader(xml));
			Assert.AreEqual("NetReflector", result.ProjectName);
			Assert.AreEqual("http://localhost/ccnet", result.ProjectUrl);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual("1.0.0.7", result.Label);
			Assert.AreEqual(@"C:\dev\ccnet\integrationTests\netreflector", result.WorkingDirectory);
			Assert.AreEqual(@"C:\dev\ccnet\trunk4\build\server\NetReflector\Artifacts", result.ArtifactDirectory);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Success, result.LastIntegrationStatus);
			Assert.AreEqual("1.0.0.7", result.LastSuccessfulIntegrationLabel);
			Assert.AreEqual(new DateTime(2006, 12, 10, 22, 41, 50), result.StartTime.ToUniversalTime());
		}

		private string StateFilename()
		{
			return Path.Combine(executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server), DefaultStateFilename);
		}
	}
}