using System;
using System.Collections.Specialized;
using System.Web.UI;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class ServerAndProjectCheckingProxyActionTest : Assertion
	{
		private DynamicMock cruiseRequestFactoryMock;
		private DynamicMock errorViewBuilderMock;
		private DynamicMock proxiedActionMock;
		private ServerAndProjectCheckingProxyAction checkingAction;

		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			cruiseRequestFactoryMock = new DynamicMock(typeof(ICruiseRequestFactory));
			errorViewBuilderMock = new DynamicMock(typeof(IErrorViewBuilder));
			proxiedActionMock = new DynamicMock(typeof(IAction));
			checkingAction = new ServerAndProjectCheckingProxyAction(
				(IAction) proxiedActionMock.MockInstance,
				(IErrorViewBuilder) errorViewBuilderMock.MockInstance,
				(ICruiseRequestFactory) cruiseRequestFactoryMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
			request = new NameValueCollectionRequest(new NameValueCollection());
		}

		private void VerifyAll()
		{
			cruiseRequestFactoryMock.Verify();
			errorViewBuilderMock.Verify();
			cruiseRequestMock.Verify();
			proxiedActionMock.Verify();
		}

		[Test]
		public void ShouldProxyIfServerAndProjectBothPresent()
		{
			Control view = new Control();
			// Setup
			cruiseRequestFactoryMock.ExpectAndReturn("CreateCruiseRequest", cruiseRequest, request);
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			errorViewBuilderMock.ExpectNoCall("BuildView", typeof(string));
			proxiedActionMock.ExpectAndReturn("Execute", view, request);

			// Execute
			Control returnedView = checkingAction.Execute(request);

			// Verify
			AssertEquals(view, returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldNotProxyAndShowErrorMessageIfServerMissing()
		{
			Control view = new Control();
			// Setup
			cruiseRequestFactoryMock.ExpectAndReturn("CreateCruiseRequest", cruiseRequest, request);
			cruiseRequestMock.ExpectAndReturn("ServerName", "");
			errorViewBuilderMock.ExpectAndReturn("BuildView", view, new IsTypeOf(typeof(string)));
			proxiedActionMock.ExpectNoCall("Execute", typeof(IRequest));

			// Execute
			Control returnedView = checkingAction.Execute(request);

			// Verify
			AssertEquals(view, returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldNotProxyAndShowErrorMessageIfProjectMissing()
		{
			Control view = new Control();
			// Setup
			cruiseRequestFactoryMock.ExpectAndReturn("CreateCruiseRequest", cruiseRequest, request);
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "");
			errorViewBuilderMock.ExpectAndReturn("BuildView", view, new IsTypeOf(typeof(string)));
			proxiedActionMock.ExpectNoCall("Execute", typeof(IRequest));

			// Execute
			Control returnedView = checkingAction.Execute(request);

			// Verify
			AssertEquals(view, returnedView);
			VerifyAll();
		}
	}
}
