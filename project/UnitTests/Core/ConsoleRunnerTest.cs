using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ConsoleRunnerTest : CustomAssertion
	{
		private TestTraceListener listener;
		private TraceListenerBackup backup;

		[SetUp]
		public void SetUp()
		{
			backup = new TraceListenerBackup();
			listener = backup.AddTestTraceListener();
		}

		[TearDown]
		public void TearDown()
		{
			backup.Reset();
		}

		[Test, Ignore("This test has initimate knowledge of the logging implementation; it should not")]
		public void ShowHelp()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "-remoting:on", "-help" });
			Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
			mockCruiseServerFactory.ExpectNoCall("Create", typeof(bool), typeof(string));

			ConsoleRunner runner = new ConsoleRunner(parser, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance);
			runner.Run();

			Assert.AreEqual(1, listener.Traces.Count);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf(ArgumentParser.Usage) > 0, "Wrong message was logged.");

			mockCruiseServerFactory.Verify();
		}

		[Test]
		public void ForceBuildCruiseServerProject()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "-project:test" });
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
            mockCruiseServer.Expect("ForceBuild", (string)null, "test", "Forcing build on start");
            mockCruiseServer.Expect("WaitForExit", "test");
			Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
			mockCruiseServerFactory.ExpectAndReturn("Create", mockCruiseServer.MockInstance, parser.UseRemoting, parser.ConfigFile);

			new ConsoleRunner(parser, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance).Run();

			mockCruiseServer.Verify();
		}	

		[Test]
		public void StartCruiseServerProject()
		{
			ArgumentParser parser = new ArgumentParser(new string[0]);
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("Start");
			mockCruiseServer.Expect("WaitForExit");
			Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
			mockCruiseServerFactory.ExpectAndReturn("Create", mockCruiseServer.MockInstance, parser.UseRemoting, parser.ConfigFile);

			new ConsoleRunner(parser, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance).Run();

			mockCruiseServer.Verify();
		}

        [Test]
        public void ValidateConfigFileShouldNotStartServer()
        {
            ArgumentParser parser = new ArgumentParser(new string[] { "-validate" });
            Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
            Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
            mockCruiseServerFactory.ExpectAndReturn("Create", mockCruiseServer.MockInstance, false, parser.ConfigFile);

            new ConsoleRunner(parser, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance).Run();

            mockCruiseServer.Verify();
        }
    }
}