using System;
using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.State
{
	[TestFixture]
	public class FileStateManagerTest : CustomAssertion
	{
		private const string ProjectName = IntegrationResultMother.DefaultProjectName;
		private const string DefaultStateFilename = "Test.state";
		private FileStateManager state;
		private IntegrationResult result;
		private IMock mockIO;

		[SetUp]
		public void SetUp()
		{
			mockIO = new DynamicMock(typeof (IFileSystem));
			mockIO.Strict = true;

			state = new FileStateManager((IFileSystem) mockIO.MockInstance);
			state.StateFileDirectory = Path.GetTempPath();
			result = IntegrationResultMother.CreateSuccessful();
			result.ProjectName = ProjectName;
		}

		[TearDown]
		public void TearDown()
		{
			mockIO.Verify();
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"<state><directory>c:\temp</directory></state>";
			state = (FileStateManager) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\temp", state.StateFileDirectory);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void LoadShouldThrowExceptionIfStateFileDoesNotExist()
		{
			mockIO.ExpectAndThrow("Load", new FileNotFoundException(), StateFilename());		
			state.LoadState(ProjectName);
		}

		[Test]
		public void HasPreviousStateIsTrueIfStateFileExists()
		{
			mockIO.ExpectAndReturn("FileExists", true, StateFilename());		
			Assert.IsTrue(state.HasPreviousState(ProjectName));			
		}
		
		[Test]
		public void SaveAndReload()
		{
			CollectingConstraint contents = new CollectingConstraint();
			mockIO.Expect("Save", StateFilename(), contents);
			state.SaveState(result);

			mockIO.ExpectAndReturn("Load", new StringReader(contents.Parameter.ToString()), StateFilename());
			IIntegrationResult actual = state.LoadState(ProjectName);
			Assert.AreEqual(result, actual);
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void SaveWithInvalidDirectory()
		{
			state.StateFileDirectory = @"c:\invalid\folder\";
		}

		[Test]
		public void AttemptToSaveWithInvalidXml()
		{
			mockIO.Expect("Save", StateFilename(), new IsAnything());

			result.Label = "<&/<>";
			result.AddTaskResult("<badxml>>");
			state.SaveState(result);
		}

		[Test]
		public void SaveProjectWithSpacesInName()
		{
			mockIO.Expect("Save", Path.Combine(Path.GetTempPath(), "MyProject.state"), new IsAnything());

			result.ProjectName = "my project";
			state.SaveState(result);
		}

		[Test]
		public void ShouldWriteXmlUsingUTF8Encoding()
		{
			mockIO.Expect("Save", StateFilename(), new StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));

			result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			state.SaveState(result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void HandleExceptionSavingStateFile()
		{
			mockIO.ExpectAndThrow("Save", new SystemException(), StateFilename(), new IsAnything());
			state.SaveState(result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void HandleExceptionLoadingStateFile()
		{
			mockIO.ExpectAndThrow("Load", new SystemException(), StateFilename());
			state.LoadState(ProjectName);
		}

		private string StateFilename()
		{
			return Path.Combine(Path.GetTempPath(), DefaultStateFilename);
		}
	}
}