using System.Collections.Specialized;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.IO
{
	[TestFixture]
	public class QueryStringRequestWrapperTest
	{
		private NameValueCollection queryString;
		private IRequest underlyingRequest;
		private RequestWrappingCruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			queryString = new NameValueCollection();
			underlyingRequest = new NameValueCollectionRequest(queryString);
			cruiseRequest = new RequestWrappingCruiseRequest(underlyingRequest);
		}

		[Test]
		public void ReturnsEmptyStringIfNoProjectSpecified()
		{
			Assert.AreEqual(string.Empty, cruiseRequest.ProjectName);
		}

		[Test]
		public void ReturnsProjectNameIfProjectSpecified()
		{
			queryString.Add("project", "myproject");
			Assert.AreEqual("myproject", cruiseRequest.ProjectName);
		}

		[Test]
		public void ReturnsEmptyStringIfNoServerSpecified()
		{
			Assert.AreEqual(string.Empty, cruiseRequest.ServerName);
		}

		[Test]
		public void ReturnsServerNameIfServerSpecified()
		{
			queryString.Add("server", "myserver");
			Assert.AreEqual("myserver", cruiseRequest.ServerName);
		}

		[Test]
		public void ReturnsEmptyStringIfNoBuildSpecified()
		{
			Assert.AreEqual(string.Empty, cruiseRequest.BuildName);
		}

		[Test]
		public void ReturnsBuildNameIfBuildSpecified()
		{
			queryString.Add("build", "mybuild");
			Assert.AreEqual("mybuild", cruiseRequest.BuildName);
		}
	}
}
