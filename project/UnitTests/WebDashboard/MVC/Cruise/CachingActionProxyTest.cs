using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class CachingActionProxyTest
	{
		private DynamicMock mockCache;
		private DynamicMock mockAction;
		private CachingActionProxy proxy;

		[SetUp]
		public void SetUp()
		{
			mockCache = new DynamicMock(typeof (IResponseCache));
			mockCache.Strict = true;
			mockAction = new DynamicMock(typeof (IAction));
			mockAction.Strict = true;
			proxy = new CachingActionProxy((IAction) mockAction.MockInstance, (IResponseCache) mockCache.MockInstance);
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

			mockCache.ExpectAndReturn("Get", expectedResponse, request);

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

			mockCache.ExpectAndReturn("Get", null, request);
			mockAction.ExpectAndReturn("Execute", generatedResponse, request);
			mockCache.Expect("Insert", request, generatedResponse);

			IResponse actualResponse = proxy.Execute(request);
			Assert.AreSame(generatedResponse, actualResponse);

			VerifyAll();
		}
	}
}
