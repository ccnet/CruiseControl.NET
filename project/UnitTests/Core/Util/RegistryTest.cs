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
