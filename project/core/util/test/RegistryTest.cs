using NUnit.Framework;
using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class RegistryTest : Assertion
	{
		private const string VALID_REGISTRY_PATH = @"SOFTWARE\Microsoft\Shared Tools";

		[Test]
		public void GetLocalMachineSubKeyValue()
		{
			string sharedPath = new Registry().GetLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "SharedFilesDir");
			Assert("SharedFilesDir does not exist: " + sharedPath, Directory.Exists(sharedPath));
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void TryToGetInvalidSubKey()
		{
			new Registry().GetLocalMachineSubKeyValue(@"SOFTWARE\BozosSoftwareEmporium\Clowns", "Barrios");
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void TryToGetInvalidSubKeyValue()
		{
			new Registry().GetLocalMachineSubKeyValue(VALID_REGISTRY_PATH, "Barrios");
		}
	}
}
