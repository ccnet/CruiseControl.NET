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
	public class LocalFileCacheManagerTest : Assertion
	{
		private DynamicMock pathMapperMock;
		private DynamicMock configurationGetterMock;
		private string cacheRootDirectory;
		private string servername;
		private string projectname;
		private string subdirectory;
		private string filename;
		private string content;

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
			content = "some\r\ncontent";
			TempFileUtil.DeleteTempDir("cache");
		}

		[TearDown]
		public void Teardown()
		{
			TempFileUtil.DeleteTempDir("cache");
		}

		[Test]
		public void CreatesCacheDirectoryIfItDoesntAlreadyExist()
		{
			Assert(! Directory.Exists(cacheRootDirectory));
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			manager.AddContent(servername, projectname, subdirectory, filename, content);
			Assert(Directory.Exists(cacheRootDirectory));
		}

		[Test]
		public void CanAddNewContent()
		{
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			manager.AddContent(servername, projectname, subdirectory, filename, content);
			string expectedFile = Path.Combine(cacheRootDirectory, @"myserver\myproject\subdir\myfile.xml");

			string readContent = null;
			using (StreamReader sr = new StreamReader(expectedFile)) 
			{
				readContent = sr.ReadToEnd();
			}

			AssertEquals(content, readContent);
		}

		[Test]
		public void GetContentReturnsNullIfRequestedContentDoesntExist()
		{
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			AssertNull(manager.GetContent(servername, projectname, subdirectory, filename));
		}

		[Test]
		public void GetContentReturnsContentIfItExistsAcrossInstances()
		{
			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			manager.AddContent(servername, projectname, subdirectory, filename, content);
			manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			AssertEquals(content, manager.GetContent(servername, projectname, subdirectory, filename));
		}

		[Test]
		public void ReturnsUrlForFile()
		{
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://foo.bar/baz", Path.Combine(cacheRootDirectory, @"myserver\myproject\subdir\myfile.xml"));

			LocalFileCacheManager manager = new LocalFileCacheManager((IPathMapper) pathMapperMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
			AssertEquals("http://foo.bar/baz", manager.GetURLForFile(servername, projectname, subdirectory, filename));
			pathMapperMock.Verify();
		}
	}
}
