namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Tests.Stubs;
    using NUnit.Framework;

    [TestFixture]
    public class GetSourceTests
    {
        #region Tests
        [Test]
        public void RunCallsDefaultSourceControlBlock()
        {
            var getSourceCalled = false;
            var block = new SourceControlBlockStub
                            {
                                GetSourceAction = p => getSourceCalled = true
                            };
            var task = new GetSource();
            var project = new Project("Name", task);
            project.SourceControl.Add(block);
            var result = task.Run(null);
            result.Count();
            Assert.IsTrue(getSourceCalled);
        }
        #endregion
    }
}
