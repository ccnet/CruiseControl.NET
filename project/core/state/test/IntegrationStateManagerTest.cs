using System;
using System.IO;
using System.Xml;

using Exortech.NetReflector;
using NUnit.Framework;
using tw.ccnet.core.test;
using tw.ccnet.core.util;

namespace tw.ccnet.core.state.test
{
	[TestFixture]
	public class IntegrationStateManagerTest : CustomAssertion
	{
		private const string TEMP = "integrationstate";
		private string _tempDir;
		IntegrationStateManager _state;
		IntegrationResult _result;

		[SetUp]
		protected void SetUp()
		{
			_state = new IntegrationStateManager();
			_result = IntegrationResultFixture.CreateIntegrationResult();
			_tempDir = TempFileUtil.CreateTempDir(TEMP);
		}

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		[Test]
		public void PopulateFromReflector()
		{
			XmlNode node = XmlUtil.CreateDocumentElement(@"<state><directory>c:\temp</directory><filename>project.state</filename></state>");

			_state = (IntegrationStateManager)new XmlPopulator().Populate(node);
			AssertEquals(@"c:\temp", _state.Directory);
			AssertEquals("project.state", _state.Filename);
		}

		[Test]
		public void SaveAndReload()
		{
			_state.Directory = _tempDir;
			_state.Filename = "ccnet.state";
			AssertFalse(_state.Exists());
			_state.Save(_result);
			Assert(_state.Exists());
			IntegrationResult actual = _state.Load();
			AssertEquals(_result, actual);
		}

		[Test]
		public void ExistsIsFalseIfFolderIsInvalid()
		{
			_state.Directory = @"z:\folder\is\invalid";
			_state.Filename = "ccnet.state";
			AssertFalse(_state.Exists());
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
				_state.Save(_result);
				AssertEquals(1, Directory.GetFiles(_tempDir).Length);
				Assert(File.Exists(_state.GetFilePath()));
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
			_state.Save(_result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void LoadWithInvalidDirectory()
		{
			_state.Directory = @"c:\invalid\folder\";
			_state.Filename = "ccnet.state";
			_state.Load();
		}

		[Test]
		public void SaveMultipleTimes()
		{
			_state.Directory = _tempDir;
			_state.Save(IntegrationResultMother.CreateFailed());
			_state.Save(IntegrationResultMother.CreateSuccessful());

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.Label = "10";
			_state.Save(result);

			AssertEquals(1, Directory.GetFiles(_tempDir).Length);
			IntegrationResult actual = _state.Load();
			AssertEquals("10", actual.Label);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void Load_NoPreviousStateFileExist()
		{
			_state.Directory = _tempDir;
			AssertFalse(_state.Exists());
			AssertNull(_state.Load());
		}
	}
}
