using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Util.Test;

namespace ThoughtWorks.CruiseControl.Core.Config.Test
{
	[TestFixture]
	public class ConfigurationContainerTest : Assertion
	{
		private IMock mockConfig;
		private IMock mockConfig2;

		private IMock mockLoader;
		private bool configurationChanged;

		[SetUp]
		protected void SetUp()
		{
			mockConfig = new DynamicMock(typeof(IConfiguration));
			mockConfig2 = new DynamicMock(typeof(IConfiguration));
			mockLoader = new DynamicMock(typeof(IConfigurationLoader));
			mockLoader.ExpectAndReturn("Load", mockConfig.MockInstance);
			configurationChanged = false;
		}

		[Test]
		public void CreateConfigurationContainer()
		{
			IConfigurationContainer container = new ConfigurationContainer((IConfigurationLoader)mockLoader.MockInstance, new MockFileWatcher());

			mockLoader.Verify();
		}

		[Test]
		public void HandleConfigurationChangedEvent()
		{
			mockLoader.ExpectAndReturn("Load", mockConfig2.MockInstance);
			MockFileWatcher watcher = new MockFileWatcher();

			IConfigurationContainer container = new ConfigurationContainer((IConfigurationLoader)mockLoader.MockInstance, watcher);
			container.AddConfigurationChangedHandler(new ConfigurationChangedHandler(HandleConfigurationChanged));

			watcher.RaiseEvent();

			Assert("configuration changed event not called", configurationChanged);
			mockLoader.Verify();
		}

		[Test]
		public void ReloadConfigurationThrowsAnExceptionThatShouldBeLogged()
		{
			mockLoader.ExpectAndThrow("Load", new CruiseControlException("can't load config"));
			MockFileWatcher watcher = new MockFileWatcher();

			IConfigurationContainer container = new ConfigurationContainer((IConfigurationLoader)mockLoader.MockInstance, watcher);
			container.AddConfigurationChangedHandler(new ConfigurationChangedHandler(HandleConfigurationChanged));

			watcher.RaiseEvent();

			Assert("configuration changed should not be called because of exception", ! configurationChanged);
			mockLoader.Verify();
		}

		private void HandleConfigurationChanged(IConfiguration newConfiguration)
		{
			AssertEquals(mockConfig2.MockInstance, newConfiguration);
			configurationChanged = true;
		}
	}
}
