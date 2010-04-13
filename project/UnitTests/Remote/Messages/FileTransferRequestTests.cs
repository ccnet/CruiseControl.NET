namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class FileTransferRequestTests
    {
        #region Tests
        #region Constructor tests
        [Test]
        public void SessionConstructorInitialisesTheValues()
        {
            var sessionId = "MyNewSession";
            var request = new FileTransferRequest(sessionId);
            Assert.AreEqual(sessionId, request.SessionToken);
        }

        [Test]
        public void FullConstructorInitialisesTheValues()
        {
            var sessionId = "MyNewSession";
            var projectName = "projectName";
            var fileName = "fileName";
            var request = new FileTransferRequest(sessionId, projectName, fileName);
            Assert.AreEqual(sessionId, request.SessionToken);
            Assert.AreEqual(projectName, request.ProjectName);
            Assert.AreEqual(fileName, request.FileName);
        }
        #endregion

        #region Getter/setter tests
        [Test]
        public void FileNameCanBeSetAndRetrieved()
        {
            var request = new FileTransferRequest();
            var projectName = "projectName";
            request.FileName = projectName;
            Assert.AreEqual(projectName, request.FileName);
        }
        #endregion
        #endregion
    }
}
