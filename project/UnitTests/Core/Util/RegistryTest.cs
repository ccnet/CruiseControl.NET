using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class RegistryTest
	{
		private const string VALID_REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion";

		[Test]
        [Platform(Exclude = "Mono", Reason = "No real registry under Linux")]
		public void GetLocalMachineSubKeyValue()
		{
			string programFilesPath = new Registry().GetLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "ProgramFilesPath");
			Assert.IsNotNull(programFilesPath, "#A1");
			Assert.AreNotEqual(string.Empty, programFilesPath, "#A2");
			Assert.IsTrue(Directory.Exists(programFilesPath), "#A3");
		}

		[Test]
		public void TryToGetInvalidSubKey()
		{
			Assert.IsNull(new Registry().GetLocalMachineSubKeyValue(@"SOFTWARE\BozosSoftwareEmporium\Clowns", "Barrios"), "#B1");
		}

		[Test]
		public void TryToGetInvalidSubKeyValue()
		{
			Assert.IsNull(new Registry().GetLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "Barrios"), "#C1");
		}

		[Test]
		public void TryToGetExpectedInvalidSubKey()
		{
			Assert.That(delegate { new Registry().GetExpectedLocalMachineSubKeyValue(@"SOFTWARE\BozosSoftwareEmporium\Clowns", "Barrios"); },
                        Throws.TypeOf<CruiseControlException>());
		}

		[Test]
		public void TryToGetExpectedInvalidSubKeyValue()
		{
            Assert.That(delegate { new Registry().GetExpectedLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "Barrios"); },
                        Throws.TypeOf<CruiseControlException>());
		}
	}
}
