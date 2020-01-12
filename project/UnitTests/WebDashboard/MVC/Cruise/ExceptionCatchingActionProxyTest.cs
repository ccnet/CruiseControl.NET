using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class ExceptionCatchingActionProxyTest
	{
		private Mock<IAction> actionMock;
		private Mock<IVelocityViewGenerator> velocityViewGeneratorMock;
		private ExceptionCatchingActionProxy exceptionCatchingAction;
		private IResponse response;
		private HtmlFragmentResponse errorResponse;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			actionMock = new Mock<IAction>();
			velocityViewGeneratorMock = new Mock<IVelocityViewGenerator>();
			exceptionCatchingAction = new ExceptionCatchingActionProxy((IAction) actionMock.Object, (IVelocityViewGenerator) velocityViewGeneratorMock.Object);
			response = new HtmlFragmentResponse("my view");
			errorResponse = new HtmlFragmentResponse("error view");
            request = new NameValueCollectionRequest(null, null, null, null, null);
		}

		private void VerifyAll()
		{
			actionMock.Verify();
			velocityViewGeneratorMock.Verify();
		}

		[Test]
		public void ShouldReturnProxiedViewIfProxiedActionDoesntThrowException()
		{
			// Setup
			actionMock.Setup(_action => _action.Execute(request)).Returns(response).Verifiable();

			// Execute & Verify
			Assert.AreEqual(response, exceptionCatchingAction.Execute(request));
			VerifyAll();
		}

		[Test]
		public void ShouldGiveViewOfExceptionIfProxiedActionThowsException()
		{
			// Setup
			CruiseControlException e = new CruiseControlException("A nasty exception");
			actionMock.Setup(_action => _action.Execute(request)).Throws(e).Verifiable();

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView("ActionException.vm", It.Is<Hashtable>(t => t.Count == 1 && t["exception"] == e))).
				Returns(errorResponse).Verifiable();

			// Execute & Verify
			Assert.AreEqual(errorResponse, exceptionCatchingAction.Execute(request));
			VerifyAll();
		}
	}
}
