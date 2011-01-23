namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Monitor
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Monitor;

    public class BuildQueueChangedArgsTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsBuildQueue()
        {
            var mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var watcher = mocks.Stub<IServerWatcher>();
            var monitor = new Server(client, watcher);
            var queue = new BuildQueue(client, monitor, new QueueSnapshot());
            var args = new BuildQueueChangedArgs(queue);
            Assert.AreSame(queue, args.BuildQueue);
        }
        #endregion
    }
}
