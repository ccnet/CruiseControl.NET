namespace CruiseControl.Core.Tests
{
    using System;
    using System.IO;
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
        public void ConstructorSetsProperties()
        {
            var request = new IntegrationRequest("Test");
            var project = new Project();
            var context = new TaskExecutionContext(new TaskExecutionParameters
                                                       {
                                                           IntegrationRequest = request,
                                                           Project = project
                                                       });
            Assert.AreSame(request, context.Request);
            Assert.AreSame(project, context.Project);
            Assert.IsNotNull(context.ModificationSets);
        }

        [Test]
        public void CompleteClosesWriter()
        {
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteEndDocument()).Verifiable();
            writerMock.Setup(w => w.Close()).Verifiable();
            writerMock.MockWriteElementString("finish", "2010-01-01T12:01:01");
            writerMock.MockWriteElementString("status", "Success");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        Clock = clockMock.Object
                    });
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
            writerMock.MockWriteElementString("status", "Success");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        Clock = clockMock.Object
                    });
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
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        Clock = clockMock.Object
                    });
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
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        Clock = clockMock.Object
                    });
            var child = context.StartChild(task);
            Assert.IsNotNull(child);
            Assert.AreSame(context, child.Parent);
            Assert.AreSame(context.ModificationSets, child.ModificationSets);
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
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        Clock = clockMock.Object
                    });
            context.AddEntryToBuildLog("This is a test");
            writerMock.Verify();
        }

        [Test]
        public void ImportFileCopiesFile()
        {
            var source = "C:\\data.tst";
            var destination = Path.Combine(
                Environment.CurrentDirectory,
                "Test",
                "20100101120101",
                Path.GetFileName(source));
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "file", null)).Verifiable();
            writerMock.MockWriteAttributeString("time", "2010-01-01T12:01:01");
            writerMock.Setup(w => w.WriteString(Path.GetFileName(destination))).Verifiable();
            writerMock.Setup(w => w.WriteEndElement()).Verifiable();
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CopyFile(source, destination)).Verifiable();
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        FileSystem = fileSystemMock.Object,
                        Clock = clockMock.Object,
                        Project = new Project("Test"),
                        BuildName = "20100101120101"
                    });
            context.ImportFile(source, false);
            writerMock.Verify();
            fileSystemMock.Verify();
        }

        [Test]
        public void ImportFileMovesFile()
        {
            var source = "C:\\data.tst";
            var destination = Path.Combine(
                Environment.CurrentDirectory,
                "Test",
                "20100101120101",
                Path.GetFileName(source));
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "file", null)).Verifiable();
            writerMock.MockWriteAttributeString("time", "2010-01-01T12:01:01");
            writerMock.Setup(w => w.WriteString(Path.GetFileName(destination))).Verifiable();
            writerMock.Setup(w => w.WriteEndElement()).Verifiable();
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.MoveFile(source, destination)).Verifiable();
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        FileSystem = fileSystemMock.Object,
                        Clock = clockMock.Object,
                        Project = new Project("Test"),
                        BuildName = "20100101120101"
                    });
            context.ImportFile(source, true);
            writerMock.Verify();
            fileSystemMock.Verify();
        }

        [Test]
        public void AddModificationsAddsANewModificationSet()
        {
            var context = new TaskExecutionContext(new TaskExecutionParameters());
            var modificationSet = new ModificationSet();
            context.AddModifications(modificationSet);
            CollectionAssert.AreEqual(new[] { modificationSet }, context.ModificationSets);
        }

        [Test]
        public void StartOutputStreamStartsANewStream()
        {
            var source = "data.tst";
            var destination = Path.Combine(
                Environment.CurrentDirectory,
                "Test",
                "20100101120101",
                source);
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "file", null)).Verifiable();
            writerMock.MockWriteAttributeString("time", "2010-01-01T12:01:01");
            writerMock.Setup(w => w.WriteString(source)).Verifiable();
            writerMock.Setup(w => w.WriteEndElement()).Verifiable();
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            var expected = new MemoryStream();
            fileSystemMock.Setup(fs => fs.OpenFileForWrite(destination)).Returns(expected).Verifiable();
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        XmlWriter = writerMock.Object,
                        FileSystem = fileSystemMock.Object,
                        Clock = clockMock.Object,
                        Project = new Project("Test"),
                        BuildName = "20100101120101"
                    });
            var actual = context.StartOutputStream(source);
            writerMock.Verify();
            fileSystemMock.Verify();
            Assert.AreSame(expected, actual);
        }
        #endregion
    }
}
