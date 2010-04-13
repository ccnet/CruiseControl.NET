namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class ProjectItemRequestTests
    {
        #region Tests
        #region Constructor tests
        [Test]
        public void SessionConstructorInitialisesTheValues()
        {
            var sessionId = "MyNewSession";
            var request = new ProjectItemRequest(sessionId);
            Assert.AreEqual(sessionId, request.SessionToken);
        }

        [Test]
        public void FullConstructorInitialisesTheValues()
        {
            var sessionId = "MyNewSession";
            var projectName = "TheNameOfTheProject";
            var request = new ProjectItemRequest(sessionId, projectName);
            Assert.AreEqual(sessionId, request.SessionToken);
            Assert.AreEqual(projectName, request.ProjectName);
        }
        #endregion
        #endregion
    }
}
