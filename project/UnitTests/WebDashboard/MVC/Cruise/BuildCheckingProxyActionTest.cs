using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class BuildCheckingProxyActionTest
	{
		private Mock<IErrorViewBuilder> errorViewBuilderMock;
		private Mock<ICruiseAction> proxiedActionMock;
		private BuildCheckingProxyAction checkingAction;

		private Mock<ICruiseRequest> cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			errorViewBuilderMock = new Mock<IErrorViewBuilder>();
			proxiedActionMock = new Mock<ICruiseAction>();
			checkingAction = new BuildCheckingProxyAction(
				(ICruiseAction) proxiedActionMock.Object,
				(IErrorViewBuilder) errorViewBuilderMock.Object);

			cruiseRequestMock = new Mock<ICruiseRequest>();
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.Object;
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
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("myBuild").Verifiable();
			proxiedActionMock.Setup(_action => _action.Execute(cruiseRequest)).Returns(response).Verifiable();

			// Execute
			IResponse returnedResponse = checkingAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
			errorViewBuilderMock.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldNotProxyAndShowErrorMessageIfBuildMissing()
		{
			IResponse response = new HtmlFragmentResponse("foo");
			// Setup
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("").Verifiable();
			errorViewBuilderMock.Setup(builder => builder.BuildView(It.IsAny<string>())).Returns(response).Verifiable();

			// Execute
			IResponse returnedResponse = checkingAction.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
			proxiedActionMock.VerifyNoOtherCalls();
		}
	}
}
