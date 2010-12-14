namespace CruiseControl.Core.Tests.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CruiseControl.Core.Structure;
    using CruiseControl.Core.Tests.Stubs;
    using NUnit.Framework;

    public class QueueTests
    {
        #region Tests
        [Test]
        public void AskToIntegrateWillTriggerProjectIfFirst()
        {
            var project = new ProjectStub();
            var queue = new Queue();
            var context = new IntegrationContext(project);
            queue.AskToIntegrate(context);
            var canIntegrate = context.Wait(TimeSpan.FromSeconds(5));
            Assert.IsTrue(canIntegrate);
            var active = queue.GetActiveRequests();
            Assert.AreEqual(1, active.Count());
            Assert.AreEqual(0, queue.GetPendingRequests().Count());
            Assert.AreSame(active.First(), context);
        }

        [Test]
        public void CompletingAnIntegrationRemovesItFromActiveRequests()
        {
            var project = new ProjectStub();
            var queue = new Queue();
            var context = new IntegrationContext(project);
            queue.AskToIntegrate(context);
            context.Complete();
            Assert.AreEqual(0, queue.GetActiveRequests().Count());
            Assert.AreEqual(0, queue.GetPendingRequests().Count());
        }

        [Test]
        public void AskToIntegrateWillQueueSubsequentItems()
        {
            var queue = new Queue();
            var project1 = new ProjectStub();
            var project2 = new ProjectStub();
            var context1 = new IntegrationContext(project1);
            var context2 = new IntegrationContext(project2);
            queue.AskToIntegrate(context1);
            queue.AskToIntegrate(context2);
            var active = queue.GetActiveRequests();
            var pending = queue.GetPendingRequests();
            Assert.AreEqual(1, active.Count());
            Assert.AreEqual(1, pending.Count());
            Assert.AreSame(active.First(), context1);
            Assert.AreSame(pending.First(), context2);
        }

        [Test]
        public void CompletingReleasingSubsequentItems()
        {
            var queue = new Queue();
            var project1 = new ProjectStub();
            var project2 = new ProjectStub();
            var context1 = new IntegrationContext(project1);
            var context2 = new IntegrationContext(project2);
            queue.AskToIntegrate(context1);
            queue.AskToIntegrate(context2);
            context1.Complete();
            var active = queue.GetActiveRequests();
            Assert.AreEqual(1, active.Count());
            Assert.AreEqual(0, queue.GetPendingRequests().Count());
            Assert.AreSame(context2, active.First());
        }

        [Test]
        public void ItemTypeIsQueue()
        {
            var item = new Queue();
            Assert.AreEqual("Queue", item.ItemType);
        }

        [Test]
        public void AddingAProjectSetsItsHostToTheQueue()
        {
            var queue = new Queue();
            var project = new Project();
            queue.Children.Add(project);
            Assert.AreSame(queue, project.Host);
        }

        [Test]
        public void AddingAQueueSetsItsHostToTheQueue()
        {
            var queue1 = new Queue();
            var queue2 = new Queue();
            queue1.Children.Add(queue2);
            Assert.AreSame(queue1, queue2.Host);
        }

        [Test]
        public void StartingAQueueWithAProjectSetsTheHost()
        {
            var project = new Project();
            var queue = new Queue("Test", project);
            Assert.AreSame(queue, project.Host);
        }

        [Test]
        public void HandlesQueueOfQueues()
        {
            var integrations = new List<string>();
            var contexts = new List<IntegrationContext>();
            var projects = new Project[6];
            for (var loop = 0; loop < projects.Length; loop++)
            {
                var project = new Project("Project" + loop);
                projects[loop] = project;
            }

            var queue1 = new Queue("Queue1", projects[0], projects[1]);
            var queue2 = new Queue("Queue2", projects[2], projects[3]);
            var queue3 = new Queue("Queue3", projects[4], projects[5]);
            var queue4 = new Queue("Queue4", queue1, queue2, queue3)
                             {
                                 AllowedActive = 2
                             };

            // Trying to simulate async code here - need to fire the completion events in the
            // order they are released
            var completed = new List<IntegrationContext>();
            foreach (var project in projects)
            {
                var context = new IntegrationContext(project);
                contexts.Add(context);
                project.Host.AskToIntegrate(context);
                if (context.IsLocked)
                {
                    context.Released += (o, e) =>
                                            {
                                                var subContext = o as IntegrationContext;
                                                completed.Add(subContext);
                                                integrations.Add(subContext.Item.Name);
                                            };
                }
                else
                {
                    completed.Add(context);
                    integrations.Add(project.Name);
                }
            }

            // completed will be modified at the same time we are iterating through it so can't
            // use foreach here
            for (var loop = 0; loop < completed.Count; loop++)
            {
                completed[loop].Complete();
            }

            // These should both be empty at the end of the process
            Assert.AreEqual(0, queue4.GetActiveRequests().Count());
            Assert.AreEqual(0, queue4.GetPendingRequests().Count());

            // Check that there is the correct order
            var expected = new[]
                               {
                                   "Project0",
                                   "Project2",
                                   "Project4",
                                   "Project1",
                                   "Project3",
                                   "Project5"
                               };
            CollectionAssert.AreEqual(expected, integrations);
        }
        #endregion
    }
}
