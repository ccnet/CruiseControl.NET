using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.Core.State.Test
{
	[TestFixture]
	public class IntegrationStateManagerTest : CustomAssertion
	{
		private const string TEMP = "integrationstate";
		private string _tempDir;
		IntegrationStateManager _state;
		IntegrationResult _result;

		[SetUp]
		public void SetUp()
		{
			_state = new IntegrationStateManager();
			_result = IntegrationResultFixture.CreateIntegrationResult();
			_tempDir = TempFileUtil.CreateTempDir(TEMP);
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"<state><directory>c:\temp</directory><filename>project.state</filename></state>";
			_state = (IntegrationStateManager)NetReflector.Read(xml);
			Assert.AreEqual(@"c:\temp", _state.Directory);
			Assert.AreEqual("project.state", _state.Filename);
		}

		[Test]
		public void SaveAndReload()
		{
			_state.Directory = _tempDir;
			_state.Filename = "ccnet.state";
			AssertFalse(_state.StateFileExists());
			_state.SaveState(_result);
			Assert.IsTrue(_state.StateFileExists());
			IIntegrationResult actual = _state.LoadState();
			Assert.AreEqual(_result, actual);
		}

		[Test]
		public void ExistsIsFalseIfFolderIsInvalid()
		{
			_state.Directory = @"z:\folder\is\invalid";
			_state.Filename = "ccnet.state";
			AssertFalse(_state.StateFileExists());
		}

		[Test]
		public void SaveWithNullDirectory()
		{
			string dir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(_tempDir);
			_state = new IntegrationStateManager();
			_state.Filename = "ccnet.state";
			try
			{
				AssertFalse(File.Exists(_state.GetFilePath()));
				_state.SaveState(_result);
				Assert.AreEqual(1, Directory.GetFiles(_tempDir).Length);
				Assert.IsTrue(File.Exists(_state.GetFilePath()));
			}
			finally
			{
				Directory.SetCurrentDirectory(dir);
			}
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void SaveWithInvalidDirectory()
		{
			_state.Directory = @"c:\invalid\folder\";
			_state.Filename = "ccnet.state";
			_state.SaveState(_result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void LoadWithInvalidDirectory()
		{
			_state.Directory = @"c:\invalid\folder\";
			_state.Filename = "ccnet.state";
			_state.LoadState();
		}

		[Test]
		public void SaveMultipleTimes()
		{
			_state.Directory = _tempDir;
			_state.SaveState(IntegrationResultMother.CreateFailed());
			_state.SaveState(IntegrationResultMother.CreateSuccessful());

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.Label = "10";
			_state.SaveState(result);

			Assert.AreEqual(1, Directory.GetFiles(_tempDir).Length);
			IIntegrationResult actual = _state.LoadState();
			Assert.AreEqual("10", actual.Label);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void Load_NoPreviousStateFileExist()
		{
			_state.Directory = _tempDir;
			AssertFalse(_state.StateFileExists());
			Assert.IsNotNull(_state.LoadState());
		}

		[Test]
		public void AttemptToSaveWithInvalidXml()
		{
			_state.Directory = _tempDir;
			IntegrationResult result = new IntegrationResult();
			result.ProjectName = "<<%_&";
			result.Label = "<&/<>";
			result.AddTaskResult("<badxml>>");
			_state.SaveState(result);
		}

		[Test]
		public void SaveAndReloadWithUnicodeCharacters()
		{
			_state.Directory = _tempDir;

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.ProjectName = "hi there? håkan! \u307b";
			_state.SaveState(result);

			IIntegrationResult actual = _state.LoadState();
			Assert.AreEqual(result.ProjectName, actual.ProjectName);
		}
	}
}
