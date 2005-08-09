using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class BuildCheckingProxyActionTest
	{
		private DynamicMock errorViewBuilderMock;
		private DynamicMock proxiedActionMock;
		private BuildCheckingProxyAction checkingAction;

		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			errorViewBuilderMock = new DynamicMock(typeof(IErrorViewBuilder));
			proxiedActionMock = new DynamicMock(typeof(ICruiseAction));
			checkingAction = new BuildCheckingProxyAction(
				(ICruiseAction) proxiedActionMock.MockInstance,
				(IErrorViewBuilder) errorViewBuilderMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}

		private void VerifyAll()
		{
			errorViewBuilderMock.Verify();
			cruiseRequestMock.Verify();
			proxiedActionMock.Verify();
		}

		[Test]
		public void ShouldProxyIfBuildPresent()
		{
			IResponse response = new HtmlFragmentResponse("foo");
			// Setup
			cruiseRequestMock.ExpectAndReturn("BuildName", "myBuild");
			errorViewBuilderMock.ExpectNoCall("BuildView", typeof(string));
			proxiedActionMock.ExpectAndReturn("Execute", response, cruiseRequest);

			// Execute
			IResponse returnedResponse = checkingAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}

		[Test]
		public void ShouldNotProxyAndShowErrorMessageIfBuildMissing()
		{
			IResponse response = new HtmlFragmentResponse("foo");
			// Setup
			cruiseRequestMock.ExpectAndReturn("BuildName", "");
			errorViewBuilderMock.ExpectAndReturn("BuildView", response, new IsTypeOf(typeof(string)));
			proxiedActionMock.ExpectNoCall("Execute", typeof(ICruiseRequest));

			// Execute
			IResponse returnedResponse = checkingAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}
	}
}
