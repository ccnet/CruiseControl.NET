using System.Web.UI;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.DeleteProject
{
	[TestFixture]
	public class ShowDeleteProjectActionTest
	{
		private DynamicMock viewBuilderMock;
		private ShowDeleteProjectAction showDeleteProjectAction;

		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			viewBuilderMock = new DynamicMock(typeof(IDeleteProjectViewBuilder));
			showDeleteProjectAction = new ShowDeleteProjectAction((IDeleteProjectViewBuilder) viewBuilderMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}

		private void VerifyAll()
		{
			viewBuilderMock.Verify();
			cruiseRequestMock.Verify();
		}

		[Test]
		public void ShouldPassValidModelToBuilderAndReturnBuildersResult()
		{
			Control view = new Control();
			// Setup
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			string expectedMessage = "Please confirm you want to delete myProject, and choose which extra delete actions you want to perform";
			viewBuilderMock.ExpectAndReturn("BuildView", view, new DeleteProjectModel(projectSpecifier, expectedMessage, true, true, true, true));

			// Execute
			Control returnedView = showDeleteProjectAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(view, returnedView);
			VerifyAll();
		}
	}
}
