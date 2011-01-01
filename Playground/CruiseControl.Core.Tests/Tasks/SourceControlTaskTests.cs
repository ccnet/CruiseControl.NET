namespace CruiseControl.Core.Tests.Tasks
{
    using System;
    using System.Collections.Generic;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Tests.Stubs;
    using NUnit.Framework;

    [TestFixture]
    public class SourceControlTaskTests
    {
        #region Tests
        [Test]
        public void GetSourceControlBlockRetrievesSingleBlockFromProject()
        {
            var expectedBlock = new SourceControlBlockStub();
            var task = new SourceControlTaskStub();
            var project = new Project("test", task);
            project.SourceControl.Add(expectedBlock);
            var actualBlock = task.RunGetSourceControlBlock();
            Assert.AreSame(expectedBlock, actualBlock);
        }

        [Test]
        public void GetSourceControlBlockRetrievesNamedBlockFromProject()
        {
            var blockName = "BlockToFind";
            var expectedBlock = new SourceControlBlockStub
                                    {
                                        Name = blockName
                                    };
            var task = new SourceControlTaskStub
                           {
                               Use = blockName
                           };
            var project = new Project("test", task);
            project.SourceControl.Add(expectedBlock);
            project.SourceControl.Add(new SourceControlBlockStub());
            var actualBlock = task.RunGetSourceControlBlock();
            Assert.AreSame(expectedBlock, actualBlock);
        }

        [Test]
        public void GetSourceControlBlockFailsIfNotInProject()
        {
            var task = new SourceControlTaskStub();
            Assert.Throws<NotSupportedException>(() => task.RunGetSourceControlBlock());
        }

        [Test]
        public void GetSourceControlBlockFailsIfNoBlocksInProject()
        {
            var task = new SourceControlTaskStub();
            new Project("test", task);
            Assert.Throws<NotSupportedException>(() => task.RunGetSourceControlBlock());
        }

        [Test]
        public void GetSourceControlBlockFailsIfBlockNotFound()
        {
            var task = new SourceControlTaskStub
                           {
                               Use = "SomeBlock"
                           };
            var project = new Project("test", task);
            project.SourceControl.Add(new SourceControlBlockStub());
            Assert.Throws<NotSupportedException>(() => task.RunGetSourceControlBlock());
        }

        [Test]
        public void UniversalNameUsesType()
        {
            var task = new SourceControlTaskStub();
            new Project("ProjectName", task);
            var actual = task.UniversalName;
            Assert.AreEqual("urn:ccnet::ProjectName:SourceControlTaskStub", actual);
        }

        [Test]
        public void UniversalNameUsesName()
        {
            var task = new SourceControlTaskStub
                           {
                               Name = "TaskName"
                           };
            new Project("ProjectName", task);
            var actual = task.UniversalName;
            Assert.AreEqual("urn:ccnet::ProjectName:TaskName", actual);
        }
        #endregion

        #region Stub class
        private class SourceControlTaskStub
            : SourceControlTask
        {
            public SourceControlBlock RunGetSourceControlBlock()
            {
                return this.GetSourceControlBlock();
            }

            protected override IEnumerable<Task> OnRun(TaskExecutionContext context)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
