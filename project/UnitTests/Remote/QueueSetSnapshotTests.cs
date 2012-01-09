namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    public class QueueSetSnapshotTests
    {
        #region Tests
        [Test]
        public void ConstructorWorks()
        {
            var snapshot = new QueueSetSnapshot();
            Assert.IsNotNull(snapshot.Queues);
            Assert.IsEmpty(snapshot.Queues);
        }
        #endregion
    }
}
