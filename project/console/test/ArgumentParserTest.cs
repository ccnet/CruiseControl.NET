using System;
using System.Diagnostics;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console.Test
{
	[TestFixture]
	public class ArgumentParserTest : CustomAssertion
	{
		private TestTraceListener listener;

		[SetUp]
		protected void AddListener()
		{
			Trace.Listeners.Clear();
			listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
		}

		[TearDown]
		protected void RemoveListener()
		{
			Trace.Listeners.Remove(listener);
		}

		[Test]
		public void InstantiateWithSpecifiedArguments()
		{
			string[] args = new string[] { "-remoting:on", "-project:foo", @"-config:C:\test dir\cruise.config", "-help" };
			ArgumentParser parser = new ArgumentParser(args);
			AssertEquals(true, parser.IsRemote);
			AssertEquals("foo", parser.Project);
			AssertEquals(@"C:\test dir\cruise.config", parser.ConfigFile);
			AssertEquals(true, parser.ShowHelp);
		}

		[Test]
		public void InstantiateWithDefaultArguments()
		{
			ArgumentParser parser = new ArgumentParser(new string[0]);
			AssertEquals(false, parser.IsRemote);
			AssertNull(parser.Project);
			AssertEquals(ArgumentParser.DEFAULT_CONFIG_PATH, parser.ConfigFile);	
			AssertEquals(false, parser.ShowHelp);
		}

		[Test]
		public void InstantiateWithInvalidArguments()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "foo" });
			AssertEquals(true, parser.ShowHelp);
		}

		[Test]
		public void InstantiateWithMoreInvalidArguments()
		{
			ArgumentParser parser = new ArgumentParser(new string[] { "-foo:bar" });
			AssertEquals(true, parser.ShowHelp);
			AssertEquals(1, listener.Traces.Count);
			Assert(listener.Traces[0].ToString().IndexOf("-foo:bar") >= 0);
		}
	}
}