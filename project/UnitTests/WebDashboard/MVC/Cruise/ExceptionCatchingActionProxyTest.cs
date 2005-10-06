using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class ExceptionCatchingActionProxyTest
	{
		private DynamicMock actionMock;
		private DynamicMock velocityViewGeneratorMock;
		private ExceptionCatchingActionProxy exceptionCatchingAction;
		private IResponse response;
		private IResponse errorResponse;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			actionMock = new DynamicMock(typeof(IAction));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator)) ;
			exceptionCatchingAction = new ExceptionCatchingActionProxy((IAction) actionMock.MockInstance, (IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance);
			response = new HtmlFragmentResponse("my view");
			errorResponse = new HtmlFragmentResponse("error view");
			request = new NameValueCollectionRequest(null, null);
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
			actionMock.ExpectAndReturn("Execute", response, request);

			// Execute & Verify
			Assert.AreEqual(response, exceptionCatchingAction.Execute(request));
			VerifyAll();
		}

		[Test]
		public void ShouldGiveViewOfExceptionIfProxiedActionThowsException()
		{
			// Setup
			CruiseControlException e = new CruiseControlException("A nasty exception");
			actionMock.ExpectAndThrow("Execute", e, request);

			Hashtable expectedContext = new Hashtable();
			expectedContext["exception"] = e;
			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", errorResponse, "ActionException.vm", new HashtableConstraint(expectedContext));

			// Execute & Verify
			Assert.AreEqual(errorResponse, exceptionCatchingAction.Execute(request));
			VerifyAll();
		}
	}
}
