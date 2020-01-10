using System;
using System.IO;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
	[TestFixture]
	public class FileStateManagerTest : CustomAssertion
	{
		private const string ProjectName = IntegrationResultMother.DefaultProjectName;
		private const string DefaultStateFilename = "Test.state";
		private string applicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Path.Combine("CruiseControl.NET", "Server"));

		private FileStateManager state;
		private IntegrationResult result;
		private MockRepository mocks;
		private IFileSystem fileSystem;
		private IExecutionEnvironment executionEnvironment;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository(MockBehavior.Default);
			fileSystem = mocks.Create<IFileSystem>().Object;
			executionEnvironment = mocks.Create<IExecutionEnvironment>().Object;
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
			state = (FileStateManager)NetReflector.Read(xml);
			Assert.AreEqual(@"c:\temp", state.StateFileDirectory);
		}

        [Test]
        public void SaveToNonExistingFolder()
        {
            string newDirectory = Directory.GetCurrentDirectory() + "\\NewDirectory";
            Assert.IsFalse(Directory.Exists(newDirectory), "The test directory should not exist");

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureGivenFolderExists(newDirectory)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.AtomicSave(It.IsNotNull<string>(), It.IsAny<string>())).Verifiable();

            state = new FileStateManager(fileSystem, executionEnvironment);
            state.StateFileDirectory = newDirectory;
            result = IntegrationResultMother.CreateSuccessful();
            result.ProjectName = "my project";
            state.SaveState(result);
        }

        [Test]
		public void LoadShouldThrowExceptionIfStateFileDoesNotExist()
		{
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.Load(It.IsNotNull<string>())).Throws(new FileNotFoundException()).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);
			result = IntegrationResultMother.CreateSuccessful();
			result.ProjectName = ProjectName;

		    Assert.That(delegate { state.LoadState(ProjectName); },
		                Throws.TypeOf<CruiseControlException>().With.Property("InnerException").TypeOf<FileNotFoundException>());
		}

		[Test]
		public void HasPreviousStateIsTrueIfStateFileExists()
		{
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(It.IsNotNull<String>())).Returns(true).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);
			Assert.IsTrue(state.HasPreviousState(ProjectName));
		}

		[Test]
		public void SaveWithInvalidDirectory()
		{
			string foldername = @"c:\CCNet_remove_invalid";
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureGivenFolderExists(foldername)).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);
			state.StateFileDirectory = foldername;

            // get the value so that the folder is created 
            foldername = state.StateFileDirectory;
		}

		[Test]
		public void AttemptToSaveWithInvalidXml()
		{
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.AtomicSave(It.IsNotNull<string>(), It.IsAny<string>())).Verifiable();

			result = IntegrationResultMother.CreateSuccessful();
			result.Label = "<&/<>";
			result.AddTaskResult("<badxml>>");
			state = new FileStateManager(fileSystem, executionEnvironment);
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
  <FailureTasks />
</IntegrationResult>";

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.Load(It.IsNotNull<string>())).Returns(new StringReader(data)).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);
			state.LoadState(ProjectName);
		}

		[Test]
		public void LoadStateThrowsAnExceptionWithInvalidData()
		{
			var data = @"<?xml version=""1.0"" encoding=""utf-8""?><garbage />";

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.Load(It.IsNotNull<string>())).Returns(new StringReader(data)).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);

		    Assert.That(delegate { state.LoadState(ProjectName); }, Throws.TypeOf<CruiseControlException>());
		}

        [Test]
        public void SaveProjectWithSpacesInName()
        {
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.AtomicSave(It.IsNotNull<string>(), It.IsAny<string>())).Verifiable();

            result = IntegrationResultMother.CreateSuccessful();
            result.ProjectName = "my project";
            state = new FileStateManager(fileSystem, executionEnvironment);
            state.SaveState(result);
        }

        [Test]
        public void SaveProjectWithManySpacesInName()
        {
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.AtomicSave(It.IsNotNull<string>(), It.IsAny<string>())).Verifiable();

            result = IntegrationResultMother.CreateSuccessful();
            result.ProjectName = "my    project     with   many    spaces";
            state = new FileStateManager(fileSystem, executionEnvironment);
            state.SaveState(result);
        }


	    [Test]
		public void ShouldWriteXmlUsingUTF8Encoding()
		{
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.AtomicSave(It.IsNotNull<string>(), It.Is<string>(_content => _content.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"))))
                .Verifiable();

			result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			state = new FileStateManager(fileSystem, executionEnvironment);
			state.SaveState(result);
		}

		[Test]
		public void HandleExceptionSavingStateFile()
		{
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.AtomicSave(It.IsNotNull<string>(), It.IsAny<string>())).Throws(new CruiseControlException()).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);

            Assert.That(delegate { state.SaveState(result); }, Throws.TypeOf<CruiseControlException>());
		}

		[Test]
		public void HandleExceptionLoadingStateFile()
		{
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(applicationDataPath)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.Load(It.IsNotNull<string>())).Throws(new CruiseControlException()).Verifiable();

			state = new FileStateManager(fileSystem, executionEnvironment);

            Assert.That(delegate { state.LoadState(ProjectName); }, Throws.TypeOf<CruiseControlException>());
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
	}
}
