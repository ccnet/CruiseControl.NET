using System;
using System.Diagnostics;

using NMock;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console.Test
{
	[TestFixture]
	public class ConsoleRunnerTest : CustomAssertion
	{
		private TestTraceListener listener;

		[SetUp]
		protected void SetUp()
		{
			Trace.Listeners.Clear();
			listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
			TempFileUtil.CreateTempDir("ConsoleRunnerTest");
		}

		[TearDown]
		protected void TearDown()
		{
			Trace.Listeners.Remove(listener);
			TempFileUtil.DeleteTempDir("ConsoleRunnerTest");
		}

		[Test]
		public void ShowHelp()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "-remoting:on", "-help" });
			ConsoleRunner runner = new ConsoleRunner(parser, null);
			runner.Run();

			AssertEquals(1, listener.Traces.Count);
			Assert("Wrong message was logged.", listener.Traces[0].ToString().IndexOf(ArgumentParser.Usage) > 0);
		}

		[Test] // integration test
		public void ForceBuildCruiseServerProject()
		{
			string xml = @"<cruisecontrol><workflow name=""test""><tasks /></workflow></cruisecontrol>";
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			Mock mockTimeout = new DynamicMock(typeof(ITimeout));
			mockTimeout.Expect("Wait");

			ArgumentParser parser = new ArgumentParser(new string[] { "-project:test", "-config:" + configFile });
			new ConsoleRunner(parser, (ITimeout)mockTimeout.MockInstance).Run();
		}	

		[Test] // integration test
		public void StartCruiseServerProject()
		{
			string xml = @"<cruisecontrol><workflow name=""test""><tasks /></workflow></cruisecontrol>";
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			Mock mockTimeout = new DynamicMock(typeof(ITimeout));
			mockTimeout.Expect("Wait");

			ArgumentParser parser = new ArgumentParser(new string[] { "-config:" + configFile });
			new ConsoleRunner(parser, (ITimeout)mockTimeout.MockInstance).Run();

			mockTimeout.Verify();
		}	
	}
}