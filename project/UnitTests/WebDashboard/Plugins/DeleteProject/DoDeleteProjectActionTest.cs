using System.Web.UI;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
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
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			viewBuilderMock = new DynamicMock(typeof(IDeleteProjectViewBuilder));
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			doDeleteProjectAction = new DoDeleteProjectAction((IDeleteProjectViewBuilder) viewBuilderMock.MockInstance, (IFarmService) farmServiceMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}

		private void VerifyAll()
		{
			viewBuilderMock.Verify();
			cruiseRequestMock.Verify();
		}

		[Test]
		public void ShouldCallFarmServiceAndIfSuccessfulShowSuccessMessage()
		{
			Control view = new Control();
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			farmServiceMock.Expect("DeleteProject", "myServer", "myProject");
			string expectedMessage = "Project Deleted";
			viewBuilderMock.ExpectAndReturn("BuildView", view, new DeleteProjectModel("myServer", "myProject", expectedMessage, false));

			// Execute
			Control returnedView = doDeleteProjectAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(view, returnedView);
			VerifyAll();
		}
	}
}
