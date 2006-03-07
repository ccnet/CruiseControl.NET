using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
{
	[TestFixture]
	public class P4ConfigProcessInfoCreatorTest 
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

			Assert.AreEqual("myExecutable", info.FileName);
			Assert.AreEqual("-s -c myClient -p anotherserver:2666 -u myUser my arguments", info.Arguments);
		}

		[Test]
		public void ShouldCreateProcessWithDefaultArgumentsIfNoneAreSet()
		{
			// Setup
			P4 p4 = new P4();

			ProcessInfo info = new P4ConfigProcessInfoCreator().CreateProcessInfo(p4, "my arguments");

			Assert.AreEqual("p4", info.FileName);
			Assert.AreEqual("-s my arguments", info.Arguments);
		}
		
		[Test]
		public void ShouldCreateProcessWithDefaultArgumentsIfOnlyUserIsSet()
		{
			// Setup
			P4 p4 = new P4();
			p4.User = "myUser";

			ProcessInfo info = new P4ConfigProcessInfoCreator().CreateProcessInfo(p4, "my arguments");

			Assert.AreEqual("p4", info.FileName);
			Assert.AreEqual("-s -u myUser my arguments", info.Arguments);
		}
	}
}
