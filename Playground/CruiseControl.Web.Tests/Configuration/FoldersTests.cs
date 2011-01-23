namespace CruiseControl.Web.Tests.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using CruiseControl.Web.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class FoldersTests
    {
        #region Tests
        [Test]
        public void DataDirectoryReturnsDirectory()
        {
            var directory = "c:\\somewhere";
            try
            {
                Folders.AppSettings = new NameValueCollection();
                Folders.AppSettings["CCNetDataDirectory"] = directory;
                Assert.That(Folders.DataDirectory, Is.EqualTo(directory));
            }
            finally
            {
                Folders.Reset();
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
                Folders.AppSettings = new NameValueCollection();
                Assert.That(Folders.DataDirectory, Is.EqualTo(directory));
            }
            finally
            {
                Folders.Reset();
            }
        }

        [Test]
        public void PluginDirectoryReturnsDirectory()
        {
            var directory = "c:\\somewhere";
            try
            {
                Folders.AppSettings = new NameValueCollection();
                Folders.AppSettings["CCNetPluginDirectory"] = directory;
                Assert.That(Folders.PluginDirectory, Is.EqualTo(directory));
            }
            finally
            {
                Folders.Reset();
            }
        }

        [Test]
        public void PluginDirectoryReturnsDirectoryFromDataDirectory()
        {
            var directory = "c:\\somewhere";
            try
            {
                Folders.AppSettings = new NameValueCollection();
                Folders.AppSettings["CCNetDataDirectory"] = directory;
                Assert.That(Folders.PluginDirectory,
                    Is.EqualTo(Path.Combine(directory, "Plugins")));
            }
            finally
            {
                Folders.Reset();
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
                Folders.AppSettings = new NameValueCollection();
                Assert.That(Folders.PluginDirectory, Is.EqualTo(directory));
            }
            finally
            {
                Folders.Reset();
            }
        }
        #endregion
    }
}
