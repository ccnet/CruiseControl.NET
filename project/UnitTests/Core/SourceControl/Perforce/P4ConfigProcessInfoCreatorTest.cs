using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
{
	[TestFixture]
	public class P4ConfigProcessInfoCreatorTest : Assertion
	{
		[Test]
		public void ShouldCreateProcessUsingAllConfigurationVariablesIfTheyAreSet()
		{
			// Setup
			P4 p4 = new P4();
			p4.Executable = "myExecutable";
			p4.Client = "myClient";
			p4.User = "myUser";
			p4.Port = "anotherserver:2666";

			ProcessInfo info = new P4ConfigProcessInfoCreator().CreateProcessInfo(p4, "my arguments");

			AssertEquals("myExecutable", info.FileName);
			AssertEquals("-s -c myClient -p anotherserver:2666 -u myUser my arguments", info.Arguments);
		}

		[Test]
		public void ShouldCreateProcessWithDefaultArgumentsIfNoneAreSet()
		{
			// Setup
			P4 p4 = new P4();

			ProcessInfo info = new P4ConfigProcessInfoCreator().CreateProcessInfo(p4, "my arguments");

			AssertEquals("p4", info.FileName);
			AssertEquals("-s my arguments", info.Arguments);
		}
	}
}
