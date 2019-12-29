using System;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
	[TestFixture]
	public class XmlProjectStateManagerTest
	{
		#region Fields
		private string applicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Path.Combine("CruiseControl.NET", "Server"));
		private string persistanceFilePath;

		private XmlProjectStateManager stateManager;
		private MockRepository mocks;
		private IFileSystem fileSystem;
		private IExecutionEnvironment executionEnvironment;
		#endregion

		#region Public methods
		#region Setup
		[SetUp]
		public void Setup()
		{
			persistanceFilePath = Path.Combine(applicationDataPath, "ProjectsState.xml");
			mocks = new MockRepository(MockBehavior.Default);
			fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
			executionEnvironment = mocks.Create<IExecutionEnvironment>(MockBehavior.Strict).Object;
		}

		private void SetupDefaultContent()
		{
			var defaultFile = "<state><project>Test Project #3</project></state>";
			var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(defaultFile));
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(persistanceFilePath)).Returns(true).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenInputStream(persistanceFilePath)).Returns(stream).Verifiable();
		}
		#endregion

		#region Constructors
		[Test]
		public void DefaultConstructorSetsPersistanceLocation()
		{
			// This is an indirect test - if the correct location is set, then the FileExists call
			// will be valid
            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(persistanceFilePath)).Returns(false).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			stateManager.CheckIfProjectCanStart("Project");
			mocks.VerifyAll();
		}
		#endregion

		#region RecordProjectAsStopped() tests
		[Test]
		public void RecordProjectAsStopped()
		{
			var projectName = "Test Project #1";
			SetupDefaultContent();
			var stream = InitialiseOutputStream();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			stateManager.RecordProjectAsStopped(projectName);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsFalse(result, "Project state incorrect");
			mocks.VerifyAll();

			var expectedData = "<state><project>Test Project #3</project><project>Test Project #1</project></state>";
			ValidateStreamData(stream, expectedData);
		}

		[Test]
		public void RecordProjectAsStoppedAlreadyStopped()
		{
			var projectName = "Test Project #1";
			SetupDefaultContent();
			var stream = InitialiseOutputStream();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			stateManager.RecordProjectAsStopped(projectName);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsFalse(result, "Project state incorrect");
			mocks.VerifyAll();

			var expectedData = "<state><project>Test Project #3</project><project>Test Project #1</project></state>";
			ValidateStreamData(stream, expectedData);
		}
		#endregion

		#region RecordProjectAsStartable() tests
		[Test]
		public void RecordProjectAsStartable()
		{
			var projectName = "Test Project #1";
			SetupDefaultContent();
			var stream = InitialiseOutputStream();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			stateManager.RecordProjectAsStartable(projectName);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsTrue(result, "Project state incorrect");
			mocks.VerifyAll();

			var expectedData = "<state><project>Test Project #3</project></state>";
			ValidateStreamData(stream, expectedData);
		}

		[Test]
		public void RecordProjectAsStartableAlreadyStarted()
		{
			var projectName = "Test Project #1";
			SetupDefaultContent();
			var stream = InitialiseOutputStream();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			stateManager.RecordProjectAsStartable(projectName);
			stateManager.RecordProjectAsStartable(projectName);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsTrue(result, "Project state incorrect");
			mocks.VerifyAll();
			var expectedData = "<state><project>Test Project #3</project></state>";
			ValidateStreamData(stream, expectedData);
		}

		[Test]
		public void RecordProjectAsStartableAfterStopped()
		{
			var projectName = "Test Project #1";
			SetupDefaultContent();
			var stream1 = new MemoryStream();
			var stream2 = new MemoryStream();
			Mock.Get(fileSystem).SetupSequence(_fileSystem => _fileSystem.OpenOutputStream(persistanceFilePath))
				.Returns(stream1).Returns(stream2);

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			stateManager.RecordProjectAsStopped(projectName);
			stateManager.RecordProjectAsStartable(projectName);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsTrue(result, "Project state incorrect");
			mocks.VerifyAll();

			var expectedData1 = "<state><project>Test Project #3</project><project>Test Project #1</project></state>";
			ValidateStreamData(stream1, expectedData1);
			var expectedData2 = "<state><project>Test Project #3</project></state>";
			ValidateStreamData(stream2, expectedData2);
		}
		#endregion

		#region CheckIfProjectCanStart() tests
		[Test]
		public void CheckIfProjectCanStartUnknownProject()
		{
			var projectName = "Test Project #2";
			SetupDefaultContent();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsTrue(result, "Project state incorrect");
			mocks.VerifyAll();
		}

		[Test]
		public void CheckIfProjectCanStartKnownStoppedProject()
		{
			var projectName = "Test Project #3";
			SetupDefaultContent();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsFalse(result, "Project state incorrect");
			mocks.VerifyAll();
		}

		[Test]
		public void CheckIfProjectCanStartKnownStartableProject()
		{
			var projectName = "Test Project #4";
			SetupDefaultContent();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsTrue(result, "Project state incorrect");
			mocks.VerifyAll();
		}

		[Test]
		public void CheckIfProjectCanStartKnownStoppedProjectFromFile()
		{
			var projectName = "Test Project #3";
			SetupDefaultContent();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsFalse(result, "Project state incorrect");
			mocks.VerifyAll();
		}

		[Test]
		public void CheckIfProjectCanStartKnownStartableProjectFromFile()
		{
			var projectName = "Test Project #4";
			SetupDefaultContent();

            Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(It.IsNotNull<ApplicationType>()))
                .Returns(applicationDataPath).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(It.IsNotNull<string>())).Verifiable();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			var result = stateManager.CheckIfProjectCanStart(projectName);
			Assert.IsTrue(result, "Project state incorrect");
			mocks.VerifyAll();
		}
		#endregion

		private MemoryStream InitialiseOutputStream()
		{
			var stream = new MemoryStream();
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenOutputStream(persistanceFilePath)).Returns(stream).Verifiable();
			return stream;
		}

		private void ValidateStreamData(MemoryStream stream, string expectedData)
		{
			using (var inStream = new MemoryStream(stream.GetBuffer()))
			{
				using (var reader = new StreamReader(inStream))
				{
					var streamData = reader.ReadToEnd();
					var zeroPos = streamData.IndexOf('\x0');
					if (zeroPos >= 0) streamData = streamData.Substring(0, zeroPos);
					Assert.AreEqual(expectedData, streamData);
				}
			}
		}
		#endregion
	}
}
