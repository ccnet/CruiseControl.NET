using System;
using System.Diagnostics;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
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
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			ArgumentParser parser = new ArgumentParser(new string[] { "-remoting:on", "-help" });
			ConsoleRunner runner = new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance, null);
			runner.Run();

			AssertEquals(1, listener.Traces.Count);
			Assert("Wrong message was logged.", listener.Traces[0].ToString().IndexOf(ArgumentParser.Usage) > 0);
		}

		[Test]
		public void ForceBuildCruiseServerProject()
		{
			Mock mockTimeout = new DynamicMock(typeof(ITimeout));
			mockTimeout.Expect("Wait");

			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("ForceBuild", "test");

			ArgumentParser parser = new ArgumentParser(new string[] { "-project:test" });
			new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance, (ITimeout)mockTimeout.MockInstance).Run();

			mockTimeout.Verify();
			mockCruiseServer.Verify();
		}	

		[Test]
		public void StartCruiseServerProject()
		{
			string xml = @"<cruisecontrol><workflow name=""test""><tasks /></workflow></cruisecontrol>";
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			Mock mockTimeout = new DynamicMock(typeof(ITimeout));
			mockTimeout.Expect("Wait");

			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("Start");

			ArgumentParser parser = new ArgumentParser(new string[0]);
			new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance, (ITimeout)mockTimeout.MockInstance).Run();

			mockTimeout.Verify();
			mockCruiseServer.Verify();
		}	
	}
}