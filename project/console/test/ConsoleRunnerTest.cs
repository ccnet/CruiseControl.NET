using NUnit.Framework;
using tw.ccnet.core;
using tw.ccnet.core.util;
using System;
using System.Diagnostics;

namespace tw.ccnet.console.test
{
	[TestFixture]
	public class ConsoleRunnerTest : CustomAssertion
	{
		[Test]
		public void ShowHelp()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);

			new ConsoleRunner(new ArgumentParser(new string[] { "-remoting:on", "-help" })).Run();
			AssertEquals(1, listener.Traces.Count);
			AssertEquals(ArgumentParser.Usage, listener.Traces[0].ToString());

			Trace.Listeners.Remove(listener);
		}

		[Test] // integration test
		public void ForceBuildCruiseServerProject()
		{
			string xml = @"<cruisecontrol><generic name=""test""><tasks /></generic></cruisecontrol>";
			TempFileUtil.CreateTempDir("ConsoleRunnerTest");
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			new ConsoleRunner(new ArgumentParser(new string[] { "-project:test", "-config:" + configFile })).Run();

			TempFileUtil.DeleteTempDir("ConsoleRunnerTest");
		}	
	}
}