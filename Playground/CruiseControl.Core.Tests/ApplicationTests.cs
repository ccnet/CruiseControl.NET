namespace CruiseControl.Core.Tests
{
    using System;
    using System.IO;
    using System.Threading;
    using CruiseControl.Core.Exceptions;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tests.Stubs;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ApplicationTests
    {
        #region Tests
        [Test]
        public void LoadConfigurationRequiresConfigurationService()
        {
            var application = new Application();
            Assert.Throws<CruiseControlException>(application.LoadConfiguration);
        }

        [Test]
        public void LoadConfigurationRequiresFileSystem()
        {
            var serviceMock = new Mock<IConfigurationService>();
            var application = new Application
                                  {
                                      ConfigurationService = serviceMock.Object
                                  };
            Assert.Throws<CruiseControlException>(application.LoadConfiguration);
        }

        [Test]
        public void LoadConfigurationRequiresValidationLog()
        {
            var serviceMock = new Mock<IConfigurationService>();
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            var application = new Application
                                  {
                                      ConfigurationService = serviceMock.Object,
                                      FileSystem = fileSystemMock.Object
                                  };
            Assert.Throws<CruiseControlException>(application.LoadConfiguration);
        }

        [Test]
        public void LoadConfigurationLoadsConfiguration()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "ccnet.config");
            var server = new Server("Test");
            var stream = new MemoryStream();
            var serviceMock = new Mock<IConfigurationService>(MockBehavior.Strict);
            serviceMock.Setup(s => s.Load(stream)).Returns(server);
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.OpenFileForRead(path)).Returns(stream);
            var validationLogMock = new Mock<IValidationLog>(MockBehavior.Strict);
            validationLogMock.Setup(vl => vl.NumberOfErrors).Returns(0);
            validationLogMock.Setup(vl => vl.NumberOfWarnings).Returns(1);
            validationLogMock.Setup(vl => vl.Reset());
            var application = new Application
                                  {
                                      ConfigurationService = serviceMock.Object,
                                      FileSystem = fileSystemMock.Object,
                                      ValidationLog = validationLogMock.Object
                                  };
            application.LoadConfiguration();
            Assert.AreSame(server, application.Configuration);
        }

        [Test]
        public void LoadConfigurationFailsIfErrorInFile()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "ccnet.config");
            var serviceMock = new Mock<IConfigurationService>(MockBehavior.Strict);
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            var validationLogMock = new Mock<IValidationLog>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.OpenFileForRead(path))
                .Throws(new FileNotFoundException("Unable to find file", path));
            var application = new Application
                                  {
                                      ConfigurationService = serviceMock.Object,
                                      FileSystem = fileSystemMock.Object,
                                      ValidationLog = validationLogMock.Object
                                  };
            Assert.Throws<FileNotFoundException>(application.LoadConfiguration);
        }

        [Test]
        public void LoadConfigurationFailsErrorInValidation()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "ccnet.config");
            var project = new ProjectStub
                              {
                                  OnValidate = vl =>
                                                   {
                                                       throw new ConfigurationException("Oops");
                                                   }
                              };
            var server = new Server("Test", project);
            var stream = new MemoryStream();
            var serviceMock = new Mock<IConfigurationService>(MockBehavior.Strict);
            serviceMock.Setup(s => s.Load(stream)).Returns(server);
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.OpenFileForRead(path)).Returns(stream);
            var application = new Application
                                  {
                                      ConfigurationService = serviceMock.Object,
                                      FileSystem = fileSystemMock.Object,
                                      ValidationLog = new LoggingValidationLog()
                                  };
            Assert.Throws<ConfigurationException>(application.LoadConfiguration);
        }

        [Test]
        public void LoadConfigurationFailsIfValidationFails()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "ccnet.config");
            var project = new ProjectStub
            {
                OnValidate = vl => vl.AddError("Something's wrong!")
            };
            var server = new Server("Test", project);
            var stream = new MemoryStream();
            var serviceMock = new Mock<IConfigurationService>(MockBehavior.Strict);
            serviceMock.Setup(s => s.Load(stream)).Returns(server);
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.OpenFileForRead(path)).Returns(stream);
            var application = new Application
            {
                ConfigurationService = serviceMock.Object,
                FileSystem = fileSystemMock.Object,
                ValidationLog = new LoggingValidationLog()
            };
            Assert.Throws<ConfigurationException>(application.LoadConfiguration);
        }

        [Test]
        public void StartRequiresConfiguration()
        {
            var application = new Application();
            Assert.Throws<CruiseControlException>(application.Start);
        }

        [Test]
        public void StopRequiresConfiguration()
        {
            var application = new Application();
            Assert.Throws<CruiseControlException>(application.Stop);
        }

        [Test]
        public void StartStartsProjects()
        {
            var started = false;
            var project = new ProjectStub
                              {
                                  Name = "Test",
                                  OnStart = () => started = true,
                                  OnLoadState = () => { }
                              };
            var server = new Server(project);
            var application = new Application
                                  {
                                      Configuration = server
                                  };
            application.Start();

            // Give the projects time to start
            SpinWait.SpinUntil(() => started, TimeSpan.FromSeconds(5));
            Assert.IsTrue(started);
        }

        [Test]
        public void StartOpensCommunications()
        {
            var initialised = false;
            var channel = new ChannelStub
                              {
                                  OnInitialiseAction = () => initialised = true
                              };
            var server = new Server("Test", new[] { channel });
            var application = new Application
                                  {
                                      Configuration = server
                                  };
            application.Start();
            Assert.IsTrue(initialised);
        }

        [Test]
        public void StartHandlesError()
        {
            var project = new ProjectStub();
            var server = new Server(project);
            var application = new Application
                                  {
                                      Configuration = server
                                  };
            var error = Assert.Throws<InvalidOperationException>(application.Start);
            Assert.AreEqual("Cannot start a project without a name", error.Message);
        }

        [Test]
        public void StopStopsProjects()
        {
            var started = false;
            var stopped = false;
            var project = new ProjectStub
                              {
                                  Name = "Test",
                                  OnStart = () => started = true,
                                  OnStop = () => stopped = true,
                                  OnLoadState = () => { }
                              };
            var server = new Server(project);
            var application = new Application
                                  {
                                      Configuration = server
                                  };
            application.Start();

            // Give the projects time to start
            SpinWait.SpinUntil(() => started, TimeSpan.FromSeconds(5));
            Assert.IsTrue(started); 
            application.Stop();

            // Give the projects time to stop
            SpinWait.SpinUntil(() => stopped, TimeSpan.FromSeconds(5));
            Assert.IsTrue(stopped);
        }

        [Test]
        public void StopClosesCommunications()
        {
            var cleaned = false;
            var channel = new ChannelStub
                              {
                                  OnCleanUpAction = () => cleaned = true
                              };
            var server = new Server("Test", new[] { channel });
            var application = new Application
                                  {
                                      Configuration = server
                                  };
            application.Stop();
            Assert.IsTrue(cleaned);
        }
        #endregion
    }
}
