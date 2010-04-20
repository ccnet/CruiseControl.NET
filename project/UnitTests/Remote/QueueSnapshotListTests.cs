namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    public class QueueSnapshotListTests
    {
        #region Tests
        [Test]
        public void ConstructorWorks()
        {
            var snapshot = new QueuedRequestSnapshotList();
            Assert.IsNotNull(snapshot.GetEnumerator());
        }
        #endregion
    }
}
