namespace CruiseControl.Core.Tests
{
    using System;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class IntegrationContextTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsProject()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            Assert.AreSame(project, context.Item);
        }

        [Test]
        public void WaitWillTimeoutIfNotReleased()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            context.Lock();
            var released = context.Wait(TimeSpan.FromMilliseconds(5));
            Assert.IsFalse(released);
            Assert.IsFalse(context.WasCancelled);
        }

        [Test]
        public void WaitWillContinueIfNotLocked()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            var released = context.Wait(TimeSpan.FromMilliseconds(5));
            Assert.IsTrue(released);
        }

        [Test]
        public void WaitWillContinueWhenReleased()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            var releaseThread = new Thread(o =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    context.Release();
                });
            releaseThread.Start();
            context.Lock();
            var released = context.Wait(TimeSpan.FromSeconds(5));
            Assert.IsTrue(released);
        }

        [Test]
        public void CompletedIsFiredOnComplete()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            var wasFired = false;
            context.Completed += (o, e) => wasFired = true;
            context.Complete();
            Assert.IsTrue(wasFired);
        }

        [Test]
        public void CancelWillReleaseALock()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            var releaseThread = new Thread(o =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    context.Cancel();
                });
            releaseThread.Start();
            context.Lock();
            var released = context.Wait(TimeSpan.FromSeconds(5));
            Assert.IsFalse(released);
            Assert.IsTrue(context.WasCancelled);
        }
        #endregion
    }
}
