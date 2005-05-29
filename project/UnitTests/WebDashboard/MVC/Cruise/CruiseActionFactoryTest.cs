using NMock;
using NUnit.Framework;
using ObjectWizard;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class CruiseActionFactoryTest
	{
		private DynamicMock objectGiverMock;
		private CruiseActionFactory actionFactory;
		private DynamicMock requestMock;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			objectGiverMock = new DynamicMock(typeof(ObjectGiver))	;
			actionFactory = new CruiseActionFactory((ObjectGiver) objectGiverMock.MockInstance);

			requestMock = new DynamicMock(typeof(IRequest));
			request = (IRequest) requestMock.MockInstance;
		}

		private void VerifyAll()
		{
			objectGiverMock.Verify();
			requestMock.Verify();
		}

		[Test]
		public void ShouldReturnUnknownActionIfActionIsntAvailable()
		{
			requestMock.ExpectAndReturn("FindParameterStartingWith", DefaultUrlBuilder.ACTION_PARAMETER_PREFIX + "myAction", DefaultUrlBuilder.ACTION_PARAMETER_PREFIX);
			objectGiverMock.ExpectAndReturn("GiveObjectById", null, "myAction");

			Assert.IsTrue(actionFactory.Create(request) is UnknownActionAction);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfNoActionSpecified()
		{
			requestMock.ExpectAndReturn("FindParameterStartingWith", "", DefaultUrlBuilder.ACTION_PARAMETER_PREFIX);

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectGiverMock.ExpectAndReturn("GiveObjectByType", stubAction, typeof(DefaultAction));

			Assert.AreEqual(stubAction, actionFactory.Create(request));

			VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailable()
		{
			requestMock.ExpectAndReturn("FindParameterStartingWith", DefaultUrlBuilder.ACTION_PARAMETER_PREFIX + "myAction", DefaultUrlBuilder.ACTION_PARAMETER_PREFIX);

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectGiverMock.ExpectAndReturn("GiveObjectById", stubAction, "myAction");

			Assert.AreSame(stubAction, actionFactory.Create(request));

			VerifyAll();
		}
	}
}
