using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util.Test;

namespace ThoughtWorks.CruiseControl.Core.Config.Test
{
	[TestFixture]
	public class ConfigurationContainerTest : Assertion
	{
		private IMock mockConfig;
		private IMock mockLoader;
		private bool configurationChanged;
		private MockFileWatcher watcher;
		private IConfigurationContainer container;

		[SetUp]
		protected void SetUp()
		{
			mockConfig = new DynamicMock(typeof(IConfiguration));
			mockLoader = new DynamicMock(typeof(IConfigurationPersister));
			configurationChanged = false;
			watcher = new MockFileWatcher();
			container = new ConfigurationContainer((IConfigurationPersister)mockLoader.MockInstance, watcher);
		}

		[TearDown]
		protected void TearDown()
		{
			mockConfig.Verify();
			mockLoader.Verify();
		}

		[Test]
		public void HandleConfigurationChangedEvent()
		{
			mockLoader.ExpectAndReturn("Load", mockConfig.MockInstance);

			container.AddConfigurationChangedHandler(new ConfigurationChangedHandler(HandleConfigurationChanged));
			watcher.RaiseEvent();

			Assert("configuration changed event not called", configurationChanged);
		}

		[Test]
		public void ReloadConfigurationThrowsAnExceptionThatShouldBeLogged()
		{
			mockLoader.ExpectAndThrow("Load", new CruiseControlException("can't load config"));

			container.AddConfigurationChangedHandler(new ConfigurationChangedHandler(HandleConfigurationChanged));
			watcher.RaiseEvent();

			Assert("configuration changed should not be called because of exception", ! configurationChanged);
		}

		private void HandleConfigurationChanged(IConfiguration newConfiguration)
		{
			AssertEquals(mockConfig.MockInstance, newConfiguration);
			configurationChanged = true;
		}
	}
}
