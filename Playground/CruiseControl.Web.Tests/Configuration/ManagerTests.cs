namespace CruiseControl.Web.Tests.Configuration
{
    using System;
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using Utilities;
    using Web.Configuration;

    [TestFixture]
    public class ManagerTests
    {
        #region Tests
        [Test]
        public void ReloadSetsDefaultsIfFileIsMissing()
        {
            try
            {
                var configPath = Path.Combine(Folders.DataDirectory, "web.settings");
                var fileSystemMock = new Mock<FileSystem>(MockBehavior.Strict);
                fileSystemMock.Setup(fs => fs.CheckFileExists(configPath)).Returns(false);
                Manager.Reset();
                Manager.FileSystem = fileSystemMock.Object;
                var settings = Manager.Current;
                Assert.That(settings.Servers.Count, Is.EqualTo(0));
                Assert.That(settings.ReportLevels.Count, Is.EqualTo(4));
                Assert.That(settings.ReportLevels[0].Target, Is.EqualTo(ActionHandlerTargets.Root));
                Assert.That(settings.ReportLevels[1].Target, Is.EqualTo(ActionHandlerTargets.Server));
                Assert.That(settings.ReportLevels[2].Target, Is.EqualTo(ActionHandlerTargets.Project));
                Assert.That(settings.ReportLevels[3].Target, Is.EqualTo(ActionHandlerTargets.Build));
                foreach (var reportLevel in settings.ReportLevels)
                {
                    Assert.That(reportLevel.Reports.Count, Is.EqualTo(0));
                }
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void ReloadLoadsFileIfItExists()
        {
            try
            {
                var configPath = Path.Combine(Folders.DataDirectory, "web.settings");
                var fileSystemMock = new Mock<FileSystem>(MockBehavior.Strict);
                var xaml = "<Settings xmlns=\"urn:cruisecontrol:web:config\">" + Environment.NewLine +
                           "  <Settings.ReportLevels>" + Environment.NewLine +
                           "    <ReportLevel Target=\"Root\">" + Environment.NewLine +
                           "      <Report ActionName=\"anAction\" DisplayName=\"theAction\" IsDefault=\"True\" />" + Environment.NewLine +
                           "    </ReportLevel>" + Environment.NewLine +
                           "  </Settings.ReportLevels>" + Environment.NewLine +
                           "  <Settings.Servers>" + Environment.NewLine +
                           "    <Server DisplayName=\"TestServer\" Uri=\"http://somewhere\" />" + Environment.NewLine +
                           "  </Settings.Servers>" + Environment.NewLine +
                           "</Settings>";
                var reader = new StringReader(xaml);
                fileSystemMock.Setup(fs => fs.CheckFileExists(configPath)).Returns(true);
                fileSystemMock.Setup(fs => fs.ReadFromFile(configPath)).Returns(reader);
                Manager.Reset();
                Manager.FileSystem = fileSystemMock.Object;
                var settings = Manager.Current;
                Assert.That(settings.Servers.Count, Is.EqualTo(1));
                Assert.That(settings.Servers[0].DisplayName, Is.EqualTo("TestServer"));
                Assert.That(settings.Servers[0].Uri, Is.EqualTo("http://somewhere"));
                Assert.That(settings.ReportLevels.Count, Is.EqualTo(1));
                Assert.That(settings.ReportLevels[0].Target, Is.EqualTo(ActionHandlerTargets.Root));
                Assert.That(settings.ReportLevels[0].Reports.Count, Is.EqualTo(1));
                Assert.That(settings.ReportLevels[0].Reports[0].ActionName, Is.EqualTo("anAction"));
                Assert.That(settings.ReportLevels[0].Reports[0].DisplayName, Is.EqualTo("theAction"));
                Assert.That(settings.ReportLevels[0].Reports[0].IsDefault, Is.EqualTo(true));
                Assert.That(settings.Servers[0].Uri, Is.EqualTo("http://somewhere"));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void UpdateSavesTheNewSettings()
        {
            try
            {
                var configPath = Path.Combine(Folders.DataDirectory, "web.settings");
                var fileSystemMock = new Mock<FileSystem>(MockBehavior.Strict);
                var writer = new StringWriter();
                fileSystemMock.Setup(fs => fs.WriteToFile(configPath)).Returns(writer);
                Manager.Reset();
                Manager.FileSystem = fileSystemMock.Object;
                var newSettings = new Settings();
                newSettings.Servers.Add(new Server { DisplayName = "AServer", Uri = "http://somewhere" });
                newSettings.ReportLevels.Add(new ReportLevel { Target = ActionHandlerTargets.Root });
                newSettings.ReportLevels[0].Reports.Add(new Report
                                                            {
                                                                DisplayName = "A Test Report",
                                                                ActionName = "aReport",
                                                                IsDefault = true
                                                            });
                Manager.Update(newSettings);
                var actualXaml = writer.GetStringBuilder().ToString();
                var expectedXaml = "<Settings xmlns=\"urn:cruisecontrol:web:config\">" + Environment.NewLine + 
                    "  <Settings.ReportLevels>" + Environment.NewLine + 
                    "    <ReportLevel Target=\"Root\">" + Environment.NewLine + 
                    "      <Report ActionName=\"aReport\" DisplayName=\"A Test Report\" IsDefault=\"True\" />" + Environment.NewLine + 
                    "    </ReportLevel>" + Environment.NewLine + 
                    "  </Settings.ReportLevels>" + Environment.NewLine + 
                    "  <Settings.Servers>" + Environment.NewLine + 
                    "    <Server DisplayName=\"AServer\" Uri=\"http://somewhere\" />" + Environment.NewLine +
                    "  </Settings.Servers>" + Environment.NewLine +
                    "</Settings>";
                Assert.That(actualXaml, Is.EqualTo(expectedXaml));
                Assert.That(Manager.Current, Is.SameAs(newSettings));
            }
            finally
            {
                Manager.Reset();
            }
        }
        #endregion
    }
}
