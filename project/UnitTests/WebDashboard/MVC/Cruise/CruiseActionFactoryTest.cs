using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
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
			requestMock.ExpectAndReturn("FindParameterStartingWith", CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction", CruiseActionFactory.ACTION_PARAMETER_PREFIX);
			objectGiverMock.ExpectAndReturn("GiveObjectById", null, "myAction");

			Assert.IsTrue(actionFactory.Create(request) is UnknownActionAction);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailable()
		{
			requestMock.ExpectAndReturn("FindParameterStartingWith", CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction", CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectGiverMock.ExpectAndReturn("GiveObjectById", stubAction, "myAction");

			Assert.AreSame(stubAction, actionFactory.Create(request));

			VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailableAndActionArgSpecified()
		{
			string actionParam = CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction" + CruiseActionFactory.ACTION_ARG_SEPARATOR + "myArg1";
			requestMock.ExpectAndReturn("FindParameterStartingWith", actionParam, CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectGiverMock.ExpectAndReturn("GiveObjectById", stubAction, "myAction");

			Assert.AreSame(stubAction, actionFactory.Create(request));

			VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailableAndMultipleActionArgsSpecified()
		{
			string actionParam = CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction" + CruiseActionFactory.ACTION_ARG_SEPARATOR + "myArg1" + CruiseActionFactory.ACTION_ARG_SEPARATOR + "myArg2";
			requestMock.ExpectAndReturn("FindParameterStartingWith", actionParam, CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			IAction stubAction = (IAction) new DynamicMock(typeof(IAction)).MockInstance;
			objectGiverMock.ExpectAndReturn("GiveObjectById", stubAction, "myAction");

			Assert.AreSame(stubAction, actionFactory.Create(request));

			VerifyAll();
		}

		[Test]
		public void ShouldReturnEmptyArgsIfNoAction()
		{
			requestMock.ExpectAndReturn("FindParameterStartingWith", "", CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			Assert.AreEqual(0, actionFactory.ActionArguments(request).Length);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnEmptyArgsIfActionButNoArgs()
		{
			requestMock.ExpectAndReturn("FindParameterStartingWith", CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction", CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			Assert.AreEqual(0, actionFactory.ActionArguments(request).Length);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnSingletonListIfOneArg()
		{
			string actionParam = CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction" + CruiseActionFactory.ACTION_ARG_SEPARATOR + "myArg1";
			requestMock.ExpectAndReturn("FindParameterStartingWith", actionParam, CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			string[] args = actionFactory.ActionArguments(request);
			Assert.AreEqual(1, args.Length);
			Assert.AreEqual("myArg1", args[0]);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnSeveralArgsIfAvailable()
		{
			string actionParam = CruiseActionFactory.ACTION_PARAMETER_PREFIX + "myAction" + CruiseActionFactory.ACTION_ARG_SEPARATOR + "myArg1" + CruiseActionFactory.ACTION_ARG_SEPARATOR + "myArg2";
			requestMock.ExpectAndReturn("FindParameterStartingWith", actionParam, CruiseActionFactory.ACTION_PARAMETER_PREFIX);

			string[] args = actionFactory.ActionArguments(request);
			Assert.AreEqual(2, args.Length);
			Assert.AreEqual("myArg1", args[0]);
			Assert.AreEqual("myArg2", args[1]);

			VerifyAll();
		}
	}
}
