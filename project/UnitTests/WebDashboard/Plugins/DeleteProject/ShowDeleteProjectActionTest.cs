using System.Collections.Specialized;
using System.Web.UI;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.DeleteProject
{
	[TestFixture]
	public class ShowDeleteProjectActionTest : Assertion
	{
		private DynamicMock cruiseRequestFactoryMock;
		private DynamicMock viewBuilderMock;
		private ShowDeleteProjectAction showDeleteProjectAction;

		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			cruiseRequestFactoryMock = new DynamicMock(typeof(ICruiseRequestFactory));
			viewBuilderMock = new DynamicMock(typeof(IDeleteProjectViewBuilder));
			showDeleteProjectAction = new ShowDeleteProjectAction((ICruiseRequestFactory) cruiseRequestFactoryMock.MockInstance, 
				(IDeleteProjectViewBuilder) viewBuilderMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
			request = new NameValueCollectionRequest(new NameValueCollection());
		}

		private void VerifyAll()
		{
			cruiseRequestFactoryMock.Verify();
			viewBuilderMock.Verify();
			cruiseRequestMock.Verify();
		}

		[Test]
		public void ShouldPassValidModelToBuilderAndReturnBuildersResult()
		{
			Control view = new Control();
			// Setup
			cruiseRequestFactoryMock.ExpectAndReturn("CreateCruiseRequest", cruiseRequest, request);
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			string expectedMessage = "Are you sure you want to delete myProject on myServer?";
			viewBuilderMock.ExpectAndReturn("BuildView", view, new DeleteProjectModel("myServer", "myProject", expectedMessage, true));

			// Execute
			Control returnedView = showDeleteProjectAction.Execute(request);

			// Verify
			AssertEquals(view, returnedView);
			VerifyAll();
		}
	}
}
