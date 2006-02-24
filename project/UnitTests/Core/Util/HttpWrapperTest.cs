using System;
using System.Net;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture, Ignore("Test connect to external uris and hence are slow to run and do not work disconnected.")]
	public class HttpWrapperTest
	{
		private HttpWrapper httpWrapper;

		[SetUp]
		protected void SetUp()
		{
			httpWrapper = new HttpWrapper();			
		}

		[Test]
		public void TestValidUrlThatReturnsLastModified()
		{
			DateTime lastModTime = httpWrapper.GetLastModifiedTimeFor(new Uri(@"http://www.apache.org"), DateTime.MinValue);
			Assert.IsTrue(lastModTime > DateTime.MinValue);
		}

		[Test]
		public void TestValidDynamicUrlThatDoesNotReturnLastModified()
		{
			DateTime lastModTime = httpWrapper.GetLastModifiedTimeFor(new Uri(@"http://confluence.public.thoughtworks.org/homepage.action"), DateTime.MinValue);
			Assert.AreEqual(DateTime.MinValue, lastModTime);
		}

		[Test, ExpectedException(typeof(WebException))]
		public void TestInvalidUrl()
		{
			httpWrapper.GetLastModifiedTimeFor(new Uri(@"http://wibble.wibble/"), DateTime.MinValue);
		}

		[Test]
		public void TestLastModifiedIsNotChanged()
		{
			Uri urlToRequest = new Uri(@"http://www.apache.org/");
			DateTime lastModified = httpWrapper.GetLastModifiedTimeFor(urlToRequest, DateTime.MinValue);
			Assert.IsTrue(lastModified > DateTime.MinValue);

			DateTime notModified = httpWrapper.GetLastModifiedTimeFor(urlToRequest, lastModified);
			Assert.AreEqual(notModified, lastModified);
		}
	}
}