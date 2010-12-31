namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Tests.Stubs;
    using NUnit.Framework;

    [TestFixture]
    public class SequenceTests
    {
        #region Tests
        [Test]
        public void RunReturnsAllTasks()
        {
            var childTask = new TaskStub();
            var task = new Sequence(childTask);
            var result = task.Run(null);
            Assert.AreEqual(1, result.Count());
            Assert.AreSame(childTask, result.First());
        }
        #endregion
    }
}
