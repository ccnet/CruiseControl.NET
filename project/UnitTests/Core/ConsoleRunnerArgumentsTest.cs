using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ConsoleRunnerArgumentsTest : CustomAssertion
	{
		private TraceListenerBackup backup;
		private TestTraceListener listener;

		[SetUp]
		protected void AddListener()
		{
			backup = new TraceListenerBackup();
			backup.Reset();
			listener = backup.AddTestTraceListener();
		}

		[TearDown]
		protected void RemoveListener()
		{
			backup.Reset();
		}

		[Test]
		public void TestDefaultArguments()
		{
			ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
			Assert.AreEqual(true, consoleArgs.UseRemoting);
			Assert.IsNull(consoleArgs.Project);
			Assert.AreEqual(ConsoleRunnerArguments.DEFAULT_CONFIG_PATH, consoleArgs.ConfigFile);
            Assert.AreEqual(false, consoleArgs.ValidateConfigOnly);
            Assert.AreEqual(true, consoleArgs.Logging);
            Assert.AreEqual(true, consoleArgs.PauseOnError);
            Assert.AreEqual(false, consoleArgs.ShowHelp);
		}
	}
}