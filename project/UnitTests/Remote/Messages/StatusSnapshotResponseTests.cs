namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class StatusSnapshotResponseTests
    {
        #region Tests
        #region Constructor tests
        [Test]
        public void RequestConstructorInitialisesTheValues()
        {
            var request = new EncryptedRequest();
            var response = new StatusSnapshotResponse(request);
            // Only check one property is set, since the properties are set by the base class
            Assert.AreEqual(request.Identifier, response.RequestIdentifier);
        }

        [Test]
        public void FullConstructorInitialisesTheValues()
        {
            var response1 = new StatusSnapshotResponse();
            response1.RequestIdentifier = "12345";
            var response2 = new StatusSnapshotResponse(response1);
            // Only check one property is set, since the properties are set by the base class
            Assert.AreEqual(response1.RequestIdentifier, response2.RequestIdentifier);
        }
        #endregion

        #region Getter/setter tests
        [Test]
        public void SnapshotCanBeSetAndRetrieved()
        {
            var request = new StatusSnapshotResponse();
            var snapshot = new ProjectStatusSnapshot();
            request.Snapshot = snapshot;
            Assert.AreSame(snapshot, request.Snapshot);
        }
        #endregion
        #endregion
    }
}
