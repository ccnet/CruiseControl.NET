using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CruiseServerSnapshotTest
    {
        QueueSetSnapshot snapshot1;
        QueueSetSnapshot snapshot2;

        [SetUp]
        public void Setup()
        {
            snapshot1 = new QueueSetSnapshot();
            snapshot2 = new QueueSetSnapshot();
        }

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
		public void ShouldFindProjectStatusBasedOnProjectName()
		{
			ProjectStatus[] projectStatuses = new ProjectStatus[2];
			projectStatuses[0] = new ProjectStatus("test1", IntegrationStatus.Failure, DateTime.Now);
			projectStatuses[1] = new ProjectStatus("test2", IntegrationStatus.Success, DateTime.Now);

			CruiseServerSnapshot cruiseServerSnapshot = new CruiseServerSnapshot(projectStatuses, null);

			Assert.AreSame(projectStatuses[0], cruiseServerSnapshot.GetProjectStatus("test1"));
			Assert.AreSame(projectStatuses[1], cruiseServerSnapshot.GetProjectStatus("test2"));
		}

		[Test]
		public void ShouldReturnNullIfNamedProjectNotFound()
		{
			ProjectStatus[] projectStatuses = new ProjectStatus[1];
			projectStatuses[0] = new ProjectStatus("no names", IntegrationStatus.Failure, DateTime.Now);

			CruiseServerSnapshot cruiseServerSnapshot = new CruiseServerSnapshot(projectStatuses, null);

			Assert.IsNull(cruiseServerSnapshot.GetProjectStatus("this doesn't match"));
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
        public void DetectQueueSetNotChanged()
        {
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            snapshot2.Queues.Add(new QueueSnapshot("Test1"));
            QueueSnapshot queue1 = snapshot1.Queues[0];
            QueueSnapshot queue2 = snapshot2.Queues[0];

            // Same number of projects with no content
            AssertQueueSetChanged(false, snapshot1, snapshot2);

            // Same number of projects with content
            queue1.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications));
            queue2.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications));
            AssertQueueSetChanged(false, snapshot1, snapshot2);
        }

        [Test]
        public void DetectQueueSetChangedWithDifferingNumberOfProjects()
        {
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            snapshot2.Queues.Add(new QueueSnapshot("Test1"));
            QueueSnapshot queue1 = snapshot1.Queues[0];
            QueueSnapshot queue2 = snapshot2.Queues[0];

            queue1.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications));
            queue1.Requests.Add(new QueuedRequestSnapshot("Project2", ProjectActivity.CheckingModifications));
            queue2.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications));
            AssertQueueSetChanged(true, snapshot1, snapshot2);
        }

        [Test]
        public void DetectQueueSetChangedWithDifferingProjectNames()
        {
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            snapshot2.Queues.Add(new QueueSnapshot("Test1"));
            QueueSnapshot queue1 = snapshot1.Queues[0];
            QueueSnapshot queue2 = snapshot2.Queues[0];

            queue1.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications));
            queue2.Requests.Add(new QueuedRequestSnapshot("Project2", ProjectActivity.CheckingModifications));
            AssertQueueSetChanged(true, snapshot1, snapshot2);
        }

        [Test]
        public void DetectQueueSetChangedWithDifferingProjectStatus()
        {
            snapshot1.Queues.Add(new QueueSnapshot("Test1"));
            snapshot2.Queues.Add(new QueueSnapshot("Test1"));
            QueueSnapshot queue1 = snapshot1.Queues[0];
            QueueSnapshot queue2 = snapshot2.Queues[0];

            queue1.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications));
            queue2.Requests.Add(new QueuedRequestSnapshot("Project1", ProjectActivity.Building));
            AssertQueueSetChanged(true, snapshot1, snapshot2);
        }

        private void AssertQueueSetChanged(bool result, QueueSetSnapshot firstSnapshot, QueueSetSnapshot secondSnapshot)
        {
            CruiseServerSnapshot cruiseServerSnapshot1 = new CruiseServerSnapshot(null, firstSnapshot);
            CruiseServerSnapshot cruiseServerSnapshot2 = new CruiseServerSnapshot(null, secondSnapshot);
            Assert.AreEqual(result, cruiseServerSnapshot1.IsQueueSetSnapshotChanged(cruiseServerSnapshot2.QueueSetSnapshot));
        }
    }
}
