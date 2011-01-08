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
        public void StartNewStartsANewContext()
        {
            var path = Path.Combine(
                Environment.CurrentDirectory,
                "Test",
                "20100101120101");
            var writerMock = new Mock<XmlWriter>(MockBehavior.Strict);
            writerMock.Setup(w => w.WriteStartElement(null, "project", null)).Verifiable();
            writerMock.MockWriteAttributeString("name", "Test");
            writerMock.MockWriteElementString("start", "2010-01-01T12:01:01");
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2010, 1, 1, 12, 1, 1));
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.EnsureFolderExists(path));
            fileSystemMock.Setup(fs => fs.CreateXmlWriter(Path.Combine(path, "build.log"))).Returns(writerMock.Object);
            var factory = new TaskExecutionFactory
                              {
                                  Clock = clockMock.Object,
                                  FileSystem = fileSystemMock.Object
                              };
            var project = new Project("Test");
            var request = new IntegrationRequest("Testing");
            var actual = factory.StartNew(project, request);
            Assert.IsNotNull(actual);
            Assert.AreSame(project, actual.Project);
            Assert.AreSame(request, actual.Request);
            writerMock.Verify();
        }
        #endregion
    }
}
