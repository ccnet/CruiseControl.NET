using System.Collections.Specialized;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class CruiseActionProxyActionTest
	{
		private Mock<ICruiseRequestFactory> cruiseRequestFactoryMock;
		private Mock<ICruiseAction> proxiedActionMock;
        private Mock<ICruiseUrlBuilder> urlBuilderMock;
		private CruiseActionProxyAction proxy;

		private ICruiseRequest cruiseRequest;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			cruiseRequestFactoryMock = new Mock<ICruiseRequestFactory>();
			proxiedActionMock = new Mock<ICruiseAction>();
            urlBuilderMock = new Mock<ICruiseUrlBuilder>();

			proxy = new CruiseActionProxyAction(
				(ICruiseAction) proxiedActionMock.Object,
				(ICruiseRequestFactory) cruiseRequestFactoryMock.Object,
                (ICruiseUrlBuilder)urlBuilderMock.Object,
                null);

            cruiseRequest = new RequestWrappingCruiseRequest(null, (ICruiseUrlBuilder)urlBuilderMock.Object, null);
            request = new NameValueCollectionRequest(new NameValueCollection(), null, null, null, null);
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
			cruiseRequestFactoryMock.Setup(factory => factory.CreateCruiseRequest(request,
                (ICruiseUrlBuilder)urlBuilderMock.Object, null)).Returns(cruiseRequest).Verifiable();
			proxiedActionMock.Setup(_action => _action.Execute(cruiseRequest)).Returns(response).Verifiable();

			// Execute
			IResponse returnedResponse = proxy.Execute(request);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}
	}
}
