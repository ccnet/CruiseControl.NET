namespace ThoughtWorks.CruiseControl.UnitTests.Core.Extensions
{
    using System;
    using System.Xml;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Extensions;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Events;

    /// <summary>
    /// Test the disk space monitor.
    /// </summary>
    [TestFixture]
    public class DiskSpaceMonitorExtensionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Strict);
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForGb()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Gb", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(107374182400, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForMb()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Mb", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForKb()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Kb", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(102400, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForB()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("b", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(100, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForMissing()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement(null, 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseThrowsAnErrorForUnknownUnit()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("garbage", 100, "C:\\")
            };
            Assert.That(delegate { extension.Initialise(server, configuration); },
                        Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InitialiseThrowsAnErrorForUnknownElement()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            var document = new XmlDocument();
            configuration.Items = new XmlElement[] {
                document.CreateElement("garbage")
            };
            Assert.That(delegate { extension.Initialise(server, configuration); },
                        Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InitialiseThrowsAnErrorWithNoDrives()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
            };
            Assert.That(delegate { extension.Initialise(server, configuration); },
                        Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForMultipleDrives()
        {
            var server = mocks.Create<ICruiseServer>().Object;
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Mb", 100, "C:\\"),
                CreateSizeElement("Kb", 100, "D:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
            Assert.AreEqual(102400, extension.RetrieveMinimumSpaceRequired("D:\\"));
        }

        [Test]
        public void IntegrationIsSuccessfulWhenSufficientSpace()
        {
            // Initialise the file system
            var fileSystem = mocks.Create<IFileSystem>().Object;
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.GetFreeDiskSpace("c:\\"))
                .Returns(104857600);

            // Initialise the server
            var server = mocks.Create<ICruiseServer>().Object;
            Mock.Get(server).Setup(_server => _server.RetrieveService(typeof(IFileSystem)))
                .Returns(fileSystem);

            // Initialise the extension
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Mb", 1, "C:\\")
            };

            // Run the actual test
            extension.Initialise(server, configuration);
            var args = new IntegrationStartedEventArgs(null, "Project 1");
            Mock.Get(server).Raise(_server => _server.IntegrationStarted += null, args);
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Continue, args.Result);
        }

        [Test]
        public void IntegrationIsStoppedWhenInsufficientSpace()
        {
            // Initialise the file system
            var fileSystem = mocks.Create<IFileSystem>().Object;
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.GetFreeDiskSpace("c:\\"))
                .Returns(102400);

            // Initialise the server
            var server = mocks.Create<ICruiseServer>().Object;
            Mock.Get(server).Setup(_server => _server.RetrieveService(typeof(IFileSystem)))
                .Returns(fileSystem);

            // Initialise the extension
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Mb", 1, "C:\\")
            };

            // Run the actual test
            extension.Initialise(server, configuration);
            var args = new IntegrationStartedEventArgs(null, "Project 1");
            Mock.Get(server).Raise(_server => _server.IntegrationStarted += null, args);
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Cancel, args.Result);
        }

        [Test]
        public void StartAndStopDoesNothing()
        {
            var extension = new DiskSpaceMonitorExtension();
            var serverMock = this.mocks.Create<ICruiseServer>(MockBehavior.Strict).Object;
            var config = new ExtensionConfiguration();

            extension.Start();
            extension.Stop();

            this.mocks.VerifyAll();
        }

        [Test]
        public void StartAndAbortDoesNothing()
        {
            var extension = new DiskSpaceMonitorExtension();
            var serverMock = this.mocks.Create<ICruiseServer>(MockBehavior.Strict).Object;
            var config = new ExtensionConfiguration();

            extension.Start();
            extension.Abort();

            this.mocks.VerifyAll();
        }

        private XmlElement CreateSizeElement(string unit, int size, string drive)
        {
            var document = new XmlDocument();
            var element = document.CreateElement("drive");
            if (!string.IsNullOrEmpty(unit)) element.SetAttribute("unit", unit);
            element.SetAttribute("name", drive);
            element.InnerText = size.ToString();
            return element;
        }
    }
}
