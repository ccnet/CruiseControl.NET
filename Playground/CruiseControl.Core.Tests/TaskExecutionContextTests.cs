namespace CruiseControl.Core.Tests
{
    using System;
    using System.Xml;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TaskExecutionContextTests
    {
        #region Tests
        [Test]
        public void CompleteClosesWriter()
        {
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteEndDocument()).Verifiable();
            writerMock.Setup(w => w.Close()).Verifiable();
            writerMock.MockWriteElementString("finish", "2010-01-01T12:01:01");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(writerMock.Object, null, clockMock.Object);
            Assert.IsFalse(context.IsCompleted);
            context.Complete();
            writerMock.Verify();
            Assert.IsTrue(context.IsCompleted);
        }

        [Test]
        public void CompleteDoesNothingIfAlreadyClosed()
        {
            var action = 0;
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteEndDocument()).Callback(() => action++);
            writerMock.Setup(w => w.Close()).Callback(() => action++);
            writerMock.MockWriteElementString("finish", "2010-01-01T12:01:01");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(writerMock.Object, null, clockMock.Object);
            context.Complete();
            context.Complete();
            Assert.AreEqual(2, action);
        }

        [Test]
        public void CompleteClosesElementIfChild()
        {
            var writerMock = new Mock<XmlWriter>(MockBehavior.Loose);
            writerMock.Setup(w => w.WriteEndElement()).Verifiable();
            writerMock.Setup(w => w.WriteEndDocument()).Throws(new AssertionException("WriteEndDocument called"));
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(writerMock.Object, null, clockMock.Object);
            var child = context.StartChild(new Comment());
            child.Complete();
            writerMock.Verify();
        }

        [Test]
        public void StartChildStartsANewChildContext()
        {
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "task", null)).Verifiable();
            writerMock.MockWriteAttributeString("name", "TestComment");
            writerMock.MockWriteAttributeString("type", "Comment");
            writerMock.MockWriteElementString("start", "2010-01-01T12:01:01");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var task = new Comment("TestComment");
            var context = new TaskExecutionContext(writerMock.Object, null, clockMock.Object);
            var child = context.StartChild(task);
            Assert.IsNotNull(child);
            Assert.AreSame(context, child.Parent);
            writerMock.Verify();
        }

        [Test]
        public void AddEntryToBuildLogAddsElement()
        {
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "entry", null)).Verifiable();
            writerMock.MockWriteAttributeString("time", "2010-01-01T12:01:01");
            writerMock.Setup(w => w.WriteString("This is a test")).Verifiable();
            writerMock.Setup(w => w.WriteEndElement()).Verifiable();
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(writerMock.Object, null, clockMock.Object);
            context.AddEntryToBuildLog("This is a test");
            writerMock.Verify();
        }
        #endregion
    }
}
