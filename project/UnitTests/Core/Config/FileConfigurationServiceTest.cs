using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.UnitTests.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class FileConfigurationServiceTest
	{
		private DynamicMock configurationFileLoaderMock;
		private DynamicMock configurationFileSaverMock;
		private MockFileWatcher fileWatcher;
		private FileConfigurationService fileService;

		private DynamicMock configurationMock;
		private IConfiguration configuration;
		private FileInfo configFile;

		[SetUp]
		public void Setup()
		{
			configurationFileLoaderMock = new DynamicMock(typeof(IConfigurationFileLoader));
			configurationFileSaverMock = new DynamicMock(typeof(IConfigurationFileSaver));
			fileWatcher = new MockFileWatcher();
			configFile = new FileInfo("testFileName");
			fileService = new FileConfigurationService((IConfigurationFileLoader) configurationFileLoaderMock.MockInstance,
				(IConfigurationFileSaver) configurationFileSaverMock.MockInstance,
				fileWatcher,
				configFile);

			configurationMock = new DynamicMock(typeof(IConfiguration));
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

		[Test]
		public void CallsUpdateHandlersWhenFileWatcherChanges()
		{
			// Setup
			fileService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(OnUpdate));
			updateCalled = false;

			// Execute
			fileWatcher.RaiseEvent();

			// Verify
			Assert.IsTrue(updateCalled);
		}

		bool updateCalled = false;
		public void OnUpdate()
		{
			updateCalled = true;
		}

		[Test]
		public void DoesSomethingSaneWhenBadLoadThingsHappen()
		{
			
		}
	}
}
