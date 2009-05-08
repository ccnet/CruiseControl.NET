using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Extensions;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote.Events;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Extensions
{
    /// <summary>
    /// Test the disk space monitor.
    /// </summary>
    [TestFixture]
    public class DiskSpaceMonitorExtensionTests
    {
        private MockRepository mocks = new MockRepository();

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForGb()
        {
            var server = mocks.DynamicMock<ICruiseServer>();
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
            var server = mocks.DynamicMock<ICruiseServer>();
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
            var server = mocks.DynamicMock<ICruiseServer>();
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
            var server = mocks.DynamicMock<ICruiseServer>();
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
            var server = mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement(null, 100, "C:\\")
            };
            extension.Initialise(server, configuration);
            Assert.AreEqual(104857600, extension.RetrieveMinimumSpaceRequired("C:\\"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InitialiseThrowsAnErrorForUnknownUnit()
        {
            var server = mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("garbage", 100, "C:\\")
            };
            extension.Initialise(server, configuration);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InitialiseThrowsAnErrorForUnknownElement()
        {
            var server = mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            var document = new XmlDocument();
            configuration.Items = new XmlElement[] {
                document.CreateElement("garbage")
            };
            extension.Initialise(server, configuration);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InitialiseThrowsAnErrorWithNoDrives()
        {
            var server = mocks.DynamicMock<ICruiseServer>();
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
            };
            extension.Initialise(server, configuration);
        }

        [Test]
        public void InitialiseLoadsTheSpaceCorrectlyForMultipleDrives()
        {
            var server = mocks.DynamicMock<ICruiseServer>();
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
            var fileSystem = mocks.DynamicMock<IFileSystem>();
            SetupResult.For(fileSystem.GetFreeDiskSpace("c:\\"))
                .Return(104857600);

            // Initialise the server
            var server = mocks.DynamicMock<ICruiseServer>();
            SetupResult.For(server.RetrieveService<IFileSystem>())
                .Return(fileSystem);
            server.IntegrationStarted += null;
            LastCall.IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();

            // Initialise the extension
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Mb", 1, "C:\\")
            };
            mocks.ReplayAll();

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
            var fileSystem = mocks.DynamicMock<IFileSystem>();
            SetupResult.For(fileSystem.GetFreeDiskSpace("c:\\"))
                .Return(102400);

            // Initialise the server
            var server = mocks.DynamicMock<ICruiseServer>();
            SetupResult.For(server.RetrieveService<IFileSystem>())
                .Return(fileSystem);
            server.IntegrationStarted += null;
            LastCall.IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();

            // Initialise the extension
            var extension = new DiskSpaceMonitorExtension();
            var configuration = new ExtensionConfiguration();
            configuration.Items = new XmlElement[] {
                CreateSizeElement("Mb", 1, "C:\\")
            };
            mocks.ReplayAll();

            // Run the actual test
            extension.Initialise(server, configuration);
            var args = new IntegrationStartedEventArgs(null, "Project 1");
            eventRaiser.Raise(null, args);
            Assert.AreEqual(IntegrationStartedEventArgs.EventResult.Cancel, args.Result);
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
