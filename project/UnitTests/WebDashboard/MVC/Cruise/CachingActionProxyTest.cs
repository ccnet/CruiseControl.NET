using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class CachingActionProxyTest
	{
		private Mock<IResponseCache> mockCache;
		private Mock<IAction> mockAction;
		private CachingActionProxy proxy;

		[SetUp]
		public void SetUp()
		{
			mockCache = new Mock<IResponseCache>(MockBehavior.Strict);
			mockAction = new Mock<IAction>(MockBehavior.Strict);
			proxy = new CachingActionProxy((IAction) mockAction.Object, (IResponseCache) mockCache.Object);
		}

		private void VerifyAll()
		{
			mockCache.Verify();
			mockAction.Verify();
		}

		[Test]
		public void WhenARequestIsFoundInTheCacheItIsReturned()
		{
			IResponse expectedResponse = new HtmlFragmentResponse("<html />");
			IRequest request = CreateRequest();

			mockCache.Setup(cache => cache.Get(request)).Returns(expectedResponse).Verifiable();

			IResponse actualResponse = proxy.Execute(request);
			Assert.AreSame(expectedResponse, actualResponse);

			VerifyAll();
		}

		private IRequest CreateRequest()
		{
            return new NameValueCollectionRequest(null, null, null, null, null);
		}

		[Test]
		public void WhenARequestIsNotFoundInTheCacheTheWrappedActionIsCalledAndInsertedIntoTheCache()
		{
			IResponse generatedResponse = new HtmlFragmentResponse("<html />");
			IRequest request = CreateRequest();

			mockCache.Setup(cache => cache.Get(request)).Returns(() => null).Verifiable();
			mockAction.Setup(_action => _action.Execute(request)).Returns(generatedResponse).Verifiable();
			mockCache.Setup(cache => cache.Insert(request, generatedResponse)).Verifiable();

			IResponse actualResponse = proxy.Execute(request);
			Assert.AreSame(generatedResponse, actualResponse);

			VerifyAll();
		}
	}
}
