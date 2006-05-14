using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class AddBuildServerTest
	{
		// This isn't really a test, just a quick way to invoke and display the
		// dialog for interactive testing
		[Test]
		[Explicit]
		public void ShowDialogForInteractiveTesting()
		{
			AddBuildServer addBuildServer = new AddBuildServer(null);
			BuildServer server = addBuildServer.ChooseNewBuildServer(null);
			Console.WriteLine(server);
		}

	}
}