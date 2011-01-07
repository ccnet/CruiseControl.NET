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
            var context = new TaskExecutionContext(writerMock.Object, null, null);
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
            var context = new TaskExecutionContext(writerMock.Object, null, null);
            context.Complete();
            context.Complete();
            Assert.AreEqual(2, action);
        }

        [Test]
        public void StartChildStartsANewChildContext()
        {
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "task", null)).Verifiable();
            writerMock.MockWriteAttributeString("type", "Comment");
            writerMock.MockWriteElementString("start", "2010-01-01T12:01:01");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var task = new Comment();
            var context = new TaskExecutionContext(writerMock.Object, null, clockMock.Object);
            var child = context.StartChild(task);
            Assert.IsNotNull(child);
            writerMock.Verify();
        }
        #endregion
    }
}
