using NMock;
using NUnit.Framework;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class CruiseActionFactoryTest
	{
		private DynamicMock objectSourceMock;
		private CruiseActionFactory actionFactory;
		private DynamicMock requestMock;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			objectSourceMock = new DynamicMock(typeof(ObjectSource))	;
			actionFactory = new CruiseActionFactory((ObjectSource) objectSourceMock.MockInstance);

			requestMock = new DynamicMock(typeof(IRequest));
			request = (IRequest) requestMock.MockInstance;
		}

		private void VerifyAll()
		{
			objectSourceMock.Verify();
			requestMock.Verify();
		}

		[Test]
		public void ShouldReturnUnknownActionIfActionIsntAvailable()
		{
			requestMock.ExpectAndReturn("FileNameWithoutExtension", "ThisAintNoAction");
			objectSourceMock.ExpectAndReturn("GetByName", null, "ThisAintNoAction");

			Assert.IsTrue(actionFactory.Create(request) is UnknownActionAction);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfNoActionSpecified()
		{
			requestMock.ExpectAndReturn("FileNameWithoutExtension", "");

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectSourceMock.ExpectAndReturn("GetByType", stubAction, typeof(DefaultAction));

			Assert.AreEqual(stubAction, actionFactory.Create(request));

			VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfDefaultActionSpecified()
		{
			requestMock.ExpectAndReturn("FileNameWithoutExtension", "default");

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectSourceMock.ExpectAndReturn("GetByType", stubAction, typeof(DefaultAction));

			Assert.AreEqual(stubAction, actionFactory.Create(request));

			VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailable()
		{
			requestMock.ExpectAndReturn("FileNameWithoutExtension", "myAction");

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectSourceMock.ExpectAndReturn("GetByName", stubAction, "myAction");

			Assert.AreSame(stubAction, actionFactory.Create(request));

			VerifyAll();
		}
	}
}
