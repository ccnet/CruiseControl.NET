using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console.test
{
	[TestFixture]
	public class ArgumentParserTest : CustomAssertion
	{
		[Test]
		public void InstantiateWithSpecifiedArguments()
		{
			string[] args = new string[] { "-remoting:on", "-project:foo", "-config:cruise.config", "-help" };
			ArgumentParser parser = new ArgumentParser(args);
			AssertEquals(true, parser.IsRemote);
			AssertEquals("foo", parser.Project);
			AssertEquals("cruise.config", parser.ConfigFile);
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
	}
}