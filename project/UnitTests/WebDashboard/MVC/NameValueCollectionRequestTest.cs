using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class NameValueCollectionRequestTest
	{
		[Test]
		public void ShouldReturnFileNameWithoutExtension()
		{
			NameValueCollectionRequest request = new NameValueCollectionRequest(null, null, "/ccnet/file1.aspx", null, null);
			Assert.AreEqual("file1", request.FileNameWithoutExtension);

            request = new NameValueCollectionRequest(null, null, "/file2.aspx", null, null);
			Assert.AreEqual("file2", request.FileNameWithoutExtension);

            request = new NameValueCollectionRequest(null, null, "/ccnet/file3", null, null);
			Assert.AreEqual("file3", request.FileNameWithoutExtension);

            request = new NameValueCollectionRequest(null, null, "/file4", null, null);
			Assert.AreEqual("file4", request.FileNameWithoutExtension);

            request = new NameValueCollectionRequest(null, null, "/", null, null);
			Assert.AreEqual("", request.FileNameWithoutExtension);
		}
	}
}
