using System;
using System.Collections.Specialized;
using System.Web.UI;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class CruiseActionProxyActionTest
	{
		private DynamicMock cruiseRequestFactoryMock;
		private DynamicMock proxiedActionMock;
		private CruiseActionProxyAction proxy;

		private ICruiseRequest cruiseRequest;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			cruiseRequestFactoryMock = new DynamicMock(typeof(ICruiseRequestFactory));
			proxiedActionMock = new DynamicMock(typeof(ICruiseAction));
			proxy = new CruiseActionProxyAction(
				(ICruiseAction) proxiedActionMock.MockInstance,
				(ICruiseRequestFactory) cruiseRequestFactoryMock.MockInstance);

			cruiseRequest = new QueryStringRequestWrapper(null);
			request = new NameValueCollectionRequest(new NameValueCollection());
		}

		private void VerifyAll()
		{
			cruiseRequestFactoryMock.Verify();
			proxiedActionMock.Verify();
		}

		[Test]
		public void ShouldGetCruiseRequestForRequestAndProxyAction()
		{
			Control view = new Control();
			// Setup
			cruiseRequestFactoryMock.ExpectAndReturn("CreateCruiseRequest", cruiseRequest, request);
			proxiedActionMock.ExpectAndReturn("Execute", view, cruiseRequest);

			// Execute
			Control returnedView = proxy.Execute(request);

			// Verify
			Assert.AreEqual(view, returnedView);
			VerifyAll();
		}
	}
}
