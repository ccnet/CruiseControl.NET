using System;
using NUnit.Framework;
using tw.ccnet.core;

namespace tw.ccnet.console.test
{
	[TestFixture]
	public class ConsoleRunnerTest
	{
		public void TestReadArgs_help()
		{
			string[] args = new String[]{"-help"};
			Assertion.AssertNull("config should be null", ConsoleRunner.GetConfigFileName(args));
		}

		// TODO: test running cruisecontrol from console
		public void TestReadArgs_config()
		{
//		string file = FileMother.CreateConfigFile();
//
//			string[] args = new String[]{file};
//			Configuration config = ConsoleRunner.ReadArgs(args);
//			Assertion.AssertNotNull("config should not be null", config);
//			Assertion.AssertEquals(2, config.Publishers.Count);
//			Assertion.AssertEquals(typeof(tw.ccnet.core.publishers.EmailPublisher), config.Publishers[0].GetType());
//			Assertion.AssertEquals(typeof(tw.ccnet.core.publishers.XmlLogPublisher), config.Publishers[1].GetType());
//			Assertion.AssertEquals(typeof(tw.ccnet.core.sourcecontrol.test.MockSourceControl), config.SourceControl.GetType());
//			FileMother.CleanUp();
		}

		public void TestGetConfigFileName()
		{
			string[] args = new String[]{"cruise.config"};
			Assertion.AssertEquals("Invalid config file","cruise.config",ConsoleRunner.GetConfigFileName(args));
			args = new String[]{"-remoting:on","cruise.config"};
			Assertion.AssertEquals("Invalid config file", "cruise.config", ConsoleRunner.GetConfigFileName(args));
			args = new String[]{};
			Assertion.AssertEquals("Invalid config file", ".\\ccnet.config", ConsoleRunner.GetConfigFileName(args));
		}

		public void TestGetOption(){
			string[] args = new String[]{"cruise.config"};
			Assertion.AssertNull("Invalid Option", ConsoleRunner.GetOption("remoting",args));
			args = new String[]{"-remoting:on", "cruise.config"};
			Assertion.AssertEquals("Invalid Option", "on", ConsoleRunner.GetOption("remoting",args));
			args = new String[]{"-remoting:off", "-process:XP", "cruise.config"};;
			Assertion.AssertEquals("Invalid Option" ,"off", ConsoleRunner.GetOption("remoting",args));
			Assertion.AssertEquals("Invalid Option", "XP", ConsoleRunner.GetOption("process",args));
		}

	}
}