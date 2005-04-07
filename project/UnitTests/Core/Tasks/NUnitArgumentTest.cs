using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NUnitArgumentTest : CustomAssertion
	{
		[Test]
		public void	IfNoAssembliesAreSpecifiedThenTheArgumentIsInvalid()
		{
			NUnitArgument arg=new NUnitArgument(null);
		    string argString = arg.ToString();
			Assert.AreEqual(String.Empty,argString);
		}
		[Test]
		public void	NoLogoIsOnAlwaysIfValidAssemblyExists()
		{
			NUnitArgument arg = new NUnitArgument(new string[] {"foo.dll"});
			string argString = arg.ToString();
			AssertContains(" /nologo ", argString);
		}
		[Test]
		public void	XmlConsoleFlagIsAlwaysOnIfAssemblyExists()
		{
			NUnitArgument arg = new NUnitArgument(new string[] {"foo.dll"});
			string argString = arg.ToString();
			AssertContains(" /xmlConsole ", argString);
		}

		[Test]
		public void	IfAssembliesAreSpecifiedAllAssembliesExistInTheResult()
		{
			NUnitArgument arg = new NUnitArgument(new string[]{"foo.dll","bar.dll","car.dll"});
			string argString = arg.ToString();
			AssertContains(" foo.dll ",argString);
			AssertContains(" bar.dll ",argString);
			AssertContains(" car.dll ",argString);
		}
	}
}
