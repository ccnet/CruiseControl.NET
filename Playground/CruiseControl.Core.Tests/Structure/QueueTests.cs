namespace CruiseControl.Core.Tests.Structure
{
    using System;
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
            Assert.AreEqual(1, queue.ActiveRequests.Count());
            Assert.AreEqual(0, queue.PendingRequests.Count());
        }

        [Test]
        public void CompletingAnIntegrationRemovesItFromActiveRequests()
        {
            var project = new ProjectStub();
            var queue = new Queue();
            var context = new IntegrationContext(project);
            queue.AskToIntegrate(context);
            context.Complete();
            Assert.AreEqual(0, queue.ActiveRequests.Count());
            Assert.AreEqual(0, queue.PendingRequests.Count());
        }

        [Test]
        public void ItemTypeIsQueue()
        {
            var item = new Queue();
            Assert.AreEqual("Queue", item.ItemType);
        }
        #endregion
    }
}
