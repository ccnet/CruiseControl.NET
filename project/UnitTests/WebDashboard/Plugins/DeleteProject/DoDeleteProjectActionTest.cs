using System.Web.UI;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.DeleteProject
{
	[TestFixture]
	public class DoDeleteProjectActionTest
	{
		private DynamicMock viewBuilderMock;
		private DynamicMock farmServiceMock;
		private DoDeleteProjectAction doDeleteProjectAction;

		private DynamicMock cruiseRequestMock;
		private DynamicMock requestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			viewBuilderMock = new DynamicMock(typeof(IDeleteProjectViewBuilder));
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			doDeleteProjectAction = new DoDeleteProjectAction((IDeleteProjectViewBuilder) viewBuilderMock.MockInstance, (IFarmService) farmServiceMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
			requestMock = new DynamicMock(typeof(IRequest));
			cruiseRequestMock.SetupResult("Request", requestMock.MockInstance);
		}

		private void VerifyAll()
		{
			viewBuilderMock.Verify();
			cruiseRequestMock.Verify();
			requestMock.Verify();
		}

		[Test]
		public void ShouldCallFarmServiceAndIfSuccessfulShowSuccessMessage()
		{
			Control view = new Control();
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			requestMock.ExpectAndReturn("GetChecked", true, "PurgeWorkingDirectory");
			requestMock.ExpectAndReturn("GetChecked", false, "PurgeArtifactDirectory");
			requestMock.ExpectAndReturn("GetChecked", true, "PurgeSourceControlEnvironment");
			farmServiceMock.Expect("DeleteProject", "myServer", "myProject", true, false, true);
			string expectedMessage = "Project Deleted";
			viewBuilderMock.ExpectAndReturn("BuildView", view, new DeleteProjectModel("myServer", "myProject", expectedMessage, false, true, false, true));

			// Execute
			Control returnedView = doDeleteProjectAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(view, returnedView);
			VerifyAll();
		}
	}
}
