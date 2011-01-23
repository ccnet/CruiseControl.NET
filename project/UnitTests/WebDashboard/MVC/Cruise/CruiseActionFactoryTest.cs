using Rhino.Mocks;
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
        private MockRepository mocks = new MockRepository();
        private ObjectSource objectSource;
		private CruiseActionFactory actionFactory;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			objectSource = mocks.StrictMock<ObjectSource>();
			actionFactory = new CruiseActionFactory(objectSource);
			request = mocks.StrictMock<IRequest>();
		}

		[Test]
		public void ShouldReturnUnknownActionIfActionIsntAvailable()
		{
			Expect.Call(request.FileNameWithoutExtension)
                .Return("ThisAintNoAction");
			Expect.Call(objectSource.GetByName("thisaintnoaction"))
                .Return(null);
            mocks.ReplayAll();

			Assert.IsTrue(actionFactory.Create(request) is UnknownActionAction);

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfNoActionSpecified()
		{
			Expect.Call(request.FileNameWithoutExtension)
                .Return(string.Empty);

			var stubAction = mocks.DynamicMock<IAction>();
			Expect.Call(objectSource.GetByType(typeof(DefaultAction)))
                .Return(stubAction);
            mocks.ReplayAll();

			Assert.AreEqual(stubAction, actionFactory.Create(request));

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfDefaultActionSpecified()
		{
			Expect.Call(request.FileNameWithoutExtension)
                .Return("default");

			var stubAction = mocks.DynamicMock<IAction>();
			Expect.Call(objectSource.GetByType(typeof(DefaultAction)))
                .Return(stubAction);
            mocks.ReplayAll();

			Assert.AreEqual(stubAction, actionFactory.Create(request));

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailable()
		{
			Expect.Call(request.FileNameWithoutExtension)
                .Return("myAction");

			var stubAction = mocks.DynamicMock<IAction>();
			Expect.Call(objectSource.GetByName("myaction"))
                .Return(stubAction);
            mocks.ReplayAll();

			Assert.AreSame(stubAction, actionFactory.Create(request));

            mocks.VerifyAll();
		}
	}
}
