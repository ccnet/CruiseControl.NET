using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class FileConfigurationServiceTest
	{
		private DynamicMock configurationFileLoaderMock;
		private DynamicMock configurationFileSaverMock;
		private FileConfigurationService fileService;

		private DynamicMock configurationMock;
		private IConfiguration configuration;
		private FileInfo configFile;

		[SetUp]
		public void Setup()
		{
			configurationFileLoaderMock = new DynamicMock(typeof (IConfigurationFileLoader));
			configurationFileSaverMock = new DynamicMock(typeof (IConfigurationFileSaver));
			configFile = new FileInfo("testFileName");

			fileService = new FileConfigurationService((IConfigurationFileLoader) configurationFileLoaderMock.MockInstance,
			                                           (IConfigurationFileSaver) configurationFileSaverMock.MockInstance,
			                                           configFile);

			configurationMock = new DynamicMock(typeof (IConfiguration));
			configuration = (IConfiguration) configurationMock.MockInstance;
		}

		private void VerifyAll()
		{
			configurationFileLoaderMock.Verify();
			configurationFileSaverMock.Verify();
		}

		[Test]
		public void ShouldDelegateLoadRequests()
		{
			// Setup
			configurationFileLoaderMock.ExpectAndReturn("Load", configuration, configFile);

			// Execute & Verify
			Assert.AreEqual(configuration, fileService.Load());

			VerifyAll();
		}

		[Test]
		public void ShouldDelegateSaveRequests()
		{
			// Setup
			configurationFileSaverMock.Expect("Save", configuration, configFile);

			// Execute & Verify
			fileService.Save(configuration);

			VerifyAll();
		}

		[Test, Ignore("unimplemented")]
		public void DoesSomethingSaneWhenBadLoadThingsHappen()
		{}
	}
}