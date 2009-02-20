using NUnit.Framework;
using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
    [TestFixture]
    public class XmlProjectStateManagerTest
    {
        #region Fields
        private IProjectStateManager _stateManager = new XmlProjectStateManager();
        private readonly string _persistanceFilePath = Path.Combine(Environment.CurrentDirectory, "ProjectsState.xml");
        #endregion

        #region Public methods
        #region Setup
        [TestFixtureSetUp]
        public void Setup()
        {
            if (File.Exists(_persistanceFilePath)) File.Delete(_persistanceFilePath);
            _stateManager.RecordProjectAsStopped("Test Project #3");
            _stateManager.RecordProjectAsStartable("Test Project #4");
        }
        #endregion

        #region CleanUp
        [TestFixtureTearDown]
        public void CleanUp()
        {
            if (File.Exists(_persistanceFilePath)) File.Delete(_persistanceFilePath);
        }
        #endregion

        #region RecordProjectAsStopped() tests
        [Test]
        public void RecordProjectAsStopped()
        {
            if (File.Exists(_persistanceFilePath)) File.Delete(_persistanceFilePath);
            string projectName = "Test Project #1";
            _stateManager.RecordProjectAsStopped(projectName);
            bool result = _stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsFalse(result, "Project state incorrect");
            Assert.IsTrue(File.Exists(_persistanceFilePath), "Persistence file not generated");
        }

        [Test]
        public void RecordProjectAsStoppedAlreadyStopped()
        {
            if (File.Exists(_persistanceFilePath)) File.Delete(_persistanceFilePath);
            string projectName = "Test Project #1";
            _stateManager.RecordProjectAsStopped(projectName);
            bool result = _stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsFalse(result, "Project state incorrect");
            Assert.IsFalse(File.Exists(_persistanceFilePath), "Persistence file generated");
        }
        #endregion

        #region RecordProjectAsStartable() tests
        [Test]
        public void RecordProjectAsStartable()
        {
            if (File.Exists(_persistanceFilePath)) File.Delete(_persistanceFilePath);
            string projectName = "Test Project #1";
            _stateManager.RecordProjectAsStartable(projectName);
            bool result = _stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
            Assert.IsTrue(File.Exists(_persistanceFilePath), "Persistence file not generated");
        }
        #endregion

        #region CheckIfProjectCanStart() tests
        [Test]
        public void CheckIfProjectCanStartUnknownProject()
        {
            string projectName = "Test Project #2";
            bool result = _stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
        }

        [Test]
        public void CheckIfProjectCanStartKnownStoppedProject()
        {
            string projectName = "Test Project #3";
            bool result = _stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsFalse(result, "Project state incorrect");
        }

        [Test]
        public void CheckIfProjectCanStartKnownStartableProject()
        {
            string projectName = "Test Project #4";
            bool result = _stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
        }

        [Test]
        public void CheckIfProjectCanStartKnownStoppedProjectFromFile()
        {
            string projectName = "Test Project #3";
            IProjectStateManager stateManager = new XmlProjectStateManager();
            bool result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsFalse(result, "Project state incorrect");
        }

        [Test]
        public void CheckIfProjectCanStartKnownStartableProjectFromFile()
        {
            string projectName = "Test Project #4";
            IProjectStateManager stateManager = new XmlProjectStateManager();
            bool result = stateManager.CheckIfProjectCanStart(projectName);
            Assert.IsTrue(result, "Project state incorrect");
        }
        #endregion
        #endregion
    }
}
