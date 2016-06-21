namespace ThoughtWorks.CruiseControl.UnitTests.Core.Extensions
{
    using System;
    using System.Xml;
    using NUnit.Framework;
    using Rhino.Mocks;
    using CruiseControl.Core.Extensions;
    using CruiseControl.Core.Util;
    using CruiseControl.Remote;
    using CruiseControl.Remote.Events;

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
            this.mocks = new MockRepository();
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForGb()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("Gb", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(107374182400, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForMb()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("Mb", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForKb()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("Kb", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(102400, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForB()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("b", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(100, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForMissing()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement(null, 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        public void InitialiseThrowsAnErrorForUnknownUnit()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("garbage", 100, "C:\\")
            };
            Assert.That(delegate { extension.Initialise(server, configuration); },
                        Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InitialiseThrowsAnErrorForUnknownElement()
        {
            var server = this.mocks.DynamicMock<ICruiseServer>();
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
            var server = this.mocks.DynamicMock<ICruiseServer>();
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
            var server = this.mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("Mb", 100, "C:\\"), this.CreateSizeElement("Kb", 100, "D:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
            Assert.AreEqual(102400, extension.RetrieveMinimumSpaceRequired("D:\\"));
        }

        [Test]
        public void IntegrationIsSuccessfulWhenSufficientSpace()
        {
            // Initialise the file system
            var fileSystem = this.mocks.DynamicMock<IFileSystem>();
            SetupResult.For(fileSystem.GetFreeDiskSpace("c:\\"))
                .Return(104857600);

            // Initialise the server
            var server = this.mocks.DynamicMock<ICruiseServer>();
            SetupResult.For(server.RetrieveService(typeof(IFileSystem)))
                .Return(fileSystem);
            server.IntegrationStarted += null;
            LastCall.IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();

            // Initialise the extension
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("Mb", 1, "C:\\")
            };
            this.mocks.ReplayAll();

            // Run the actual test
            extension.Initialise(server, configuration);
            var args = new IntegrationStartedEventArgs(null, "Project 1");
            eventRaiser.Raise(null, args);
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Continue, args.Result);
        }

        [Test]
        public void IntegrationIsStoppedWhenInsufficientSpace()
        {
            // Initialise the file system
            var fileSystem = this.mocks.DynamicMock<IFileSystem>();
            SetupResult.For(fileSystem.GetFreeDiskSpace("c:\\"))
                .Return(102400);

            // Initialise the server
            var server = this.mocks.DynamicMock<ICruiseServer>();
            SetupResult.For(server.RetrieveService(typeof(IFileSystem)))
                .Return(fileSystem);
            server.IntegrationStarted += null;
            LastCall.IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();

            // Initialise the extension
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                this.CreateSizeElement("Mb", 1, "C:\\")
            };
            this.mocks.ReplayAll();

            // Run the actual test
            extension.Initialise(server, configuration);
            var args = new IntegrationStartedEventArgs(null, "Project 1");
            eventRaiser.Raise(null, args);
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Cancel, args.Result);
        }

        [Test]
        public void StartAndStopDoesNothing()
        {
            var extension = new DiskSpaceMonitorExtension();

            this.mocks.ReplayAll();
            extension.Start();
            extension.Stop();

            this.mocks.VerifyAll();
        }

        [Test]
        public void StartAndAbortDoesNothing()
        {
            var extension = new DiskSpaceMonitorExtension();

            this.mocks.ReplayAll();
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
