using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProjectPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.AddProjectPlugin
{
	[TestFixture]
	public class AddProjectPageModelTest : Assertion
	{
		private AddProjectPageModel addProjectPageModel;
		private DynamicMock mockCruiseManagerWrapper;
		private DynamicMock projectSerializerMock;

		[SetUp]
		public void Setup()
		{
			mockCruiseManagerWrapper = new DynamicMock(typeof(ICruiseManagerWrapper));
			projectSerializerMock = new DynamicMock(typeof(IProjectSerializer));
			addProjectPageModel = new AddProjectPageModel((ICruiseManagerWrapper) mockCruiseManagerWrapper.MockInstance,
																				(IProjectSerializer) projectSerializerMock.MockInstance);
		}

		private void VerifyAll()
		{
			mockCruiseManagerWrapper.Verify();
			projectSerializerMock.Verify();
		}

		[Test]
		public void ReturnsAListOfServers()
		{
			mockCruiseManagerWrapper.ExpectAndReturn("GetServerNames", new string[] { "server1"});
			string[] returnedServers = addProjectPageModel.ServerNames;

			AssertEquals(1, returnedServers.Length);
			AssertEquals("server1", returnedServers[0]);

			VerifyAll();
		}

		[Test]
		public void SavesSerializedProjectAndReturnsCorrectMessageIfSuccessful()
		{
			/// Setup
			projectSerializerMock.ExpectAndReturn("Serialize", "serialisedProject", new PropertyIs("Name", "myproject"));
			mockCruiseManagerWrapper.Expect("AddProject", "myServer", "serialisedProject");

			/// Execute
			addProjectPageModel.SelectedServerName = "myServer";
			addProjectPageModel.ProjectName = "myproject";
			string message = addProjectPageModel.Save();

			/// Verify
			AssertEquals("Project saved successfully", message);
			VerifyAll();
		}

		[Test]
		public void SavesSerializedProjectAndReturnsCorrectMessageIfFailed()
		{
			/// Setup
			projectSerializerMock.ExpectAndReturn("Serialize", "serialisedProject", new PropertyIs("Name", "myproject"));
			mockCruiseManagerWrapper.ExpectAndThrow("AddProject", new CruiseControlException("It Didn't Work") ,"myServer", "serialisedProject");

			/// Execute
			addProjectPageModel.SelectedServerName = "myServer";
			addProjectPageModel.ProjectName = "myproject";
			string message = addProjectPageModel.Save();

			/// Verify
			Assert(message.IndexOf("It Didn't Work") > 0);
			VerifyAll();
		}
	}
}
