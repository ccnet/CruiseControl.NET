using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CruiseServerSnapshotTest
    {
        [Test]
        public void CorrectAssignmentOfConstructorArguments()
        {
            ProjectStatus[] projectStatuses = new ProjectStatus[0];
            QueueSetSnapshot queueSetSnapshot = new QueueSetSnapshot();

            CruiseServerSnapshot cruiseServerSnapshot = new CruiseServerSnapshot(projectStatuses, queueSetSnapshot);

            Assert.AreSame(projectStatuses, cruiseServerSnapshot.ProjectStatuses);
            Assert.AreSame(queueSetSnapshot, cruiseServerSnapshot.QueueSetSnapshot);
        }

        [Test]
        public void DetectQueueSetChangedWithNullArguments()
        {
            AssertQueueSetChanged(false, null, null);
            AssertQueueSetChanged(true, null, new QueueSetSnapshot());
            AssertQueueSetChanged(true, new QueueSetSnapshot(), null);
        }

        [Test]
        public void DetectQueueSetChangedWithQueueChanges()
        {
            QueueSetSnapshot snapshot1 = new QueueSetSnapshot();
            QueueSetSnapshot snapshot2 = new QueueSetSnapshot();

            // Same number of queues with no content
            AssertQueueSetChanged(false, snapshot1, snapshot2);

            // Same number of queues with content
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            snapshot2.Queues.Add(new QueueSnapshot("Test1"));
            AssertQueueSetChanged(false, snapshot1, snapshot2);

            // Differing number of queues
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            AssertQueueSetChanged(true, snapshot1, snapshot2);

            // Same number of queues but different queue names.
            snapshot2.Queues.Add(new QueueSnapshot("Test2"));
            AssertQueueSetChanged(true, snapshot1, snapshot2);
        }

        [Test]
        public void DetectQueueSetChangedWithProjectChanges()
        {
            QueueSetSnapshot snapshot1 = new QueueSetSnapshot();
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            QueueSetSnapshot snapshot2 = new QueueSetSnapshot();
            snapshot2.Queues.Add(new QueueSnapshot("Test1"));
            QueueSnapshot queue1 = snapshot1.Queues[0];
            QueueSnapshot queue2 = snapshot2.Queues[0];

            // Same number of projects with no content
            AssertQueueSetChanged(false, snapshot1, snapshot2);

            // Same number of projects with content
            queue1.Requests.Add(new QueuedRequestSnapshot("Project1"));
            queue2.Requests.Add(new QueuedRequestSnapshot("Project1"));
            AssertQueueSetChanged(false, snapshot1, snapshot2);

            // Differing number of projects
            queue1.Requests.Add(new QueuedRequestSnapshot("Project2"));
            AssertQueueSetChanged(true, snapshot1, snapshot2);

            // Same number of queues but different project names.
            queue2.Requests.Add(new QueuedRequestSnapshot("Project3"));
            AssertQueueSetChanged(true, snapshot1, snapshot2);
        }

        private void AssertQueueSetChanged(bool result, QueueSetSnapshot snapshot1, QueueSetSnapshot snapshot2)
        {
            CruiseServerSnapshot cruiseServerSnapshot1 = new CruiseServerSnapshot(null, snapshot1);
            CruiseServerSnapshot cruiseServerSnapshot2 = new CruiseServerSnapshot(null, snapshot2);
            Assert.AreEqual(result, cruiseServerSnapshot1.IsQueueSetSnapshotChanged(cruiseServerSnapshot2.QueueSetSnapshot));
        }
    }
}
