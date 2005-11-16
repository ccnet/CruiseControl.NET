using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUrlBuilderTest
	{
		private DefaultUrlBuilder urlBuilder;

		[SetUp]
		public void Setup()
		{
			urlBuilder = new DefaultUrlBuilder();
		}

		[Test]
		public void ShouldBuildUrlAddingCorrectlyFormattedAction()
		{
			Assert.AreEqual("myAction.aspx", urlBuilder.BuildUrl("myAction"));
		}

		[Test]
		public void ShouldBuildUrlWithActionAndQueryString()
		{
			Assert.AreEqual("myAction.aspx?myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue"));
		}

		[Test]
		public void ShouldBuildUrlWithActionAndQueryStringAndPath()
		{
			Assert.AreEqual("myPath/myAction.aspx?myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue", "myPath/"));
		}

		[Test]
		public void ShouldAddTrailingSlashToPathIfItDoesntAlreadyHaveOne()
		{
			Assert.AreEqual("myPath/myAction.aspx?myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue", "myPath"));
		}

		[Test]
		public void ShouldHandlePathsWithMoreThanOneLevel()
		{
			Assert.AreEqual("myPath/mySubPath/myAction.aspx?myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue", "myPath/mySubPath"));
		}

		[Test]
		public void ShouldUseSpecifiedExtension()
		{
			urlBuilder.Extension = "foo";
			Assert.AreEqual("myAction.foo", urlBuilder.BuildUrl("myAction"));
			Assert.AreEqual("myAction.foo?myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue"));
		}
	}
}