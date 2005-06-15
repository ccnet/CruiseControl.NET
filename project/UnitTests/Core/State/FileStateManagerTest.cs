using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
	[TestFixture]
	public class FileStateManagerTest : CustomAssertion
	{
		private const string TempDirectory = "integrationstate";
		private const string ProjectName = IntegrationResultMother.DefaultProjectName;
		private string DefaultStateFilename = "Test.state";
		private string tempDir;
		private FileStateManager state;
		private IntegrationResult result;

		[SetUp]
		public void SetUp()
		{
			tempDir = TempFileUtil.CreateTempDir(TempDirectory);

			state = new FileStateManager();
			state.StateFileDirectory = tempDir;
			result = IntegrationResultMother.CreateSuccessful();
			result.ProjectName = ProjectName;
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(tempDir);
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"<state><directory>c:\temp</directory></state>";
			state = (FileStateManager) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\temp", state.StateFileDirectory);
		}

		[Test]
		public void LoadShouldReturnInitialIntegrationResultIfStateFileDoesNotExist()
		{
			Assert.IsTrue(state.LoadState(ProjectName).IsInitial());
		}

		[Test]
		public void SaveAndReload()
		{
			Assert.IsFalse(File.Exists(StateFilename()));
			state.SaveState(result);
			Assert.IsTrue(File.Exists(StateFilename()));
			IIntegrationResult actual = state.LoadState(ProjectName);
			Assert.AreEqual(result, actual);
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void SaveWithInvalidDirectory()
		{
			state.StateFileDirectory = @"c:\invalid\folder\";
		}

		[Test]
		public void SaveMultipleTimes()
		{
			state.SaveState(IntegrationResultMother.CreateFailed());
			state.SaveState(IntegrationResultMother.CreateSuccessful());

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.Label = "10";
			state.SaveState(result);

			Assert.AreEqual(1, Directory.GetFiles(tempDir).Length);
			IIntegrationResult actual = state.LoadState(ProjectName);
			Assert.AreEqual("10", actual.Label);
		}

		[Test]
		public void AttemptToSaveWithInvalidXml()
		{
//			result.ProjectName = "<<%_&";  to do -- requires separate test where illegal characters are in project name
			result.Label = "<&/<>";
			result.AddTaskResult("<badxml>>");
			state.SaveState(result);
		}

		[Test]
		public void SaveAndReloadWithUnicodeCharacters()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.Label= "hi there? håkan! \u307b";
			state.SaveState(result);

			IIntegrationResult actual = state.LoadState(ProjectName);
			Assert.AreEqual(result.Label, actual.Label);
		}

		[Test]
		public void SaveProjectWithSpacesInName()
		{
			result.ProjectName = "my project";
			state.SaveState(result);
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "MyProject.state")));
		}

		private string StateFilename()
		{
			return Path.Combine(tempDir, DefaultStateFilename);
		}
	}
}