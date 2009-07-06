using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
    [TestFixture]
	[Ignore("Ignored until someone with Rhino.Mocks knowlege fixed this.")]
    public class XmlProjectStateManagerTest
    {
        #region Fields
		private string applicationDataPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
						 Path.Combine("CruiseControl.NET", "Server"));
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

            mocks = new MockRepository();
			fileSystem = mocks.StrictMock<IFileSystem>();
			executionEnvironment = mocks.StrictMock<IExecutionEnvironment>();

			SetupResult.For(executionEnvironment.IsRunningOnWindows).Return(true);
			SetupResult.For(executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server)).Return(applicationDataPath);
			SetupResult.For(fileSystem.DirectoryExists(applicationDataPath)).Return(true);
			Expect.Call(delegate { fileSystem.EnsureFolderExists(applicationDataPath); });
			mocks.ReplayAll();

			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
        }

        private void SetupDefaultContent()
        {
            var defaultFile = "<state><project>Test Project #3</project></state>";
            var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(defaultFile));
			Expect.Call(fileSystem.FileExists(persistanceFilePath)).Return(true);
			Expect.Call(fileSystem.OpenInputStream(persistanceFilePath))
                .Return(stream);
        }
        #endregion

        #region Constructors
        [Test]
        public void DefaultConstructorSetsPersistanceLocation()
        {
            // This is an indirect test - if the correct location is set, then the FileExists call
            // will be valid
			stateManager = new XmlProjectStateManager(fileSystem, executionEnvironment);
			Expect.Call(fileSystem.FileExists(persistanceFilePath)).Return(false);
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
            mocks.ReplayAll();
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
            mocks.ReplayAll();
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
            mocks.ReplayAll();
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
            mocks.ReplayAll();
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
            var stream1 = InitialiseOutputStream();
            var stream2 = InitialiseOutputStream();
            mocks.ReplayAll();
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
            mocks.ReplayAll();
            var result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
            mocks.VerifyAll();
        }

        [Test]
        public void CheckIfProjectCanStartKnownStoppedProject()
        {
            var projectName = "Test Project #3";
            SetupDefaultContent();
            mocks.ReplayAll();
            var result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsFalse(result, "Project state incorrect");
            mocks.VerifyAll();
        }

        [Test]
        public void CheckIfProjectCanStartKnownStartableProject()
        {
            var projectName = "Test Project #4";
            SetupDefaultContent();
            mocks.ReplayAll();
            var result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
            mocks.VerifyAll();
        }

        [Test]
        public void CheckIfProjectCanStartKnownStoppedProjectFromFile()
        {
            var projectName = "Test Project #3";
            SetupDefaultContent();
            mocks.ReplayAll();
            var result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsFalse(result, "Project state incorrect");
            mocks.VerifyAll();
        }

        [Test]
        public void CheckIfProjectCanStartKnownStartableProjectFromFile()
        {
            var projectName = "Test Project #4";
            SetupDefaultContent();
            mocks.ReplayAll();
            var result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
            mocks.VerifyAll();
        }
        #endregion

        private MemoryStream InitialiseOutputStream()
        {
            var stream = new MemoryStream();
			Expect.Call(fileSystem.OpenOutputStream(persistanceFilePath)).Return(stream);
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
