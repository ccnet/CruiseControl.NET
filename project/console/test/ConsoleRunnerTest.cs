using System.Diagnostics;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Console.Test
{
	[TestFixture]
	public class ConsoleRunnerTest : CustomAssertion
	{
		private TestTraceListener listener;

		[SetUp]
		public void SetUp()
		{
			Trace.Listeners.Clear();
			listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
			TempFileUtil.CreateTempDir("ConsoleRunnerTest");
		}

		[TearDown]
		public void TearDown()
		{
			Trace.Listeners.Remove(listener);
			TempFileUtil.DeleteTempDir("ConsoleRunnerTest");
		}

		[Test]
		public void ShowHelp()
		{
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			ArgumentParser parser = new ArgumentParser(new string[] { "-remoting:on", "-help" });
			ConsoleRunner runner = new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance);
			runner.Run();

			Assert.AreEqual(1, listener.Traces.Count);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf(ArgumentParser.Usage) > 0, "Wrong message was logged.");
		}

		[Test]
		public void ForceBuildCruiseServerProject()
		{
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("ForceBuild", "test");
			mockCruiseServer.Expect("WaitForExit","test");

			ArgumentParser parser = new ArgumentParser(new string[] { "-project:test" });
			new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance).Run();

			mockCruiseServer.Verify();
		}	

		[Test]
		public void StartCruiseServerProject()
		{
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("Start");
			mockCruiseServer.Expect("WaitForExit");

			ArgumentParser parser = new ArgumentParser(new string[0]);
			new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance).Run();

			mockCruiseServer.Verify();
		}	
	}
}