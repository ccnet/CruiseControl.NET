using System;
using System.Diagnostics;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
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
			ConsoleRunner runner = new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance);
			runner.Run();

			AssertEquals(1, listener.Traces.Count);
			Assert("Wrong message was logged.", listener.Traces[0].ToString().IndexOf(ArgumentParser.Usage) > 0);
		}

		[Test]
		public void ForceBuildCruiseServerProject()
		{
			Mock mockCruiseManager = new DynamicMock(typeof(ICruiseManager));
			mockCruiseManager.Expect("ForceBuild", "test");
			mockCruiseManager.Expect("WaitForExit");

			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);
			mockCruiseServer.ExpectAndReturn("CruiseManager", mockCruiseManager.MockInstance);

			ArgumentParser parser = new ArgumentParser(new string[] { "-project:test" });
			new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance).Run();

			mockCruiseServer.Verify();
			mockCruiseManager.Verify();
		}	

		[Test]
		public void StartCruiseServerProject()
		{
			string xml = @"<cruisecontrol><workflow name=""test""><tasks /></workflow></cruisecontrol>";
			string configFile = TempFileUtil.CreateTempXmlFile("ConsoleRunnerTest", "myconfig.config", xml);

			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("Start");
			mockCruiseServer.Expect("WaitForExit");

			ArgumentParser parser = new ArgumentParser(new string[0]);
			new ConsoleRunner(parser, (ICruiseServer)mockCruiseServer.MockInstance).Run();

			mockCruiseServer.Verify();
		}	
	}
}