namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.IO;
    using System.Xml;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TaskExecutionFactoryTests
    {
        #region Tests
        [Test]
        public void GenerateLogNameGeneratesLogNameUsingDefaultDirectory()
        {
            var dateTime = new DateTime(2011, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(dateTime);
            var factory = new TaskExecutionFactory
                              {
                                  Clock = clockMock.Object
                              };
            var project = new Project("Test");
            var actual = factory.GenerateLogName(project);
            var expected = Path.Combine(
                Environment.CurrentDirectory,
                "Test",
                dateTime.ToString("yyyyMMddHHmmss"),
                "build.log");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StartNewStartsANewContext()
        {
            var path = "LogFilePath";
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "project", null)).Verifiable();
            writerMock.MockWriteAttributeString("name", "Test");
            writerMock.MockWriteElementString("start", "2010-01-01T12:01:01");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(fs => fs.CreateXmlWriter(path))
                .Returns(writerMock.Object);
            var factory = new TaskExecutionFactory
                              {
                                  Clock = clockMock.Object,
                                  FileSystem = fileSystemMock.Object
                              };
            var project = new Project("Test");
            var actual = factory.StartNew(path, project, null);
            Assert.IsNotNull(actual);
            writerMock.Verify();
        }
        #endregion
    }
}
