using NMock;
using NUnit.Framework;
using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console.test
{
	[TestFixture]
	public class ConsoleRunnerTest : CustomAssertion
	{
		[Test]
		public void ShowHelp()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);

			ArgumentParser parser = new ArgumentParser(new string[] { "-remoting:on", "-help" });
			ConsoleRunner runner = new ConsoleRunner(parser, null);
			runner.Run();
			AssertEquals(1, listener.Traces.Count);
			AssertEquals(ArgumentParser.Usage, listener.Traces[0].ToString());

			Trace.Listeners.Remove(listener);
		}

		[Test] // integration test
		public void ForceBuildCruiseServerProject()
		{
			string xml = @"<cruisecontrol><workflow name=""test""><tasks /></workflow></cruisecontrol>";
			TempFileUtil.CreateTempDir("ConsoleRunnerTest");
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			ArgumentParser parser = new ArgumentParser(new string[] { "-project:test", "-config:" + configFile });
			new ConsoleRunner(parser, null).Run();

			TempFileUtil.DeleteTempDir("ConsoleRunnerTest");
		}	

		[Test] // integration test
		public void StartCruiseServerProject()
		{
			string xml = @"<cruisecontrol><workflow name=""test""><tasks /></workflow></cruisecontrol>";
			TempFileUtil.CreateTempDir("ConsoleRunnerTest");
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			Mock mockTimeout = new DynamicMock(typeof(ITimeout));
			mockTimeout.Expect("Wait");

			ArgumentParser parser = new ArgumentParser(new string[] { "-config:" + configFile });
			new ConsoleRunner(parser, (ITimeout)mockTimeout.MockInstance).Run();

			mockTimeout.Verify();
			TempFileUtil.DeleteTempDir("ConsoleRunnerTest");
		}	
	}
}