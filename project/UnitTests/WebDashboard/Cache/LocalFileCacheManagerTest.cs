using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Cache
{
	[TestFixture]
	public class LocalFileCacheManagerTest
	{
		private DynamicMock pathMapperMock;
		private DynamicMock configurationGetterMock;
		private string cacheRootDirectory;
		private string servername;
		private string projectname;
		private string subdirectory;
		private string filename;
		private string content;
		private string fullfilename;

		[SetUp]
		public void Setup()
		{
			pathMapperMock = new DynamicMock(typeof(IPathMapper));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			cacheRootDirectory = TempFileUtil.GetTempPath("cache");
			configurationGetterMock.SetupResult("GetSimpleConfigSetting", cacheRootDirectory, typeof(string));
			servername = "myserver";
			projectname = "myproject";
			subdirectory = "subdir";
			filename = "myfile.xml";
			fullfilename = Path.Combine(cacheRootDirectory, @"myserver\myproject\subdir\myfile.xml");
			content = "some\r\ncontent";
			TempFileUtil.DeleteTempDir("cache");
		}

		private void VerifyAll()
		{
			pathMapperMock.Verify();
			configurationGetterMock.Verify();
		}

		[TearDown]
		public void Teardown()
		{
			TempFileUtil.DeleteTempDir("cache");
		}

		[Test]
		public void CreatesCacheDirectoryIfItDoesntAlreadyExist()
		{
			Assert.IsTrue(! Directory.Exists(cacheRootDirectory));
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", fullfilename, fullfilename);
			manager.AddContent(servername, projectname, subdirectory, filename, content);
			Assert.IsTrue(Directory.Exists(cacheRootDirectory));

			VerifyAll();
		}

		[Test]
		public void CanAddNewContent()
		{
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", fullfilename, fullfilename);
			manager.AddContent(servername, projectname, subdirectory, filename, content);
			string expectedFile = Path.Combine(cacheRootDirectory, @"myserver\myproject\subdir\myfile.xml");

			string readContent = null;
			using (StreamReader sr = new StreamReader(expectedFile)) 
			{
				readContent = sr.ReadToEnd();
			}

			Assert.AreEqual(content, readContent);
			VerifyAll();
		}

		[Test]
		public void GetContentReturnsNullIfRequestedContentDoesntExist()
		{
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", fullfilename, fullfilename);
			Assert.IsNull(manager.GetContent(servername, projectname, subdirectory, filename));
			VerifyAll();
		}

		[Test]
		public void GetContentReturnsContentIfItExistsAcrossInstances()
		{
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", fullfilename, fullfilename);
			manager.AddContent(servername, projectname, subdirectory, filename, content);

			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", fullfilename, fullfilename);
			manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			Assert.AreEqual(content, manager.GetContent(servername, projectname, subdirectory, filename));

			VerifyAll();
		}

		[Test]
		public void ReturnsUrlForFile()
		{
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://foo.bar/baz", Path.Combine(cacheRootDirectory, @"myserver\myproject\subdir\myfile.xml"));

			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			Assert.AreEqual("http://foo.bar/baz", manager.GetURLForFile(servername, projectname, subdirectory, filename));
			VerifyAll();
		}
	}
}
