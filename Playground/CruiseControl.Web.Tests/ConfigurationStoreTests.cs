namespace CruiseControl.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using System.Collections.Specialized;
    using System.IO;

    [TestFixture]
    public class ConfigurationStoreTests
    {
        #region Tests
        [Test]
        public void DataDirectoryReturnsDirectory()
        {
            var directory = "c:\\somewhere";
            try
            {
                ConfigurationStore.AppSettings = new NameValueCollection();
                ConfigurationStore.AppSettings["CCNetDataDirectory"] = directory;
                Assert.That(ConfigurationStore.DataDirectory, Is.EqualTo(directory));
            }
            finally
            {
                ConfigurationStore.Reset();
            }
        }

        [Test]
        public void DataDirectoryReturnsDefault()
        {
            var directory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "CCNet",
                        "Web");
            try
            {
                ConfigurationStore.AppSettings = new NameValueCollection();
                Assert.That(ConfigurationStore.DataDirectory, Is.EqualTo(directory));
            }
            finally
            {
                ConfigurationStore.Reset();
            }
        }

        [Test]
        public void PluginDirectoryReturnsDirectory()
        {
            var directory = "c:\\somewhere";
            try
            {
                ConfigurationStore.AppSettings = new NameValueCollection();
                ConfigurationStore.AppSettings["CCNetPluginDirectory"] = directory;
                Assert.That(ConfigurationStore.PluginDirectory, Is.EqualTo(directory));
            }
            finally
            {
                ConfigurationStore.Reset();
            }
        }

        [Test]
        public void PluginDirectoryReturnsDirectoryFromDataDirectory()
        {
            var directory = "c:\\somewhere";
            try
            {
                ConfigurationStore.AppSettings = new NameValueCollection();
                ConfigurationStore.AppSettings["CCNetDataDirectory"] = directory;
                Assert.That(ConfigurationStore.PluginDirectory,
                    Is.EqualTo(Path.Combine(directory, "Plugins")));
            }
            finally
            {
                ConfigurationStore.Reset();
            }
        }

        [Test]
        public void PluginDirectoryReturnsDefault()
        {
            var directory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "CCNet",
                        "Web",
                        "Plugins");
            try
            {
                ConfigurationStore.AppSettings = new NameValueCollection();
                Assert.That(ConfigurationStore.PluginDirectory, Is.EqualTo(directory));
            }
            finally
            {
                ConfigurationStore.Reset();
            }
        }
        #endregion
    }
}
