using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	[Category("WindowsOnly")]
	public class RegistryTest
	{
		private const string VALID_REGISTRY_PATH = @"SOFTWARE\Microsoft\Shared Tools";

		[Test]
		public void GetLocalMachineSubKeyValue()
		{
			string sharedPath = new Registry().GetLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "SharedFilesDir");
			Assert.IsTrue(Directory.Exists(sharedPath), "#A1");
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

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void TryToGetExpectedInvalidSubKey()
		{
			new Registry().GetExpectedLocalMachineSubKeyValue(@"SOFTWARE\BozosSoftwareEmporium\Clowns", "Barrios");
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void TryToGetExpectedInvalidSubKeyValue()
		{
			new Registry().GetExpectedLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "Barrios");
		}
	}
}
