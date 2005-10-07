using System.Collections.Specialized;
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

			cruiseRequest = new RequestWrappingCruiseRequest(null);
			request = new NameValueCollectionRequest(new NameValueCollection(), null, null);
		}

		private void VerifyAll()
		{
			cruiseRequestFactoryMock.Verify();
			proxiedActionMock.Verify();
		}

		[Test]
		public void ShouldGetCruiseRequestForRequestAndProxyAction()
		{
			IResponse response = new HtmlFragmentResponse("foo");
			// Setup
			cruiseRequestFactoryMock.ExpectAndReturn("CreateCruiseRequest", cruiseRequest, request);
			proxiedActionMock.ExpectAndReturn("Execute", response, cruiseRequest);

			// Execute
			IResponse returnedResponse = proxy.Execute(request);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}
	}
}
