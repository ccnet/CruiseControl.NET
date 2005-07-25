using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NUnitArgumentTest : CustomAssertion
	{
		[Test, ExpectedException(typeof(CruiseControlException))]
		public void IfNoAssembliesAreSpecifiedThenTheArgumentIsInvalid()
		{
			new NUnitArgument(null, null);
		}

		[Test]
		public void ShouldUseNoLogoArgument()
		{
			string argString = new NUnitArgument(new string[] {"foo.dll"}, "testfile.xml").ToString();
			AssertContains("/nologo", argString);
		}

		[Test]
		public void ShouldSpecifyXmlOutputFileToUse()
		{
			string argString = new NUnitArgument(new string[] {"foo.dll"}, "testfile.xml").ToString();
			AssertContains(@"/xml=testfile.xml", argString);
		}

		[Test]
		public void ShouldWrapOutputFileInQuotesIfItContainsASpace()
		{
			string argString = new NUnitArgument(new string[] {"foo.dll"}, @"c:\program files\cruisecontrol.net\testfile.xml").ToString();
			AssertContains(@"/xml=""c:\program files\cruisecontrol.net\testfile.xml""", argString);
		}

		[Test]
		public void AllAssembliesShouldBeIncludedInTheArgument()
		{
			string argString = new NUnitArgument(new string[] {"foo.dll", "bar.dll", "car.dll"}, "testfile.xml").ToString();
			AssertContains(" foo.dll ", argString);
			AssertContains(" bar.dll ", argString);
			AssertContains(" car.dll", argString);
		}
	}
}