using System;
using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

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

		[Test]
		public void LoadStateFromVersionedXml()
		{
			string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<IntegrationResult xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ProjectName>NetReflector</ProjectName>
  <ProjectUrl>http://localhost/ccnet</ProjectUrl>
  <BuildCondition>ForceBuild</BuildCondition>
  <Label>1.0.0.7</Label>
  <WorkingDirectory>C:\dev\ccnet\integrationTests\netreflector</WorkingDirectory>
  <ArtifactDirectory>C:\dev\ccnet\trunk4\build\server\NetReflector\Artifacts</ArtifactDirectory>
  <StatisticsFile>report.xml</StatisticsFile>
  <Status>Success</Status>
  <LastIntegrationStatus>Success</LastIntegrationStatus>
  <LastSuccessfulIntegrationLabel>1.0.0.7</LastSuccessfulIntegrationLabel>
  <StartTime>2006-12-10T14:41:50-08:00</StartTime>
  <EndTime>2006-12-10T14:42:12-08:00</EndTime>
</IntegrationResult>";

			result = (IntegrationResult) state.LoadState(new StringReader(xml));
			Assert.AreEqual("NetReflector", result.ProjectName);
			Assert.AreEqual("http://localhost/ccnet", result.ProjectUrl);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual("1.0.0.7", result.Label);
			Assert.AreEqual(@"C:\dev\ccnet\integrationTests\netreflector", result.WorkingDirectory);
			Assert.AreEqual(@"C:\dev\ccnet\trunk4\build\server\NetReflector\Artifacts", result.ArtifactDirectory);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Success, result.LastIntegrationStatus);
			Assert.AreEqual("1.0.0.7", result.LastSuccessfulIntegrationLabel);
			Assert.AreEqual(new DateTime(2006, 12, 10, 22, 41, 50), result.StartTime.ToUniversalTime());
		}

		private string StateFilename()
		{
			return Path.Combine(Path.GetTempPath(), DefaultStateFilename);
		}
	}
}