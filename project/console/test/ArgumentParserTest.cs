using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console.Test
{
	/* These aren't run anyway since NUnit can't run against a .exe !

	[TestFixture]
	public class ArgumentParserTest : CustomAssertion
	{
		private TraceListenerBackup backup;
		private TestTraceListener listener;

		[SetUp]
		protected void AddListener()
		{
			backup = new TraceListenerBackup();
			listener = backup.AddTestTraceListener();
		}

		[TearDown]
		protected void RemoveListener()
		{
			backup.Reset();
		}

		[Test]
		public void InstantiateWithSpecifiedArguments()
		{
			string[] args = new string[] { "-remoting:off", "-project:foo", @"-config:C:\test dir\cruise.config", "-help" };
			ArgumentParser parser = new ArgumentParser(args);
			Assert.AreEqual(false, parser.IsRemote);
			Assert.AreEqual("foo", parser.Project);
			Assert.AreEqual(@"C:\test dir\cruise.config", parser.ConfigFile);
			Assert.AreEqual(true, parser.ShowHelp);
		}

		[Test]
		public void InstantiateWithDefaultArguments()
		{
			ArgumentParser parser = new ArgumentParser(new string[0]);
			Assert.AreEqual(true, parser.IsRemote);
			Assert.IsNull(parser.Project);
			Assert.AreEqual(ArgumentParser.DEFAULT_CONFIG_PATH, parser.ConfigFile);	
			Assert.AreEqual(false, parser.ShowHelp);
		}

		[Test]
		public void InstantiateWithInvalidArguments()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "foo" });
			Assert.AreEqual(true, parser.ShowHelp);
		}

		[Test]
		public void InstantiateWithMoreInvalidArguments()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "-foo:bar" });
			Assert.AreEqual(true, parser.ShowHelp);
			Assert.AreEqual(1, listener.Traces.Count);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf("-foo:bar") >= 0);
		}
	}
	*/
}