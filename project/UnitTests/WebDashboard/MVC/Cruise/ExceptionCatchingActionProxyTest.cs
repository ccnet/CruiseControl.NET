using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class ExceptionCatchingActionProxyTest
	{
		private DynamicMock actionMock;
		private ExceptionCatchingActionProxy exceptionCatchingAction;
		private IView view;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			actionMock = new DynamicMock(typeof(IAction));
			exceptionCatchingAction = new ExceptionCatchingActionProxy((IAction) actionMock.MockInstance);
			view = new DefaultView("my view");
			request = new NameValueCollectionRequest(null);
		}

		private void VerifyAll()
		{
			actionMock.Verify();
		}

		[Test]
		public void ShouldReturnProxiedViewIfProxiedActionDoesntThrowException()
		{
			// Setup
			actionMock.ExpectAndReturn("Execute", view, request);

			// Execute
			IView returnedControl = exceptionCatchingAction.Execute(request);

			// Verify
			Assert.AreEqual(view, returnedControl);
			VerifyAll();
		}

		[Test]
		public void ShouldGiveViewOfExceptionIfProxiedActionThowsException()
		{
			// Setup
			CruiseControlException e = new CruiseControlException("A nasty exception");
			actionMock.ExpectAndThrow("Execute", e, request);

			// Execute
			IView returnedView = exceptionCatchingAction.Execute(request);

			// Verify
			Assert.IsTrue(((HtmlGenericControl) returnedView.Control).InnerHtml.IndexOf("A nasty exception") > -1);
			VerifyAll();
		}
	}
}
