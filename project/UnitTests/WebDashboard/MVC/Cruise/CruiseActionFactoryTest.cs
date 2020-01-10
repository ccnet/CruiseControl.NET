using Moq;
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
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
        private ObjectSource objectSource;
		private CruiseActionFactory actionFactory;
		private IRequest request;

		[SetUp]
		public void Setup()
		{
			objectSource = mocks.Create<ObjectSource>(MockBehavior.Strict).Object;
			actionFactory = new CruiseActionFactory(objectSource);
			request = mocks.Create<IRequest>(MockBehavior.Strict).Object;
		}

		[Test]
		public void ShouldReturnUnknownActionIfActionIsntAvailable()
		{
			Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension)
                .Returns("ThisAintNoAction").Verifiable();
			Mock.Get(objectSource).Setup(_objectSource => _objectSource.GetByName("thisaintnoaction"))
                .Returns(null).Verifiable();

			Assert.IsTrue(actionFactory.Create(request) is UnknownActionAction);

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfNoActionSpecified()
		{
			Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension)
                .Returns(string.Empty).Verifiable();

			var stubAction = mocks.Create<IAction>().Object;
			Mock.Get(objectSource).Setup(_objectSource => _objectSource.GetByType(typeof(DefaultAction)))
                .Returns(stubAction).Verifiable();

			Assert.AreEqual(stubAction, actionFactory.Create(request));

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultActionIfDefaultActionSpecified()
		{
			Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension)
                .Returns("default").Verifiable();

			var stubAction = mocks.Create<IAction>().Object;
			Mock.Get(objectSource).Setup(_objectSource => _objectSource.GetByType(typeof(DefaultAction)))
                .Returns(stubAction).Verifiable();

			Assert.AreEqual(stubAction, actionFactory.Create(request));

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldReturnRequestedActionIfAvailable()
		{
			Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension)
                .Returns("myAction").Verifiable();

			var stubAction = mocks.Create<IAction>().Object;
			Mock.Get(objectSource).Setup(_objectSource => _objectSource.GetByName("myaction"))
                .Returns(stubAction).Verifiable();

			Assert.AreSame(stubAction, actionFactory.Create(request));

            mocks.VerifyAll();
		}
	}
}
